using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature {
    public override ECreatureState CreatureState {
        get { return base.CreatureState; }
        set {
            if (_creatureState != value) {
                base.CreatureState = value;
            }

            switch (value) {
                case ECreatureState.Idle:
                    UpdateAITick = 0.5f;
                    break;
                case ECreatureState.Move:
                    UpdateAITick = 0.0f;
                    break;
                case ECreatureState.Skill:
                    UpdateAITick = 0.0f;
                    break;
                case ECreatureState.Dead:
                    UpdateAITick = 1.0f;
                    break;
            }
        }
    }

    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        CreatureType = ECreatureType.Monster;
        MoveSpeed = 3.0f;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID) {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;

        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData.SkillIdList);
    }

    // Init 당시에는 position이 선언되어 있지 않음
    void Start() {
        _initPos = transform.position;
    }

    #region AI
    Vector3 _destPos;
    Vector3 _initPos;

    protected override void UpdateIdle() {
        // Patrol
        {
            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if (rand <= patrolPercent) {
                _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CreatureState = ECreatureState.Move;
                return;
            }
        }

        // Search Player
        Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Heroes, func: IsValid) as Creature;
        if (creature != null) {
            Target = creature;
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateMove() {
        if (Target == null) {
            // Patrol or Return
            Vector3 dir = _destPos - transform.position;

            // 충분히 Return 했으면
            if (dir.sqrMagnitude <= 0.01f) {
                CreatureState = ECreatureState.Idle;
                return;
            }

            SetRigidBodyVelocity(dir.normalized * MoveSpeed);

        } else {
            // Chase
            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(MONSTER_SEARCH_DISTANCE, skill);

            // 멀어지면 포기
            if (Target.IsValid() == false) {
                Target = null;
                _destPos = _initPos;
                return;
            }
        }
    }

    protected override void UpdateSkill() {
        if (Target.IsValid() == false) {
			Target = null;
			_destPos = _initPos;
			CreatureState = ECreatureState.Move;
			return;
		}
    }

    protected override void UpdateDead() {
        SetRigidBodyVelocity(Vector2.zero);
    }
    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill) {
        base.OnDamaged(attacker, skill);
    }

    public override void OnDead(BaseObject attacker, SkillBase skill) {
        base.OnDead(attacker, skill);

        // TODO : Drop Item

        Managers.Object.Despawn(this);
    }
    #endregion
}