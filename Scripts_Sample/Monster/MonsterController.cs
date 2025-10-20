// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;
// using ScriptableObjects.MonsterData;
// using ScriptableObjects.PlayerData;
// using NeoSteelRift.Scripts.Logger;
// using Managers;
// using ScriptableObjects.PlayerDataStatus;
// using UnityEngine.VFX;

// namespace SSW.Monster
// {
//     // Done
//     // public enum MonsterState
//     // {
//     //     Spawn,
//     //     Idle,
//     //     Chase,
//     //     Attack,
//     //     Hit,
//     //     Dead
//     // }
//     [DefaultExecutionOrder(-70)]
//     [RequireComponent(typeof(NavMeshAgent), typeof(RobotAnimator), typeof(RendererController))]
//     public class MonsterController : MonoBehaviour
//     {
//         #region Variables
//         // Done
//         [Header("ScriptableObject Reference")]
//         [SerializeField] private MonsterDataSO _monsterData;
//         [SerializeField] private string _monsterKey;

//         [Header("AI Settings")]
//         [SerializeField] Transform _target;
//         [SerializeField] private float _spawnDelay = 1.5f;
//         [SerializeField] private MonsterState _state = MonsterState.Spawn;
//         private RobotAnimator _animator;
//         private NavMeshAgent _agent;
//         private MonsterDataSO.MonsterItem _monsterStats;
//         private int _layerAgentMask;
//         private int _layerPhysicsMask;

//         // Done
//         [Header("Knockback Settings")]
//         private Rigidbody _rigidbody;

//         // Done
//         [Header("Stats")]
//         [SerializeField] private float _currentHP;
//         [SerializeField] private float _moveSpeed;
//         [SerializeField] private int _attackDamage;
//         [SerializeField] private float _attackRange;
//         [SerializeField] private float _attackCooldown;
//         [SerializeField] private string _attackType;
//         [SerializeField] private float _attackDelay;
//         [SerializeField] private float _attackTimer = 0f;
//         [SerializeField] private float _experience;

//         // Done
//         [Header("Animator Settings")]
//         [SerializeField] private float _defaultSpeed = 1.0f;
//         [SerializeField] private float _idleTimer = 4f;
//         [SerializeField] private float _targetTransformReferenceDelay = 0.2f;
//         [SerializeField] private float _turnSpeed = 5f;
//         private float _lastTurnSpeed = 0f;


//         [Header("Rob01 Attack Test Settings")]
//         [SerializeField] Transform _bulletSpawnPointLeft;
//         [SerializeField] Transform _bulletSpawnPointRight;
//         [SerializeField] GameObject _bulletPrefab;
//         [SerializeField] private string _bulletKey = "MonsterBullet";
//         [SerializeField] float _bulletSpeed = 5f;
//         [SerializeField] float _particleDelay = 0.2f;

//         [Header("Coroutine")]
//         private Coroutine _rotateCoroutine;
//         private Coroutine _attackCoroutine;
//         private Coroutine _knockbackCoroutine;

//         [Header("Spawn Settings")]
//         [SerializeField] private VisualEffect _spawnEffect;
//         private bool _isSpawned = false;
//         private bool _isInvincible = true;
//         private bool _isFacedToTarget = false;

//         [Header("Hit Settings")]
//         private Vector3 _hitPoint;
//         private bool _isKnockedBack = false;

//         [Header("Dead Settings")]
//         private bool _isDead = false;
//         [SerializeField] private VisualEffect _dieEffect;
//         [SerializeField] private UNI_Materialize _uniMaterialize;
//         // [Header("Dual Renderer Settings")]
//         // [SerializeField] private Renderer[] _bodyRenderers;
//         // [SerializeField] private Renderer[] _vfxRenderers;
//         private RendererController _rendererController;

//         [Header("Audio Settings")]
//         [SerializeField] private MonsterAudioController _monsterAudioController;
//         #endregion

//         #region Setting Methods

//     // Done
//         private void GetComponentReferences()
//         {
//             _agent = GetComponent<NavMeshAgent>();
//             if (_agent == null)
//             {
//                 CustomLogger.LogError("NavMeshAgent component is missing.", this);
//             }

