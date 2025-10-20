using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using ScriptableObjects.PlayerDataStatus;


namespace SSW.Monster
{
    public class MonsterBullet : MonoBehaviour
    {

        [Header("Pooling Settings")]
        [SerializeField] private string _poolKey = "Rob01_Bullet";
        [SerializeField] private string _hitEffectPoolKey = "Rob01_Bullet_HitEffect";

        [Header("Bullet Settings")]
        [SerializeField] private float _lifeTime = 4f;
        [SerializeField] private float _timer = 0f;
        private float _attackDamage;


        private void OnEnable()
        {
            StartCoroutine(CoLifeTime());
            _timer = 0f;
        }

        public void Initialize(float damage)
        {
            _attackDamage = damage;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _lifeTime)
            {
                _timer = 0f;
                PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
                //CustomLogger.LogInfo($"Bullet returned to pool: {_poolKey}", this);
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            StopAllCoroutines();

            GameObject hitEffect = PoolManager.Instance.GetFromPool(_hitEffectPoolKey);
            if (hitEffect != null)
            {
                ContactPoint contact = collision.contacts[0];
                hitEffect.transform.position = contact.point;
                hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, contact.normal);
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerDataStatus.Instance.TakeDamage(_attackDamage);
            }

            PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
        }

        private IEnumerator CoLifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
        }
    }
}
