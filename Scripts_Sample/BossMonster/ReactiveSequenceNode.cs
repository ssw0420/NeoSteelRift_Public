using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SSW.BossMonster
{
    public class ReactiveSequenceNode : BTNode
    {
        private readonly List<BTNode> _children;

        public ReactiveSequenceNode(BTBlackboard blackboard, List<BTNode> children) : base(blackboard)
        {
            _children = children;
        }

        public override NodeState Evaluate()
        {
            if (_children == null || _children.Count == 0)
            {
                return _state = NodeState.Success;
            }

            foreach (BTNode child in _children)
            {
                NodeState childState = child.Tick();

                if (childState == NodeState.Failure)
                {
                    return _state = NodeState.Failure;
                }
                
                if (childState == NodeState.Running)
                {
                    return _state = NodeState.Running;
                }
            }

            return _state = NodeState.Success;
        }
    }
}