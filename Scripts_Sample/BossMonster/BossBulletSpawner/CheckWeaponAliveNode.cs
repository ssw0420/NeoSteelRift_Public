using System.Collections;
using System.Collections.Generic;
using SSW.BossMonster;
using UnityEngine;

public class CheckWeaponAliveNode : BTNode
{
    public CheckWeaponAliveNode(BTBlackboard blackboard) : base(blackboard)
    {

    }

    public override NodeState Evaluate()
    {
        if (_blackboard.IsDead) return _state = NodeState.Failure;
            bool leftDestroyed = _blackboard.WeaponLeftHP == 0;
            bool rightDestroyed = _blackboard.WeaponRightHP == 0;

            if (leftDestroyed && rightDestroyed)
            {
                return _state = NodeState.Failure;
            }
            return _state = NodeState.Success;
    }
}
