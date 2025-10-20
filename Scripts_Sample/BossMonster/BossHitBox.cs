using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster
{
    public enum BossHitBoxType
    {
        Body = 0,
        LeftArm = 1,
        RightArm = 2,
    }
    public class BossHitBox : MonoBehaviour
    {
        [SerializeField] private BossHitHandler _owner;
        [SerializeField] private BossHitBoxType _partType;
        public BossHitHandler owner
        {
            get => _owner;
            set => _owner = value;
        }
        public BossHitBoxType partType
        {
            get => _partType;
            set => _partType = value;
        }
    }
}