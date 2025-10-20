using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.PoolData
{
    [CreateAssetMenu(fileName = "PoolData", menuName = "ScriptableObjects/PoolData")]
    public class PoolDataSO : ScriptableObject
    {
        [System.Serializable]
        public struct PoolItem
        {
            [Tooltip("A unique key to identify this prefab.")]
            public string key;

            [Tooltip("The actual prefab to be pooled.")]
            public GameObject prefab;

            [Tooltip("The amount of objects to preload.")]
            public int preloadAmount;

            [Tooltip("The maximum amount of objects to pool.")]
            public int maxAmount;
        }

        [Header("Prefabs to pool settings")]
        [Tooltip("List of (key, prefab) pairs.")]
        public List<PoolItem> _poolItems;
    }
}
