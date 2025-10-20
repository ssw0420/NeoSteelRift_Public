using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;


namespace SSW.BossMonster
{
    public enum NodeState
    {
        Success,
        Failure,
        Running
    }

    public abstract class BTNode
    {
        protected NodeState _state = NodeState.Running; // Default state
        protected BTBlackboard _blackboard;

        private bool _entered = false;
        public NodeState State => _state;
        public BTNode(BTBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        public NodeState Tick()
        {
            if (!_entered)
            {
                _entered = true;
                _state = NodeState.Running;
                OnEnter();
            }

            _state = Evaluate();

            if (_state != NodeState.Running)
            {
                OnExit();
                _entered = false;
            }

            return _state;
        }

        public abstract NodeState Evaluate();
        protected virtual void OnEnter()
        {
            //CustomLogger.LogInfo($"{GetType().Name} entered.");
        }
        protected virtual void OnExit()
        {
            //CustomLogger.LogInfo($"{GetType().Name} exited with state: {_state}.");
        }

        public virtual void OnAbort()
        {
            if (_entered)
            {
                OnExit();
                _entered = false;
                _state = NodeState.Failure;
            }
        }
    }
}
