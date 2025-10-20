using SSW.BossMonster;
using UnityEngine;

namespace SSW.HitSystem
{
    public struct HitData
    {
        public float damage;
        public float knockbackForce; 
        public Vector3 knockbackDirection;
        public Vector3 hitPoint;
        public string attackType;
        public BossHitBoxType? partType;

        // Constructor for convenience
        public HitData(float damage, float knockbackForce, Vector3 knockbackDirection, Vector3 hitPoint, string attackType = "Normal", BossHitBoxType? partType = null)
        {
            this.damage = damage;
            this.knockbackForce = knockbackForce;
            this.knockbackDirection = knockbackDirection;
            this.hitPoint = hitPoint;
            this.attackType = attackType;
            this.partType = partType;
        }
    }
}