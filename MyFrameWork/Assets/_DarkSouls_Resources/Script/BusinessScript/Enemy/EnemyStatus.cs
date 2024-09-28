using Mr3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : CharacteStats
{
    Animator animator;
    Transform enemyModeTrs;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyModeTrs = transform.GetChild(0);
    }
    public void SetDamage(int damage)
    {
        curHpValue -= damage;
        float value = curHpValue / maxHpValue;
        if (value < 0)
        {
            value = 0;
        }
        if (value > 0)
        {
            animator.Play(DarkSoulsConst.DAMAGE_01);
            enemyModeTrs.localPosition = Vector3.zero;
        }
        else
        {
            animator.Play(DarkSoulsConst.DEAD_01);
            StartCoroutine(GameOver());
            enemyModeTrs.localPosition = Vector3.zero;
        }
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        animator.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        EventMgr.Instance.Emit(DarkSoulsConst.EVENT_OVER, true);
    }


}
