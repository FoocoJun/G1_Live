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

        CreatureData = Managers.Data.CreatureDic[templateId];

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";
        
        // Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffstY);
        Collider.radius = CreatureData.ColliderRadius;

        // RigidBody
        RigidBody.mass = CreatureData.Mass;

        // Spine
        SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.CREATURE);

        // TODO
        // Skills

        // state
        HP = CreatureData.MaxHp;
        MaxHp = CreatureData.MaxHp;
        Atk = CreatureData.Atk;

        MoveSpeed = CreatureData.MoveSpeed;

        CreatureState = ECreatureState.Idle;
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