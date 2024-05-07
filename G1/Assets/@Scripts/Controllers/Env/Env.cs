using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Env : BaseObject {
    private Data.EnvData _data;

    private EEnvState _envState = EEnvState.Idle;
    public EEnvState EnvState {
        get { return _envState; }
        set {
            _envState = value;
            UpdateAnimation();
        }
    }

    #region  Stats
    public float HP;
    public float MaxHp;
    #endregion

    public override bool Init()
    {
        if (base.Init() == false) {
            return false;
        }

        ObjectType = EObjectType.Env;

        return true;
    }

    public virtual void SetInfo(int templateId) {
        DataTemplateID = templateId;
        _data = Managers.Data.EnvDic[templateId];

        // Spine
        string ranSpine = _data.SkeletonDataIDs[Random.Range(0, _data.SkeletonDataIDs.Count)];
        SetSpineAnimation(ranSpine, SortingLayers.ENV);

        // Stat
        HP = _data.MaxHp;
        MaxHp = _data.MaxHp;
    }

    protected override void UpdateAnimation() {
        switch (EnvState) {
            case EEnvState.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case EEnvState.OnDamaged:
                PlayAnimation(0, AnimName.DAMAGED, false);
                break;
            case EEnvState.Dead:
                PlayAnimation(0, AnimName.DEAD, false);
                break;
            default:
                break;
        }
    }
}
