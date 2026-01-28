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
        private float _attackDamage;
        private Coroutine _lifeTimeCoroutine;

        private void OnEnable()
        {
            _lifeTimeCoroutine = StartCoroutine(CoLifeTime());
        }

        private void OnDisable()
        {
            // 풀로 반환될 때 코루틴 정리 (안전성)
            if (_lifeTimeCoroutine != null)
            {
                StopCoroutine(_lifeTimeCoroutine);
                _lifeTimeCoroutine = null;
            }
        }

        public void Initialize(float damage)
        {
            _attackDamage = damage;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 충돌 시 lifetime 코루틴 중지
            if (_lifeTimeCoroutine != null)
            {
                StopCoroutine(_lifeTimeCoroutine);
                _lifeTimeCoroutine = null;
            }

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
