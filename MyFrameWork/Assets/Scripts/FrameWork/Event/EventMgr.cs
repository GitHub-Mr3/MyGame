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
        // 事件委托定义
        public delegate void OnEventAction(string eventName, object userData);
        
        // 事件监听器字典
        private Dictionary<string, OnEventAction> _eventActions = new Dictionary<string, OnEventAction>();

        /// <summary>
        /// 初始化事件管理器
        /// </summary>
        public void Init()
        {
            _eventActions.Clear();
        }

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="onEvent">事件回调函数</param>
        public void AddListener(string eventName, OnEventAction onEvent)
        {
            if (string.IsNullOrEmpty(eventName) || onEvent == null)
            {
                Debug.LogWarning("AddListener: Invalid parameters");
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
        }

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="onEvent">事件回调函数</param>
        public void RemoveListener(string eventName, OnEventAction onEvent)
        {
            if (string.IsNullOrEmpty(eventName) || onEvent == null || _eventActions == null)
            {
                return;
            }

            if (_eventActions.ContainsKey(eventName))
            {
                _eventActions[eventName] -= onEvent;
                
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
                Debug.LogWarning("Emit: Event name is null or empty");
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
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Event '{eventName}' handler threw exception: {ex}");
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
        /// 清理所有事件监听器
        /// </summary>
        public void ClearAllListeners()
        {
            _eventActions?.Clear();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ClearAllListeners();
            _eventActions = null;
        }
    }
}