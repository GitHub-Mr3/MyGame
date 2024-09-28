using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;
public class DarkSoulsConst : Singleton<DarkSoulsConst>
{
    //动画里面是否有事件的标识
    public const string ISINTERACTING = "isInteracting";
    //动画行为树垂直的值
    public const string VERTICAL = "Vertical";
    //动画行为树水平的值
    public const string HORIZONTAL = "Horizontal";

    //轻攻击
    public const string OH_LIGHT_ATTACK_01 = "OH_Light_Attack_01";
    public const string OH_LIGHT_ATTACK_02 = "OH_Light_Attack_02";
    //重攻击
    public const string OH_HEAVY_ATTACK_01 = "OH_Heavy_Attack_01";
    /// <summary>
    /// 跳跃
    /// </summary>
    public const string JUMP = "Jump";
    /// <summary>
    /// 前滚动画名称
    /// </summary>
    public const string ROLLING = "Rolling";
    /// <summary>
    /// 后退动画名称
    /// </summary>
    public const string BACKSTEP = "Backstep";

    public const string EMPTY = "Empty";
    /// <summary>
    /// 角色落地动画
    /// </summary>
    public const string LAND = "Land";
    /// <summary>
    /// 角色在空中的动画
    /// </summary>
    public const string FALLING = "Falling";
    //伤害 
    public const string DAMAGE_01 = "Damage_01";
    //死亡
    public const string DEAD_01 = "Dead_01";

    //玩家tag
    public const string PLYAER = "Player";
    //敌人tat
    public const string ENEMY = "Enemy";
    //边击开关
    public const string CANDOCOMBO = "canDoCombo";

    public const string LEFT_IDLE_01 = "Left_idle_01";
    public const string LEFTARMEMPT = "LeftArmEmpt";
    public const string RIGHT_IDLE_01 = "Right_idle_01";
    public const string RIGHTARMEMPT = "RightArmEmpt";

    public const string EVENT_SETHP = "event_set_hp";
    public const string EVENT_OVER = "event_over";


}
