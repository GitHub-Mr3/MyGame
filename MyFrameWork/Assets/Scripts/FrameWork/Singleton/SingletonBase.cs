using System;
using UnityEngine;

/// <summary>
/// 线程安全的单例基类（非MonoBehaviour）
/// 支持延迟初始化和实例清理
/// </summary>
/// <typeparam name="T">继承此基类的类型</typeparam>
public abstract class SingletonBase<T> where T : SingletonBase<T>, new()
{
    // 线程锁对象
    private static readonly object _lock = new object();
    
    // 单例实例（volatile确保多线程可见性）
    private static volatile T _instance;
    
    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// 清理单例实例（用于测试或重置场景）
    /// </summary>
    public static void ClearInstance()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }
    
    /// <summary>
    /// 检查实例是否存在
    /// </summary>
    public static bool HasInstance => _instance != null;
}