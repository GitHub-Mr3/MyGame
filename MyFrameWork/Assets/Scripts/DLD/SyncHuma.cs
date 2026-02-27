using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 同步人类角色类，用于处理其他玩家角色的网络同步
/// </summary>
public class SyncHuma : BaseHuman
{
    /// <summary>
    /// 每帧更新逻辑（继承自BaseHuman）
    /// </summary>
    new void Update()
    {
        base.Update();
    }
    
    /// <summary>
    /// 设置同步角色的唯一ID
    /// </summary>
    /// <param name="id">用户ID</param>
    public void SetUserID(string id)
    {
        base.Start();
        base.SetUserID(id);
    }
    
    /// <summary>
    /// 初始化同步角色的位置信息
    /// </summary>
    /// <param name="pos">初始位置</param>
    public void SetInitInfos(Vector3 pos)
    {
        base.SetInitInfo(pos);
    }
}