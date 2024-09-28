using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mr3;


public class PlayerStatus : CharacteStats
{

    AnimatorHandler animatorHandler;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }
    public void SetPlayerStatus(int curHP,float maxHP)
    {
        curHpValue = curHP;
        maxHpValue = maxHP;
        EventMgr.Instance.Emit(DarkSoulsConst.EVENT_SETHP, 1.0f);
    }
    public void SetDamage(int damage)
    {
        curHpValue -= damage;
        float value = curHpValue / maxHpValue;
        if (value < 0)
        {
            value = 0;
        }

        EventMgr.Instance.Emit(DarkSoulsConst.EVENT_SETHP, value);
        if (value > 0)
        {
            animatorHandler.PlayerTargetAnimation(DarkSoulsConst.DAMAGE_01, true);
        }
        else
        {
            StartCoroutine(GameOver());
            animatorHandler.PlayerTargetAnimation(DarkSoulsConst.DEAD_01, true);
        }
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3f);

        EventMgr.Instance.Emit(DarkSoulsConst.EVENT_OVER, false);
    }

}
