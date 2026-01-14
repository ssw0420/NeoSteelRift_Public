using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ScriptableObjects.MonsterData;
using ScriptableObjects.PlayerData;
using NeoSteelRift.Scripts.Logger;
using Managers;
using ScriptableObjects.PlayerDataStatus;
using UnityEngine.VFX;
using SSW.Monster.States;
using SSW.HitSystem;
using SSW.Monster.Attack;
namespace SSW.Monster
{
        public enum MonsterState
    {
        Spawn,
        Idle,
        Chase,
        Attack,
        Hit,
        Dead
    }

    [DefaultExecutionOrder(-70)]
    [RequireComponent(typeof(NavMeshAgent), typeof(RobotAnimator), typeof(RendererController))]
    public class MonsterControllerRefactored : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("ScriptableObject Reference")]
        [SerializeField] private MonsterDataSO _monsterData;
        [SerializeField] private string _monsterKey;
        [SerializeField] private AttackSO _attackPattern;

        [Header("AI Settings")]
        [SerializeField] Transform _target;
        [SerializeField] private float _spawnDelay = 1.5f;
        [SerializeField] private MonsterState _currentStateEnum = MonsterState.Spawn;

        [Header("HitBox Settings")]
        [Tooltip("Farm monsters can have low detail hitboxes to optimize performance")]
        [SerializeField] private List<Collider> _highDetailHitBoxes = new List<Collider>();
        [SerializeField] private GameObject _lowDetailHitBox;

        [Tooltip("distance to switch between high and low quality hitboxes")]
        [SerializeField] private float _highDetailHitBoxDistance = 10f;
        [SerializeField] private float _lowDetailHitBoxDistance = 25f;
        private bool _isHighDetail = true;
        private float _checkHitBoxInterval = 1.0f;

        // Core Components
        private RobotAnimator _animator;
        private NavMeshAgent _agent;
        private NavMeshObstacle _navMeshObstacle;
        private Rigidbody _rigidbody;
        private RendererController _rendererController;
        private MonsterAudioController _monsterAudioController;
        //private MonsterRotationController _rotationController;
        private IHitProcessor _hitProcessor;

        // Data
        private MonsterDataSO.MonsterItem _monsterStats;

        // FSM
        private MonsterStateBase _currentState;
        private Dictionary<MonsterState, MonsterStateBase> _states;

        // Layer masks
        private int _layerAgentMask;
        private int _layerPhysicsMask;
        [SerializeField] private LayerMask _obstacleLayerMask;

        [Header("Stats")]
        [SerializeField] private float _currentHP;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _experience;

        [Header("VFX")]
        [SerializeField] private VisualEffect _spawnEffect;
        [SerializeField] private VisualEffect _dieEffect;
        [SerializeField] private UNI_Materialize _uniMaterialize;

        [Header("Animator Settings")]
        [SerializeField] private float _defaultSpeed = 1.0f;
        [SerializeField] private float _targetTransformReferenceDelay = 0.2f;
        [SerializeField] private float _turnSpeed = 5f;
        private const float RotationThreshold = 3f;
        private float _lastTurnSpeed = 0f;

        [Header("Status Flags")]
        private bool _isSpawned = false;
        private bool _isInvincible = true;
        private bool _isDead = false;
        private bool _isKnockedBack = false;
        private bool _isFacedToTarget = false;
        private bool _isReadyForFastAttack = true;

        [Header("Coroutine References")]
        [SerializeField] private Coroutine _hitProcessCoroutine;
        public Coroutine KnockbackCoroutine { get; set; }

        #endregion

        #region Properties
        public RobotAnimator Animator => _animator;
        public NavMeshAgent Agent => _agent;
        public Rigidbody Rigidbody => _rigidbody;
        public RendererController RendererController => _rendererController;
        public MonsterAudioController MonsterAudioController => _monsterAudioController;
        //public MonsterRotationController MonsterRotationController => _rotationController;
        public Transform Target => _target;
        public float SpawnDelay => _spawnDelay;
        public VisualEffect SpawnEffect => _spawnEffect;
        public VisualEffect DieEffect => _dieEffect;
        public UNI_Materialize UniMaterialize => _uniMaterialize;
        public float TargetTransformReferenceDelay => _targetTransformReferenceDelay;
        public float MoveSpeed => _moveSpeed;
        public float LastTurnSpeed => _lastTurnSpeed;
        public float TurnSpeed => _turnSpeed;
        public LayerMask ObstacleLayerMask => _obstacleLayerMask;
        public AttackSO AttackPattern => _attackPattern;
        public float AttackRange => _attackPattern != null ? _attackPattern.attackRange : 0f;
        public float AttackCooldown => _attackPattern != null ? _attackPattern.attackCooldown : 0f;
        public float AttackDelay => _attackPattern != null ? _attackPattern.attackDelay : 0f;
        public bool IsInvincible => _isInvincible;
        public bool IsDead => _isDead;
        public bool IsKnockedBack => _isKnockedBack;
        public bool IsFacedToTarget => _isFacedToTarget;
        public bool IsReadyForFastAttack => _isReadyForFastAttack;
        public string MonsterKey => _monsterKey;
        public float Experience => _experience;
        public float CurrentHP => _currentHP;
        public string MonsterType => _monsterKey;
        public AttackRaycastPoints RaycastPoints { get; private set; }
        // Status setters
        public void SetIsInvincible(bool value) => _isInvincible = value;
        public void SetIsSpawned(bool value) => _isSpawned = value;
        public void SetIsDead(bool value) => _isDead = value;
        public void SetIsKnockedBack(bool value) => _isKnockedBack = value;
        public void SetIsFacedToTarget(bool value) => _isFacedToTarget = value;
        public void SetIsReadyForFastAttack(bool value) => _isReadyForFastAttack = value;

        #endregion

        #region Unity Methods
        private void Awake()
        {
            GetComponentReferences();
            InitializeStates();
            InitializeHitProcessor();

            _rendererController.SetAllRenderersActive(false);
            _layerAgentMask = LayerMask.NameToLayer("EnemyAgent");
            _layerPhysicsMask = LayerMask.NameToLayer("EnemyPhysics");
        }

        private void Start()
        {
            if (_monsterData == null)
            {
                CustomLogger.LogError("MonsterDataSO is not assigned in the inspector.", this);
                return;
            }

            _monsterStats = _monsterData.GetMonsterData(_monsterKey);
            if (_monsterStats.key == null)
            {
                CustomLogger.LogError("Monster data not found.", this);
                return;
            }

            SetupMonsterStats();
            _agent.speed = _moveSpeed;
            _rigidbody.isKinematic = true;
            _animator.SetSpeedMultiplier(_defaultSpeed);
        }

        private void OnEnable()
        {
            SetupMonsterStats();
            ResetMonster();
            if(_target == null)
            {
                FindTarget();
            }
            FaceTarget();

            ChangeState(MonsterState.Spawn);
        }

        private void Update()
        {
            _checkHitBoxInterval -= Time.deltaTime;
            if (_checkHitBoxInterval <= 0f)
            {
                UpdateHitBoxQuality();
                _checkHitBoxInterval = 1.0f;
            }
            if (_target == null || _isKnockedBack || _isDead)
                return;
            // 현재 상태 업데이트
            _currentState?.OnUpdate();
        }
        #endregion

        #region Initialization
        private void GetComponentReferences()
        {
            _agent = GetComponent<NavMeshAgent>();
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _animator = GetComponent<RobotAnimator>();
            _rigidbody = GetComponent<Rigidbody>();
            _rendererController = GetComponent<RendererController>();
            _monsterAudioController = GetComponent<MonsterAudioController>();
            RaycastPoints = GetComponent<AttackRaycastPoints>();
            //_rotationController = GetComponent<MonsterRotationController>();

            _agent.updateRotation = false;

            // HitBox 설정
            HitBox[] hitBoxes = GetComponentsInChildren<HitBox>(true);
            foreach (HitBox hitBox in hitBoxes)
            {
                hitBox.owner = this;
            }
        }

        private void InitializeStates()
        {
            _states = new Dictionary<MonsterState, MonsterStateBase>
            {
                { MonsterState.Spawn, new SpawnState(this) },
                { MonsterState.Idle, new IdleState(this) },
                { MonsterState.Chase, new ChaseState(this) },
                { MonsterState.Hit, new HitState(this) },
                { MonsterState.Attack, new AttackState(this) },
                { MonsterState.Dead, new DeadState(this) }
            };
        }

        private void InitializeHitProcessor()
        {
            // Pre-initialize HitProcessor since all monsters will be hit due to game genre characteristics
            _hitProcessor = new MonsterHitProcessor();
        }

        private void SetupMonsterStats()
        {
            _currentHP = _monsterStats.baseHP;
            _moveSpeed = _monsterStats.moveSpeed;
            _experience = _monsterStats.experience;
        }

        private void ResetMonster()
        {
            StopAllCoroutines();

            _isSpawned = false;
            _isDead = false;
            _isKnockedBack = false;
            _isInvincible = true;
            _isReadyForFastAttack = true;

            foreach (var col in GetComponentsInChildren<Collider>())
            {
                col.enabled = true;
            }

            // Animator Reset
            _animator.SetDie(false);
            _animator.SetShoot(false);
            _animator.SetHitLeft(false);
            _animator.SetHitRight(false);
            _animator.SetTurnLeft(false);
            _animator.SetTurnRight(false);
            _animator.SetSpeed(0f);
            _animator.SetTurns(0f);
            _animator.RebindAnimator();
            _animator.ApplyRootMotion();

            // Physics Reset
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

            // NavMeshAgent Reset
            _navMeshObstacle.enabled = false;
            _agent.enabled = true;
            _agent.isStopped = true;
            _agent.Warp(transform.position);
            _agent.ResetPath();
            _agent.speed = _moveSpeed;
            _agent.stoppingDistance = _attackPattern.attackRange > 0.5f ? _attackPattern.attackRange - 0.5f : 0f;

            // Attack Reset
            _targetTransformReferenceDelay = 0.2f;
            _lastTurnSpeed = 0f;

            // Renderer Reset
            _rendererController.SetAllRenderersActive(false);

            // HitBox Quality Reset
            InitializeColliderState();
        }

        private void FindTarget()
        {
            // In the editor, first look for "NavTarget" tag, then fall back to "Player" tag if not found.
#if UNITY_EDITOR
            GameObject targetObject = GameObject.FindGameObjectWithTag("NavTarget");
            if (targetObject != null)
            {
                _target = targetObject.transform;
            }
            else
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    _target = playerObject.transform;
                    CustomLogger.LogWarning("PlayerNavTarget not found. Falling back to Player tag.", this);
                }
                else
                {
                    CustomLogger.LogError("Target with tag PlayerNavTarget or Player not found.", this);
                }
            }
            // If not in the editor, directly look for "Player" tag.
