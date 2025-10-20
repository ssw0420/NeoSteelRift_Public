using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;
using SSW.Test;
using SSW.Monster;
using ScriptableObjects.PlayerDataStatus;
using Managers;
using SSW.BossMonster;
using SSW.HitSystem;

namespace SSW.Player
{
    public class PlayerBullet : MonoBehaviour
    {
        Rigidbody _rb;
        SphereCollider _sphereCollider;

        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _knockbackForce = 7f;

        [Header("Effect")]
        [SerializeField] private string _hitEffectPoolKey;

        [Header("Bullet Type")]
        [SerializeField] private string _attackType = "Normal";

        [Header("Pool Settings")]
        [SerializeField] private string _poolKey;
        private float _timer = 0f;

        [Header("Audio")]
        [SerializeField] private PlayerPistolAudioController _pistolAudioController;
        private bool _hasDealtDamage = false;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _sphereCollider = GetComponent<SphereCollider>();
        }

        void OnEnable()
        {
            _rb.isKinematic = false;
            _sphereCollider.enabled = true;
            _timer = 0f;
            _hasDealtDamage = false;
        }

        void OnDisable()
        {
            _rb.isKinematic = true;
            _sphereCollider.enabled = false;
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _lifeTime)
            {
                _timer = 0f;
                PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
            }
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (_hasDealtDamage) return; // Prevent multiple damage applications
            _hasDealtDamage = true;
            Vector3 collisionPoint = collision.contacts[0].point;
            Vector3 normal = collision.contacts[0].normal;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);

            _pistolAudioController.PlayBulletImpactSound();

            IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();
            GameObject hitVFX = PoolManager.Instance.GetFromPool(_hitEffectPoolKey);

            if (hitVFX != null)
            {
                // Use Original scale as the default scale when Object Pooling
                // Do not use Vector3.one as the default scale when Object Pooling
                if (damageable != null)
                {
                    // Logic for hitting an ENEMY
                    // If it's an enemy, calculate and apply a dynamic scale.
                    Transform hitTargetTransform = collision.collider.transform;
                    float averageScale = (hitTargetTransform.lossyScale.x + hitTargetTransform.lossyScale.y + hitTargetTransform.lossyScale.z) / 3f;
                    float scaleMultiplier = Mathf.Clamp(averageScale, 1f, 3f);
                    hitVFX.transform.localScale *= scaleMultiplier;
                }

                hitVFX.transform.position = collisionPoint;
                hitVFX.transform.rotation = rot;
            }
            
            if (damageable != null)
            {
                _pistolAudioController.PlayBulletMetalHitSound();

                // If general Monster get the hit -> hitbox = null
                BossHitBoxType? hitPart = null;
                BossHitBox bossHitBox = collision.collider.GetComponent<BossHitBox>();
                if (bossHitBox != null)
                {
                    hitPart = bossHitBox.partType;
                }

                HitData hitData = new HitData(
                    damage: PlayerDataStatus.Instance.GetPlayerDamage(),
                    knockbackForce: _knockbackForce,
                    knockbackDirection: transform.forward,
                    hitPoint: collisionPoint,
                    attackType: _attackType,
                    partType: hitPart
                );

                damageable.OnHit(hitData);
            }

            PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
        }
    }
}
