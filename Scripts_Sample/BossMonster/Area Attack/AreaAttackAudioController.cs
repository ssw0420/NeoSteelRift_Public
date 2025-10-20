using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public class AreaAttackAudioController : MonoBehaviour
    {
        [Header("Audio Event")]
        [SerializeField] private FMODUnity.EventReference _areaAttackEvent;

        [Header("Audio Transform")]
        [SerializeField] private Transform _areaAttackTransform;

        public void PlayAreaAttackSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_areaAttackEvent, _areaAttackTransform.position);
        }
    }
}