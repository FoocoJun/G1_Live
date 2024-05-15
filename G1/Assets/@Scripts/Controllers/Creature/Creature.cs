using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Creature : BaseObject {
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
            PlayAnimation(0, AnimName.ATTACK_A, true);
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
    public override void OnDamaged(BaseObject attacker) {
        base.OnDamaged(attacker);

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
            OnDead(attacker);
            CreatureState = ECreatureState.Dead;
        }
    }

    public override void OnDead(BaseObject attacker) {
        base.OnDead(attacker);
    }
    #endregion

    #region  Wait
    protected Coroutine _coWait;

    protected void StartWait(float seconds) {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }
    
    IEnumerator CoWait(float seconds) {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait() {
        if (_coWait != null) {
            StopCoroutine(_coWait);
        }
        _coWait = null;
    }
    #endregion
}