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
    }

    // Init 당시에는 position이 선언되어 있지 않음
    void Start() {
        _initPos = transform.position;
    }

    #region AI
    public float SearchDistance { get; private set; } = 8.0f;
    public float AttacnDistance { get; private set; } = 4.0f;
    Creature _target;
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
        {
            Creature target = null;
            float bestDistanceSqr = float.MaxValue;
            // 제곱연산인 이유 - 현재는 크기 비교만 하면 되기 때문. (루트연산 비용이 훨 크기 때문)
            float searchDistanceSqr = SearchDistance * SearchDistance;

            foreach (Hero hero in Managers.Object.Heroes) {
                Vector3 dir = hero.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                Debug.Log(distToTargetSqr);

                if (distToTargetSqr > searchDistanceSqr) {
                    continue;
                }

                if (distToTargetSqr > bestDistanceSqr) {
                    continue;
                }

                target = hero;
                bestDistanceSqr = distToTargetSqr;
            }

            _target = target;

            if (_target != null) {
                CreatureState = ECreatureState.Move;
            }
        }
    }

    protected override void UpdateMove() {
        if (_target == null) {
            // Patrol or Return
            Vector3 dir = _destPos - transform.position;

            // 충분히 Return 했으면
            if (dir.sqrMagnitude <= 0.01f) {
                CreatureState = ECreatureState.Idle;
                return;
            }

            SetRigidBodyVelocity(dir.normalized * MoveSpeed);

        } else {
            Vector3 dir = _target.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;
            float attackDistanceSqr = AttacnDistance * AttacnDistance;

            if (distToTargetSqr < attackDistanceSqr) {
                // 공격범위 이내
                CreatureState = ECreatureState.Skill;
                StartWait(2.0f);
            } else {
                // 공격범위 밖
                SetRigidBodyVelocity(dir.normalized * MoveSpeed);

                // 너무 멀어지면 포기
                float searchDistanceSqr = SearchDistance * SearchDistance;
                if (distToTargetSqr > searchDistanceSqr) {
                    _destPos = _initPos;
                    _target = null;
                    CreatureState = ECreatureState.Move;
                }
            }
        }
    }

    protected override void UpdateSkill() {
        if (_coWait != null) {
            return;
        }

        CreatureState = ECreatureState.Move;
    }

    protected override void UpdateDead() {
    }
    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker) {
        base.OnDamaged(attacker);
    }

    public override void OnDead(BaseObject attacker) {
        base.OnDead(attacker);

        // TODO : Drop Item

        Managers.Object.Despawn(this);
    }
    #endregion
}