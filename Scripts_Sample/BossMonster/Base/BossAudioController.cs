using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public class BossAudioController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private FMODUnity.EventReference _bossAttackEvent;
        [SerializeField] private FMODUnity.EventReference _bossDeathEvent;
        [SerializeField] private FMODUnity.EventReference _bossDestroyEvent;
        [SerializeField] private FMODUnity.EventReference _bossWalkEvent;
        [SerializeField] private FMODUnity.EventReference _bossBurnEvent;
        private FMOD.Studio.EventInstance _burnLeftInstance;
        private FMOD.Studio.EventInstance _burnRightInstance;
        private bool _burnLeftPlaying = false;
        private bool _burnRightPlaying = false;

        [Header("Footstep Transform")]
        [SerializeField] private GameObject _leftToe;
        [SerializeField] private GameObject _rightToe;

        [Header("Audio Transform")]
        [SerializeField] private GameObject _leftAttackBone;
        [SerializeField] private GameObject _rightAttackBone;
        [SerializeField] private GameObject _deathBone;
        [SerializeField] private GameObject _destroyLeftBone;
        [SerializeField] private GameObject _destroyRightBone;
        [SerializeField] private GameObject _burnLeftOrigin;
        [SerializeField] private GameObject _burnRightOrigin;

        public void PlayAttackLeftSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossAttackEvent, _leftAttackBone);
        }
        public void PlayAttackRightSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossAttackEvent, _rightAttackBone);
        }

        public void PlayDeathSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossDeathEvent, _deathBone);
        }
        public void PlayDestroyLeftSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossDestroyEvent, _destroyLeftBone);
        }
        public void PlayDestroyRightSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossDestroyEvent, _destroyRightBone);
        }
        public void PlayWalkSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossWalkEvent, gameObject);
        }

        public void PlayLeftFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossWalkEvent, _leftToe);
        }
        public void PlayRightFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_bossWalkEvent, _rightToe);
        }

        public void PlayBurnLeftSound()
        {
            if (_burnLeftPlaying) return;

            _burnLeftInstance = FMODUnity.RuntimeManager.CreateInstance(_bossBurnEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_burnLeftInstance, _burnLeftOrigin);
            _burnLeftInstance.start();
            _burnLeftPlaying = true;
        }

        public void PlayBurnRightSound()
        {
            if (_burnRightPlaying) return;

            _burnRightInstance = FMODUnity.RuntimeManager.CreateInstance(_bossBurnEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_burnRightInstance, _burnRightOrigin);
            _burnRightInstance.start();
            _burnRightPlaying = true;
        }

        public void StopBurnSounds()
        {
            if (_burnLeftPlaying)
            {
                _burnLeftInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _burnLeftInstance.release();
                _burnLeftPlaying = false;
            }

            if (_burnRightPlaying)
            {
                _burnRightInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _burnRightInstance.release();
                _burnRightPlaying = false;
            }
        }
    }
}