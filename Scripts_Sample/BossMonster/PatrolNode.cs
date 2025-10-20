using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

namespace SSW.BossMonster
{
    public class PatrolNode : BTNode
    {
        private NavMeshAgent _agent;
        private Transform[] _patrolPoints;
        private Transform _currentTarget;

        public PatrolNode(BTBlackboard blackboard, NavMeshAgent agent, Transform[] patrolPoints) : base(blackboard)
        {
            _agent = agent;
            _patrolPoints = patrolPoints;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            if (_patrolPoints == null || _patrolPoints.Length == 0 || !_agent.enabled)
            {
                _state = NodeState.Failure;
                return;
            }

            _currentTarget = _patrolPoints[_blackboard.CurrentPatrolIndex];
            _agent.SetDestination(_currentTarget.position);
            _agent.isStopped = false;
        }

        public override NodeState Evaluate()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0 || !_agent.enabled || _blackboard.IsDead)
                return NodeState.Failure;

            if (_agent.pathPending || !_agent.hasPath)
                return NodeState.Running;

            if (_agent.remainingDistance <= _agent.stoppingDistance + 0.05f)
            {
                _agent.isStopped = true;
                _blackboard.RobotAnimator.SetSpeed(0f);
                _blackboard.RobotAnimator.SetTurns(0f);
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        protected override void OnExit()
        {
            base.OnExit();
            _agent.isStopped = true;
            _blackboard.SetNextPatrolPoint();
        }
    }
}