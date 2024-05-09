using System.Collections;
using System.Collections.Generic;
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
                // ChangeColliderSize
            }
        }
    }

    public override ECreatureState CreatureState {
        get { return _creatureState; }
        set {
            if (_creatureState != value) {
                base.CreatureState = value;
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
                    //
                    break;
                case EHeroMoveState.TargetMonster:
                    //
                    break;
                case EHeroMoveState.ForceMove:
                    //
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
    public float SearchDistance { get; private set; } = 8.0f;
    public float AttackDistance {
        get {
            float targetRadius = _target.IsValid() ? _target.ColliderRadius : 0;
            // 상대 Collider가 커지면(큰 몹) 근접 공격 distance로는 안 될 가능성이 있기 때문에 추가 연산
            return ColliderRadius + targetRadius + 2.0f;
        }
    }

    BaseObject _target;

    protected override void UpdateIdle(){
        // 0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove) {
            CreatureState = ECreatureState.Move;
            return;
        }

        // 0. 너무 멀어졌다면 강제로 이동

        // 1. 몬스터 사냥
        Creature creature = FindClosestInRange(SearchDistance, Managers.Object.Monsters) as Creature;

        if (creature != null) {
            _target = creature;
            HeroMoveState = EHeroMoveState.TargetMonster;
            CreatureState = ECreatureState.Move;
            return;
        }
        // 2. 주변 Env 채굴
        Env env = FindClosestInRange(SearchDistance, Managers.Object.Envs) as Env;

        if (env != null) {
            _target = env;
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
            if (_target.IsValid() == false) {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }


        // 2. 주변 Env 채굴
        if (HeroMoveState == EHeroMoveState.CollectEnv) {
            Creature creature = FindClosestInRange(SearchDistance, Managers.Object.Monsters) as Creature;

            // 주변에 몬스터 있으면 우선순위 변경
            if (creature != null) {
                _target = creature;
                HeroMoveState = EHeroMoveState.TargetMonster;
                CreatureState = ECreatureState.Move;
                return;
            }

            // 이미 채집했으면 포기.
            if (_target.IsValid() == false) {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }

        // 3. Camp 주변으로 모이기

        // 4. 기타 (누르다 땠을 때)
        CreatureState = ECreatureState.Idle;
    }
    protected override void UpdateSkill(){
        // 0. 누르고 있다면 강제 이동.
        if (HeroMoveState == EHeroMoveState.ForceMove) {
            CreatureState = ECreatureState.Move;

            Vector3 dir = HeroCampDest.position - transform.position;

            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            return;
        }
        
        // // 몬스터 공격중이면 계속
        // if (HeroMoveState == EHeroMoveState.TargetMonster) {
        //     ChaseOrAttackTarget(AttackDistance, SearchDistance);
        //     return;
        // }

        // // 채집중이면 계속
        // if (HeroMoveState == EHeroMoveState.CollectEnv) {
        //     ChaseOrAttackTarget(AttackDistance, SearchDistance);
        //     return;
        // }

    }
    protected override void UpdateDead(){

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

    BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs) {
        BaseObject target = null;
        float bestDistanceSqr = float.MaxValue;
        float searchDistanceSqr = range * range;

        foreach(BaseObject obj in objs) {
            Vector3 dir = obj.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;

            // 서치 범위보다 멀리 있으면 스킵.
            if (distToTargetSqr > searchDistanceSqr) {
                continue;
            }

            // 이미 더 좋은 후보를 찾았으면 스킵.
            if (distToTargetSqr > bestDistanceSqr) {
                continue;
            }

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }

        return target;
    }

    /// <summary>
    /// 타겟을 추격 혹은 공격
    /// </summary>
    /// <param name="attackRange">공격 사정거리</param>
    /// <param name="chaseRange">최대 추격거리</param>
    void ChaseOrAttackTarget(float attackRange, float chaseRange) {
        Vector3 dir = _target.transform.position - transform.position;
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = attackRange * attackRange;

        if (distToTargetSqr > attackDistanceSqr) {
            // 좇아가기
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            CreatureState = ECreatureState.Move;

            // 너무 멀어지면 포기
            float searchDistanceSqr = chaseRange * chaseRange;
            
            if (distToTargetSqr > searchDistanceSqr) {
                _target = null;
                HeroMoveState = EHeroMoveState.None;
            }

        } else {
            CreatureState = ECreatureState.Skill;
            return;
        }
    }
}
