using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

namespace SSW.BossMonster
{
    public class MoveToTargetNode : BTNode
    {
        private NavMeshAgent _agent;
        private Transform _currentTarget;

        public MoveToTargetNode(BTBlackboard blackboard, NavMeshAgent agent, Transform target) : base(blackboard)
        {
            _agent = agent;
            _currentTarget = target;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _agent.isStopped = false;
            _agent.SetDestination(_currentTarget.position);
        }

        public override NodeState Evaluate()
        {
            if (_currentTarget == null || !_agent.enabled || _blackboard.IsDead) return _state = NodeState.Failure;

            if (_agent.pathPending || !_agent.hasPath) return _state = NodeState.Running;

            if (_agent.remainingDistance <= _agent.stoppingDistance + 6f)
            {
                _agent.isStopped = true;
                _blackboard.RobotAnimator.SetSpeed(0f);
                _blackboard.RobotAnimator.SetTurns(0f);
                return _state = NodeState.Success;
            }

            return _state = NodeState.Running;
        }

        protected override void OnExit()
        {
            base.OnExit();
            _agent.isStopped = true;
        }
    }
}