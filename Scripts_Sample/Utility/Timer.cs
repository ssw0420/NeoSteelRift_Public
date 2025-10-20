using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class Timer
    {
        private float _startTime;
        private float _duration;

        public Timer(float duration)
        {
            _duration = duration;
            Reset();
        }

        public void Reset()
        {
            _startTime = Time.time;
        }

        public bool IsFinished()
        {
            return Time.time >= _startTime + _duration;
        }

        public float RemainingTime()
        {
            return Mathf.Max(0, (_startTime + _duration) - Time.time);
        }

        public float ProgressRatio()
        {
            if (_duration <= 0) return 1f; // Avoid division by zero
            return Mathf.Clamp01((Time.time - _startTime) / _duration); // elapsed / duration
        }
    }
}