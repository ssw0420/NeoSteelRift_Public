using System.Collections;
using System.Collections.Generic;
using NeoSteelRift.Scripts.Logger;
using SSW.Monster;
using ScriptableObjects.PlayerDataStatus;
using UnityEngine;
using SSW.HitSystem;

namespace SSW.BossMonster
{
    public class BossHitHandler : MonoBehaviour, IDamageable
    {
        private BTBlackboard _blackboard;
        void Awake()
        {
            _blackboard = GetComponent<BTBlackboard>();
            if (_blackboard == null) return;

            BossHitBox[] hitBoxes = GetComponentsInChildren<BossHitBox>();

            foreach (BossHitBox hitbox in hitBoxes)
            {
                hitbox.owner = this;
            }
        }

        public void OnHit(HitData hitData)
        {
            if (_blackboard == null) return;
            if (!hitData.partType.HasValue) return;
            CustomLogger.LogInfo($"Boss OnHit: {hitData.damage}, PartType: {hitData.partType}", this);
            _blackboard.TakeDamage((int)hitData.damage, hitData.partType.Value);
        }

        // public void OnHit(BossHitBoxType partType)
        // {
        //     if (_blackboard == null) return;
        //     CustomLogger.LogInfo($"Boss OnHit: {PlayerDataStatus.Instance.GetPlayerDamage()}", this);
        //     _blackboard.TakeDamage(PlayerDataStatus.Instance.GetPlayerDamage(), partType);
        // }
    }
}