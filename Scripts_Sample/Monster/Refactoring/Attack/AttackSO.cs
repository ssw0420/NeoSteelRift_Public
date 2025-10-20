using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SSW.Monster;

public abstract class AttackSO : ScriptableObject
{
    [Header("Common Attack Stats")]
    public float attackRange = 10f;
    public float attackCooldown = 3f;
    public float attackDelay = 0.5f;
    public int _attackDamage = 10;

    // This is the core method that executes the attack logic.
    // 'owner' is the monster that is performing this attack.
    public abstract IEnumerator CoExecute(MonsterControllerRefactored owner);
}