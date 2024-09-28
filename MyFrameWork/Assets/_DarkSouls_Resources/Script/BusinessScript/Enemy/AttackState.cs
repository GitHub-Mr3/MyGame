using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override State Tick(EnemyManager enemyManager, EnemyStatus enemyStatus, EnemyAnimatorMgr enemyAnimatorMgr)
    {
        //根据 attack score选择众多攻击之一
        //如果由于角度或者距离不好而无法使用所选攻击，重新选择攻击
        //如果攻击可行的，停止移动并攻击目标 
        //恢复攻击时间 
        //返回战斗状态
        return this;
    }
}
