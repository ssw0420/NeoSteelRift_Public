using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SSW.Monster;

namespace SSW.BossMonster
{
    public class BossAnimatorController : MonoBehaviour
    {
        private BTBlackboard _blackboard;
        private NavMeshAgent _agent;
        private RobotAnimator _animator;
        [SerializeField] private float _turnLerpSpeed = 5f;
        private float _lastTurnSpeed = 0f;


        public void Initialize(BTBlackboard blackboard)
        {
            _blackboard = blackboard;
            _agent = GetComponent<NavMeshAgent>();
            _animator = _blackboard.RobotAnimator;
        }

        void Update()
        {
            if (_blackboard == null) return;
            if (_agent == null) return;
            if (_animator == null) return;

            // 이동 속도 계산
            float speed = _agent.velocity.magnitude;
            float normalizedSpeed = speed / _agent.speed;
            _animator.SetSpeed(normalizedSpeed);

            // 회전량 계산
            Vector3 desiredVelocity = _agent.desiredVelocity;

            if (desiredVelocity.sqrMagnitude > 0.01f)
            {
                float angle = Vector3.SignedAngle(transform.forward, desiredVelocity, Vector3.up);
                float normalizedTurn = Mathf.Clamp(angle / 90f, -1f, 1f);
                _lastTurnSpeed = Mathf.Lerp(_lastTurnSpeed, normalizedTurn, Time.deltaTime * _turnLerpSpeed);
                _animator.SetTurns(_lastTurnSpeed);
                Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime
                );
            }
            else
            {
                _animator.SetTurns(0f);
            }
        }
        
    }
}