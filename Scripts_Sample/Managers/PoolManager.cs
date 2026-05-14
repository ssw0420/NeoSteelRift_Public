using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects.PoolData;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

namespace Managers
{
    /// <summary>
    /// This class is a singleton that manages object pooling using a Dictionary.
    /// 
    /// Can register multiple prefabs by a string key (e.g. "FireBullet", "Goblin", etc.).
    /// Then, call GetFromPool("FireBullet") to retrieve an active instance from that pool,
    /// or DeactivatePool("FireBullet") to turn them all off, etc.
    /// </summary>
    [DefaultExecutionOrder(-80)]
    public class PoolManager : Singleton<PoolManager>
    {
        #region  Types

        private class PoolEntry
        {
            public Stack<GameObject> InactiveObjects;
            public GameObject Prefab;
            public int MaxAmount;
            public int TotalCount;
        }

        #endregion
        #region Variables

        [Header("Pool Data (ScriptableObject)")]
        [Tooltip("Reference to PoolDataSO asset that holds the (key, prefab) list.")]
        [SerializeField]
        private PoolDataSO _poolDataSO;

        // private Dictionary<string, List<GameObject>> _pools;   // key -> list of objects
        // private Dictionary<string, GameObject> _prefabDict;    // key -> prefab reference
        // private Dictionary<string, int> _maxAmounts;           // key -> max amount
        private Dictionary<string, PoolEntry> _pools; // key -> PoolEntry containing prefab, max amount, and stack of available objects
        private bool _isInitalized = false;

        #endregion

        #region Profiler Variables
        private static readonly string GetFromPoolMarkerName = "PoolManager.GetFromPool";
        private static readonly string ReturnToPoolMarkerName = "PoolManager.ReturnToPool";
        private Unity.Profiling.ProfilerMarker _getFromPoolMarker = new Unity.Profiling.ProfilerMarker(GetFromPoolMarkerName);
        private Unity.Profiling.ProfilerMarker _returnToPoolMarker = new Unity.Profiling.ProfilerMarker(ReturnToPoolMarkerName);

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Pooling Methods

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "HomeScene_Main")
            {
                ClearAllPools();
            }
            else if (scene.name == "PlayScene_VRMain")
            {
                InitializePools();
            }
        }

        private void ClearAllPools()
        {
            if (_pools == null) return;
            foreach (var entry in _pools.Values)
            {
                foreach (var obj in entry.InactiveObjects)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }
            _pools.Clear();
             _isInitalized = false;
        }


        private void InitializePools()
        {
            if (_isInitalized) return;
            // Initialize dictionaries
            _pools = new Dictionary<string, PoolEntry>();

            // If the ScriptableObject is missing, can't build the dictionary
            if (_poolDataSO == null)
            {
                _poolDataSO = DataManager.Instance.PoolData;
                if (_poolDataSO == null)
                {
                    Debug.LogError("PoolManager: PoolDataSO not assigned in inspector or DataManager.");
                    return;
                }
            }

            // Build the dictionary from the ScriptableObject's list
            foreach (var item in _poolDataSO._poolItems)
            {
                if (string.IsNullOrEmpty(item.key))
                {
                    Debug.LogWarning("PoolManager: Found an empty key in PoolDataSO. Skipping.");
                    continue;
                }
                if (_pools.ContainsKey(item.key))
                {
                    Debug.LogWarning($"PoolManager: Duplicate key '{item.key}' found in PoolDataSO. Skipping second occurrence.");
                    continue;
                }

                _pools[item.key] = new PoolEntry
                {
                    InactiveObjects = new Stack<GameObject>(),
                    Prefab = item.prefab,
                    MaxAmount = (item.maxAmount <= 0) ? 0 : item.maxAmount,
                    TotalCount = 0
                };
            }

            // Preload objects
            // For each item, create item.preloadCount objects in advance
            foreach (var item in _poolDataSO._poolItems)
            {
                if (!_pools.TryGetValue(item.key, out PoolEntry entry)) continue; // If we skipped it above, ignore
                if (item.preloadAmount <= 0) continue;
                if (entry.Prefab == null)
                {
                    Debug.LogWarning($"PoolManager: prefab is null for key '{item.key}', skipping preload.");
                    continue;
                }

                // Pre-instantiate 'preloadCount' objects
                for (int i = 0; i < item.preloadAmount; ++i)
                {
                    if (entry.MaxAmount > 0 && entry.TotalCount >= entry.MaxAmount)
                    {
                        Debug.LogWarning($"PoolManager: Cannot preload more than max amount for key '{item.key}'. Stopping preload.");
                        break;
                    }

                    GameObject newObj = Instantiate(entry.Prefab);
                    newObj.name = $"{item.key}_Pooled_{i}";
                    newObj.SetActive(false);
                    entry.InactiveObjects.Push(newObj);
                    entry.TotalCount++;
                }
            }
            
            _isInitalized = true;

        }

        // /// <summary>
        // /// Retrieves (or instantiates) an object from the pool of given key.
        // /// </summary>
        // /// <param name="key">The string key identifying the prefab.</param>
        // /// <returns>An active GameObject from the pool. Null if the key is invalid.</returns>
        public GameObject GetFromPool(string key)
        {
            using (_getFromPoolMarker.Auto())
            {
                if (!_isInitalized)
                {
                    Debug.LogError("PoolManager: Not initialized. Call InitializePools() first.");
                    return null;
                }

                if (!_pools.TryGetValue(key, out PoolEntry entry))
                {
                    Debug.LogError($"[Pool] Key '{key}' is not registered in PoolManager.");
                    return null;
                }

                while (entry.InactiveObjects.Count > 0)
                {
                    GameObject obj = entry.InactiveObjects.Pop();
                    if (obj == null) continue;
                    obj.SetActive(true);
                    return obj;
                }

                if (entry.MaxAmount > 0 && entry.TotalCount >= entry.MaxAmount)
                {
                    Debug.LogWarning($"[Pool] Key '{key}' pool exhausted (max = {entry.MaxAmount}).");
                    return null;
                }

                if (entry.Prefab == null)
                {
                    Debug.LogError($"[Pool] Prefab reference for key '{key}' is null.");
                    return null;
                }

                GameObject newObj = Instantiate(entry.Prefab);
                newObj.name = $"{key}_Pooled_{entry.TotalCount}";
                entry.TotalCount++;
                newObj.SetActive(true);
                return newObj;
            }

        }

        /// <summary>
        /// Returns a specific object to the pool by key, forcibly deactivating it.
        /// </summary>
        /// <param name="obj">The object to deactivate.</param>
        /// <param name="key">The string key representing its pool.</param>
        public void ReturnToPool(GameObject obj, string key)
        {
            using (_returnToPoolMarker.Auto())
            {
                if (obj == null)
                {
                    Debug.LogError("PoolManager: Cannot return null object to pool.");
                    return;
                }

                if (!_pools.TryGetValue(key, out PoolEntry entry))
                {
                    Debug.LogError($"[Pool] Key '{key}' is not registered in PoolManager. Cannot return object to pool.");
                    return;
                }

                if (!obj.activeSelf)
                {
                    Debug.LogWarning($"[Pool] Object '{obj.name}' is already inactive. Skipping return to pool for key '{key}'.");
                    return;
                }

                obj.SetActive(false);
                entry.InactiveObjects.Push(obj);
            }
        }

        #endregion
    }
}
