using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SSW.Monster;
using Managers;
using SSW.Monster.Attack;

[CreateAssetMenu(fileName = "Rob01Attack", menuName = "ScriptableObjects/Attack/Rob01Attack")]
public class Rob01AttackSO : AttackSO
{
    [Header("Ranged Specific Stats")]
    public string bulletKey = "MonsterBullet";
    public float bulletSpeed = 5f;
    public override IEnumerator CoExecute(MonsterControllerRefactored owner)
    {
        Rob01AttackHandler attackHandler = owner.GetComponent<Rob01AttackHandler>();
        if (attackHandler == null)
        {
            Debug.LogError("Rob01AttackHandler component not found on the owner.");
            yield break;
        }

        owner.Animator.SetShoot(true);
        yield return new WaitForSeconds(attackDelay);

        FireBulletFrom(attackHandler.BulletSpawnPointLeft);
        FireBulletFrom(attackHandler.BulletSpawnPointRight);
    }

    private void FireBulletFrom(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        GameObject bullet = PoolManager.Instance.GetFromPool(bulletKey);
        if (bullet != null)
        {
            bullet.transform.position = spawnPoint.position;
            bullet.transform.rotation = spawnPoint.rotation;
            MonsterBullet monsterBullet = bullet.GetComponent<MonsterBullet>();
            if (monsterBullet != null)
            {
                monsterBullet.Initialize(_attackDamage);
            }

            if (bullet.TryGetComponent(out Rigidbody rb_bullet))
            {
                rb_bullet.velocity = Vector3.zero; // Reset velocity before applying new one
                rb_bullet.angularVelocity = Vector3.zero; // Reset angular velocity
                rb_bullet.velocity = spawnPoint.forward * bulletSpeed;
            }
        }
    }
}