//             _animator = GetComponent<RobotAnimator>();
//             if (_animator == null)
//             {
//                 CustomLogger.LogError("Animator component is missing.", this);
//             }
//             if (_monsterData == null)
//             {
//                 CustomLogger.LogError("MonsterDataSO is not assigned in the inspector.", this);
//             }
//             _agent.updateRotation = false;

//             _target = GameObject.FindGameObjectWithTag("Player")?.transform;
//             if (_target == null)
//             {
//                 CustomLogger.LogError("Target not found. Make sure the target has the 'Player' tag.", this);
//             }

//             if (_spawnEffect == null)
//             {
//                 CustomLogger.LogError("Spawn effect not found.", this);
//             }

//             if (_dieEffect == null)
//             {
//                 CustomLogger.LogError("Die effect not found.", this);
//             }

//             HitBox[] hitBoxes = GetComponentsInChildren<HitBox>();
//             foreach (HitBox hitBox in hitBoxes)
//             {
//                 //hitBox.owner = this;
//                 // 이거 리팩토링때문에 주석으로 바꾼거니까 되돌리려면 반드시 HitBox.cs의 Owner를 MonsterController로 바꿔야함
//             }

//             _rigidbody = GetComponent<Rigidbody>();
//             if (_rigidbody == null)
//             {
//                 CustomLogger.LogError("Rigidbody component is missing.", this);
//             }

//             _rendererController = GetComponent<RendererController>();
//             if (_rendererController == null)
//             {
//                 CustomLogger.LogError("RendererController is not assigned in the inspector.", this);
//             }
//         }
        
//         private void ResetTargetDelay()
//         {
//             _targetTransformReferenceDelay = 0f;
//         }


//     // Done
//         private void SetAgentLayer()
//         {
//             gameObject.layer = _layerAgentMask;
//         }
//     // Done
//         private void SetPhysicsLayer()
//         {
//             gameObject.layer = _layerPhysicsMask;
//         }

//     // Done
//         private void ResetAction()
//         {
//             StopAllCoroutines();
//             _rotateCoroutine = null;
//             _attackCoroutine = null;
//             _knockbackCoroutine = null;

//             _isSpawned = false;
//             _isFacedToTarget = false;
//             _isDead = false;
//             _isKnockedBack = false;

//             _animator.SetDie(false);
//             _animator.SetShoot(false);
//             _animator.SetHitLeft(false);
//             _animator.SetHitRight(false);
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);
//             _animator.SetSpeed(0f);
//             _animator.SetTurns(0f);
//             _animator.RebindAnimator();
//             _animator.ApplyRootMotion();

//             _rigidbody.useGravity = false;
//             if (!_rigidbody.isKinematic)
//             {
//                 _rigidbody.velocity = Vector3.zero;
//                 _rigidbody.angularVelocity = Vector3.zero;
//             }
//             _rigidbody.isKinematic = true;
//             _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

//             _agent.enabled = true;
//             _agent.isStopped = true;
//             _agent.Warp(transform.position);
//             _agent.ResetPath();
//             _agent.speed = _moveSpeed;
//             _agent.stoppingDistance = _attackRange;

//             _attackTimer = 0f;
//             _targetTransformReferenceDelay = 0.2f;
//             _lastTurnSpeed = 0f;

//             _rendererController.SetAllRenderersActive(false);
//         }

//     // Done
//         // private void SetupMonsterStats()
//         // {
//         //     _currentHP = _monsterStats.baseHP;
//         //     _moveSpeed = _monsterStats.moveSpeed;
//         //     _attackDamage = _monsterStats.attackDamage;
//         //     _attackRange = _monsterStats.attackRange;
//         //     _attackCooldown = _monsterStats.attackCooldown;
//         //     _attackDelay = _monsterStats.attackDelay;
//         //     _attackType = _monsterStats.attackType;
//         //     _experience = _monsterStats.experience;
//         // }

//         #endregion

