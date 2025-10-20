using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.Player
{
    public class PlayerPistolAudioController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private FMODUnity.EventReference _bulletImpactEvent;
        [SerializeField] private FMODUnity.EventReference _bulletMetalHitEvent;

        [Header("Transform")]
        [SerializeField] private Transform _hitTransform;

        public void PlayBulletImpactSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_bulletImpactEvent, _hitTransform.position);
        }
        public void PlayBulletMetalHitSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(_bulletMetalHitEvent, _hitTransform.position);
        }
    }
}