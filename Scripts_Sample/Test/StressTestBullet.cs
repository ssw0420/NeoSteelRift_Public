using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class StressTestBullet : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private string _poolKey = "StressTestBullet";
    private float _lifeTime = 0.5f;

    private Coroutine _lifeTimeCoroutine;
    private WaitForSeconds _lifeTimeWait;

    private void Awake()
    {
        _lifeTimeWait = new WaitForSeconds(_lifeTime);
    }

    private void OnEnable()
    {
        _lifeTimeCoroutine = StartCoroutine(CoLifeTime());
    }

    private void OnDisable()
    {
        if (_lifeTimeCoroutine != null)
        {
            StopCoroutine(_lifeTimeCoroutine);
            _lifeTimeCoroutine = null;
        }
    }

    private IEnumerator CoLifeTime()
    {
        yield return _lifeTimeWait;
        PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
    }
}
