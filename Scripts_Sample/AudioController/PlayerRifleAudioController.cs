using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using NeoSteelRift.Scripts.Logger;

namespace Weapons
{
    public class PlayerRifleAudioController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private EventReference _rifleFireEvent;
        [SerializeField] private EventReference _rifleReloadEvent;
        [SerializeField] private EventReference _rifleEmptyEvent;
        [SerializeField] private EventReference _rifleDropEvent;
        [SerializeField] private EventReference _rifleEquipEvent;
        [SerializeField] private EventReference _rifleSkillEvent;

        [Header("Transform")]
        [SerializeField] private Transform _muzzleTransform;
        [SerializeField] private Transform _ejectTransform;
        [SerializeField] private Transform _clipTransform;

        public void PlayFireSound()
        {
            RuntimeManager.PlayOneShotAttached(_rifleFireEvent, _muzzleTransform.gameObject); // audio will be attached to the game object
            //CustomLogger.LogInfo("Muzzle transform: " + _muzzleTransform.position);
        }
        public void PlayReloadSound()
        {
            RuntimeManager.PlayOneShot(_rifleReloadEvent, _clipTransform.position);
        }
        public void PlayEmptySound()
        {
            RuntimeManager.PlayOneShot(_rifleEmptyEvent, _clipTransform.position);
        }
        public void PlaySkillSound()
        {
            RuntimeManager.PlayOneShotAttached(_rifleSkillEvent, _muzzleTransform.gameObject);
        }
    }
}