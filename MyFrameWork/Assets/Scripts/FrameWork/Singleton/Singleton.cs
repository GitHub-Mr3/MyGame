using System;
using UnityEngine;

namespace Mr3
{
    /// <summary>
    /// 创建普通单例 - 线程安全的单例基类
    /// 支持延迟初始化和资源清理
    /// </summary>
    /// <typeparam name="T">单例类型，必须有无参构造函数</typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _disposed = false;

        /// <summary>
        /// 单例实例属性
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(typeof(T).Name);
                }

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null && !_disposed)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取单例实例的便捷方法
        /// </summary>
        /// <returns>单例实例</returns>
        public static T GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// 释放单例实例（用于资源清理）
        /// 注意：调用后再次访问 Instance 会抛出 ObjectDisposedException
        /// </summary>
        public static void DisposeInstance()
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    // 如果单例实现了 IDisposable，调用 Dispose
                    if (_instance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    _instance = null;
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// 检查单例是否已被释放
        /// </summary>
        /// <returns>是否已释放</returns>
        public static bool IsDisposed()
        {
            return _disposed;
        }

        /// <summary>
        /// 重置单例状态（用于测试或特殊场景）
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _instance = null;
                _disposed = false;
            }
        }
    }
}