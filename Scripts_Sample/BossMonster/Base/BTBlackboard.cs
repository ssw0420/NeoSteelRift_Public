using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;
using System;
using SSW.Monster;
using Managers;

namespace SSW.BossMonster
{
    public class BTBlackboard : MonoBehaviour
    {
        #region Serialized Variables
        [Header("Default Stats")]
        [SerializeField] private int _maxHP = 300;
        [SerializeField] private int _WeaponLeftHP = 50;
        [SerializeField] private int _WeaponRightHP = 50;
        [SerializeField] private float _moveSpeed = 3.5f;
        [SerializeField] private float _attackCooldown = 2f;
        [SerializeField] private float _laserCooldown = 10f;

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] _patrolPoints;

        [Header("Attack Settings")]
        [SerializeField] private Transform _leftBulletSpawnPoint;
        [SerializeField] private Transform _rightBulletSpawnPoint;
        [SerializeField] private GameObject _areaAttackPrefab;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private float _bulletSpeed = 5f;
        [SerializeField] private float _particleDelay = 0.5f;

        [Header("Death Settings")]
        [SerializeField] private DeathHandler _deathHandler;

        [Header("Animations")]
        [SerializeField] private RobotAnimator _robotAnimator;

        [Header("Audio Settings")]
        [SerializeField] private BossAudioController _bossAudioController;
        public enum BossPhase
        {
            Phase1,
            Phase2,
            Phase3
        }
        private BossPhase _currentPhase = BossPhase.Phase1;
        #endregion

        #region Runtime Variables
        private int _currentHP;
        private float _currentLaserTimer;
        private bool _weakPointExposed;
        private bool _isDead = false;
        private Transform _target;
        private int _currentPatrolIndex;


        #endregion

        #region Methods
        public void Initialize()
        {
            _currentHP = _maxHP;
            _currentLaserTimer = 0f;
            _weakPointExposed = false;
            _isDead = false;
            _currentPatrolIndex = 0;

#if UNITY_EDITOR
            GameObject targetObject = GameObject.FindGameObjectWithTag("NavTarget");
            if (targetObject != null)
            {
                _target = targetObject.transform;
            }
            else
            {
                 _target = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
#else
            _target = GameObject.FindGameObjectWithTag("Player")?.transform;
#endif

            if (_target == null)
            {
                CustomLogger.LogError("Target not found. Make sure the target has the 'Player' tag.", this);
            }

            if (_robotAnimator == null)
            {
                _robotAnimator = GetComponent<RobotAnimator>();
            }
        }

        public void TakeDamage(int damage, BossHitBoxType partType)
        {
            if (_isDead) return;

            switch (partType)
            {
                case BossHitBoxType.Body:
                    _currentHP -= damage;
                    break;
                case BossHitBoxType.LeftArm:
                    _WeaponLeftHP -= damage;
                    if (_WeaponLeftHP < 0) _WeaponLeftHP = 0;
                    break;
                case BossHitBoxType.RightArm:
                    _WeaponRightHP -= damage;
                    if (_WeaponRightHP < 0) _WeaponRightHP = 0;
                    break;
            }
            CustomLogger.LogInfo($"Boss took {damage} damage. Current HP: {_currentHP}", this);

            if (_currentHP <= 0)
            {
                _currentHP = 0;
                if (_WeaponLeftHP > 0)
                {
                    _WeaponLeftHP = 0;
                }
                if (_WeaponRightHP > 0)
                {
                    _WeaponRightHP = 0;
                }
                _deathHandler?.TriggerDeath();
            }
        }

        void Awake()
        {
            Initialize();
        }

        void Start()
        {
            if (_target == null)
            {
#if UNITY_EDITOR
                GameObject targetObject = GameObject.FindGameObjectWithTag("NavTarget");
                if (targetObject != null)
                {
                    _target = targetObject.transform;
                }
                else
                {
                    _target = GameObject.FindGameObjectWithTag("Player")?.transform;
                }
#else
                _target = GameObject.FindGameObjectWithTag("Player")?.transform;
#endif
                if (_target == null)
                {
                    CustomLogger.LogError("Target not found. Make sure the target has the 'Player' tag.", this);
                }
            }
        }

        public void Reset()
        {
            Initialize();
        }
        public void SetWeakPoint(bool exposed)
        {
            _weakPointExposed = exposed;
            CustomLogger.LogInfo($"Weak point exposed: {_weakPointExposed}", this);
        }

        public void SetNextPatrolPoint()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0) return;

            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            CustomLogger.LogInfo($"Next patrol point set to index: {_currentPatrolIndex}", this);
        }

        public void SetPhase(BossPhase phase)
        {
            _currentPhase = phase;
            CustomLogger.LogInfo($"Boss phase set to: {_currentPhase}", this);
        }

#if UNITY_EDITOR
        public void SetWeaponLeftHP(int hp)
        {
            _WeaponLeftHP = hp;
            CustomLogger.LogInfo($"Weapon Left HP set to: {_WeaponLeftHP}", this);
        }

        public void SetWeaponRightHP(int hp)
        {
            _WeaponRightHP = hp;
            CustomLogger.LogInfo($"Weapon Right HP set to: {_WeaponRightHP}", this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _WeaponLeftHP = 0;
                Debug.Log("Left Weapon HP set to 0 (Debug)");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _WeaponRightHP = 0;
                Debug.Log("Right Weapon HP set to 0 (Debug)");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _currentHP = 0;
                TakeDamage(1, BossHitBoxType.Body);
                Debug.Log("Boss Death");
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("Change to Boss State");
                GameManager.Instance.ChangeGameState(GameManager.GameState.Boss);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("Change to Victory State");
                GameManager.Instance.ChangeGameState(GameManager.GameState.Victory);
            }
        }
#endif

        public void UpdateLaserCooldown(float deltaTime)
        {
            _currentLaserTimer = Mathf.Max(0, _currentLaserTimer - deltaTime);
        }

        public void ResetLaserCooldown()
        {
            _currentLaserTimer = _laserCooldown;
        }

        #endregion
        #region Properties
        public int MaxHP => _maxHP;
        public int CurrentHP => _currentHP;
        public float HealthPercentage => (float)_currentHP / _maxHP;
        public float MoveSpeed => _moveSpeed;
        public float AttackCooldown => _attackCooldown;
        public float LaserCooldown => _laserCooldown;
        public float CurrentLaserTimer => _currentLaserTimer;
        public Transform[] PatrolPoints => _patrolPoints;
        public Transform LeftBulletSpawnPoint => _leftBulletSpawnPoint;
        public Transform RightBulletSpawnPoint => _rightBulletSpawnPoint;
        public GameObject BulletPrefab => _bulletPrefab;
        public float BulletSpeed => _bulletSpeed;
        public float ParticleDelay => _particleDelay;
        public Transform Target => _target;
        public int CurrentPatrolIndex
        {
            get => _currentPatrolIndex;
            set => _currentPatrolIndex = value;
        }
        public bool WeakPointExposed => _weakPointExposed;
        public bool IsDead
        {
            get => _isDead;
            set => _isDead = value;
        }
        public BossPhase CurrentPhase => _currentPhase;
        public RobotAnimator RobotAnimator => _robotAnimator;
        public int WeaponLeftHP => _WeaponLeftHP;
        public int WeaponRightHP => _WeaponRightHP;
        public GameObject AreaAttackPrefab => _areaAttackPrefab;
        public DeathHandler DeathHandler => _deathHandler;
        public BossAudioController BossAudioController => _bossAudioController;

        #endregion

    }
}