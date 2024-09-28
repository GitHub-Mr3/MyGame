using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;

public class DebugMgr : Singleton<DebugMgr>
{
    public void Log(object message)
    {
        Debug.Log($"{message}");
    }
    public void LogError(string message)
    {
        Debug.LogError($"{message}");
    }

}