//         #region Unity Methods
//         // Done
//         private void Awake()
//         {
//             // #1
//             GetComponentReferences();
//             _rendererController.SetAllRenderersActive(false);
//             _layerAgentMask = LayerMask.NameToLayer("EnemyAgent");
//             _layerPhysicsMask = LayerMask.NameToLayer("EnemyPhysics");
//         }
//         // Done
//         private void Start()
//         {
//             // #2
//             if (_monsterData == null)
//             {
//                 CustomLogger.LogError("MonsterDataSO is not assigned in the inspector.", this);
//                 return;
//             }

//             _monsterStats = _monsterData.GetMonsterData(_monsterKey);
//             if (_monsterStats.key == null)
//             {
//                 CustomLogger.LogError("Monster data not found.", this);
//                 return;
//             }

//             //SetupMonsterStats();
//             _agent.speed = _moveSpeed;
//             _rigidbody.isKinematic = true;
//             _animator.SetSpeedMultiplier(_defaultSpeed);
//         }
//         // Done
//         private void OnEnable()
//         {
//             // #3
//             //SetupMonsterStats();
//             ResetAction();
            
//             foreach (var col in GetComponentsInChildren<Collider>())
//             {
//                 col.enabled = true;
//             }

//             _target = GameObject.FindGameObjectWithTag("Player")?.transform;
//             if (_target == null)
//             {
//                 CustomLogger.LogError("Target not found. Make sure the target has the 'Player' tag.", this);
//             }

//             if (_target != null)
//             {
//                 Vector3 dir = _target.position - transform.position;
//                 dir.y = 0f;
//                 if (dir != Vector3.zero)
//                 {
//                     transform.rotation = Quaternion.LookRotation(dir);
//                 }
//             }
//             _state = MonsterState.Spawn;
//             ResetTargetDelay();
//             SetAgentLayer();
//             StartCoroutine(SpawnDelayCoroutine());
//         }

//         private void Update()
//         {
//             // Done
//             if (_target == null)
//             {
//                 // _target = GameObject.FindGameObjectWithTag("Player")?.transform;
//                 // CustomLogger.LogError("Target not found. Make sure the target has the 'Player' tag.", this);
//                 return;
//             }

//             if (_isKnockedBack)
//             {
//                 return;
//             }

//             if (_isDead)
//             {
//                 return;
//             }
//             //_targetTransformReferenceDelay -= Time.deltaTime;
//             // if (_targetTransformReferenceDelay <= 0f)
//             // {
//             //     _target = GameObject.FindGameObjectWithTag("Player").transform;
//             //     _targetTransformReferenceDelay = 0.5f;
//             // }
//             // _agent.SetDestination(_target.position);
//             //CustomLogger.LogInfo("Player position: " + _target.position, this);
//             //CustomLogger.LogInfo($"{_target}");

//             switch (_state)
//             {
//                 // Done
//                 case MonsterState.Spawn:
//                     // #6
//                     SetAgentLayer();
//                     UpdateSpawnState();
//                     break;
//                 case MonsterState.Idle:
//                     // #8
//                     SetAgentLayer();
//                     UpdateIdleState();
//                     break;
//                 case MonsterState.Chase:
//                     // #12
//                     // #19
//                     SetAgentLayer();
//                     UpdateChaseState();
//                     break;
//                 case MonsterState.Attack:
//                     // #14
//                     SetAgentLayer();
//                     UpdateAttackState();
//                     break;
//                 case MonsterState.Hit:
//                     // #21
//                     UpdateHitState();
//                     break;
//                 case MonsterState.Dead:
//                     // #28
//                     SetAgentLayer();
//                     UpdateDeadState();
//                     break;
//             }
//         }
//         #endregion

//         #region Spawn State Methods
//         // Done
//         private void UpdateSpawnState()
//         {
//             // #7
//             _animator.SetSpeed(0f);
//             _animator.SetTurns(0f);
//             _agent.isStopped = true;
//         }
//         // Done
//         private IEnumerator SpawnDelayCoroutine()
//         {
//             // #4
//             _spawnEffect.Reinit();
//             _spawnEffect.SendEvent("in-top");
//             _monsterAudioController.PlaySpawnSound();
//             _isInvincible = true;
//             _rendererController.SetAllRenderersActive(false);

