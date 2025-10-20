using System;
using System.Collections;
using System.Collections.Generic;
using SSW.Monster;
using TMPro;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;
using NeoSteelRift.Scripts.Logger;
using UnityEngine.AI;
using Utility;
using Managers;

namespace SSW.BossMonster
{
    public class DeathHandler : MonoBehaviour
    {
        private BTBlackboard _blackboard;
        private RobotAnimator _robotAnimator;
        private NavMeshAgent _agent;
        private bool _hasStartedDeathSequence = false;
        public event Action OnBossDied;
        [SerializeField] private VisualEffect[] _deathEffects;
        [SerializeField] private VisualEffect _destroyedEffect;
        [SerializeField] private Color _destroyedBaseColor = new Color(137f / 255f, 0f, 8f / 255f);
        [SerializeField] private Color _destroyedEmissionColor = Color.black;
        [SerializeField] private Renderer _bodyRenderer;

        void Awake()
        {
            _blackboard = GetComponent<BTBlackboard>();
            if (_blackboard == null) return;

            _robotAnimator = _blackboard.RobotAnimator;
            _agent = GetComponent<NavMeshAgent>();
        }

        public void TriggerDeath()
        {
            if (_blackboard.IsDead) return;
            //_agent.enabled = false;
            _robotAnimator.SetShoot(false);
            _robotAnimator.SetHitLeft(false);
            _robotAnimator.SetHitRight(false);
            _robotAnimator.SetTurnLeft(false);
            _robotAnimator.SetTurnRight(false);
            _robotAnimator.SetLook1(false);
            _robotAnimator.SetSpeed(0f);
            _robotAnimator.SetTurns(0f);
            
            _blackboard.IsDead = true;
        }

        void Update()
        {
            if (!_blackboard.IsDead || _hasStartedDeathSequence) return;

            _robotAnimator.SetShoot(false);
            _robotAnimator.SetHitLeft(false);
            _robotAnimator.SetHitRight(false);
            _robotAnimator.SetTurnLeft(false);
            _robotAnimator.SetTurnRight(false);
            _robotAnimator.SetLook1(false);
            _robotAnimator.SetSpeed(0f);
            _robotAnimator.SetTurns(0f);

            AnimatorStateInfo stateInfo = _blackboard.RobotAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle_walking"))
            {
                _hasStartedDeathSequence = true;
                _robotAnimator.SetDie(true);
                
                StartCoroutine(CoDestroyBody());
                StartCoroutine(CoDie());
            }

        }

        private IEnumerator CoDestroyBody()
        {
            Material bodyMaterial = _bodyRenderer.material;
            Color startBaseColor = bodyMaterial.GetColor("_BaseColor");
            Color startEmissionColor = bodyMaterial.GetColor("_EmissionColor");

            _destroyedEffect?.SendEvent("create");

            Timer timer = new Timer(0.9f);
            while (!timer.IsFinished())
            {
                float t = timer.ProgressRatio();
                Color baseColor = Color.Lerp(startBaseColor, _destroyedBaseColor, t);
                bodyMaterial.SetColor("_BaseColor", baseColor);

                Color emissionColor = Color.Lerp(startEmissionColor, _destroyedEmissionColor, t);
                bodyMaterial.SetColor("_EmissionColor", emissionColor);
                yield return null;
            }

            bodyMaterial.SetColor("_BaseColor", _destroyedBaseColor);
            bodyMaterial.SetColor("_EmissionColor", _destroyedEmissionColor);
        }

        private IEnumerator CoDie()
        {
            foreach (var vfx in _deathEffects)
            {
                vfx.SendEvent("OnPlay");
                _blackboard.BossAudioController.PlayDeathSound();
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(5f);
            OnBossDied?.Invoke();
            GameManager.Instance.ChangeGameState(GameManager.GameState.Victory);
            _blackboard.BossAudioController.StopBurnSounds();
            Destroy(gameObject);
        }
    }
}