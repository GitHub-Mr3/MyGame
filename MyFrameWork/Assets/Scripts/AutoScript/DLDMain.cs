using UnityEngine;

using UnityEngine.UI;

using TMPro;

using UnityEngine.EventSystems;

using System;
using Mr3;
using System.Collections.Generic;
using LitJson;

public class DLDMain : MonoBehaviour
{
    /// <summary>
    /// 是否已建立网络监听
    /// </summary>
    bool isAddNetLister = false;
    /// <summary>
    /// 自己的UserID
    /// </summary>
    private string selfUserID = "";
    /// <summary>
    /// 角色模型
    /// </summary>
    public GameObject _humanObj = null;

    /// <summary>
    /// 自己玩家的脚本
    /// </summary>
    CtrlHuman ctrlHuman;

    /// <summary>
    /// 记录场景中所有玩家的数据
    /// </summary>
    Dictionary<string, BaseHuman> allBaseHumanByUserID = new Dictionary<string, BaseHuman>();
    /// <summary>
    /// 场景中所有玩家模型obj
    /// </summary>
    Dictionary<string, GameObject> humanObjDic = new Dictionary<string, GameObject>();

    //autoStart
    public Transform Tran_PlayerParent = null;

    private void Awake()
    {
        Tran_PlayerParent = gameObject.transform.Find("Tran_PlayerParent");

    }
    //autoEnd
    //TODO  
    private void Start()
    {
        InitGame();
    }
    /// <summary>
    /// 初始化游戏
    /// </summary>
    void InitGame()
    {
        //BitConverter
        EventMgr.Instance.Init();
        NetMgr.Instance.Init();
        isAddNetLister = true;
        AddLister();
        InitNetModule();

    }
    void AddLister()
    {
        EventMgr.Instance.AddListener(Constant.LISTER_INITINFO, Lister_InitInfo);
        EventMgr.Instance.AddListener(Constant.LISTER_SYNCHUMAN, ListerSyncHuman);
        EventMgr.Instance.AddListener(Constant.LISTER_MOVE, ListerMove);
        EventMgr.Instance.AddListener(Constant.LISTER_ENDMOVE, ListerEndMove);
        EventMgr.Instance.AddListener(Constant.LISTER_HUMAN_LEAVE, ListerHumanLeave);

    }
    #region 事件监听
    void Lister_InitInfo(string evetnName, object _data)
    {
        string userID = (string)_data;
        CreateHumanObj(Vector3.zero, true, userID);
    }
    void ListerSyncHuman(string eventName, object _data)
    {
        JsonData data = (JsonData)_data;
        RefreshOtherHuma(data);
    }
    void ListerMove(string eventName, object _data)
    {
        JsonData data = (JsonData)_data;
        OnMsgMove(data);
    }
    void ListerEndMove(string eventName, object _data)
    {
        JsonData data = (JsonData)_data;
        OnMsgEndMove(data);
    }

    void ListerHumanLeave(string eventName, object _data)
    {
        string userID = (string)_data;
        OnMsgHumanLeave(userID);
    }
    #endregion



