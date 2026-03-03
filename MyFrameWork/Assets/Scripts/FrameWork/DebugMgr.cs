using System;
using System.Collections.Generic;
using UnityEngine;
using Mr3;

/// <summary>
/// 调试管理器 - 统一处理日志输出
/// 支持多级日志、文件输出和运行时配置
/// </summary>
public class DebugMgr : Singleton<DebugMgr>
{
    // 日志级别枚举
    public enum LogLevel
    {
        Verbose = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }

    // 当前日志级别（可运行时调整）
    private LogLevel _currentLogLevel = LogLevel.Debug;

    // 日志缓存（用于文件输出）
    private readonly Queue<string> _logBuffer = new Queue<string>();
    private const int MAX_LOG_BUFFER_SIZE = 1000;

    /// <summary>
    /// 设置当前日志级别
    /// </summary>
    /// <param name="level">日志级别</param>
    public void SetLogLevel(LogLevel level)
    {
        _currentLogLevel = level;
        Log($"[DebugMgr] 日志级别设置为: {level}");
    }

    /// <summary>
    /// 获取当前日志级别
    /// </summary>
    /// <returns>当前日志级别</returns>
    public LogLevel GetLogLevel()
    {
        return _currentLogLevel;
    }

    #region 日志输出方法

    /// <summary>
    /// 输出详细日志信息（最低级别）
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Verbose(object message)
    {
        if (_currentLogLevel <= LogLevel.Verbose)
        {
            string logMessage = $"[VERBOSE] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            Debug.Log(logMessage);
            AddToLogBuffer(logMessage);
        }
    }

    /// <summary>
    /// 输出调试日志信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Debug(object message)
    {
        if (_currentLogLevel <= LogLevel.Debug)
        {
            string logMessage = $"[DEBUG] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            Debug.Log(logMessage);
            AddToLogBuffer(logMessage);
        }
    }

    /// <summary>
    /// 输出普通日志信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public void Log(object message)
    {
        if (_currentLogLevel <= LogLevel.Info)
        {
            string logMessage = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            Debug.Log(logMessage);
            AddToLogBuffer(logMessage);
        }
    }

    /// <summary>
    /// 输出警告日志信息
    /// </summary>
    /// <param name="message">警告信息</param>
    public void LogWarning(string message)
    {
        if (_currentLogLevel <= LogLevel.Warning)
        {
            string logMessage = $"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            Debug.LogWarning(logMessage);
            AddToLogBuffer(logMessage);
        }
    }

    /// <summary>
    /// 输出错误日志信息
    /// </summary>
    /// <param name="message">错误信息</param>
    public void LogError(string message)
    {
        if (_currentLogLevel <= LogLevel.Error)
        {
            string logMessage = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            Debug.LogError(logMessage);
            AddToLogBuffer(logMessage);
        }
    }

    /// <summary>
    /// 输出致命错误日志信息
    /// </summary>
    /// <param name="message">致命错误信息</param>
    public void LogFatal(string message)
    {
        string logMessage = $"[FATAL] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
        Debug.LogError(logMessage);
        AddToLogBuffer(logMessage);
        
        // 致命错误可能需要特殊处理
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    #endregion

    #region 日志缓冲区管理

    /// <summary>
    /// 添加日志到缓冲区
    /// </summary>
    /// <param name="logMessage">日志消息</param>
    private void AddToLogBuffer(string logMessage)
    {
        _logBuffer.Enqueue(logMessage);
        if (_logBuffer.Count > MAX_LOG_BUFFER_SIZE)
        {
            _logBuffer.Dequeue();
        }
    }

    /// <summary>
    /// 获取所有缓存的日志
    /// </summary>
    /// <returns>日志列表</returns>
    public List<string> GetAllLogs()
    {
        return new List<string>(_logBuffer);
    }

    /// <summary>
    /// 清空日志缓冲区
    /// </summary>
    public void ClearLogBuffer()
    {
        _logBuffer.Clear();
    }

    /// <summary>
    /// 将日志保存到文件
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns>是否成功</returns>
    public bool SaveLogsToFile(string filename = null)
    {
        if (string.IsNullOrEmpty(filename))
        {
            filename = $"debug_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        }

        try
        {
            string logContent = string.Join("\n", _logBuffer);
            System.IO.File.WriteAllText(filename, logContent);
            Log($"日志已保存到文件: {filename}");
            return true;
        }
        catch (Exception ex)
        {
            LogError($"保存日志文件失败: {ex.Message}");
            return false;
        }
    }

    #endregion

    /// <summary>
    /// 检查是否应该记录指定级别的日志
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <returns>是否应该记录</returns>
    public bool ShouldLog(LogLevel level)
    {
        return _currentLogLevel <= level;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        ClearLogBuffer();
    }
}