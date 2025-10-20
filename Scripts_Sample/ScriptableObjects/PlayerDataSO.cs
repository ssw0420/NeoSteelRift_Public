using UnityEngine;
using Managers;

namespace ScriptableObjects.PlayerData
{
    [CreateAssetMenu(fileName = "PlayerDataSO", menuName = "ScriptableObjects/PlayerDataSO")]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("Health")]
        [Tooltip("최대 체력 수치")]
        public float maxHealth = 100f;

        [Tooltip("현재 체력 수치")]
        public float currentHealth = 100f;

        [Header("Experience")]
        [Tooltip("현재 레벨")]
        public int level = 1;

        [Tooltip("현재 경험치")]
        public float currentExp = 0;

        [Tooltip("다음 레벨까지 필요한 경험치")]
        public float expToNextLevel = 100;

        [Header("Stats")]
        [Tooltip("플레이어 공격력")]
        public int attackPower = 10;

        [Header("킬 카운트")]
        [Tooltip("몬스터 처치 수")]
        public int killCount = 0;
    }
}
