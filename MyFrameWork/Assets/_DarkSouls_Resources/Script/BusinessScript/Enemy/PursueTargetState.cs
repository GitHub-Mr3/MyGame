using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueTargetState : State
{
    public override State Tick(EnemyManager enemyManager, EnemyStatus enemyStatus, EnemyAnimatorMgr enemyAnimatorMgr)
    {
        //追逐目标
        //如果在攻击范围内，则返回战斗姿态
        //如果目标超出攻击范围，则返回此状态并继续追逐目标
        return this;

    }
}
