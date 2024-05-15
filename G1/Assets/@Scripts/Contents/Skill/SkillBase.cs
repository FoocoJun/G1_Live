using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

public abstract class SkillBase : InitBase
{
    public Creature Owner { get; protected set; }
    
    public Data.SkillData SkillData { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false) {
            return false;
        }
        return true;
    }

    public virtual void SetInfo(Creature owner, int skillTemlateID) {
        Owner = owner;
        SkillData = Managers.Data.SkillDic[skillTemlateID];

        // Register AnimEvent
        if (Owner.SkeletonAnim != null && Owner.SkeletonAnim.AnimationState != null) {
            Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
            Owner.SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
            Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
            Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;
        }
    }

    private void OnDisable() {
        if (Managers.Game == null) {
            return;
        }
        if (Owner.IsValid() == false) {
            return;
        }
        if (Owner.SkeletonAnim == null) {
            return;
        }
        if (Owner.SkeletonAnim.AnimationState == null) {
            return;
        }

        Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
        Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
    }

    public virtual void DoSkill() {
        // RemainCoolTime = SkillData.CoolTime
    }

    protected abstract void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e);
    protected abstract void OnAnimCompleteHandler(TrackEntry trackEntry);
}
