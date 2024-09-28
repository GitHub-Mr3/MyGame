using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant
{
    public const string RUN = "Run";
    public const string WALK = "Walk";
    public const string WIN = "Win";
    public const string SAD = "Sad";
    public const string JUMP = "Jump";
    public const string IDLE = "Idle";
    public const string TERRAIN = "Terrain";

    public const string MOVEPOS_X = "movePos_x";
    public const string MOVEPOS_Y = "movePos_y";
    public const string MOVEPOS_Z = "movePos_z";
    public const string USER_ID = "userId";

    /// <summary>
    /// 监听消息
    /// </summary>
    public const string LISTERMSG = "ListerMsg";
    /// <summary>
    ///监听 初始化自己的数据
    /// </summary>
    public const string LISTER_INITINFO = "lister_initinfo";
    /// <summary>
    /// 监听 其他玩家的数据
    /// </summary>
    public const string LISTER_SYNCHUMAN = "lister_synchuman";
    /// <summary>
    /// 监听玩家移动
    /// </summary>
    public const string LISTER_MOVE = "lister_move";
    /// <summary>
    /// 监听玩家移动结束
    /// </summary>
    public const string LISTER_ENDMOVE = "lister_endmove";
    /// <summary>
    /// 监听玩家离场
    /// </summary>
    public const string LISTER_HUMAN_LEAVE = "lister_human_leave";
}