//             yield return new WaitForSeconds(_spawnDelay);
//             //CustomLogger.LogInfo("Monster spawned.", this);

//             _isInvincible = false;
//             _rendererController.SetBodyRendererActive(true);
//             _isSpawned = true;
//             _state = MonsterState.Idle;
//             _agent.isStopped = false;
//         }

//         #endregion

//         #region Idle State Methods
//         /// <summary>
//         /// State when the monster is idle.
//         /// </summary>
//         private void UpdateIdleState()
//         {
//             // #9
//             if(_isFacedToTarget)
//             {
//                 return;
//             }
//             //TODO - Refactoring -> Rotate Coroutine
//             if (_rotateCoroutine == null)
//             {
//                 _rotateCoroutine = StartCoroutine(CoStartFacingTarget());
//             }
//             // //_target = GameObject.FindGameObjectWithTag("Player").transform;
//             // _agent.SetDestination(_target.position);
//             // _agent.isStopped = false;
//             // _state = MonsterState.Chase;
//             // CustomLogger.LogInfo("Monster is now chasing the target.", this);

//             // _animator.SetSpeed(0f);
//             // _animator.SetTurns(0f);
//         }

//         // Done
//         private IEnumerator CoStartFacingTarget()
//         {
//             // #10
//             // #27
//             _isFacedToTarget = false;
//             yield return StartCoroutine(CoRotateToTarget());
//             yield return null;

//             //float dist = Vector3.Distance(transform.position, _target.position);

//             if (_agent.remainingDistance <= _attackRange)
//             {
//                 _agent.isStopped = true;
//                 _animator.SetSpeed(0f);
//                 _animator.SetTurns(0f);

//                 _isFacedToTarget = true;
//                 _rotateCoroutine = null;
//                 _state = MonsterState.Attack;
//             }
//             else
//             {
//                 _agent.isStopped = false;
//                 yield return null;
//                 _agent.SetDestination(_target.position);

//                 _animator.SetSpeed(0f);
//                 _animator.SetTurns(0f);

//                 _isFacedToTarget = true;
//                 _rotateCoroutine = null;
//                 _agent.speed = _moveSpeed;
//                 _state = MonsterState.Chase;
//             }
//         }


//         #endregion

//         #region Chase State Methods

//         // Done
//         /// <summary>
//         /// State when the monster is chasing the target.
//         /// </summary>
//         private void UpdateChaseState()
//         {
//             // #13
//             if (_target == null)
//             {
//                 _state = MonsterState.Idle;
//                 return;
//             }
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);

//             _targetTransformReferenceDelay -= Time.deltaTime;
//             if (_targetTransformReferenceDelay <= 0f)
//             {
//                 _targetTransformReferenceDelay = 0.5f;
//                 _agent.SetDestination(_target.position);
//             }

//             float speed = _agent.velocity.magnitude;
//             if (_agent.desiredVelocity.sqrMagnitude < 0.01f)
//             {
//                 _animator.SetSpeed(0f);
//                 _animator.SetTurns(0f);
//             }
//             else
//             {
//                 _animator.SetSpeed(speed);
//             }

//             Vector3 desiredVelocity = _agent.desiredVelocity;

//             if (desiredVelocity.sqrMagnitude > 0.01f)
//             {
//                 float angle = Vector3.SignedAngle(transform.forward, desiredVelocity, Vector3.up);
//                 float normalizedTurn = Mathf.Clamp(angle / 90f, -1f, 1f);
//                 _lastTurnSpeed = Mathf.Lerp(_lastTurnSpeed, normalizedTurn, Time.deltaTime * _turnSpeed);
//                 _animator.SetTurns(_lastTurnSpeed);
//                 Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity);
//                 transform.rotation = Quaternion.Slerp(
//                     transform.rotation,
//                     targetRotation,
//                     Time.deltaTime * _moveSpeed
//                 );
//             }
//             else
//             {
//                 _animator.SetTurns(0f);
//             }

