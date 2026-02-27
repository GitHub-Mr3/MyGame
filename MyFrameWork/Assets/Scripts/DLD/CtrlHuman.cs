using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制角色类，处理玩家输入和本地操作
/// </summary>
public class CtrlHuman : BaseHuman
{
    /// <summary>
    /// 设置玩家唯一ID
    /// </summary>
    /// <param name="id">用户ID</param>
    public void SetUserID(string id)
    {
        base.Start();
        DebugMgr.Instance.Log($"CtrlHuman SetUserID {id} ");
        base.SetUserID(id);
    }

    /// <summary>
    /// 初始化玩家信息
    /// </summary>
    /// <param name="pos">初始位置</param>
    public void SetInitInfos(Vector3 pos)
    {
        base.SetInitInfo(pos);
    }

    /// <summary>
    /// 每帧更新玩家输入检测
    /// </summary>
    new void Update()
    {
        base.Update();
        
        // 左键点击地面移动
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider != null)
            {
                if (hit.collider.tag == Constant.TERRAIN)
                {
                    // 发送移动消息到服务器
                    JsonData moveJson = new JsonData();
                    moveJson["userId"] = userId;
                    moveJson["movePos_x"] = hit.point.x;
                    moveJson["movePos_y"] = hit.point.y;
                    moveJson["movePos_z"] = hit.point.z;
                    NetMgr.Instance.SendMst(NetMgr.MSG_MOVE, moveJson);
                }
            }
        }
        
        // 右键播放悲伤动画
        if (Input.GetMouseButtonDown(2))
        {
            PlayAnimByTrigger(Constant.SAD);
        }
        
        // 中键播放胜利动画
        if (Input.GetMouseButtonDown(1))
        {
            Win();
            return;
            
            // 以下代码为预留的视线检测逻辑（当前未启用）
            RaycastHit hit;
            Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
            Vector3 lineStart = lineEnd + 20 * transform.forward;
            if (Physics.Linecast(lineStart, lineEnd, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj == gameObject)
                {
                    return;
                }
                SyncHuma h = hitObj.GetComponent<SyncHuma>();
                if (h == null)
                {
                    return;
                }
                Win();
                DebugMgr.Instance.LogError("检测到目标");
            }
        }
    }
}