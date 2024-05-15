using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
	// 구조로서의 최상위
    #region CreatureData
	[Serializable]
	public class CreatureData
	{
		public int DataId;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float ColliderOffsetX;
		public float ColliderOffstY;
		public float ColliderRadius;
		public float Mass;
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
		public string IconImage;
		public string SkeletonDataID;
		public string AnimatorName;
		public List<int> SkillIdList = new List<int>();
		public int DropItemId;
    }
    #endregion

	// CreatureData를 상속받은 몬스터
	#region MonsterData
	[Serializable]
	public class MonsterData : CreatureData {
	}

	public class MonsterDataLoader : ILoader<int, MonsterData> {
        public List<MonsterData> monsters = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict() {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters) {
                dict.Add(monster.DataId, monster);
            }
            return dict;
        }
    }
	#endregion

	// CreatureData를 상속받은 Hero
	#region HeroData
	[Serializable]
	public class HeroData : CreatureData {

	}

	public class HeroDataLoader : ILoader<int, HeroData> {
        public List<HeroData> heros = new List<HeroData>();
        public Dictionary<int, HeroData> MakeDict() {
            Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
            foreach (HeroData hero in heros) {
                dict.Add(hero.DataId, hero);
            }
            return dict;
        }
    }
	#endregion

    #region EnvData
	[Serializable]
	public class EnvData
	{
		public int DataId;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float MaxHp;
		public int ResourceAmount;
		public float RegenTime;
		public List<String> SkeletonDataIDs = new List<String>();
		public int DropItemId;
	}

	[Serializable]
	public class EnvDataLoader : ILoader<int, EnvData>
	{
		public List<EnvData> envs = new List<EnvData>();
		public Dictionary<int, EnvData> MakeDict()
		{
			Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
			foreach (EnvData env in envs)
				dict.Add(env.DataId, env);
			return dict;
		}
	}
	#endregion
}
