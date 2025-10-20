using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;

namespace SSW.Player
{
    public class NavTargetMover : MonoBehaviour
    {
        [Tooltip("Camera Position and Rotation")]
        [SerializeField] private Transform _hmd;

        [Tooltip("Fixed Height")]
        [SerializeField] private float _fixedHeight = 0f;

        void Awake()
        {
            if (_hmd == null)
            {
                CustomLogger.LogError("HMD Transform is not assigned in the inspector.", this);
                _hmd = Camera.main?.transform;
            }
        }

        void LateUpdate()
        {
            if(_hmd == null) return;

            Vector3 targetPosition = _hmd.position;
            targetPosition.y = _fixedHeight;
            transform.position = targetPosition;
        }
    }
}