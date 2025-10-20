using System.Collections;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;
using SSW.Monster;
using SSW.Monster.States;
using ScriptableObjects.PlayerDataStatus;

namespace SSW.HitSystem
{
    public class MonsterHitProcessor : IHitProcessor
    {

        public bool CanBeHit(MonsterControllerRefactored controller)
        {
            return !controller.IsInvincible && !controller.IsDead;
        }
        public IEnumerator CoProcessHit(MonsterControllerRefactored controller, HitData hitData)
        {
            controller.ChangeState(MonsterState.Hit);
            CustomLogger.LogInfo("Hit Processing Started", controller);
            controller.TakeDamage(hitData.damage);

            if (controller.CurrentHP <= 0)
            {
                controller.SetIsDead(true);
                controller.ChangeState(MonsterState.Dead);
                PlayerDataStatus.Instance.AddKill();
                PlayerDataStatus.Instance.AddExp(controller.Experience);
                yield break;
            }

            ProcessHitReaction(controller, hitData);
            yield return null; // Wait for one frame to ensure the hit animation is triggered

            CustomLogger.LogInfo($"After yield null - ShouldApplyKnockback: {ShouldApplyKnockback(controller, hitData)}, AttackType: {hitData.attackType}", controller);

            if (ShouldApplyKnockback(controller, hitData))
            {
                yield return controller.StartCoroutine(CoProcessKnockback(controller, hitData));
            }
            else
            {
                AnimatorStateInfo stateInfo = controller.Animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Armature|takeDamageLeft") || stateInfo.IsName("Armature|takeDamageRight"))
                {
                    yield return new WaitForSeconds(stateInfo.length);
                }
            }

            CustomLogger.LogInfo("Hit processing completed", controller);
            if (!controller.IsKnockedBack && !controller.IsDead)
            {
                controller.SetIsFacedToTarget(false);
                controller.SetAgentLayer();
                controller.ChangeState(MonsterState.Idle);
            }
        }


        private void ProcessHitReaction(MonsterControllerRefactored controller, HitData hitData)
        {
            if (controller.Agent.enabled)
            {
                controller.Agent.isStopped = true;
            }
            
            controller.Animator.SetSpeed(0f);
            controller.Animator.SetTurns(0f);
            // controller.Animator.SetTurnLeft(false);
            // controller.Animator.SetTurnRight(false);
            // controller.Animator.SetShoot(false);
            // controller.Animator.SetHitLeft(false);
            // controller.Animator.SetHitRight(false);

            // Hit Direction for Animation
            Vector3 toHitPoint = (hitData.hitPoint - controller.transform.position);
            toHitPoint.y = 0f;
            toHitPoint = toHitPoint.normalized;
            

            float dot = Vector3.Dot(controller.transform.right, toHitPoint);

            CustomLogger.LogInfo($"Hit Direction - toHitPoint: {toHitPoint}, dot: {dot}", controller);

            if (dot < 0)
            {

                CustomLogger.LogInfo("Hit on right side - playing right hit animation", controller);
                controller.Animator.SetHitRight(true);
                controller.Animator.SetHitLeft(false);
            }
            else
            {
                
                CustomLogger.LogInfo("Hit on left side - playing left hit animation", controller);
                controller.Animator.SetHitLeft(true);
                controller.Animator.SetHitRight(false);
            }
            controller.StartCoroutine(CoTurnOffHitFlag(controller));
        }

        /// <summary>
        /// Prevent multiple hit animations from playing at the same time.
        /// HitAnimation can be triggered from AnyState. This coroutine will turn off the hit animation transtion after it is played.
        /// </summary>
        private IEnumerator CoTurnOffHitFlag(MonsterControllerRefactored controller)
        {
            yield return null;
            controller.Animator.SetHitLeft(false);
            controller.Animator.SetHitRight(false);
        }


        public IEnumerator CoProcessKnockback(MonsterControllerRefactored controller, HitData hitData)
        {
            controller.SetIsKnockedBack(true);
            controller.SetPhysicsLayer();
            AnimatorStateInfo stateInfo = controller.Animator.GetCurrentAnimatorStateInfo(0);
            float duration = 1.1f; // Default duration for knockback
            if (stateInfo.IsName("Armature|takeDamageLeft") || stateInfo.IsName("Armature|takeDamageRight"))
            {
                duration = stateInfo.length * (1.0f - stateInfo.normalizedTime); // Remaining time of the animation
            }
            //Vector3 knockbackDir = (transform.position - hitPoint).normalized;
            hitData.knockbackDirection.y = 0f; // Ignore Y-axis for knockback direction

            controller.Agent.enabled = false;
            controller.EnableObstacle();
            controller.Rigidbody.isKinematic = false;
            controller.Rigidbody.useGravity = true;
            controller.Rigidbody.velocity = Vector3.zero; // Reset velocity before applying new one
            controller.Rigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
            controller.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

            CustomLogger.LogInfo($"Applying knockback - Force: {hitData.knockbackForce}, Direction: {hitData.knockbackDirection}", controller);
            controller.Rigidbody.AddForce(hitData.knockbackDirection * hitData.knockbackForce, ForceMode.Impulse);
            controller.Animator.RemoveRootMotion();

            yield return new WaitForSeconds(duration);
            yield return null;
            controller.Animator.SetHitLeft(false);
            controller.Animator.SetHitRight(false);

            controller.Animator.ApplyRootMotion();
            controller.Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            controller.Rigidbody.velocity = Vector3.zero; // Reset velocity after knockback
            controller.Rigidbody.angularVelocity = Vector3.zero; // Reset angular velocity after knockback
            controller.Rigidbody.isKinematic = true;
            controller.Rigidbody.useGravity = false;

            yield return null;
            controller.DisableObstacle();
            controller.Agent.enabled = true;
            controller.Agent.Warp(controller.transform.position);
            controller.Agent.nextPosition = controller.transform.position;

            controller.Agent.ResetPath();
            controller.Agent.SetDestination(controller.Target.position);
            controller.Agent.stoppingDistance = controller.AttackRange;
            controller.Agent.isStopped = true;
            controller.ChangeState(MonsterState.Idle);
            controller.SetIsFacedToTarget(false);
            controller.SetIsKnockedBack(false);
            controller.SetAgentLayer();
        }

        private bool ShouldApplyKnockback(MonsterControllerRefactored controller, HitData hitData)
        {
            // Boss monster does not get knocked back
            if (controller.MonsterType.Contains("Boss"))
            {
                return false;
            }

            // Prevent multiple knockbacks
            if (controller.IsKnockedBack)
            {
                CustomLogger.LogInfo("Monster is already knocked back, preventing additional knockback", controller);
                return false;
            }

            // Only skill attacks apply knockback
            return hitData.attackType == "Skill";
        }
        
    }
}
