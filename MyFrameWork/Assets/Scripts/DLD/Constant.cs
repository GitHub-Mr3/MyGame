using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏常量定义类
/// </summary>
public class Constant
{
    // 动画状态常量
    public const string RUN = "Run";
    public const string WALK = "Walk";
    public const string WIN = "Win";
    public const string SAD = "Sad";
    public const string JUMP = "Jump";
    public const string IDLE = "Idle";
    public const string TERRAIN = "Terrain";

    // 网络消息字段常量
    public const string MOVEPOS_X = "movePos_x";
    public const string MOVEPOS_Y = "movePos_y";
    public const string MOVEPOS_Z = "movePos_z";
    public const string USER_ID = "userId";

    // 事件监听器常量
    /// <summary>
    /// 网络消息监听器
    /// </summary>
    public const string LISTERMSG = "ListerMsg";
    
    /// <summary>
    /// 初始化玩家信息监听器
    /// </summary>
    public const string LISTER_INITINFO = "lister_initinfo";
    
    /// <summary>
    /// 同步其他玩家信息监听器
    /// </summary>
    public const string LISTER_SYNCHUMAN = "lister_synchuman";
    
    /// <summary>
    /// 玩家移动监听器
    /// </summary>
    public const string LISTER_MOVE = "lister_move";
    
    /// <summary>
    /// 玩家移动结束监听器
    /// </summary>
    public const string LISTER_ENDMOVE = "lister_endmove";
    
    /// <summary>
    /// 玩家离开场景监听器
    /// </summary>
    public const string LISTER_HUMAN_LEAVE = "lister_human_leave";
}