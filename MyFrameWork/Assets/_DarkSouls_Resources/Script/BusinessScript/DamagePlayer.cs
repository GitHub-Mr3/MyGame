using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
        if (playerStatus!=null)
        {
            playerStatus.SetDamage(damage);
        }
        EnemyStatus enemyStatus = other.GetComponent<EnemyStatus>();
        if (enemyStatus!=null)
        {
            enemyStatus.SetDamage(damage);
        }
    }
}
