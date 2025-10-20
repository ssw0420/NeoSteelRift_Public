using System.Collections;
using System.Collections.Generic;
using NeoSteelRift.Scripts.Logger;
using UnityEngine;

namespace SSW.BossMonster
{
    public sealed class CheckPlayerDistanceNode : BTNode
    {
        private readonly float _thresholdSqr;
        private readonly ComparisonMode _comparisonMode;
        public CheckPlayerDistanceNode(BTBlackboard blackboard, float threshold, ComparisonMode comparisonMode) : base(blackboard)
        {
            _thresholdSqr = threshold * threshold;
            _comparisonMode = comparisonMode;
        }

        public override NodeState Evaluate()
        {
            if (_blackboard.Target == null)
            {
                return NodeState.Failure;
            }
            _state = NodeState.Running;
            float distanceSqr = (_blackboard.Target.position - _blackboard.transform.position).sqrMagnitude;
            CustomLogger.LogInfo($"Checking player distance: Target position {_blackboard.Target.position}, Boss position {_blackboard.transform.position}, distanceSqr:  {distanceSqr}");
            CustomLogger.LogInfo($"comparsionMode : {_comparisonMode}");
            bool conditionMet = _comparisonMode switch
            {
                ComparisonMode.LessThanOrEqual => distanceSqr <= _thresholdSqr,
                ComparisonMode.GreaterThan => distanceSqr > _thresholdSqr,
                _ => false
            };

            return conditionMet ? NodeState.Success : NodeState.Failure;
        }
    }
}