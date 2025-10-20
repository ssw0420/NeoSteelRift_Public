using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using ScriptableObjects.PlayerDataStatus;

public class BossBullet : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _lifeTime = 5.0f;
    [SerializeField] private float _attackDamage = 5.0f;

    [Header("Effects")]
    [SerializeField] private GameObject HitSplash;

    [Header("Pooling Settings")]
    [SerializeField] private string _poolKey = "Boss_BulletAttack";
    [SerializeField] private string _hitEffectPoolKey = "Boss_BulletAttack_HitEffect";
    public string PoolKey => _poolKey;
    private void OnEnable()
    {
        StartCoroutine(ReturnToPoolAfterLifetime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();

        if (HitSplash != null)
        {
            GameObject splashVFX = PoolManager.Instance.GetFromPool(_hitEffectPoolKey);
            if (splashVFX != null)
            {
                ContactPoint contact = collision.contacts[0];
                splashVFX.transform.position = contact.point;
                splashVFX.transform.rotation = Quaternion.FromToRotation(Vector3.forward, contact.normal);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (DataManager.Instance?.PlayerData != null)
            {
                PlayerDataStatus.Instance.TakeDamage(_attackDamage);
            }
        }

        PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
    }

    private IEnumerator ReturnToPoolAfterLifetime()
    {
        yield return new WaitForSeconds(_lifeTime);
        PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
    }
}
