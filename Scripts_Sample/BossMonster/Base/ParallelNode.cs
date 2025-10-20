using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public class ParallelNode : BTNode
    {
        private readonly List<BTNode> _children;

        public ParallelNode(BTBlackboard blackboard, List<BTNode> children) : base(blackboard)
        {
            _children = children;
        }

        public override NodeState Evaluate()
        {
            bool anyRunning = false;
            bool anySuccess = false;

            foreach (BTNode child in _children)
            {
                NodeState result = child.Tick();

                if (result == NodeState.Failure)
                {
                    return _state = NodeState.Failure;
                }

                if (result == NodeState.Success)
                {
                    anySuccess = true;
                }

                if (result == NodeState.Running)
                {
                    anyRunning = true;
                }
            }

            if (anySuccess)
            {
                return _state = NodeState.Success;
            }

            return _state = anyRunning ? NodeState.Running : NodeState.Failure;
        }

        public override void OnAbort()
        {
            foreach (BTNode child in _children)
            {
                child.OnAbort();
            }
            _state = NodeState.Failure;
            base.OnAbort();
        }
    }
}