//             if (_agent.remainingDistance <= _attackRange)
//             {
//                 _agent.isStopped = true;
//                 _animator.SetSpeed(0f);
//                 _animator.SetTurns(0f);
//                 _state = MonsterState.Attack;
//                 return;
//             }
//         }

//         #endregion

//         #region Hit State Methods
//         /// <summary>
//         /// When the monster is hit by an attack.
//         /// </summary>
//         /// <param name="damage"></param>
//         /// <param name="hitPoint"></param>
//         public void OnHit(Vector3 hitPoint, float knockbackForce, Vector3 knockbackDirection)
//         {
//             // #20
//             if (_isInvincible)
//             {
//                 CustomLogger.LogInfo("Monster is invincible.", this);
//                 return;
//             }

//             if(_state == MonsterState.Dead)
//             {
//                 CustomLogger.LogInfo("Monster is already dead.", this);
//                 return;
//             }

//             _hitPoint = hitPoint;
//             CustomLogger.LogInfo("OnHit", this);
//             _currentHP -= PlayerDataStatus.Instance.GetPlayerDamage();
            
//             if (_currentHP <= 0)
//             {
//                 _isKnockedBack = false;
//                 StopAllCoroutines();
//                 _agent.enabled = false;
//                 _rigidbody.velocity = Vector3.zero;
//                 _rigidbody.angularVelocity = Vector3.zero;
//                 _rigidbody.isKinematic = true;
//                 _rigidbody.useGravity = false;
//                 _state = MonsterState.Dead;
//                 PlayerDataStatus.Instance.AddExperience(_experience);
//                 CustomLogger.LogInfo("Monster is dead.", this);
//                 return;
//             }

//             CustomLogger.LogInfo($"Monster took {PlayerDataStatus.Instance.GetPlayerDamage()} damage. Current HP: {_currentHP}", this);
//             _state = MonsterState.Hit;

//             PlayHitAnimation(knockbackForce, knockbackDirection);
//         }

//         public void PlayHitAnimation(float knockbackForce, Vector3 knockbackDirection)
//         {
//             // #23
//             StopAllCoroutines();
//             _attackCoroutine = null;
//             _rotateCoroutine = null;
//             _knockbackCoroutine = null;
//             _animator.SetSpeed(0f);
//             _animator.SetTurns(0f);
//             _animator.SetHitRight(false);
//             _animator.SetHitLeft(false);
//             _animator.SetShoot(false);
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);
//             _agent.isStopped = true;

//             Vector3 hitDirection = (_hitPoint - transform.position).normalized;
//             float dot = Vector3.Dot(transform.right, hitDirection);
//             if (dot < 0)
//             {
//                 CustomLogger.LogInfo("Monster is hit from left.", this);
//                 _animator.SetHitLeft(true);
//                 _animator.SetHitRight(false);
//             }
//             else
//             {
//                 CustomLogger.LogInfo("Monster is hit from right.", this);
//                 _animator.SetHitRight(true);
//                 _animator.SetHitLeft(false);
//             }
            
//             StartCoroutine(CoTurnOffHitFlag());
//             Knockback(knockbackForce, knockbackDirection);
//         }

//         /// <summary>
//         /// Prevent multiple hit animations from playing at the same time.
//         /// HitAnimation can be triggered from AnyState. This coroutine will turn off the hit animation transtion after it is played.
//         /// </summary>
//         private IEnumerator CoTurnOffHitFlag()
//         {
//             // #24
//             yield return null;
//             _animator.SetHitLeft(false);
//             _animator.SetHitRight(false);
//         }

//         /// <summary>
//         /// State the hit state when the monster is hit by an attack.
//         /// </summary>
//         private void UpdateHitState()
//         {
//             // #22
//             var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

//             if (stateInfo.IsName("Armature|takeDamageLeft") || stateInfo.IsName("Armature|takeDamageRight"))
//             {
//                 if (stateInfo.normalizedTime >= 1.0f)
//                 {
//                     _animator.SetHitLeft(false);
//                     _animator.SetHitRight(false);
//                 }
//             }
//         }


//         #endregion

//         #region Knockback Methods

