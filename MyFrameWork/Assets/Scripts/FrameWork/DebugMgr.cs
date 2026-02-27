using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;

/// <summary>
/// 调试管理器 - 统一处理日志输出
/// </summary>
public class DebugMgr : Singleton<DebugMgr>
{
    /// <summary>
    /// 输出普通日志信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Log(object message)
    {
        Debug.Log($"{message}");
    }
    
    /// <summary>
    /// 输出错误日志信息
    /// </summary>
    /// <param name="message">错误信息</param>
    public void LogError(string message)
    {
        Debug.LogError($"{message}");
    }
}