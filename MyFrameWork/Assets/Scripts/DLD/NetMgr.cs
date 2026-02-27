using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;
using LitJson;

/// <summary>
/// 网络管理器，处理客户端与服务器的消息通信
/// </summary>
public class NetMgr : Singleton<NetMgr>
{
    // 客户端发送消息类型
    public const string MSG_MOVE = "msg_move";
    public const string MSG_ENDMOVE = "msg_end_move";
    
    /// <summary>
    /// 初始化玩家信息消息
    /// </summary>
    public const string MSG_INITINFO = "msg_init_user_info";
    
    /// <summary>
    /// 同步其他玩家信息消息
    /// </summary>
    public const string MSG_SYNCHUMA = "msg_sync_huma";
    
    /// <summary>
    /// 玩家离开场景消息
    /// </summary>
    public const string MSG_HUMAN_LEAVE = "msg_human_leave";

    /// <summary>
    /// 发送网络消息到服务器
    /// </summary>
    /// <param name="msgId">消息ID</param>
    /// <param name="msgData">消息数据（JsonData格式）</param>
    public void SendMst(string msgId, JsonData msgData)
    {
        string msgDataStr = msgData.ToJson();
        string msg = $"{msgId}|{msgDataStr}";
        NetManager.Instance.Send(msg);
    }
    
    /// <summary>
    /// 初始化网络监听器
    /// </summary>
    public void Init()
    {
        EventMgr.Instance.AddListener(Constant.LISTERMSG, ListerMsg);
    }

    /// <summary>
    /// 网络消息监听回调
    /// </summary>
    /// <param name="evetnName">事件名称</param>
    /// <param name="obj">事件参数</param>
    void ListerMsg(string evetnName, object obj)
    {
        string msg = (string)obj;
        DebugMgr.Instance.Log($"evetnName {evetnName} msg:{msg}");
        switch (evetnName)
        {
            case Constant.LISTERMSG:
                AnalyzeMsg(msg);
                break;
            default:
                break;
        }
    }
    
    /// <summary>
    /// 解析网络消息并分发到对应事件
    /// </summary>
    /// <param name="msg">原始消息字符串（格式：msgId|jsonData）</param>
    void AnalyzeMsg(string msg)
    {
        var msgData = msg.Split('|');
        if (msgData.Length < 2)
        {
            DebugMgr.Instance.Log($"消息数据长度异常: {msg}");
            return;
        }
        var msgID = msgData[0];
        var msgContent = msgData[1];
        DebugMgr.Instance.Log($"消息解析成功，msgID {msgID} msgContent {msgContent}");
        switch (msgID)
        {
            case MSG_INITINFO:
                EventMgr.Instance.Emit(Constant.LISTER_INITINFO, msgContent);
                break;
            case MSG_SYNCHUMA:
                JsonData json_syncdata = JsonMapper.ToObject(msgContent);
                EventMgr.Instance.Emit(Constant.LISTER_SYNCHUMAN, json_syncdata);
                break;
            case MSG_MOVE:
                JsonData json_value = JsonMapper.ToObject(msgContent);
                EventMgr.Instance.Emit(Constant.LISTER_MOVE, json_value);
                break;
            case MSG_ENDMOVE:
                JsonData endMove = JsonMapper.ToObject(msgContent);
                EventMgr.Instance.Emit(Constant.LISTER_ENDMOVE, endMove);
                break;
            case MSG_HUMAN_LEAVE:
                EventMgr.Instance.Emit(Constant.LISTER_HUMAN_LEAVE, msgContent);
                break;
            default:
                break;
        }
    }
}