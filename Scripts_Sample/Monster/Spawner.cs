using System.Collections;
using UnityEngine;
using Managers;

namespace SSW.Spawner
{
    [DefaultExecutionOrder(-70)]
    public class Spawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        [Tooltip("The key to identify the prefab to spawn.")]
        [SerializeField]
        private string _key;

        [Tooltip("The time interval to spawn.")]
        [SerializeField]
        private float _spawnInterval = 5.0f;

        private float _timer;
        private Coroutine _spawnCoroutine;
        /// <summary>
        /// When the game object is enabled, this method is automatically called.
        /// </summary>
        private void OnEnable()
        {
            // If there is a previously running coroutine, safely stop it and start a new one.
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            _spawnCoroutine = StartCoroutine(CoSpawn());
        }

        /// <summary>
        /// When the game object is disabled, this method is automatically called.
        /// </summary>
        private void OnDisable()
        {
            // Stop the spawn coroutine.
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator CoSpawn()
        {
            while (true)
            {
                GameObject monster = PoolManager.Instance.GetFromPool(_key);
                if (monster != null)
                {
                    monster.transform.position = transform.position;
                    monster.transform.rotation = transform.rotation;
                }
                yield return new WaitForSeconds(_spawnInterval);
            }
        }
    }
}

