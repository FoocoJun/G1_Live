using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using static Define;

public class NormalAttack : SkillBase {
    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }
        return true;
    }

    public override void SetInfo(Creature owner, int skillTemlateID) {
        base.SetInfo(owner, skillTemlateID);
    }

    public override void DoSkill()
    {
        base.DoSkill();

        Owner.CreatureState = ECreatureState.Skill;
        Owner.PlayAnimation(0, AnimName.ATTACK_A, true);

        Owner.LookAtTarget(Owner.Target);
    }

    protected override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e) {
        if (e.ToString().Contains(SkillData.AnimName)) {
            OnAttackEvent();
        }
    }

    private void OnAttackEvent() {
       if (Owner.Target.IsValid() == false) {
        return;
       }
       if (SkillData.ProjectileId == 0) {
        // Melee
        Owner.Target.OnDamaged(Owner, this);
       } else {
        // Ranged
        // GenerateProjectile(Owner, Owner.CenterPosition);
       }
    }

    protected override void OnAnimCompleteHandler(TrackEntry trackEntry) {
        if (Owner.Target.IsValid() == false) {
            return;
        }
        if (Owner.CreatureState == ECreatureState.Skill) {
            Owner.CreatureState = ECreatureState.Move;
        }
    }

}
