using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoSteelRift.Scripts.Logger;

namespace ScriptableObjects.MonsterData
{
    [CreateAssetMenu(fileName = "MonsterDataSO", menuName = "ScriptableObjects/MonsterData", order = 1)]
    public class MonsterDataSO : ScriptableObject
    {
        [System.Serializable]
        public struct MonsterItem
        {
            [Tooltip("A unique key to identify this monster.")]
            public string key;

            [Tooltip("Base HP of the monster.")]
            public int baseHP;

            [Tooltip("Movement speed of the monster.")]
            public float moveSpeed;

            [Tooltip("Monster Experience")]
            public float experience;
        }

        [Header("Monsters settings")]
        [Tooltip("List of (key, monster) pairs.")]
        public List<MonsterItem> _monsterDataList;

        public MonsterItem GetMonsterData(string key)
        {
            foreach (var monster in _monsterDataList)
            {
                if (monster.key == key)
                {
                    return monster;
                }
            }
            CustomLogger.LogError($"Monster with key {key} not found.", this);
            return default(MonsterItem);
        }
    }
}
