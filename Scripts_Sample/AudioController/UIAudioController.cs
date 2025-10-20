using System;
using UnityEngine;
using Managers;
using NeoSteelRift.Scripts.Logger;

namespace SSW.Audio
{
    public class UIAudioController : Singleton<UIAudioController>
    {
        [Header("Audio Events")]
        [SerializeField] private FMODUnity.EventReference _menuUIClickEvent;
        [SerializeField] private FMODUnity.EventReference _hoverEvent;
        [SerializeField] private FMODUnity.EventReference _playingUIClickEvent;
        [SerializeField] private FMODUnity.EventReference _playingUIPopupEvent;

        private GameManager.GameState _currentGameState;

        protected override void Awake()
        {
            base.Awake();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

                _currentGameState = GameManager.Instance.GetGameState();

                CustomLogger.LogInfo("UIAudioController: GameManager instance found, subscribed to OnGameStateChanged.");
            }
            else
            {
                CustomLogger.LogWarning("UIAudioController: GameManager instance not found.");
            }
        }

        // private void OnDestroy()
        // {
        //     if (!_applicationIsQuitting && GameManager.Instance != null)
        //     {
        //         GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        //         CustomLogger.LogInfo("UIAudioController: Unsubscribed from OnGameStateChanged.");
        //     }
        // }

        private void OnGameStateChanged(GameManager.GameState newState)
        {
            _currentGameState = newState;
            CustomLogger.LogInfo($"UIAudioController: GameState changed to {_currentGameState}");
        }

        public void PlayMenuUIClickSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_menuUIClickEvent);
            CustomLogger.LogInfo("UIAudioController: PlayMenuUIClickSound called.");
        }

        public void UIHoverSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_hoverEvent);
            CustomLogger.LogInfo("UIAudioController: UIHoverSound called.");
        }

        public void PlayPlayingUIClickSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_playingUIClickEvent);
            CustomLogger.LogInfo("UIAudioController: PlayPlayingUIClickSound called.");
        }

        public void PlayPlayingUIPopupSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_playingUIPopupEvent);
            CustomLogger.LogInfo("UIAudioController: PlayPlayingUIPopupSound called.");
        }

        public void PlayClickSoundBasedOnGameState()
        {
            switch (_currentGameState)
            {
                case GameManager.GameState.MainMenu:
                    PlayMenuUIClickSound();
                    break;
                case GameManager.GameState.InGame:
                    PlayPlayingUIClickSound();
                    break;
                default:
                    PlayPlayingUIClickSound();
                    break;
            }
        }
    }
}