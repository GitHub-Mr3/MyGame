using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;

/// <summary>
/// 协程管理器 - 提供安全的协程执行和管理功能
/// </summary>
public class CoroutineHelper : SingleMonoBase<CoroutineHelper>
{
    // 存储正在运行的协程
    private readonly Dictionary<string, Coroutine> _runningCoroutines = new Dictionary<string, Coroutine>();

    /// <summary>
    /// 执行协程并可选地存储引用以便后续管理
    /// </summary>
    /// <param name="name">协程名称（用于管理和取消）</param>
    /// <param name="enumerator">协程迭代器</param>
    /// <param name="storeReference">是否存储协程引用以供后续管理</param>
    /// <returns>协程引用（如果storeReference为true）</returns>
    public Coroutine DoFunc(string name, IEnumerator enumerator, bool storeReference = false)
    {
        if (enumerator == null)
        {
            DebugMgr.Instance.LogError($"CoroutineHelper.DoFunc: enumerator is null for {name}");
            return null;
        }

        Coroutine coroutine = StartCoroutine(enumerator);

        if (storeReference)
        {
            if (_runningCoroutines.ContainsKey(name))
            {
                // 如果已存在同名协程，先停止它
                StopCoroutine(_runningCoroutines[name]);
                _runningCoroutines[name] = coroutine;
            }
            else
            {
                _runningCoroutines.Add(name, coroutine);
            }
        }

        return coroutine;
    }

    /// <summary>
    /// 停止指定名称的协程
    /// </summary>
    /// <param name="name">协程名称</param>
    /// <returns>是否成功停止</returns>
    public bool StopCoroutineByName(string name)
    {
        if (_runningCoroutines.TryGetValue(name, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                _runningCoroutines.Remove(name);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 停止所有存储的协程
    /// </summary>
    public void StopAllStoredCoroutines()
    {
        foreach (var coroutine in _runningCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        _runningCoroutines.Clear();
    }

    /// <summary>
    /// 检查指定名称的协程是否正在运行
    /// </summary>
    /// <param name="name">协程名称</param>
    /// <returns>是否正在运行</returns>
    public bool IsCoroutineRunning(string name)
    {
        return _runningCoroutines.ContainsKey(name);
    }

    /// <summary>
    /// 清理所有资源
    /// </summary>
    public void Cleanup()
    {
        StopAllStoredCoroutines();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Cleanup();
    }
}