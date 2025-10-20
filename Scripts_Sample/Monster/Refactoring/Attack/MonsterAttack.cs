using UnityEngine;
using Managers;
using ScriptableObjects.MonsterData;
using ScriptableObjects.PlayerDataStatus;

namespace SSW.Monster
{
    public class MonsterAttack : MonoBehaviour
    {
        private float _attackDamage;

        public void Initialize(int damage)
        {
            _attackDamage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerDataStatus.Instance.TakeDamage(_attackDamage);
            }
        }
    }
}
