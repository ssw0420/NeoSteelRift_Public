using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.Monster.States
{
    public class AttackState : MonsterStateBase
    {
        private float _attackTimer;
        private Coroutine _attackCoroutine;
        private float _lineOfSightCheckTimer;
        private const float LineOfSightCheckInterval = 1.0f;

        public AttackState(MonsterControllerRefactored controller) : base(controller, MonsterState.Attack) { }

        public override void OnEnter()
        {
            base.OnEnter();
            controller.Agent.avoidancePriority = 40; // Lower priority to block other monsters
            controller.Agent.isStopped = true;
            _lineOfSightCheckTimer = 0f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_attackCoroutine != null) return;

            if (controller.Target == null || controller.AttackPattern == null)
            {
                controller.ChangeState(MonsterState.Idle);
                return;
            }

            _lineOfSightCheckTimer += Time.deltaTime;
            if (_lineOfSightCheckTimer >= LineOfSightCheckInterval)
            {
                _lineOfSightCheckTimer = 0f;
                if (!HasLineOfSight())
                {
                    controller.ChangeState(MonsterState.Chase);
                    return;
                }
            }

            // if the target is out of attack range, switch to chase state
            float distance = Vector3.Distance(controller.transform.position, controller.Target.position);
            if (distance > controller.AttackRange + 3f)
            {
                controller.ChangeState(MonsterState.Chase);
                return;
            }

            _attackTimer += Time.deltaTime;
            if (_attackTimer >= controller.AttackCooldown || controller.IsReadyForFastAttack)
            {
                _attackTimer = 0f;
                controller.SetIsReadyForFastAttack(false);
                _attackCoroutine = controller.StartCoroutine(CoAttackSequence());
            }
        }

        private IEnumerator CoAttackSequence()
        {
            // yield return controller.MonsterRotationController.StartRotateToTargetCoroutine();
            // yield return controller.AttackPattern.CoExecute(controller);
            // _attackCoroutine = null;
            // controller.ChangeState(MonsterState.Idle);

            yield return controller.StartCoroutine(controller.CoRotateTowardsTarget(0.7f));
            yield return controller.AttackPattern.CoExecute(controller);

            _attackCoroutine = null;
            controller.ChangeState(MonsterState.Idle);
        }


        private bool HasLineOfSight()
        {
            Transform target = controller.Target;
            if (target == null) return false;

            if (controller.RaycastPoints == null || controller.RaycastPoints.RaycastOrigins.Count == 0)
            {
                Debug.LogWarning("AttackRaycastPoints is not assigned or empty in MonsterController.", controller);
                return false;
            }

            Vector3 targetCenterPosition = target.position + Vector3.up * 1.0f;

            int playerLayer = LayerMask.NameToLayer("Player");
            LayerMask combinedMask = controller.ObstacleLayerMask | (1 << playerLayer);

            foreach (var origin in controller.RaycastPoints.RaycastOrigins)
            {
                if (origin == null) continue;
                Vector3 originPoint = origin.position;
                Vector3 direction = (targetCenterPosition - originPoint).normalized;
                float distance = Vector3.Distance(originPoint, targetCenterPosition);

                Debug.DrawRay(originPoint, direction * distance, Color.green, 1f);

                RaycastHit hit;
                if (Physics.Raycast(originPoint, direction, out hit, distance, combinedMask))
                {
                    if (hit.collider.gameObject.layer == playerLayer)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            controller.Agent.avoidancePriority = 40; // Lower priority to block other monsters
            return false;
        }


        public override void OnExit()
        {
            base.OnExit();
            controller.Agent.avoidancePriority = 50; // Reset to default priority
            controller.Animator.SetShoot(false);
            if (_attackCoroutine != null)
            {
                controller.StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
    }
}

