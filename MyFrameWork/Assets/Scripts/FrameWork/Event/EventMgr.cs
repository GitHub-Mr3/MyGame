using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mr3
{
    /// <summary>
    /// 事件管理器 - 提供全局事件分发系统
    /// 支持添加/移除监听器、事件触发和资源清理
    /// </summary>
    public class EventMgr : Singleton<EventMgr>
    {
        // 事件委托定义（支持泛型）
        public delegate void OnEventAction(string eventName, object userData);
        public delegate void OnEventAction<T>(string eventName, T userData);

        // 事件监听器字典
        private readonly Dictionary<string, OnEventAction> _eventActions = new Dictionary<string, OnEventAction>();
        
        // 监听器计数器（用于性能监控）
        private readonly Dictionary<string, int> _listenerCounts = new Dictionary<string, int>();

        /// <summary>
        /// 初始化事件管理器
        /// </summary>
        public void Init()
        {
            ClearAllListeners();
            Debug.Log("[EventMgr] 初始化完成");
        }

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="onEvent">事件回调函数</param>
        public void AddListener(string eventName, OnEventAction onEvent)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning("[EventMgr] AddListener: 事件名称为空");
                return;
            }
            
            if (onEvent == null)
            {
                Debug.LogWarning($"[EventMgr] AddListener: 事件回调为空 - {eventName}");
                return;
            }

            if (_eventActions.ContainsKey(eventName))
            {
                _eventActions[eventName] += onEvent;
            }
            else
            {
                _eventActions[eventName] = onEvent;
            }
            
            // 更新监听器计数
            _listenerCounts[eventName] = _listenerCounts.GetValueOrDefault(eventName, 0) + 1;
            
            // 警告：过多监听器
            if (_listenerCounts[eventName] > 100)
            {
                Debug.LogWarning($"[EventMgr] 事件 '{eventName}' 有 {_listenerCounts[eventName]} 个监听器，可能存在内存泄漏风险");
            }
        }

        /// <summary>
        /// 添加泛型事件监听器
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="onEvent">事件回调函数</param>
        public void AddListener<T>(string eventName, OnEventAction<T> onEvent)
        {
            AddListener(eventName, (name, data) =>
            {
                if (data is T typedData)
                {
                    onEvent?.Invoke(name, typedData);
                }
                else
                {
                    Debug.LogError($"[EventMgr] 事件数据类型不匹配: 期望 {typeof(T)}, 实际 {data?.GetType() ?? typeof(object)}");
                }
            });
        }

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="onEvent">事件回调函数</param>
        public void RemoveListener(string eventName, OnEventAction onEvent)
        {
            if (string.IsNullOrEmpty(eventName) || onEvent == null)
            {
                return;
            }

            if (_eventActions.TryGetValue(eventName, out OnEventAction eventAction))
            {
                _eventActions[eventName] = eventAction - onEvent;
                
                // 更新监听器计数
                if (_listenerCounts.ContainsKey(eventName))
                {
                    _listenerCounts[eventName]--;
                    if (_listenerCounts[eventName] <= 0)
                    {
                        _listenerCounts.Remove(eventName);
                    }
                }
                
                // 如果没有监听器了，移除该事件键
                if (_eventActions[eventName] == null)
                {
                    _eventActions.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="userData">传递的数据</param>
        public void Emit(string eventName, object userData = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning("[EventMgr] Emit: 事件名称为空");
                return;
            }

            if (_eventActions.TryGetValue(eventName, out OnEventAction eventAction))
            {
                if (eventAction != null)
                {
                    try
                    {
                        eventAction(eventName, userData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[EventMgr] 事件 '{eventName}' 处理异常: {ex}");
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否存在指定事件的监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns>是否存在监听器</returns>
        public bool HasListener(string eventName)
        {
            return !string.IsNullOrEmpty(eventName) && 
                   _eventActions.ContainsKey(eventName) && 
                   _eventActions[eventName] != null;
        }

        /// <summary>
        /// 获取指定事件的监听器数量
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns>监听器数量</returns>
        public int GetListenerCount(string eventName)
        {
            return _listenerCounts.GetValueOrDefault(eventName, 0);
        }

        /// <summary>
        /// 清理所有事件监听器
        /// </summary>
        public void ClearAllListeners()
        {
            _eventActions.Clear();
            _listenerCounts.Clear();
            Debug.Log("[EventMgr] 所有事件监听器已清理");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ClearAllListeners();
        }
    }
}