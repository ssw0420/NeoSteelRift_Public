using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using NeoSteelRift.Scripts.Logger;

namespace Weapons
{
    public class PlayerPistolAudioController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private EventReference _pistolFireEvent;
        [SerializeField] private EventReference _pistolReloadEvent;
        [SerializeField] private EventReference _pistolEmptyEvent;
        [SerializeField] private EventReference _pistolDropEvent;
        [SerializeField] private EventReference _pistolEquipEvent;
        [SerializeField] private EventReference _pistolSkillEvent;

        [Header("Transform")]
        [SerializeField] private Transform _muzzleTransform;
        [SerializeField] private Transform _ejectTransform;
        [SerializeField] private Transform _clipTransform;

        public void PlayFireSound()
        {
            RuntimeManager.PlayOneShotAttached(_pistolFireEvent, _muzzleTransform.gameObject); // audio will be attached to the game object
            //CustomLogger.LogInfo("Muzzle transform: " + _muzzleTransform.position);
        }
        public void PlayReloadSound()
        {
            RuntimeManager.PlayOneShot(_pistolReloadEvent, _clipTransform.position);
        }
        public void PlayEmptySound()
        {
            RuntimeManager.PlayOneShot(_pistolEmptyEvent, _clipTransform.position);
        }
        public void PlaySkillSound()
        {
            RuntimeManager.PlayOneShotAttached(_pistolSkillEvent, _muzzleTransform.gameObject);
        }
    }
}