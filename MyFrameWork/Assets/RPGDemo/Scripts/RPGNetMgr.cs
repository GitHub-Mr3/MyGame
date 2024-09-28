using Mr3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGNetMgr : Singleton<RPGNetMgr>
{
    public void AddNetListener()
    {
        EventMgr.Instance.AddListener(FrameConst.LISTERMSG, ListerMsg);
    }

    void ListerMsg(string evetnName, object obj)
    {
        string msg = (string)obj;
        DebugMgr.Instance.Log($"evetnName {evetnName} msg:{msg}");
    }
    public void OnRemoveNetListener()
    {
        EventMgr.Instance.RemoveListener(FrameConst.LISTERMSG, ListerMsg);
    }
}

