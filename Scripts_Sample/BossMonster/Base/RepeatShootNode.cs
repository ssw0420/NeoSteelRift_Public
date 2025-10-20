using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using Utility;

namespace SSW.BossMonster
{
    public class RepeatShootNode : BTNode
    {
        private Timer _shootTimer;
        private readonly float _shootInterval = 0.3f;
        private NavMeshAgent _navMeshAgent;
        private string _poolKey = "Boss_BulletAttack";

        public RepeatShootNode(BTBlackboard blackboard, NavMeshAgent agent) : base(blackboard)
        {
            //_navMeshAgent = agent;
            _shootTimer = new Timer(_shootInterval);
        }

        protected override void OnEnter()
        {
            _shootTimer = new Timer(_shootInterval);
            //_navMeshAgent.SetDestination(_blackboard.Target.position);
        }

        public override NodeState Evaluate()
        {
            if (_blackboard.Target == null || _blackboard.IsDead)
            {
                return _state = NodeState.Failure;
            }

            if (_shootTimer.IsFinished())
            {
                Shoot();
                _shootTimer.Reset();
            }

            return _state = NodeState.Running;
        }

        private void Shoot()
        {
            // if the left arm is alive, fire from the left bullet spawn point
            if (_blackboard.WeaponLeftHP > 0 && _blackboard.LeftBulletSpawnPoint != null)
            {
                _blackboard.BossAudioController.PlayAttackLeftSound();
                FireBullet(_blackboard.LeftBulletSpawnPoint);
            }

            // if the right arm is alive, fire from the right bullet spawn point
            if (_blackboard.WeaponRightHP > 0 && _blackboard.RightBulletSpawnPoint != null)
            {
                _blackboard.BossAudioController.PlayAttackRightSound();
                FireBullet(_blackboard.RightBulletSpawnPoint);
            }
        }

        private void FireBullet(Transform spawnPoint)
        {
            GameObject bullet = PoolManager.Instance.GetFromPool(_poolKey);
            if (bullet != null)
            {
                bullet.transform.position = spawnPoint.position;
                bullet.transform.rotation = spawnPoint.rotation;

                if (bullet.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.velocity = spawnPoint.forward * _blackboard.BulletSpeed;
                }
            }
        }
    }
}