//         private void Knockback(float knockbackForce, Vector3 knockbackDirection)
//         {
//             // #25
//             if (_knockbackCoroutine != null)
//             {
//                 StopCoroutine(_knockbackCoroutine);
//             }
//             _knockbackCoroutine = StartCoroutine(CoKnockback(knockbackForce, knockbackDirection));
//         }

//         private IEnumerator CoKnockback(float knockbackForce, Vector3 knockbackDirection)
//         {
//             // #26
//             yield return null;
//             SetPhysicsLayer();
//             _isKnockedBack = true;
//             AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
//             float duration = 1.1f; // Default duration for knockback
//             if (stateInfo.IsName("Armature|takeDamageLeft") || stateInfo.IsName("Armature|takeDamageRight"))
//             {
//                 duration = stateInfo.length * (1.0f - stateInfo.normalizedTime); // Remaining time of the animation
//             }
//             //Vector3 knockbackDir = (transform.position - hitPoint).normalized;
//             knockbackDirection.y = 0f; // Ignore Y-axis for knockback direction

//             _agent.enabled = false;
//             _rigidbody.isKinematic = false;
//             _rigidbody.useGravity = true;
//             _rigidbody.velocity = Vector3.zero; // Reset velocity before applying new one
//             _rigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
//             _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

//             _rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
//             _animator.RemoveRootMotion();

//             CustomLogger.LogInfo($"Monster is knocked back in direction: {knockbackDirection}", this);
//             yield return new WaitForSeconds(duration);
//             yield return null;
//             _animator.SetHitLeft(false);
//             _animator.SetHitRight(false);
            
//             _animator.ApplyRootMotion();
//             _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
//             _rigidbody.isKinematic = true;
//             _rigidbody.useGravity = false;
//             _rigidbody.velocity = Vector3.zero; // Reset velocity after knockback
//             _rigidbody.angularVelocity = Vector3.zero; // Reset angular velocity after knockback

//             _agent.enabled = true;
//             _agent.Warp(transform.position);
//             _agent.nextPosition = transform.position;
            
//             _agent.ResetPath();
//             _agent.SetDestination(_target.position);
//             _agent.stoppingDistance = _attackRange;
//             _agent.isStopped = true;
//             _knockbackCoroutine = null;
//             _state = MonsterState.Idle;
//             _isFacedToTarget = false;
//             _isKnockedBack = false;
//             SetAgentLayer();

//             ResetTargetDelay();
//             yield return StartCoroutine(CoStartFacingTarget());
//             //yield return StartCoroutine(CoStartFacingTarget());
//             // float dist = Vector3.Distance(transform.position, _target.position);
//             // _state = (dist <= _attackRange) ? MonsterState.Attack : MonsterState.Chase;

//             // _agent.ResetPath();
//             // _agent.isStopped = false;
//             // _agent.SetDestination(_target.position);
//             // CustomLogger.LogInfo("Monster is chasing the target.", this);
//             // _state = MonsterState.Chase;

//             // float dist = Vector3.Distance(transform.position, _target.position);
//             // if (dist <= _attackRange)
//             // {
//             //     _agent.ResetPath();
//             //     _agent.isStopped = true;
//             //     CustomLogger.LogInfo("Monster is Attacking the target.", this);
//             //     _state = MonsterState.Attack;
//             // }
//             // else
//             // {
//             //     _agent.ResetPath();
//             //     _agent.SetDestination(_target.position);
//             //     CustomLogger.LogInfo("Monster is chasing the target.", this);
//             //     _state = MonsterState.Chase;
//             // }

//             // _agent.enabled = true;
//             // float dist = Vector3.Distance(transform.position, _target.position);
//             // CustomLogger.LogInfo($"After Knockback: {dist}", this);

//             // if (dist <= _attackRange)
//             // {
//             //     _state = MonsterState.Attack;
//             // }
//             // else
//             // {
//             //     _agent.ResetPath();
//             //     _agent.SetDestination(_target.position);
//             //     _agent.isStopped = false;

//             //     _animator.SetSpeed(0f);
//             //     _animator.SetTurns(0f);

