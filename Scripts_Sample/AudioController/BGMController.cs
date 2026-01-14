using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using Managers;

namespace SSW.Audio
{
    [DefaultExecutionOrder(-70)]
    public class BGMController : Singleton<BGMController>
    {
        [SerializeField] private EventReference _menuBGMEvent;
        [SerializeField] private EventReference _mainBGMEvent;
        [SerializeField] private EventReference _bossBGMEvent;
        [SerializeField] private EventReference _victoryBGMEvent;

        private EventInstance _currentBGMInstance;

        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        private void Start()
        {
            OnGameStateChanged(GameManager.Instance.GetGameState()); // Initialize the BGM based on the current game state
        }

        private void OnGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.MainMenu:
                    PlayMenuBGM();
                    break;
                case GameManager.GameState.InGame:
                    PlayMainBGM();
                    break;
                case GameManager.GameState.Boss:
                    PlayBossBGM();
                    break;
                case GameManager.GameState.Victory:
                    PlayVictoryBGM();
                    break;
                    // case GameManager.GameState.Pause:
                    //     StopCurrentBGM();
                    //     break;
                    // case GameManager.GameState.GameOver:
                    //     StopCurrentBGM();
                    //     break;
            }
        }

        private void StopCurrentBGM()
        {
            _currentBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _currentBGMInstance.release();
        }
        public void PlayMenuBGM()
        {
            StopCurrentBGM();
            _currentBGMInstance = RuntimeManager.CreateInstance(_menuBGMEvent);
            _currentBGMInstance.start();
        }

        public void PlayMainBGM()
        {
            StopCurrentBGM();
            _currentBGMInstance = RuntimeManager.CreateInstance(_mainBGMEvent);
            _currentBGMInstance.start();
        }

        public void PlayBossBGM()
        {
            StopCurrentBGM();
            _currentBGMInstance = RuntimeManager.CreateInstance(_bossBGMEvent);
            _currentBGMInstance.start();
        }

        public void PlayVictoryBGM()
        {
            StopCurrentBGM();
            _currentBGMInstance = RuntimeManager.CreateInstance(_victoryBGMEvent);
            _currentBGMInstance.start();
        }

        private void OnDestroy()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }
    }
}
