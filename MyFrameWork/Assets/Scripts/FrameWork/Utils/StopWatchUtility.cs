using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// stopwatch类的工具类，用于计算运行一段代码所用的时间
/// </summary>
public static class StopWatchUtility
{
    /// <summary>
    /// 获取执行一段代码所用的时间信息
    /// </summary>
    /// <param name="call"></param>
    /// <returns></returns>
    public static TimeSpan GetTime(UnityAction call)
    {
        //声明计时器
        Stopwatch timer = Stopwatch.StartNew();
        //开启计时器
        timer.Start();

        //要测试的代码
        call?.Invoke();


        //停止计时器
        timer.Stop();
        //返回时间信息
        return timer.Elapsed;
    }
    /// <summary>
    /// 在控制台打印执行一段代码所用的时间
    /// </summary>
    /// <param name="call">要执行的代码</param>
    /// <param name="executionNum">执行的次数</param>
    public static void PrintTime(UnityAction call, int executionNum = 1)
    {
        if (executionNum <= 0)
        {
            executionNum = 1;
        }
        //用于记录所用的总时间,单位是毫秒
        double totalMilliseconds = 0;
        for (int i = 0; i < executionNum; i++)
        {
            totalMilliseconds += GetTime(call).TotalMilliseconds;
        }
        DebugMgr.Instance.Log($"执行这段代码{executionNum}次所需要的时间是：{totalMilliseconds / 1000}");
    }

}
