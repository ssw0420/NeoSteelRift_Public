using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SSW.Player
{
    public class PlayerAudioController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private FMODUnity.EventReference _playerFootstepEvent;
        [SerializeField] private FMODUnity.EventReference _playerDeathEvent;
        [SerializeField] private FMODUnity.EventReference _playerhitEvent;
        [SerializeField] private FMODUnity.EventReference _playerheartbeatEvent;
        [SerializeField] private FMODUnity.EventReference _playerhealthEvent;
        [SerializeField] private FMODUnity.EventReference _playerbreathEvent;

        [Header("Transform")]
        [SerializeField] private Transform _footstepTransform;
        [SerializeField] private Transform _deathTransform;

        [Header("XR Input")]
        [SerializeField] private InputActionProperty _leftHandMoveAction;

        [Header("Footstep Timing")]
        //[SerializeField] private float _stepInterval = 0.5f;

        private float _stepTimer;

        private void Update()
        {
            Vector2 moveInput = _leftHandMoveAction.action.ReadValue<Vector2>();
            float moveSpeedFactor = moveInput.magnitude; // 0 (정지) ~ 1 (최대 속도)

            if (moveSpeedFactor > 0.1f) // 임계값 설정 (약간 움직이는 것도 감지됨)
            {
                // 속도에 따라 stepInterval을 0.6초~1.2초 사이로 조절
                float dynamicInterval = Mathf.Lerp(1.2f, 0.6f, moveSpeedFactor);

                _stepTimer += Time.deltaTime;

                if (_stepTimer >= dynamicInterval)
                {
                    PlayFootstepSound();
                    _stepTimer = 0f;
                }
            }
            else
            {
                _stepTimer = 0f;
            }
        }

        public void PlayFootstepSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_playerFootstepEvent, _footstepTransform.position);
        }

        public void PlayDeathSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_playerDeathEvent, _deathTransform.position);
        }

        public void PlayHitSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_playerhitEvent, _footstepTransform.position);
        }
    }
}
