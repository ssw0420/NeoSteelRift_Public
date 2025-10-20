using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class VFXAutoReturn : MonoBehaviour
{
    [Header("Auto Return Settings")]
    [SerializeField] private string _poolKey;
    [SerializeField] private float _lifetime;

    private void OnEnable()
    {
        StartCoroutine(ReturnToPoolAfterLifetime());
    }

    private IEnumerator ReturnToPoolAfterLifetime()
    {
        yield return new WaitForSeconds(_lifetime);
        PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
    }
}
