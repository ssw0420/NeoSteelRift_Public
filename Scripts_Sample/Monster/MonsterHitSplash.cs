using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using NeoSteelRift.Scripts.Logger;

namespace SSW.Test
{
    public class MonsterHitSplash : MonoBehaviour
    {
        public float _lifeTime = 1f;
        [SerializeField] private string _poolKey = "HitSplash";

        private float _timer = 0f;

        private void OnEnable()
        {
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _lifeTime)
            {
                _timer = 0f;
                PoolManager.Instance.ReturnToPool(gameObject, _poolKey);
                CustomLogger.LogInfo($"HitSplash returned to pool: {_poolKey}", this);
            }
        }
    }
}

