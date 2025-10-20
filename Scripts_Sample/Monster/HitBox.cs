using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.Monster
{
        public class HitBox : MonoBehaviour
    {
        [SerializeField] private MonsterControllerRefactored _owner;
        public MonsterControllerRefactored owner
        {
            get => _owner;
            set => _owner = value;
        }
    }
}