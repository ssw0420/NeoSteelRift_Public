using System.Collections;
using UnityEngine;

namespace SSW.Monster.States
{
    public class SpawnState : MonsterStateBase
    {
        private Coroutine _spawnCoroutine;

        public SpawnState(MonsterControllerRefactored controller) : base(controller, MonsterState.Spawn)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
     
            controller.SetAgentLayer();
            _spawnCoroutine = controller.StartCoroutine(SpawnSequence());
        }

        public override void OnUpdate()
        {
            // do not move while spawning
            controller.Animator.SetSpeed(0f);
            controller.Animator.SetTurns(0f);
            controller.Agent.isStopped = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            controller.Agent.isStopped = false;
            
            if (_spawnCoroutine != null)
            {
                controller.StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnSequence()
        {
            // start spawn effect
            if (controller.SpawnEffect != null)
            {
                controller.SpawnEffect.Reinit();
                controller.SpawnEffect.SendEvent("in-top");
            }

            // play spawn sound
            controller.MonsterAudioController?.PlaySpawnSound();

            // set invincible and disable renderers
            controller.SetIsInvincible(true);
            controller.RendererController.SetAllRenderersActive(false);

            // wait for spawn delay
            yield return new WaitForSeconds(controller.SpawnDelay);

            // spawn complete
            controller.SetIsInvincible(false);
            controller.RendererController.SetBodyRendererActive(true);
            controller.SetIsSpawned(true);

            // switch to Idle state
            controller.ChangeState(MonsterState.Idle);
        }
    }
}