#else
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _target = playerObject.transform;
            }
            else
            {
                CustomLogger.LogError("Target with tag Player not found.", this);
            }
#endif
        }

        private void FaceTarget()
        {
            if (_target != null)
            {
                Vector3 dir = _target.position - transform.position;
                dir.y = 0f;
                if (dir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }
        }
        #endregion

        #region Rotation
        /// <summary>
        /// when monster is stopped by navmesh agent, it will rotate to target smoothly
        /// </summary>
        public IEnumerator CoRotateTowardsTarget(float duration)
        {
            Vector3 directionToTargetInit = _target.position - transform.position;
            directionToTargetInit.y = 0;
            float angle = Vector3.Angle(transform.forward, directionToTargetInit);
            if (angle <= RotationThreshold)
            {
                yield break; // No need to rotate if the angle is small
            }
            Quaternion startingRotation = transform.rotation;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                Vector3 directionToTarget = _target.position - transform.position;
                directionToTarget.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                float t = elapsedTime / duration;

                transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);

                float currentSpeed = Mathf.Lerp(1f, 0f, t);
                Animator.SetSpeed(currentSpeed);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Vector3 finalDirection = _target.position - transform.position;
            finalDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(finalDirection);
            Animator.SetSpeed(0f);
        }
        #endregion

        #region State Management
        public void ChangeState(MonsterState newState)
        {
            if (_currentState != null && !_currentState.CanTransitionTo(newState))
            {
                CustomLogger.LogWarning($"Cannot transition from {_currentStateEnum} to {newState}", this);
                return;
            }

            _currentState?.OnExit();

            if (_states.TryGetValue(newState, out MonsterStateBase nextState))
            {
                _currentState = nextState;
                _currentStateEnum = newState;
                _currentState.OnEnter();
            }
            else
            {
                CustomLogger.LogError($"State {newState} not found in states dictionary", this);
            }
        }

        public MonsterState GetCurrentState()
        {
            return _currentStateEnum;
        }
        #endregion

        #region Layer Management
        public void SetAgentLayer()
        {
            gameObject.layer = _layerAgentMask;
        }

        public void SetPhysicsLayer()
        {
            gameObject.layer = _layerPhysicsMask;
        }
        #endregion

        #region Hit System        

        public void OnHit(HitData hitData)
        {
            if (!_hitProcessor.CanBeHit(this)) return;

            StopAllCoroutines();
            Animator.RebindAnimator();
            Animator.SetSpeed(0f);
            Animator.SetTurns(0f);
            Animator.SetHitLeft(false);
            Animator.SetHitRight(false);
            Animator.SetShoot(false);
            Animator.SetTurnLeft(false);
            Animator.SetTurnRight(false);
            Animator.ApplyRootMotion();

            _rigidbody.isKinematic = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            _agent.enabled = true;
            _agent.isStopped = true;
            _isKnockedBack = false;
            _isReadyForFastAttack = false;

            _hitProcessCoroutine = StartCoroutine(_hitProcessor.CoProcessHit(this, hitData));
        }

        public void TakeDamage(float damage)
        {
            _currentHP -= damage;
            CustomLogger.LogInfo($"Monster took {damage} damage. Current HP: {_currentHP}", this);
        }

        public void EnableObstacle()
        {
            if (_navMeshObstacle != null)
            {
                _navMeshObstacle.enabled = true;
            }
        }

        public void DisableObstacle()
        {
            if (_navMeshObstacle != null)
            {
                _navMeshObstacle.enabled = false;
            }
        }

        #endregion

        #region HitBox Quality Management
        private void InitializeColliderState()
        {
            if (_target == null)
            {
                SetHighDetailHitboxes(true);
                if (_lowDetailHitBox != null) _lowDetailHitBox.SetActive(false);
                _isHighDetail = true;
                return;
            }

            if (Vector3.Distance(transform.position, _target.position) > _lowDetailHitBoxDistance)
            {
                _isHighDetail = false;
                SetHighDetailHitboxes(false);
                if (_lowDetailHitBox != null) _lowDetailHitBox.SetActive(true);
            }
            else
            {
                _isHighDetail = true;
                SetHighDetailHitboxes(true);
                if (_lowDetailHitBox != null) _lowDetailHitBox.SetActive(false);
            }
        }
        private void UpdateHitBoxQuality()
        {
            if (_target == null || _isDead) return;

            float distanceToTarget = Vector3.Distance(transform.position, _target.position);

            if (!_isHighDetail && distanceToTarget < _highDetailHitBoxDistance)
            {
                StartCoroutine(SwitchToHighDetail());
            }

            else if (_isHighDetail && distanceToTarget > _lowDetailHitBoxDistance)
            {
                StartCoroutine(SwitchToLowDetail());
            }
        }

        private IEnumerator SwitchToHighDetail()
        {
            SetHighDetailHitboxes(true);
            _isHighDetail = true;
            yield return null;
            if (_lowDetailHitBox != null) _lowDetailHitBox.SetActive(false);
        }
        
        private IEnumerator SwitchToLowDetail()
        {
            if (_lowDetailHitBox != null) _lowDetailHitBox.SetActive(true);
            _isHighDetail = false;
            yield return null;
            SetHighDetailHitboxes(false);
        }
        
        void SetHighDetailHitboxes(bool isActive)
        {
            foreach (Collider collider in _highDetailHitBoxes)
            {
                if (collider != null)
                {
                    collider.enabled = isActive;
                }
            }
        }

        #endregion
    }

}
