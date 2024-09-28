using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public int curWeaponDamage = 10;
    Collider damageCollider;

    public bool isEnemy = true;
    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
        curWeaponDamage = 10;
    }
    public void SetDamageColliderEnable(bool isEnable)
    {
        damageCollider.enabled = isEnable;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == DarkSoulsConst.PLYAER)
        {
            if (isEnemy)
            {
                PlayerStatus playerStatus = collider.GetComponent<PlayerStatus>();
                if (playerStatus != null)
                {
                    playerStatus.SetDamage(curWeaponDamage);
                }
            }
        }
        if (collider.tag == DarkSoulsConst.ENEMY)
        {
            if (!isEnemy)
            {
                EnemyStatus enemyStatus = collider.GetComponent<EnemyStatus>();
                if (enemyStatus != null)
                {
                    enemyStatus.SetDamage(curWeaponDamage);
                }
            }
        }
    }
}
