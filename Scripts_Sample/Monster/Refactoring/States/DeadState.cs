using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.VFX;

namespace SSW.Monster.States
{
    /// <summary>
    /// Dead state where the monster is dead
    /// </summary>
    public class DeadState : MonsterStateBase
    {
        private Coroutine _deadCoroutine;
        public DeadState(MonsterControllerRefactored controller) : base(controller, MonsterState.Dead)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            controller.SetAgentLayer();

            controller.StopAllCoroutines();
            controller.Animator.SetShoot(false);
            controller.Animator.SetHitLeft(false);
            controller.Animator.SetHitRight(false);
            controller.Animator.SetTurnLeft(false);
            controller.Animator.SetTurnRight(false);
            controller.Animator.SetSpeed(0f);
            controller.Animator.SetTurns(0f);

            _deadCoroutine = controller.StartCoroutine(CoDeadSequence());
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private IEnumerator CoDeadSequence()
        {
            controller.Animator.SetDie(true);
            controller.RendererController.SetVfxRendererActive(true);

            VisualEffect deathVFX = controller.DieEffect;
            
            if (deathVFX != null)
            {
                deathVFX.gameObject.SetActive(true);
                deathVFX.SendEvent("in");
            }
            controller.UniMaterialize.Start_in();

            foreach (Collider collider in controller.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            // Dead Visual Effects
            yield return new WaitForSeconds(1.5f);
            controller.RendererController.SetBodyRendererActive(false);
            yield return new WaitForSeconds(0.2f);
            deathVFX?.SendEvent("out");
            controller.UniMaterialize.Start_out();
            yield return new WaitForSeconds(1.5f);
            controller.RendererController.SetVfxRendererActive(false);

            if (deathVFX != null)
            {
                PoolManager.Instance.ReturnToPool(deathVFX.gameObject, "Rob01_DeadEffect");
            }

            PoolManager.Instance.ReturnToPool(controller.gameObject, controller.MonsterKey);
        }
    }
}
