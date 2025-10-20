using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public class SequenceNode : BTNode
    {
        private readonly List<BTNode> _children;
        private int _currentIndex = 0;

        public SequenceNode(BTBlackboard blackboard, List<BTNode> children) : base(blackboard)
        {
            _children = children;
        }

        public override NodeState Evaluate()
        {
            if (_children == null || _children.Count == 0)
            {
                return _state = NodeState.Success;
            }

            NodeState childState = _children[_currentIndex].Tick();

            switch (childState)
            {
                case NodeState.Success:
                    _currentIndex++;
                    if (_currentIndex >= _children.Count)
                    {
                        _currentIndex = 0;
                        return _state = NodeState.Success;
                    }
                    return _state = NodeState.Running;

                case NodeState.Running:
                    return _state = NodeState.Running;

                case NodeState.Failure:
                    _currentIndex = 0;
                    return _state = NodeState.Failure;
            }

            _currentIndex = 0;
            return _state = NodeState.Failure;
        }

        public override void OnAbort()
        {
            base.OnAbort();
            for (int i = _currentIndex; i < _children.Count; i++)
            {
                _children[i].OnAbort();
            }
            _currentIndex = 0;
            _state = NodeState.Failure;
        }
    }
}