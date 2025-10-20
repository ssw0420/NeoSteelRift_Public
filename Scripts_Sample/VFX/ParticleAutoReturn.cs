using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class ParticleAutoReturn : MonoBehaviour
{
    [Header("Auto Return Settings")]
    [SerializeField] private string _poolKey;
    private List<ParticleSystem> _particleSystems;

    [SerializeField] private float _maxDuration;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _particleSystems = new List<ParticleSystem>();
        GetComponentsInChildren(true, _particleSystems);
        _maxDuration = 0f;
        foreach (var particleSystem in _particleSystems)
        {
            if (particleSystem.main.duration > _maxDuration)
            {
                _maxDuration = particleSystem.main.duration;
            }
        }
    }
    private void OnEnable()
    {
        transform.localScale = _originalScale;
        StartCoroutine(ReturnToPoolAfterLifetime());
    }

    private IEnumerator ReturnToPoolAfterLifetime()
    {
        yield return new WaitForSeconds(_maxDuration);
        PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
    }

}
