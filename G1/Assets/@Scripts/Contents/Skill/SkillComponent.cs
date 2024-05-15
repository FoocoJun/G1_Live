using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : InitBase {
    public List<SkillBase> SkillList { get; } = new List<SkillBase>();

    Creature _owner;

    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        return true;
    }

    public void SetInfo(Creature owner, List<int> skillTemplateIDs) {
        _owner = owner;

        foreach (int skillTemlateID in skillTemplateIDs) {
            AddSkill(skillTemlateID);
        }
    }

    private void AddSkill(int skillTemlateID) {
        string className = Managers.Data.SkillDic[skillTemlateID].ClassName;

        SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;

        if (skill == null) {
            return;
        }

        skill.SetInfo(_owner, skillTemlateID);

        SkillList.Add(skill);
    }

    public SkillBase GetReadySkill() {
        // TMP
        return SkillList[0];
    }
}
