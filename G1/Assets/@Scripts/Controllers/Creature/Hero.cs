using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class Hero : Creature {
    bool _needArrange = true;
    public bool NeedArrange {
        get { return _needArrange; }
        set {
            _needArrange = value;

            if (value) {
                ChangeColliderSize(EColliderSize.Big);
            } else {
                TryResizeCollider();
            }
        }
    }

    public override ECreatureState CreatureState {
        get { return _creatureState; }
        set {
            if (_creatureState != value) {
                base.CreatureState = value;

                switch (value) {
                    case ECreatureState.Move:
                        RigidBody.mass = CreatureData.Mass * 5.0f;
                        break;
                    case ECreatureState.Skill:
                        RigidBody.mass = CreatureData.Mass * 500.0f;
                        break;
                    default:
                        RigidBody.mass = CreatureData.Mass;
                        break;
                }
            }
        }
    }

    EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState {
        get { return _heroMoveState; }
        set {
            _heroMoveState = value;

            switch (value) {
                case EHeroMoveState.CollectEnv:
                    NeedArrange = true;
                    break;
                case EHeroMoveState.TargetMonster:
                    NeedArrange = true;
                    break;
                case EHeroMoveState.ForceMove:
                    NeedArrange = true;
                    break;
            }
        }
    }

    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        CreatureType = ECreatureType.Hero;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateId) {
        base.SetInfo(templateId);

        // State
        CreatureState = ECreatureState.Idle;

        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData.SkillIdList);
    }

    public Transform HeroCampDest {
        get {
            HeroCamp camp = Managers.Object.Camp;
            if (HeroMoveState == EHeroMoveState.ReturnToCamp) {
                return camp.Pivot;
            }
            return camp.Destination;
        }
    }

    #region AI
    protected override void UpdateIdle(){
        SetRigidBodyVelocity(Vector2.zero);

        // 0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove) {
            CreatureState = ECreatureState.Move;
            return;
        }

        // 0. 너무 멀어졌다면 강제로 이동

        // 1. 몬스터 사냥
        Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;

        if (creature != null) {
            Target = creature;
            HeroMoveState = EHeroMoveState.TargetMonster;
            CreatureState = ECreatureState.Move;
            return;
        }
        // 2. 주변 Env 채굴
        Env env = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Envs) as Env;

        if (env != null) {
            Target = env;
            HeroMoveState = EHeroMoveState.CollectEnv;
            CreatureState = ECreatureState.Move;
            return;
        }

        // 3. Camp 주변으로 모이기
        if (NeedArrange) {
            HeroMoveState = EHeroMoveState.ReturnToCamp;
            CreatureState = ECreatureState.Move;
            return;
        }

        CreatureState = ECreatureState.Idle;
    }
    protected override void UpdateMove(){
        // 0. 누르고 있다면 강제 이동.
        if (HeroMoveState == EHeroMoveState.ForceMove) {
            CreatureState = ECreatureState.Move;
            Vector3 dir = HeroCampDest.position - transform.position;

            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            return;
        }

        // 1. 주변 모스터 서치
        if (HeroMoveState == EHeroMoveState.TargetMonster) {
            // 몬스터 죽었으면 포기.
            if (Target.IsValid() == false) {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }


        // 2. 주변 Env 채굴
        if (HeroMoveState == EHeroMoveState.CollectEnv) {
            Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;

            // 주변에 몬스터 있으면 우선순위 변경
            if (creature != null) {
                Target = creature;
                HeroMoveState = EHeroMoveState.TargetMonster;
                CreatureState = ECreatureState.Move;
                return;
            }

            // 이미 채집했으면 포기.
            if (Target.IsValid() == false) {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }

        // 3. Camp 주변으로 모이기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp) {
            Vector3 dir = HeroCampDest.transform.position - transform.position;
            float distanceSqr = dir.sqrMagnitude;
            float StopDistanceSqr = HERO_DEFAULT_STOP_RANGE * HERO_DEFAULT_STOP_RANGE;

            if (distanceSqr <= StopDistanceSqr) {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Idle;
                NeedArrange = false;
                return;
            } else {
                float ratio = Mathf.Min(1, dir.magnitude); // TEMP
                float moveSpeed = MoveSpeed * (float)Math.Pow(ratio, 3);
                SetRigidBodyVelocity(dir.normalized * moveSpeed);
                return;
            }
        }

        // 4. 기타 (누르다 땠을 때)
        CreatureState = ECreatureState.Idle;
    }
    protected override void UpdateSkill(){
        SetRigidBodyVelocity(Vector2.zero);

        // 0. 누르고 있다면 강제 이동.
        if (HeroMoveState == EHeroMoveState.ForceMove) {
            CreatureState = ECreatureState.Move;
            return;
        }

        if (Target.IsValid() == false) {
            CreatureState = ECreatureState.Move;
            return;
        }
        
        // // 몬스터 공격중이면 계속
        // if (HeroMoveState == EHeroMoveState.TargetMonster) {
        //     ChaseOrAttackTarget(AttackDistance, HERO_SEARCH_DISTANCE);
        //     return;
        // }

        // // 채집중이면 계속
        // if (HeroMoveState == EHeroMoveState.CollectEnv) {
        //     ChaseOrAttackTarget(AttackDistance, HERO_SEARCH_DISTANCE);
        //     return;
        // }

    }
    protected override void UpdateDead(){
        SetRigidBodyVelocity(Vector2.zero);
    }
    #endregion

    private void HandleOnJoystickStateChanged(EJoystickState joystickState) {
        switch (joystickState) {
            case EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                break;
            default:
                break;
        }
    }

    private void TryResizeCollider() {
        ChangeColliderSize(EColliderSize.Small);

        foreach(var hero in Managers.Object.Heroes) {
            if (hero.HeroMoveState == EHeroMoveState.ReturnToCamp) {
                return;
            }
        }

        foreach (var hero in Managers.Object.Heroes) {
            if (hero.CreatureState == ECreatureState.Idle) {
                hero.ChangeColliderSize(EColliderSize.Big);
            }
        }
    }

    public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e) {
        base.OnAnimEventHandler(trackEntry, e);
    }
}