    /// <summary>
    /// 初始网络模块
    /// </summary>
    void InitNetModule()
    {
        NetManager.Instance.Connection("127.0.0.1", 10086);
    }
    /// <summary>
    /// 玩家离场
    /// </summary>
    /// <param name="leaveData"></param>
    void OnMsgHumanLeave(string leaveData)
    {
        if (humanObjDic.ContainsKey(leaveData))
        {
            humanObjDic[leaveData].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 开始移动
    /// </summary>
    /// <param name="moveData"></param>
    void OnMsgMove(JsonData moveData)
    {
        var userId = moveData[Constant.USER_ID].ToString();
        var movePos_x = float.Parse(moveData[Constant.MOVEPOS_X].ToString());
        var movePos_y = float.Parse(moveData[Constant.MOVEPOS_Y].ToString());
        var movePos_z = float.Parse(moveData[Constant.MOVEPOS_Z].ToString());
        if (allBaseHumanByUserID.ContainsKey(userId))
        {
            Vector3 moveEndPos = Vector3.zero;
            moveEndPos.x = movePos_x;
            moveEndPos.y = movePos_y;
            moveEndPos.z = movePos_z;
            allBaseHumanByUserID[userId].MoveTo(moveEndPos);
        }
    }
    /// <summary>
    /// 结束移动
    /// </summary>
    /// <param name="moveData"></param>
    void OnMsgEndMove(JsonData moveData)
    {
        var userId = moveData[Constant.USER_ID].ToString();
        if (allBaseHumanByUserID.ContainsKey(userId))
        {
            allBaseHumanByUserID[userId].EndMove();
        }
    }


    /// <summary>
    /// 创建其他玩家的obj
    /// </summary>
    /// <param name="humandata"></param>
    void RefreshOtherHuma(JsonData humandata)
    {
        string otehrID = humandata[Constant.USER_ID].ToString();
        if (otehrID == selfUserID)
        {
            return;
        }
        Vector3 pos = Vector3.zero;
        if (humandata.Keys.Contains(Constant.MOVEPOS_X))
        {
            var movePos_x = float.Parse(humandata[Constant.MOVEPOS_X].ToString());
            var movePos_y = float.Parse(humandata[Constant.MOVEPOS_Y].ToString());
            var movePos_z = float.Parse(humandata[Constant.MOVEPOS_Z].ToString());
            pos.x = movePos_x;
            pos.y = movePos_y;
            pos.z = movePos_z;
        }

        CreateHumanObj(pos, false, otehrID);
    }
    /// <summary>
    /// 创建玩家OBJ
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="isSelf"></param>
    /// <param name="userID"></param>
    void CreateHumanObj(Vector3 pos, bool isSelf, string userID)
    {
        string humanKey;
        GameObject player = GetNotInUseObj(out humanKey);
        if (player == null)
        {
            player = GameObject.Instantiate(_humanObj, Tran_PlayerParent);
        }
        if (isSelf)
        {
            if (player.GetComponent<CtrlHuman>() == null)
            {
                ctrlHuman = player.AddComponent<CtrlHuman>();
            }
            player.transform.name = $"Player|{userID}";
            ctrlHuman.SetUserID(userID);
            ctrlHuman.SetInitInfos(pos);
            allBaseHumanByUserID[userID] = ctrlHuman;
            selfUserID = userID;
        }
        else
        {
            SyncHuma syncHuma = player.GetComponent<SyncHuma>();
            if (syncHuma == null)
            {
                syncHuma = player.AddComponent<SyncHuma>();
            }
            player.transform.name = $"OtherPlayer|{userID}";
            syncHuma.SetUserID(userID);
            syncHuma.SetInitInfos(pos);
            allBaseHumanByUserID[userID] = syncHuma;

        }
        humanObjDic[userID] = player;
        player.transform.position = pos;
        player.SetActive(true);

        if (!string.IsNullOrEmpty(humanKey))
        {
            humanObjDic.Remove(humanKey);
            allBaseHumanByUserID.Remove(humanKey);
        }

    }

    /// <summary>
    /// 获取没有使用的OBJ
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    GameObject GetNotInUseObj(out string userid)
    {
        foreach (var item in humanObjDic)
        {
            if (!item.Value.activeSelf)
            {
                userid = item.Key;
                return item.Value;
            }
        }
        userid = string.Empty;
        return null;
    }
    private void OnDisable()
    {
        EventMgr.Instance.RemoveListener(Constant.LISTER_INITINFO, Lister_InitInfo);
        EventMgr.Instance.RemoveListener(Constant.LISTER_SYNCHUMAN, ListerSyncHuman);
        EventMgr.Instance.RemoveListener(Constant.LISTER_MOVE, ListerMove);
        EventMgr.Instance.RemoveListener(Constant.LISTER_ENDMOVE, ListerEndMove);
        EventMgr.Instance.RemoveListener(Constant.LISTER_HUMAN_LEAVE, ListerHumanLeave);
    }
    private void Update()
    {
        if (isAddNetLister)
        {
            NetManager.Instance.Update();
        }
    }
}
