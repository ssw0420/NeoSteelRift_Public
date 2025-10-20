using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SSW.Monster;


namespace SSW.BossMonster
{

    public class RotateTowardsTargetNode : BTNode
    {
        private NavMeshAgent _agent;
        private Transform _transform;
        private RobotAnimator _animator;
        private float _rotationSpeed;
        private float _thresholdAngle;

        public RotateTowardsTargetNode(BTBlackboard blackboard, NavMeshAgent agent, float rotationSpeed = 10f, float thresholdAngle = 5f) : base(blackboard)
        {
            _agent = agent;
            _transform = blackboard.transform;
            _animator = blackboard.RobotAnimator;
            _rotationSpeed = rotationSpeed;
            _thresholdAngle = thresholdAngle;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            if (_agent != null)
            {
                _agent.isStopped = true;
            }
        }

        public override NodeState Evaluate()
        {
            if (_blackboard.Target == null || _agent == null || !_agent.enabled || _blackboard.IsDead) return _state = NodeState.Failure;

            Vector3 directionToTarget = _blackboard.Target.position - _transform.position;
            directionToTarget.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            _animator.SetSpeed(0.1f); 

            if (Vector3.Angle(_transform.forward, directionToTarget) < _thresholdAngle)
            {
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        protected override void OnExit()
        {
            base.OnExit();
            _animator.SetSpeed(0f);
        }

    }
}