//             //     _state = MonsterState.Chase;
//             // }
//         }

//         #endregion

//         #region Dead State Methods

//         /// <summary>
//         /// State when the monster is dead.
//         /// </summary>
//         private void UpdateDeadState()
//         {
//             // #29
//             if (_isDead)
//             {
//                 return;
//             }
//             _isDead = true;

//             StopAllCoroutines();
//             _animator.SetShoot(false);
//             _animator.SetHitLeft(false);
//             _animator.SetHitRight(false);
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);
//             _animator.SetSpeed(0f);
//             _animator.SetTurns(0f);

//             CustomLogger.LogInfo("Monster is dead.", this);
//             StartCoroutine(CoDieSequence());
//         }

//         IEnumerator CoDieSequence()
//         {
//             // #30
//             _animator.SetDie(true);
//             _rendererController.SetVfxRendererActive(true);
//             if(_dieEffect != null)
//             {
//                 _dieEffect.gameObject.SetActive(true);
//                 _dieEffect.SendEvent("in");
//             }
//             _uniMaterialize.Start_in();

//             foreach (Collider collider in GetComponentsInChildren<Collider>())
//             {
//                 collider.enabled = false;
//             }

//             yield return new WaitForSeconds(1.5f);
//             _rendererController.SetBodyRendererActive(false);
//             yield return new WaitForSeconds(0.2f);
//             _dieEffect.SendEvent("out");
//             _uniMaterialize.Start_out();
//             yield return new WaitForSeconds(1.5f);
//             _rendererController.SetVfxRendererActive(false);
            
//             PoolManager.Instance.ReturnToPool(gameObject, _monsterKey);
//         }

//         #endregion

//         #region Attack State Methods

//         /// <summary>
//         /// State when the monster is attacking the target.
//         /// </summary>
//         private void UpdateAttackState()
//         {
//             // #15
//             if(_attackCoroutine != null)
//             {
//                 return;
//             }

//             if (_target == null)
//             {
//                 _state = MonsterState.Idle;
//                 return;
//             }

//             float dist = Vector3.Distance(transform.position, _target.position);
//             if (dist > _attackRange + 2f)
//             {
//                 _agent.isStopped = false;
//                 _animator.SetShoot(false);
//                 ResetTargetDelay();
//                 _state = MonsterState.Chase;
//                 return;
//             }

//             _attackTimer += Time.deltaTime;
//             if (_attackTimer >= _attackCooldown)
//             {
//                 _attackTimer = 0f;
//                 _agent.isStopped = true;
//                 _animator.SetSpeed(0f);
//                 _animator.SetTurns(0f);
//                 _attackCoroutine = StartCoroutine(CoAttackSequence());
//             }
//             else
//             {
//                 _animator.SetShoot(false);
//             }

//         }

//         IEnumerator CoRotateToTarget()
//         {
//             // #11
//             // #17
//             _agent.isStopped = true;
//             _agent.ResetPath();
//             _agent.velocity = Vector3.zero;
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);
//             _animator.SetSpeed(0f);
//             _animator.SetTurns(0f);

//             Vector3 dir = _target.position - transform.position;
//             dir.y = 0f; // Ignore Y-axis for rotation
//             float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

//             if (Mathf.Abs(angle) <= 20f || !_isFacedToTarget)
//             {
//                 _animator.SetTurnLeft(false);
//                 _animator.SetTurnRight(false);
//                 //CustomLogger.LogInfo("Monster is not turn");
//                 yield break;
//             }

//             bool turnRight = angle < 0f;
//             string turnName = turnRight ? "Armature|turnRight" : "Armature|turnLeft";

//             int steps = Mathf.RoundToInt(Mathf.Abs(angle) / 40f);

//             if (steps == 0)
//                 steps = 1;

//             //CustomLogger.LogInfo($"Monster is turning {turnName} for {steps} steps.", this);
//             for (int i = 0; i < steps; i++)
//             {
//                 if (_state == MonsterState.Hit || _state == MonsterState.Dead || _isKnockedBack)
//                 {
//                     _animator.SetTurnLeft(false);
//                     _animator.SetTurnRight(false);
//                     yield break;
//                 }

