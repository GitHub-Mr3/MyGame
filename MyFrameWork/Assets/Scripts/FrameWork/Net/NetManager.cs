using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;
using System.Linq;

/// <summary>
/// 网络管理器 - 负责TCP连接和消息处理
/// 优化点：修复内存泄漏、添加线程安全、改进异常处理
/// </summary>
public class NetManager : Singleton<NetManager>
{
    private Socket socket;
    private byte[] readBuff = new byte[4096]; // 增大缓冲区
    private int buffCount = 0;
    private readonly Queue<string> msgQueue = new Queue<string>();
    private readonly object queueLock = new object();
    private bool isDisposed = false;

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ip">服务器IP地址</param>
    /// <param name="port">服务器端口</param>
    public void Connect(string ip, int port)
    {
        if (isDisposed)
        {
            DebugMgr.Instance.LogError("NetManager已释放，无法连接");
            return;
        }

        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(ip, port, ConnectCallback, socket);
            DebugMgr.Instance.Log($"开始连接服务器: {ip}:{port}");
        }
        catch (Exception ex)
        {
            DebugMgr.Instance.LogError($"连接失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 连接回调
    /// </summary>
    private void ConnectCallback(IAsyncResult ar)
    {
        if (isDisposed) return;

        try
        {
            Socket _socket = (Socket)ar.AsyncState;
            _socket.EndConnect(ar);
            DebugMgr.Instance.Log("连接服务器成功");
            BeginReceive();
        }
        catch (Exception ex)
        {
            DebugMgr.Instance.LogError($"连接回调失败: {ex.Message}");
            Disconnect();
        }
    }

    /// <summary>
    /// 开始接收数据
    /// </summary>
    private void BeginReceive()
    {
        if (socket == null || !socket.Connected || isDisposed) return;

        try
        {
            socket.BeginReceive(readBuff, buffCount, readBuff.Length - buffCount, 
                SocketFlags.None, ReceiveCallback, socket);
        }
        catch (Exception ex)
        {
            DebugMgr.Instance.LogError($"开始接收失败: {ex.Message}");
            Disconnect();
        }
    }

    /// <summary>
    /// 接收数据回调
    /// </summary>
    private void ReceiveCallback(IAsyncResult ar)
    {
        if (isDisposed) return;

        try
        {
            Socket _socket = (Socket)ar.AsyncState;
            int count = _socket.EndReceive(ar);

            if (count <= 0)
            {
                DebugMgr.Instance.Log("连接已断开");
                Disconnect();
                return;
            }

            buffCount += count;

            // 处理完整的消息
            ProcessReceivedData();

            // 继续接收
            BeginReceive();
        }
        catch (ObjectDisposedException)
        {
            // Socket已释放，正常情况
            DebugMgr.Instance.Log("Socket已释放");
        }
        catch (Exception ex)
        {
            DebugMgr.Instance.LogError($"接收数据失败: {ex.Message}");
            Disconnect();
        }
    }

    /// <summary>
    /// 处理接收到的数据
    /// </summary>
    private void ProcessReceivedData()
    {
        while (buffCount >= 2) // 至少需要2字节的消息长度头
        {
            short msgLength = BitConverter.ToInt16(readBuff, 0);
            
            // 检查消息长度是否合理
            if (msgLength <= 0 || msgLength > 10240) // 限制最大消息长度
            {
                DebugMgr.Instance.LogError($"无效的消息长度: {msgLength}");
                buffCount = 0; // 清空缓冲区
                return;
            }

            int totalMsgSize = msgLength + 2; // 包含2字节长度头

            if (buffCount < totalMsgSize)
            {
                // 消息不完整，等待更多数据
                break;
            }

            // 提取消息内容
            string message = Encoding.UTF8.GetString(readBuff, 2, msgLength);
            
            // 线程安全地添加到队列
            lock (queueLock)
            {
                msgQueue.Enqueue(message);
            }

            DebugMgr.Instance.Log($"收到消息: length={msgLength}, content={message}");

            // 移动剩余数据到缓冲区开头
            if (buffCount > totalMsgSize)
            {
                Array.Copy(readBuff, totalMsgSize, readBuff, 0, buffCount - totalMsgSize);
            }
            buffCount -= totalMsgSize;
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="message">要发送的消息</param>
    public void Send(string message)
    {
        if (socket == null || !socket.Connected || isDisposed)
        {
            DebugMgr.Instance.LogError("无法发送消息：连接未建立或已断开");
            return;
        }

        try
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(message);
            short msgLength = (short)msgBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(msgLength);
            
            byte[] sendBuffer = new byte[2 + msgBytes.Length];
            Array.Copy(lengthBytes, 0, sendBuffer, 0, 2);
            Array.Copy(msgBytes, 0, sendBuffer, 2, msgBytes.Length);

            socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendCallback, socket);
            DebugMgr.Instance.Log($"发送消息: {message}");
        }
        catch (Exception ex)
        {
            DebugMgr.Instance.LogError($"发送消息失败: {ex.Message}");
            Disconnect();
        }
    }

    /// <summary>
    /// 发送回调
    /// </summary>
    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket _socket = (Socket)ar.AsyncState;
            int count = _socket.EndSend(ar);
            // 发送成功，无需特殊处理
        }
        catch (Exception ex)
        {
            DebugMgr.Instance.LogError($"发送回调失败: {ex.Message}");
            Disconnect();
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        if (socket != null)
        {
            try
            {
                socket.Close();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                DebugMgr.Instance.LogError($"断开连接时出错: {ex.Message}");
            }
            finally
            {
                socket = null;
            }
        }
        buffCount = 0;
        DebugMgr.Instance.Log("连接已断开");
    }

    /// <summary>
    /// Unity Update方法 - 处理消息队列
    /// </summary>
    public void Update()
    {
        if (isDisposed) return;

        string message = null;
        
        // 线程安全地从队列获取消息
        lock (queueLock)
        {
            if (msgQueue.Count > 0)
            {
                message = msgQueue.Dequeue();
            }
        }

        if (message != null)
        {
            EventMgr.Instance.Emit(Constant.LISTERMSG, message);
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (!isDisposed)
        {
            Disconnect();
            isDisposed = true;
            DebugMgr.Instance.Log("NetManager已释放");
        }
    }
}