using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public class SpawnRotator : MonoBehaviour
    {
        [SerializeField] private float angleMin = 15f;
        [SerializeField] private float angleMax = 75f;
        [SerializeField] private float duration = 2f;

        private float _currentAngle;
        private float _direction = 1f;
        private float _speed;


        private void Start()
        {
            _currentAngle = angleMin;
            _speed = (angleMax - angleMin) / (duration / 2f);
        }

        private void Update()
        {
            _currentAngle += Time.deltaTime * _speed * _direction;

            if (_currentAngle >= angleMax)
            {
                _currentAngle = angleMax;
                _direction = -1f; // Reverse direction
            }
            else if (_currentAngle <= angleMin)
            {
                _currentAngle = angleMin;
                _direction = 1f; // Reverse direction
            }

            Vector3 currentEulerAngles = transform.localEulerAngles;
            transform.localRotation = Quaternion.Euler(_currentAngle, currentEulerAngles.y, currentEulerAngles.z);
        }
    }
}