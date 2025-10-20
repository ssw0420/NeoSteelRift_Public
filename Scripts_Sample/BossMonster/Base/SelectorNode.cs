using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public class SelectorNode : BTNode
    {
        private readonly List<BTNode> _children;
        private int _currentIndex = 0;

        public SelectorNode(BTBlackboard blackboard, List<BTNode> children) : base(blackboard)
        {
            _children = children;
        }

        public override NodeState Evaluate()
        {
            if (_children == null || _children.Count == 0)
            {
                return _state = NodeState.Failure;
            }

            NodeState result = _children[_currentIndex].Tick();

            switch (result)
            {
                case NodeState.Success:
                    _currentIndex = 0;
                    return _state = NodeState.Success;

                case NodeState.Running:
                    return _state = NodeState.Running;

                case NodeState.Failure:
                    _currentIndex++;
                    if (_currentIndex >= _children.Count)
                    {
                        _currentIndex = 0;
                        return _state = NodeState.Failure;
                    }
                    return _state = NodeState.Running;
            }

            _currentIndex = 0;
            return _state = NodeState.Failure;
        }

        public override void OnAbort()
        {
            base.OnAbort();
            if (_currentIndex < _children.Count)
            {
                _children[_currentIndex].OnAbort();
            }
            _currentIndex = 0;
        }
    }
}