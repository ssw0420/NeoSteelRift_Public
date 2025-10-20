using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.PoolData;
using ScriptableObjects.MonsterData;
using ScriptableObjects.GunData;
using ScriptableObjects.PlayerData;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-90)]
    public class DataManager : Singleton<DataManager>
    {
        #region Variables

        [Header("Data (ScriptableObject)")]
        [Tooltip("Reference to PoolDataSO asset that holds the (key, prefab) list.")]
        [SerializeField]
        private PoolDataSO _poolDataSO;
        public PoolDataSO PoolData => _poolDataSO;

        [SerializeField]
        private MonsterDataSO _monsterDataSO;
        public MonsterDataSO MonsterData => _monsterDataSO;

        [SerializeField]
        private GunDataSO _gunDataSO;
        public GunDataSO GunData => _gunDataSO;

        [SerializeField]
        private PlayerDataSO _playerDataSO;
        public PlayerDataSO PlayerData => _playerDataSO;

        private bool _isDataLoaded = false;
        public bool IsDataLoaded => _isDataLoaded;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load all data from the PoolDataSO, MonsterDataSO, GunDataSO, PlayerDataSO
        /// </summary>
        public void LoadAllData()
        {
            if (_poolDataSO == null)
            {
                Debug.LogError("DataManager: PoolDataSO not assigned in inspector.");
            }

            if (_monsterDataSO == null)
            {
                Debug.LogError("DataManager: MonsterDataSO not assigned in inspector.");
            }

            if (_gunDataSO == null)
            {
                Debug.LogError("DataManager: GunDataSO not assigned in inspector.");
            }

            if (_playerDataSO == null) // ← 추가
            {
                Debug.LogError("DataManager: PlayerDataSO not assigned in inspector.");
            }

            _isDataLoaded = true;
        }

        #endregion
    }
}
