using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;


namespace SSW.BossMonster
{
    public class DefaultAttackNode : BTNode
    {
        // TODO: refactor to use a more generic attack system
        private Timer _attackCooldownTimer;

        public DefaultAttackNode(BTBlackboard blackboard) : base(blackboard)
        {
            _attackCooldownTimer = new Timer(_blackboard.AttackCooldown);
        }

        public override NodeState Evaluate()
        {
            if (_blackboard.Target == null || _blackboard.IsDead)
            {
                _state = NodeState.Failure;
                return _state;
            }

            if (_attackCooldownTimer.IsFinished())
            {
                _attackCooldownTimer = new Timer(_blackboard.AttackCooldown); // Reset cooldown timer
                _state = NodeState.Success;
            }
            else
            {
                _state = NodeState.Running;
            }

            return _state;
        }

        private void Attack()
        {
            _blackboard.RobotAnimator.SetShoot(true);

        }
    }
}