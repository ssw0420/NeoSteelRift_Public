// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;

// namespace SSW.Monster
// {
//     public class MonsterRotationController : MonoBehaviour
//     {
//         private bool _isFacedToTarget;
//         public bool IsFacedToTarget => _isFacedToTarget;
//         private Coroutine _rotateCoroutine;
//         private NavMeshAgent _agent;
//         private RobotAnimator _animator;
//         private MonsterControllerRefactored _controller;
//         private Transform _target;
//         private void Start()
//         {
//             _controller = GetComponent<MonsterControllerRefactored>();
//             _agent = _controller.Agent;
//             _animator = _controller.Animator;
//             _target = _controller.Target;

//         }

//         public void StartRotateToTarget()
//         {
//             StopRotateToTarget();
//             _rotateCoroutine = StartCoroutine(CoRotateToTarget());
//         }

//         public void StopRotateToTarget()
//         {
//             if (_rotateCoroutine != null)
//             {
//                 StopCoroutine(_rotateCoroutine);
//                 _rotateCoroutine = null;
//             }

//             if (_controller != null && _controller.Agent != null && !_controller.Agent.isActiveAndEnabled)
//             {
//                 _controller.Agent.enabled = true;
//             }
//         }

//         public Coroutine StartRotateToTargetCoroutine()
//         {
//             StopRotateToTarget();
//             _rotateCoroutine = StartCoroutine(CoRotateToTarget());
//             return _rotateCoroutine;
//         }

//         public IEnumerator CoRotateToTarget()
//         {
//             _isFacedToTarget = false;
//             //_agent.isStopped = true;
//             _agent.ResetPath();
//             _agent.velocity = Vector3.zero;
//             _animator.SetTurnLeft(false);
//             _animator.SetTurnRight(false);
//             _animator.SetSpeed(0f);
//             _animator.SetTurns(0f);

//             Vector3 dir = _target.position - transform.position;
//             dir.y = 0f; // Ignore Y-axis for rotation
//             float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

//             if (Mathf.Abs(angle) <= 20f)
//             {
//                 _animator.SetTurnLeft(false);
//                 _animator.SetTurnRight(false);
//                 //CustomLogger.LogInfo("Monster is not turn");
//                 yield break;
//             }

//             bool turnRight = angle < 0f;
//             string turnName = turnRight ? "Armature|turnRight" : "Armature|turnLeft";

//             int steps = Mathf.RoundToInt(Mathf.Abs(angle) / 25f);

//             if (steps == 0)
//             {
//                 steps = 1;
//             }

//             for (int i = 0; i < steps; i++)
//             {
//                 if (_controller.GetCurrentState() == MonsterState.Hit || _controller.GetCurrentState() == MonsterState.Dead || _controller.IsKnockedBack)
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
//                     if (st.IsName(turnName)) break;
//                     yield return null;
//                 }

//                 while (true)
//                 {
//                     var st = _animator.GetCurrentAnimatorStateInfo(0);
//                     if (st.IsName(turnName) && st.normalizedTime >= 1f) break;
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
//             yield break;
//         }
//     }
// }