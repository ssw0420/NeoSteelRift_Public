using System.Collections;
using System.Collections.Generic;
using NeoSteelRift.Scripts.Logger;
using UnityEngine;

namespace SSW.BossMonster
{
    public class CheckIfDeadNode : BTNode
    {
        public CheckIfDeadNode(BTBlackboard blackboard) : base(blackboard) { }

        public override NodeState Evaluate()
        {
            CustomLogger.LogInfo("Checking if the boss monster is dead...");
            return _blackboard.IsDead ? NodeState.Failure : NodeState.Success;
        }
    }
}