using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace SSW.Player
{
    public class PlayerColliderController : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider _playerCollider;
        [SerializeField] private CharacterController _characterCollider;

        private void Awake()
        {
            bool XRActive = XRSettings.isDeviceActive;

            if (XRActive)
            {
                _characterCollider.enabled = true;
                _playerCollider.enabled = false;
            }
            else
            {

                _characterCollider.enabled = false;
                _playerCollider.enabled = true;
            }
        }

    }
}