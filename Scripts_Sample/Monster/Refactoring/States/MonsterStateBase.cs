using UnityEngine;
using NeoSteelRift.Scripts.Logger;

namespace SSW.Monster.States
{
    public abstract class MonsterStateBase
    {
        protected MonsterControllerRefactored controller;
        protected MonsterState stateType;

        public MonsterStateBase(MonsterControllerRefactored controller, MonsterState stateType)
        {
            this.controller = controller;
            this.stateType = stateType;
        }

        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void OnEnter()
        {
            //CustomLogger.LogInfo($"[FSM] Entering {stateType} state", controller);
        }

        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        public virtual void OnUpdate()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called when exiting this state
        /// </summary>
        public virtual void OnExit()
        {
            //CustomLogger.LogInfo($"[FSM] Exiting {stateType} state", controller);
        }

        /// <summary>
        /// Check if state can transition to another state
        /// </summary>
        public virtual bool CanTransitionTo(MonsterState targetState)
        {
            return true;
        }
    }
}
