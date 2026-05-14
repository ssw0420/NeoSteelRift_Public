using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class BulletStressSpawner : MonoBehaviour
{
    private string _poolKey = "StressTestBullet";

    [SerializeField] private float _spawnsPerSecond = 1000f;
    private float _spawnAccumulator = 0f;


    private Vector3 _spawnAreaSize = new Vector3(10f, 10f, 10f);

    void Update()
    {
        _spawnAccumulator += Time.deltaTime * _spawnsPerSecond;
        int spawnCount = (int)_spawnAccumulator;
        _spawnAccumulator -= spawnCount;
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject obj = PoolManager.Instance.GetFromPool(_poolKey);

            if (obj != null)
            {
                float randomX = Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f);
                float randomY = Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f);
                float randomZ = Random.Range(-_spawnAreaSize.z / 2f, _spawnAreaSize.z / 2f);

                obj.transform.position = transform.position + new Vector3(randomX, randomY, randomZ);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, _spawnAreaSize);
    }
}
