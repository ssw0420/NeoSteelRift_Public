using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects.PoolData;
using Unity.VisualScripting;
using System;
using SSW.Monster;
using UnityEngine.SceneManagement;

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
        #region Variables

        [Header("Pool Data (ScriptableObject)")]
        [Tooltip("Reference to PoolDataSO asset that holds the (key, prefab) list.")]
        [SerializeField]
        private PoolDataSO _poolDataSO;

        private Dictionary<string, List<GameObject>> _pools;   // key -> list of objects
        private Dictionary<string, GameObject> _prefabDict;    // key -> prefab reference
        private Dictionary<string, int> _maxAmounts;           // key -> max amount
        private bool _isInitalized = false;

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
            foreach (var poolList in _pools.Values)
            {
                foreach (var obj in poolList)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }
            _pools.Clear();
            _prefabDict.Clear();
            _maxAmounts.Clear();
            _isInitalized = false;
        }


        private void InitializePools()
        {
            if (_isInitalized) return;
            // Initialize dictionaries
            _pools = new Dictionary<string, List<GameObject>>();
            _prefabDict = new Dictionary<string, GameObject>();
            _maxAmounts = new Dictionary<string, int>();

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

                _pools[item.key] = new List<GameObject>();
                _prefabDict[item.key] = item.prefab;
                int maxVal = (item.maxAmount <= 0) ? 0 : item.maxAmount;
                _maxAmounts[item.key] = maxVal;
            }

            // Preload objects
            // For each item, create item.preloadCount objects in advance
            foreach (var item in _poolDataSO._poolItems)
            {
                if (!_pools.ContainsKey(item.key))
                    continue; // If we skipped it above, ignore

                if (item.preloadAmount <= 0)
                    continue;

                var prefab = item.prefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"PoolManager: prefab is null for key '{item.key}', skipping preload.");
                    continue;
                }

                int maxAmount = _maxAmounts[item.key];

                // Pre-instantiate 'preloadCount' objects
                for (int i = 0; i < item.preloadAmount; i++)
                {
                    if (maxAmount > 0 && _pools[item.key].Count >= maxAmount)
                    {
                        Debug.LogWarning($"PoolManager: Reached max amount for key '{item.key}'. Skipping preload.");
                        break;
                    }
                    GameObject newObj = Instantiate(prefab);
                    newObj.name = $"{item.key}_Pooled";
                    newObj.SetActive(false);
                    _pools[item.key].Add(newObj);
                }
            }
            
            _isInitalized = true;

        }

        public GameObject GetFromPool(string key)
        {
            if (!_isInitalized)
            {
                Debug.LogError("PoolManager: Not initialized. Call InitializePools() first.");
                return null;
            }

            if (!_pools.TryGetValue(key, out var poolList))
            {
                Debug.LogError($"[Pool] Key '{key}' is not registered in PoolManager.");
                return null;
            }
            poolList.RemoveAll(obj => obj == null);


            foreach (GameObject item in poolList)
            {
                if (!item.activeSelf)
                {
                    item.SetActive(true);
                    return item;
                }
            }

            int maxAmount = _maxAmounts[key];
            bool canCreateMore = (maxAmount == 0 || poolList.Count < maxAmount);

            if (canCreateMore)
            {
                if (!_prefabDict.TryGetValue(key, out GameObject prefab) || prefab == null)
                {
                    Debug.LogError($"[Pool] Prefab reference for key '{key}' is null.");
                    return null;
                }

                GameObject newObj = Instantiate(prefab);
                newObj.name = $"{key}_Pooled";
                poolList.Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
            Debug.LogWarning($"[Pool] Key '{key}' pool exhausted (max = {maxAmount}).");
            return null;
        }


        // /// <summary>
        // /// Retrieves (or instantiates) an object from the pool of given key.
        // /// </summary>
        // /// <param name="key">The string key identifying the prefab.</param>
        // /// <returns>An active GameObject from the pool. Null if the key is invalid.</returns>
        // public GameObject GetFromPool(string key)
        // {
        //     if (!_pools.ContainsKey(key))
        //     {
        //         Debug.LogError($"PoolManager.GetFromPool: Key '{key}' does not exist. Returning null.");
        //         return null;
        //     }

        //     var poolList = _pools[key];
        //     poolList.RemoveAll(obj => obj == null); // Clean up null references
        //     if (poolList.Count == 0)
        //     {
        //         Debug.LogWarning($"PoolManager.GetFromPool: No objects in pool for key '{key}'. Returning null.");
        //         return null;
        //     }
        //     GameObject selected = null;

        //     // Find an inactive object
        //     foreach (GameObject obj in poolList)
        //     {
        //         if (!obj.activeSelf)
        //         {
        //             selected = obj;
        //             selected.SetActive(true);
        //             break;
        //         }
        //     }

        //     // If none found, instantiate a new one
        //     if (selected == null)
        //     {
        //         int maxAmount = _maxAmounts[key];
        //         if (maxAmount > 0 && poolList.Count >= maxAmount)
        //         {
        //             Debug.LogWarning($"PoolManager.GetFromPool: Reached max amount for key '{key}'. Returning null.");
        //             return null;
        //         }
                
        //         if (!_prefabDict.ContainsKey(key) || _prefabDict[key] == null)
        //         {
        //             Debug.LogError($"PoolManager.GetFromPool: Prefab is null for key '{key}'.");
        //             return null;
        //         }

        //         selected = Instantiate(_prefabDict[key]);
        //         selected.name = $"{key}_Pooled";
        //         poolList.Add(selected);
        //     }

        //     return selected;
        // }

        /// <summary>
        /// Deactivates all objects in the specified pool (by key).
        /// </summary>
        /// <param name="key">The string key.</param>
        public void DeactivatePool(string key)
        {
            if (!_pools.ContainsKey(key))
            {
                Debug.LogError($"PoolManager.DeactivatePool: Key '{key}' does not exist.");
                return;
            }

            foreach (GameObject obj in _pools[key])
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Deactivates all objects in all pools.
        /// </summary>
        public void DeactivateAllPools()
        {
            foreach (var kvp in _pools)
            {
                foreach (GameObject obj in kvp.Value)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a specific object to the pool by key, forcibly deactivating it.
        /// </summary>
        /// <param name="obj">The object to deactivate.</param>
        /// <param name="key">The string key representing its pool.</param>
        public void ReturnToPool(GameObject obj, string key)
        {
            if (!_pools.ContainsKey(key))
            {
                Debug.LogError($"PoolManager.ReturnToPool: Key '{key}' does not exist.");
                return;
            }
            if (obj == null)
            {
                Debug.LogWarning("PoolManager.ReturnToPool: obj is null.");
                return;
            }

            var poolList = _pools[key];
            if (!poolList.Contains(obj))
            {
                poolList.Add(obj);
            }
            obj.SetActive(false);
        }

        #endregion
    }
}
