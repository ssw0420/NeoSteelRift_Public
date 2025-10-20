using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;
using Unity.XR.Oculus;
using System.Linq;
using ScriptableObjects.PlayerDataStatus;
using ScriptableObjects.GunDataStatus;
using NeoSteelRift.Scripts.Logger;

namespace Managers
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// The game state of the game.
        /// </summary>
        /// <remarks>
        /// MainMenu: The game is in the main menu.
        /// InGame: The game is currently playing.
        /// Pause: The game is paused.
        /// GameOver: The game is over.
        /// </remarks>
        public enum GameState
        {
            Initialize,
            MainMenu,
            InGame,
            Boss,
            Victory,
            Pause,
            GameOver
        }
        [Header("Game State")]
        [SerializeField]
        private GameState _currentState;

        [Header("Managers")]
        [SerializeField]
        private DataManager _dataManager;
        [SerializeField]
        private PoolManager _poolManager;

        public GameState GetGameState() => _currentState;

        public event System.Action<GameState> OnGameStateChanged;
        public void ChangeGameState(GameState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _currentState = GameState.Initialize;
            StartCoroutine(InitializeGame());
            if (Performance.TryGetAvailableDisplayRefreshRates(out var rates))
            {
                float target = 90f;
                if (rates.Contains(target))
                {
                    Performance.TrySetDisplayRefreshRate(target);
                }
            }
        }
        #endregion

        /// <summary>
        /// Initialize the game.
        /// </summary>
        private IEnumerator InitializeGame()
        {
            // Load all data from the PoolDataSO
            _dataManager.LoadAllData();
            while (!_dataManager.IsDataLoaded)
            {
                yield return null; // Wait until the data is loaded
            }

            ChangeGameState(GameState.MainMenu);
            SceneManager.LoadScene("HomeScene_Main");
        }

    }
}