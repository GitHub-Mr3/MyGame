using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Enemy Actions/Attack Action")]
public class EnemyAttakAction : EnemyAction
{
    //¹¥»÷µÄ´ÎÊý
    public int attackSore = 3;
    //¹¥»÷µÄ»Ö¸´Ê±¼ä
    public float recoveryTime = 2;

    //¹¥»÷µÄ½Ç¶È
    public float maximumAttackAngle = 35;
    public float minimumAttackAngle = -35;
    //¹¥»÷µÄ·¶Î§
    public float minimumDistanceNeededToAttack = 0;
    public float maxmumDistanceNeededToAttack = 3;
}
