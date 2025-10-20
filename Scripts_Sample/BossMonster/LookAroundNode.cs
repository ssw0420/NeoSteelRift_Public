using System.Collections;
using System.Collections.Generic;
using NeoSteelRift.Scripts.Logger;
using UnityEngine;
using UnityEngine.AI;

namespace SSW.BossMonster
{
    public class LookAroundNode : BTNode
    {
        private bool _firstFrame = true;
        private NavMeshAgent _agent;
        public LookAroundNode(BTBlackboard blackboard, NavMeshAgent agent) : base(blackboard)
        {
            _agent = agent;
        }

        protected override void OnEnter()
        {
            _blackboard.RobotAnimator.SetLook1(true);
            _blackboard.RobotAnimator.SetSpeed(0f);
            _blackboard.RobotAnimator.SetTurns(0f);
            _agent.isStopped = true;
            _firstFrame = true;
        }

        public override NodeState Evaluate()
        {
            if (_blackboard.Target == null || _blackboard.IsDead)
                return _state = NodeState.Failure;

            if (_firstFrame)
            {
                _firstFrame = false;
                return _state = NodeState.Running;
            }

            AnimatorStateInfo stateInfo = _blackboard.RobotAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("LookAround"))
                return NodeState.Running;

            return NodeState.Success;
        }

        protected override void OnExit()
        {
            _blackboard.RobotAnimator.SetLook1(false);
            _firstFrame = true;
            _agent.isStopped = false;
        }
    }
}