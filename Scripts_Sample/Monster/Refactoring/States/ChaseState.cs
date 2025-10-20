using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace SSW.Monster.States
{
    /// <summary>
    /// Chase state where the monster pursues the target
    /// </summary>

    public class ChaseState : MonsterStateBase
    {
        private float _lastTurnSpeed;
        private float _turnSpeed;
        private float _lineOfSightCheckTimer;
        private bool _isStopping;
        private const float LineOfSightCheckInterval = 0.3f;
        private const float SpeedThreshold = 0.01f;
        public ChaseState(MonsterControllerRefactored controller) : base(controller, MonsterState.Chase)
        {
            _lastTurnSpeed = controller.LastTurnSpeed;
            _turnSpeed = controller.TurnSpeed;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            controller.SetAgentLayer();
            controller.Agent.isStopped = false;
            controller.Agent.stoppingDistance = 0f;
            _lineOfSightCheckTimer = 0f;
            _isStopping = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (controller.Target == null)
            {
                controller.ChangeState(MonsterState.Idle);
                return;
            }

            if (_isStopping)
            {
                if (controller.Animator.GetSpeed() <= SpeedThreshold)
                {
                    controller.ChangeState(MonsterState.Attack);
                }
                else
                {
                    controller.Animator.SetSpeed(0f);
                    controller.Animator.SetTurns(0f);
                }
                return;
            }
            
            Vector3 targetPosition = controller.Target.position;
            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(targetPosition, out navMeshHit, 1.0f, NavMesh.AllAreas))
            {
                controller.Agent.SetDestination(navMeshHit.position);
            }
            else
            {
                controller.Agent.SetDestination(targetPosition);
            }

            float sqrDistToTarget = (controller.transform.position - controller.Target.position).sqrMagnitude;
            if (sqrDistToTarget <= controller.AttackRange * controller.AttackRange)
            {
                _lineOfSightCheckTimer += Time.deltaTime;
                if (_lineOfSightCheckTimer >= LineOfSightCheckInterval)
                {
                    _lineOfSightCheckTimer = 0f;
                    if (HasLineOfSight())
                    {
                        controller.Agent.isStopped = true;
                        controller.Agent.ResetPath();
                        controller.Animator.SetSpeed(0f);
                        controller.Animator.SetTurns(0f);
                        controller.SetIsReadyForFastAttack(true);
                        _isStopping = true;
                        return;
                    }
                }
            }

            UpdateMovementAndAnimation();
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

                controller.Agent.avoidancePriority = 35;
                return false;
        }
        
        /// <summary>
        /// Update movement and animation parameters based on NavMeshAgent's velocity and desired velocity.
        /// </summary>
        private void UpdateMovementAndAnimation()
        {
            // Update speed parameter based on actual movement speed
            float maxSpeed = controller.Agent.speed;
            float currentSpeed = controller.Agent.velocity.magnitude;
            float normalizedSpeed = (maxSpeed > 0) ? currentSpeed / maxSpeed : 0;
            
            if (normalizedSpeed < SpeedThreshold)
            {
                normalizedSpeed = 0f;
            }
            controller.Animator.SetSpeed(normalizedSpeed);

            Vector3 desiredVelocity = controller.Agent.desiredVelocity;

            // Only process rotation and turns animation if there is movement
            if (desiredVelocity.sqrMagnitude > 0.01f)
            {
                // Turns animation and rotation
                float angle = Vector3.SignedAngle(controller.transform.forward, desiredVelocity, Vector3.up);
                float normalizedTurn = Mathf.Clamp(angle / 90f, -1f, 1f);

                // Smoothly interpolate turn speed for animation
                _lastTurnSpeed = Mathf.Lerp(_lastTurnSpeed, normalizedTurn, Time.deltaTime * _turnSpeed);
                controller.Animator.SetTurns(_lastTurnSpeed);

                // Actual monster rotation
                Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity);
                controller.transform.rotation = Quaternion.Slerp(
                    controller.transform.rotation,
                    targetRotation,
                    Time.deltaTime * _turnSpeed
                );
            }
            else
            {
                controller.Animator.SetTurns(0f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (controller.Agent.isOnNavMesh)
            {
                controller.Agent.isStopped = true;
                controller.Agent.ResetPath();
            }
            controller.Animator.SetSpeed(0f);
            controller.Animator.SetTurns(0f);
            controller.Agent.stoppingDistance = controller.AttackRange > 0.5f ? controller.AttackRange - 0.5f : 0f;
        }
    }
}