//                 if (turnRight)
//                 {
//                     _animator.SetTurnRight(true);
//                 }
//                 else
//                 {
//                     _animator.SetTurnLeft(true);
//                 }
//                 yield return null;

//                 while (true)
//                 {
//                     var st = _animator.GetCurrentAnimatorStateInfo(0);
//                     if (st.IsName(turnName))
//                         break;
//                     yield return null;
//                 }

//                 while (true)
//                 {
//                     var st = _animator.GetCurrentAnimatorStateInfo(0);
//                     if (st.IsName(turnName) && st.normalizedTime >= 1f)
//                         break;
//                     yield return null;
//                 }

//                 if (turnRight)
//                 {
//                     _animator.SetTurnRight(false);
//                 }
//                 else
//                 {
//                     _animator.SetTurnLeft(false);
//                 }

//                 while (true)
//                 {
//                     var st = _animator.GetCurrentAnimatorStateInfo(0);
//                     if (st.IsName("Idle_walking"))
//                         break;
//                     yield return null;
//                 }

//                 yield return null;
//             }
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);
//             //_agent.isStopped = false;
//             yield break;
//         }

//         IEnumerator CoAttackSequence()
//         {
//             // #16
//             if (_rotateCoroutine != null)
//             {
//                 StopCoroutine(_rotateCoroutine);
//             }
//             _rotateCoroutine = StartCoroutine(CoRotateToTarget());

//             yield return _rotateCoroutine;

//             _animator.SetShoot(true);
//             yield return StartCoroutine(CoShootBullet());

//             _attackCoroutine = null;
//         }

//         IEnumerator CoShootBullet()
//         {
//             // #18
//             yield return new WaitForSeconds(_attackDelay);
//             var bulletLeft = PoolManager.Instance.GetFromPool(_bulletKey);
//             if (bulletLeft != null)
//             {
//                 bulletLeft.transform.position = _bulletSpawnPointLeft.position;
//                 bulletLeft.transform.rotation = _bulletSpawnPointLeft.rotation;
//                 bulletLeft.SetActive(true);
//                 Rigidbody rb_bulletLeft = bulletLeft.GetComponent<Rigidbody>();
//                 if (rb_bulletLeft == null)
//                 {
//                     CustomLogger.LogError("Rigidbody component is missing on the bullet prefab.", this);
//                     yield break;
//                 }
//                 else
//                 {
//                     rb_bulletLeft.velocity = Vector3.zero; // Reset velocity before applying new one
//                     rb_bulletLeft.angularVelocity = Vector3.zero; // Reset angular velocity
//                     rb_bulletLeft.velocity = _bulletSpawnPointLeft.forward * _bulletSpeed;
//                 }
//             }
//             else
//             {
//                 CustomLogger.LogError($"No prefab found with key {_bulletKey}.", this);
//             }

//             var bulletRight = PoolManager.Instance.GetFromPool(_bulletKey);
//             if (bulletRight != null)
//             {
//                 bulletRight.transform.position = _bulletSpawnPointRight.position;
//                 bulletRight.transform.rotation = _bulletSpawnPointRight.rotation;
//                 bulletRight.SetActive(true);
//                 Rigidbody rb_bulletRight = bulletRight.GetComponent<Rigidbody>();
//                 if (rb_bulletRight == null)
//                 {
//                     CustomLogger.LogError("Rigidbody component is missing on the bullet prefab.", this);
//                     yield break;
//                 }
//                 else
//                 {
//                     rb_bulletRight.velocity = Vector3.zero; // Reset velocity before applying new one
//                     rb_bulletRight.angularVelocity = Vector3.zero; // Reset angular velocity
//                     rb_bulletRight.velocity = _bulletSpawnPointRight.forward * _bulletSpeed;
//                 }
//             }
//             else
//             {
//                 CustomLogger.LogError($"No prefab found with key {_bulletKey}.", this);
//             }
//         }

//         #endregion
//     }
// }

// //TODO : State Enter and Exit methods for each state
// //TODO : Add monster Die Effect