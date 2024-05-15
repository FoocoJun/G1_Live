using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Creature : BaseObject {
    
    public BaseObject Target { get; protected set; }
    public SkillComponent Skills {get; protected set; }

    public Data.CreatureData CreatureData { get; private set; }
    public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;

    #region  Stats
    public float HP;
    public float MaxHp;
    public float MaxHpBonus;
    public float Atk;
    public float AtkRange;
    public float AtkBonus;
    public float Def;
    public float MoveSpeed;
    public float TotalExp;
    public float HpRate;
    public float AtkRate;
    public float DefRate;
    public float MoveSpeedRate;
    public string SkeletonDataID;
    public string AnimatorName;
    public List<int> SkillIdList = new List<int>();
    public int DropItemId;
    #endregion

    protected ECreatureState _creatureState = ECreatureState.None;

    public virtual ECreatureState CreatureState {
        get { return _creatureState; }
        set {
            if (_creatureState != value) {
                _creatureState = value;
                UpdateAnimation();
            }
        }
    }

    protected override void UpdateAnimation() {
        switch (CreatureState) {
            case ECreatureState.Idle:
            PlayAnimation(0, AnimName.IDLE, true);
            break;
            case ECreatureState.Skill:
            // PlayAnimation(0, AnimName.ATTACK_A, true);
            break;
            case ECreatureState.Move:
            PlayAnimation(0, AnimName.MOVE, true);
            break;
            case ECreatureState.Dead:
            PlayAnimation(0, AnimName.DEAD, true);
            RigidBody.simulated = false;
            break;
            default:
            break;
        }
    }

    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        ObjectType = EObjectType.Creature;

        return true;
    }

    public virtual void SetInfo(int templateId) {
        DataTemplateID = templateId;


        if (CreatureType == ECreatureType.Hero) {
            CreatureData = Managers.Data.HeroDic[templateId];
        } else if (CreatureType == ECreatureType.Monster) {
            CreatureData = Managers.Data.MonsterDic[templateId];
        }

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";
        
        // Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffstY);
        Collider.radius = CreatureData.ColliderRadius;

        // RigidBody
        RigidBody.mass = CreatureData.Mass;

        // Spine
        SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.CREATURE);

        // Register AnimEvent
        if (SkeletonAnim.AnimationState != null) {
            SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
            SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
        }

        // TODO
        // Skills

        // state
        HP = CreatureData.MaxHp;
        MaxHp = CreatureData.MaxHp;
        Atk = CreatureData.Atk;
        MoveSpeed = CreatureData.MoveSpeed;

        CreatureState = ECreatureState.Idle;
    }

    public void ChangeColliderSize(EColliderSize size = EColliderSize.Normal) {
        switch (size) {
            case EColliderSize.Small:
                Collider.radius = CreatureData.ColliderRadius * 0.8f;
                break;
            case EColliderSize.Normal:
                Collider.radius = CreatureData.ColliderRadius;
                break;
            case EColliderSize.Big:
                Collider.radius = CreatureData.ColliderRadius * 1.2f;
                break;
        }
    }

    #region AI (FSM)
    public float UpdateAITick { get; protected set; } = 0.0f;
    // 프레임 드랍 방지를 위한 코루틴
    protected IEnumerator CoUpdateAI() {
        while (true) {
            switch (CreatureState) {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Skill:
                    UpdateSkill();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
            }

            if (UpdateAITick > 0) {
                yield return new WaitForSeconds(UpdateAITick);
            } else {
                yield return null;
            }
        }
    }

    protected virtual void UpdateIdle(){}
    protected virtual void UpdateMove(){}
    protected virtual void UpdateSkill(){}
    protected virtual void UpdateDead(){}

    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill) {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false) {
            return;
        }

        Creature creature = attacker as Creature;

        if (creature == null) {
            return;
        }

        float finalDamage = creature.Atk; // TODO
        HP = Mathf.Clamp(HP - finalDamage, 0, MaxHp);

        if (HP <= 0) {
            OnDead(attacker, skill);
            CreatureState = ECreatureState.Dead;
        }
    }

    public override void OnDead(BaseObject attacker, SkillBase skill) {
        base.OnDead(attacker, skill);
    }

    protected BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null) {
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

            // 추가 조건
            if (func != null && func.Invoke(obj) == false) {
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
    protected void ChaseOrAttackTarget(float chaseRange, SkillBase skill) {
        Vector3 dir = Target.transform.position - transform.position;
        float distToTargetSqr = dir.sqrMagnitude;

        float attackRange = HERO_DEFAULT_MELEE_ATTACK_RANGE;
        if (skill.SkillData.ProjectileId != 0) {
            attackRange = HERO_DEFAULT_RANGED_ATTACK_RANGE;
        }

        float finalAttackRange = attackRange + Target.ColliderRadius + ColliderRadius;

        float attackDistanceSqr = finalAttackRange * finalAttackRange;

        if (distToTargetSqr > attackDistanceSqr) {
            // 좇아가기
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            CreatureState = ECreatureState.Move;

            // 너무 멀어지면 포기
            float searchDistanceSqr = chaseRange * chaseRange;
            
            if (distToTargetSqr > searchDistanceSqr) {
                Target = null;
            }

        } else {
            CreatureState = ECreatureState.Skill;
            skill.DoSkill();
            return;
        }
    }
    #endregion

    #region Misc
    protected bool IsValid(BaseObject bo) {
        return bo.IsValid();
    }
    #endregion
}