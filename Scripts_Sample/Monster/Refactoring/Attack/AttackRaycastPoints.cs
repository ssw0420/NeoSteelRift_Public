using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.Monster.Attack
{
    /// <summary>
    /// Struct to hold raycast points for attack detection
    /// </summary>
    [System.Serializable]
    public class AttackRaycastPoints : MonoBehaviour
    {
        public List<Transform> RaycastOrigins;
    }
}