using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;
using LitJson;

public class NetMgr : Singleton<NetMgr>
{
    public const string MSG_MOVE = "msg_move";
    public const string MSG_ENDMOVE = "msg_end_move";
    /// <summary>
    /// 初始化自己的数据
    /// </summary>
    public const string MSG_INITINFO = "msg_init_user_info";
    /// <summary>
    /// 同步其他玩家的数据
    /// </summary>
    public const string MSG_SYNCHUMA = "msg_sync_huma";
    /// <summary>
    /// 有玩家离场
    /// </summary>
    public const string MSG_HUMAN_LEAVE = "msg_human_leave";

    public void SendMst(string msgId, JsonData msgData)
    {
        string msgDataStr = msgData.ToJson();
        string msg = $"{msgId}|{msgDataStr}";
        NetManager.Instance.Send(msg);
    }
    public void Init()
    {
        EventMgr.Instance.AddListener(Constant.LISTERMSG, ListerMsg);
    }

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
    void AnalyzeMsg(string msg)
    {
        var msgData = msg.Split('|');
        if (msgData.Length < 2)
        {
            DebugMgr.Instance.Log($"解析数据长度错误: {msg}");
            return;
        }
        var msgID = msgData[0];
        var msgContent = msgData[1];
        DebugMgr.Instance.Log($"解析数据成功：msgID {msgID} msgContent {msgContent}");
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
