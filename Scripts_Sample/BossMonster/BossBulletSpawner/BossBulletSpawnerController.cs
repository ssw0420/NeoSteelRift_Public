using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Utility;

namespace SSW.BossMonster.BossWeapon
{
    public class BossBulletSpawnerController : MonoBehaviour
    {
        private BTBlackboard _blackboard;

        [SerializeField] private bool _isLeftWeapon;
        [SerializeField] private Color _destroyedBaseColor = new Color(137f / 255f, 0f, 8f / 255f);
        [SerializeField] private Color _destroyedEmissionColor = Color.black;
        [SerializeField] private VisualEffect _destroyedEffect;
        [SerializeField] private VisualEffect _explosionEffect;
        [SerializeField] private Renderer _weaponRenderer;

        private bool _isDestroyed = false;

        public void Initialize(BTBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        private void Update()
        {
            if (_isDestroyed) return;
            int weaponHP = _isLeftWeapon ? _blackboard.WeaponLeftHP : _blackboard.WeaponRightHP;

            if (weaponHP <= 0)
            {
                StartCoroutine(CoDestroyWeapon(0.5f));
            }
        }

        private IEnumerator CoDestroyWeapon(float duration)
        {
            if (_isDestroyed) yield break; // Prevent multiple calls
            _isDestroyed = true;
            
            if (_isLeftWeapon)
            {
                _blackboard.BossAudioController.PlayDestroyLeftSound();
                _blackboard.BossAudioController.PlayBurnLeftSound();
                _blackboard.RobotAnimator.SetHitLeftTrigger();
            }
            else
            {
                _blackboard.BossAudioController.PlayDestroyRightSound();
                _blackboard.BossAudioController.PlayBurnRightSound();
                _blackboard.RobotAnimator.SetHitRightTrigger();
            }

            Material weaponMaterial = _weaponRenderer.material;
            Color startBaseColor = weaponMaterial.GetColor("_BaseColor");
            Color startEmissionColor = weaponMaterial.GetColor("_EmissionColor");

            _destroyedEffect?.SendEvent("create"); // Trigger the visual effect
            _explosionEffect?.SendEvent("OnPlay");

            Timer timer = new Timer(duration);

            while (!timer.IsFinished())
            {
                float t = timer.ProgressRatio();
                Color baseColor = Color.Lerp(startBaseColor, _destroyedBaseColor, t);
                weaponMaterial.SetColor("_BaseColor", baseColor);

                // Update emission color
                Color emissionColor = Color.Lerp(startEmissionColor, _destroyedEmissionColor, t);
                weaponMaterial.SetColor("_EmissionColor", emissionColor);

                yield return null; // Wait for the next frame
            }

            weaponMaterial.SetColor("_BaseColor", _destroyedBaseColor);
            weaponMaterial.SetColor("_EmissionColor", _destroyedEmissionColor);
        }
    }
}
