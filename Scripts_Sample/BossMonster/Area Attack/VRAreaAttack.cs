using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using NeoSteelRift.Scripts.Logger;
using ScriptableObjects.PlayerDataStatus;
using Managers;

namespace SSW.BossMonster
{
    public class VRAreaAttack : MonoBehaviour
    {
        [Header("VFX Settings")]
        [SerializeField] private VisualEffect _areaVFX;
        [SerializeField] private ParticleSystem _explosionVFX;

        [Header("Attack Settings")]
        [SerializeField] private float _delayBeforeExplosion = 1.8f;
        [SerializeField] private float _delayBeforeDamage = 0.1f;
        [SerializeField] private SphereCollider _damageCollider;

        [Header("Boss Reference")]
        [SerializeField] private float _skillDamage = 30.0f;
        private bool _hasDealtDamage = false;

        [Header("Audio Settings")]
        [SerializeField] private AreaAttackAudioController _areaAttackAudioController;

        [Header("Object Pooling")]
        [SerializeField] private string _poolKey = "Boss_AreaAttack";
        public string PoolKey => _poolKey;

        void OnEnable()
        {
            if (_areaVFX == null) return;
            if (_explosionVFX == null) return;
            if (_damageCollider == null) return;

            _hasDealtDamage = false;
            _damageCollider.enabled = false;
            _areaVFX.SendEvent("create");
            StartCoroutine(CoExplosion());
        }

        private IEnumerator CoExplosion()
        {
            yield return new WaitForSeconds(_delayBeforeExplosion);

            _areaVFX.SendEvent("stop");
            _explosionVFX.Play();
            yield return new WaitForSeconds(_delayBeforeDamage);

            _areaAttackAudioController.PlayAreaAttackSound();
            _damageCollider.enabled = true;
            yield return new WaitForSeconds(0.2f);

            _damageCollider.enabled = false;
            yield return new WaitForSeconds(1.1f);

            PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
        }

        void OnTriggerEnter(Collider other)
        {
            if (_hasDealtDamage) return;

            if (other.CompareTag("Player"))
            {
                CustomLogger.LogInfo("Player hit by Area Attack");
                PlayerDataStatus.Instance.TakeDamage(_skillDamage);
                _hasDealtDamage = true;
                _damageCollider.enabled = false;
            }
        }

    }
}