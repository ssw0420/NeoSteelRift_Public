using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Managers;

namespace SSW.BossMonster
{
    public class RepeatSpawnAreaAttackNode : BTNode
    {
        private readonly Timer _durationTimer;
        private readonly Timer _spawnTimer;

        private GameObject _areaAttackPrefab;

        public RepeatSpawnAreaAttackNode(BTBlackboard blackboard, GameObject areaAttackPrefab) : base(blackboard)
        {
            _areaAttackPrefab = areaAttackPrefab;
            _durationTimer = new Timer(14f);
            _spawnTimer = new Timer(3f);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _durationTimer.Reset();
            _spawnTimer.Reset();
        }

        public override NodeState Evaluate()
        {
            if (_blackboard.Target == null) return _state = NodeState.Failure;

            if (_spawnTimer.IsFinished())
            {
                SpawnAttack();
                _spawnTimer.Reset();
            }

            if (_durationTimer.IsFinished())
            {
                return _state = NodeState.Success;
            }

            return _state = NodeState.Running;
        }

        private void SpawnAttack()
        {
            Vector3 spawnPosition = _blackboard.Target.position;
            string key = _areaAttackPrefab.GetComponent<AreaAttack>().PoolKey;
            GameObject attackObject = PoolManager.Instance.GetFromPool(key);

            if (attackObject != null)
            {
                attackObject.transform.position = spawnPosition;
                attackObject.transform.rotation = Quaternion.identity;
            }
        }
    }
}