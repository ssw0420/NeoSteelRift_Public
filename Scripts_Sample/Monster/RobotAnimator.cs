using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;

namespace SSW.Monster
{
    [System.Serializable]
    public class MonsterAnimationParamNames
    {
        // Movement and Rotation animation parameters
        public string speedMultiplier = "speedMultiplier";
        public string speed = "Speed";
        public string turns = "Turns";
        public string turnLeft = "turnLeft";
        public string turnRight = "turnRight";

        // Attack animation parameters
        public string shoot = "shoot";
        public string shoot_bool = "shoot_bool";
        public string battle = "battle";

        // Hit animation parameters
        public string hitLeft = "hitLeft";
        public string hitRight = "hitRight";
        public string hitLeftTrigger = "hitLeftTrigger";
        public string hitRightTrigger = "hitRightTrigger";

        // Look Around animation parameters
        public string look1 = "look1";
        public string look2 = "look2";

        // pack animation parameters
        public string packed = "Packed";

        // Death animation parameters
        public string die = "die";

    }
    [DefaultExecutionOrder(-80)]
    [RequireComponent(typeof(Animator))]
    public class RobotAnimator : MonoBehaviour
    {
        [Header("Animator Parameter Names")]
        [SerializeField] private MonsterAnimationParamNames _animatorParamNames;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                CustomLogger.LogError("Animator component is missing.", this);
            }
            if (_animatorParamNames == null)
            {
                CustomLogger.LogError("Animator Parameter Names is not assigned in the inspector.", this);
            } 
        }

        private void OnEnable()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            if (_animatorParamNames == null)
            {
                _animatorParamNames = new MonsterAnimationParamNames();
            }
            
        }

        public void RebindAnimator()
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        public void ApplyRootMotion()
        {
            if (_animator != null)
            {
                _animator.applyRootMotion = true;
            }
        }

        public void RemoveRootMotion()
        {
            if (_animator != null)
            {
                _animator.applyRootMotion = false;
            }
        }

        // Movement and Rotation animation methods
        public void SetSpeedMultiplier(float value)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.speedMultiplier))
            {
                _animator.SetFloat(_animatorParamNames.speedMultiplier, value);
            }
        }

        public void SetSpeed(float value)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.speed))
            {
                if (value < 0.01f)
                {
                    _animator.SetFloat(_animatorParamNames.speed, value);
                }
                else
                {
                    _animator.SetFloat(_animatorParamNames.speed, value, 0.1f, Time.deltaTime);
                }
            }
        }

        public void SetTurns(float value)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.turns))
            {
                if (Mathf.Abs(value) < 0.01f)
                {
                    _animator.SetFloat(_animatorParamNames.turns, 0f);
                }
                else
                {
                    _animator.SetFloat(_animatorParamNames.turns, value, 0.1f, Time.deltaTime);
                }
            }
        }

        public float GetSpeed()
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.speed))
            {
                return _animator.GetFloat(_animatorParamNames.speed);
            }
            return 0f;
        }

        public float GetTurns()
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.turns))
            {
                return _animator.GetFloat(_animatorParamNames.turns);
            }
            return 0f;
        }

        public void SetTurnLeft(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.turnLeft))
            {
                _animator.SetBool(_animatorParamNames.turnLeft, isOn);
            }
        }

        public void SetTurnRight(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.turnRight))
            {
                _animator.SetBool(_animatorParamNames.turnRight, isOn);
            }
        }

        // Attack animation methods
        public void SetShoot(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.shoot))
            {
                _animator.SetBool(_animatorParamNames.shoot, isOn);
            }
        }

        public void SetShootBool(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.shoot_bool))
            {
                _animator.SetBool(_animatorParamNames.shoot_bool, isOn);
            }
        }

        public void SetBattle(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.battle))
            {
                _animator.SetBool(_animatorParamNames.battle, isOn);
            }
        }

        // Hit animation methods
        public void SetHitLeft(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.hitLeft))
            {
                _animator.SetBool(_animatorParamNames.hitLeft, isOn);
            }
        }

        public void SetHitRight(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.hitRight))
            {
                _animator.SetBool(_animatorParamNames.hitRight, isOn);
            }
        }

        public void SetHitLeftTrigger()
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.hitLeftTrigger))
            {
                _animator.SetTrigger(_animatorParamNames.hitLeftTrigger);
            }
        }

        public void SetHitRightTrigger()
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.hitRightTrigger))
            {
                _animator.SetTrigger(_animatorParamNames.hitRightTrigger);
            }
        }

        // Look Around animation methods
        public void SetLook1(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.look1))
            {
                _animator.SetBool(_animatorParamNames.look1, isOn);
            }
        }

        public void SetLook2(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.look2))
            {
                _animator.SetBool(_animatorParamNames.look2, isOn);
            }
        }

        // Pack animation methods
        public void SetPacked(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.packed))
            {
                _animator.SetBool(_animatorParamNames.packed, isOn);
            }
        }

        // Death animation methods
        public void SetDie(bool isOn)
        {
            if (!string.IsNullOrEmpty(_animatorParamNames.die))
            {
                _animator.SetBool(_animatorParamNames.die, isOn);
            }
        }

        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
        {
            if (_animator != null)
            {
                return _animator.GetCurrentAnimatorStateInfo(layerIndex);
            }
            return default;
        }
    }
}