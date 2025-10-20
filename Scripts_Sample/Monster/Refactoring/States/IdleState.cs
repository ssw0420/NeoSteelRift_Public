using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SSW.Monster;
using Unity.VisualScripting;

namespace SSW.Monster.States
{
    public class IdleState : MonsterStateBase
    {
        private Coroutine _idleCoroutine;
        public IdleState(MonsterControllerRefactored controller) : base(controller, MonsterState.Idle)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            controller.SetAgentLayer();
            controller.Agent.isStopped = true;
            _idleCoroutine = controller.StartCoroutine(CoRotateAndDecideNextAction());
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

        }

        private IEnumerator CoRotateAndDecideNextAction()
        {
            if (controller.IsReadyForFastAttack)
            {
                yield return controller.StartCoroutine(controller.CoRotateTowardsTarget(0.3f));
                controller.SetIsFacedToTarget(true);
            }
            float sqrDistanceToPlayer = (controller.transform.position - controller.Target.position).sqrMagnitude;
            if (sqrDistanceToPlayer <= controller.AttackRange * controller.AttackRange)
            {
                controller.ChangeState(MonsterState.Attack);
            }
            else
            {
                controller.ChangeState(MonsterState.Chase);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_idleCoroutine != null)
            {
                controller.StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
            }
        }
    }
}