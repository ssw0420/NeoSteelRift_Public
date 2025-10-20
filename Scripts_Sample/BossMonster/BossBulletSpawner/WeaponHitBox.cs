using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.BossMonster.BossWeapon
{
    public class WeaponHitBox : MonoBehaviour
    {
        [SerializeField] private BossBulletSpawnerController _owner;
        public BossBulletSpawnerController owner
        {
            get => _owner;
            set => _owner = value;
        }
    }
}