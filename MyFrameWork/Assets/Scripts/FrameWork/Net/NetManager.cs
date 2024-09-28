using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;
using System.Net.Sockets;
using System;
using System.Text;
using System.Linq;

public class NetManager : Singleton<NetManager>
{
    Socket socket;
    //接收缓冲区
    byte[] readBuff = new byte[1024];
    //接收缓冲区的数据长度
    int buffCount = 0;
    string recvStr = "";

    //消息列表
    static List<String> msgList = new List<string>();
    /// <summary>
    /// 建立连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Connection(string ip, int port)
    {
        //创建Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }
    /// <summary>
    /// 连接回调
    /// </summary>
    /// <param name="ar"></param>
    void ConnectCallback(IAsyncResult ar)
    {
        Socket _socket = (Socket)ar.AsyncState;
        _socket.EndConnect(ar);
        socket.BeginReceive(readBuff, buffCount, 1024, SocketFlags.None, ReceiveCallBack, _socket);
    }
    /// <summary>
    /// 处理接收到服务器的消息
    /// </summary>
    /// <param name="socket"></param>
    public void OnReceiveData(Socket socket)
    {
        try
        {
            //取出byte数组前两位(存放的消息长度)
            short readLength = BitConverter.ToInt16(readBuff, 0);
            //根据消息长度取出对应的消息数据
            recvStr = Encoding.UTF8.GetString(readBuff, 2, readLength);
            //+2 是因为前面两位存放的是消息长度的值 总共的长度=消息长度标识+消息长度 例(11msg_testmsg)11是消息长度。msg_testmsg是消息数据
            readLength += 2;
            //将取出来的消息添加到消息队列中
            msgList.Add(recvStr);
            DebugMgr.Instance.Log($"客户端接收的消息 ： length {readLength} Msg:" + recvStr);
            // 创建新的字节数组，长度为原始数组长度减去要移除的字节数
            byte[] newArray = new byte[1024];

            // 复制原始数组中从第numBytesToRemove个字节开始到末尾的数据到新数组中
            Array.Copy(readBuff, readLength, newArray, 0, readBuff.Length - readLength);
            //消息长度减去已经取出的长度
            buffCount -= readLength;
            //重新赋值消息数据
            readBuff = newArray;
            if (buffCount <= 2)
            {
                //当长度消息2，代表剩余的消息长度不能取出消息，则继续监听消息
                socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, socket);
                return;
            }
            //如果还有可以 取的消息，则继续取
            OnReceiveData(socket);
        }
        catch (Exception e)
        {
            DebugMgr.Instance.LogError("OnReceiveData:" + e);
            throw;
        }

    }
    /// <summary>
    /// 监听服务器发送过来的消息
    /// </summary>
    /// <param name="ar"></param>
    void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket _socket = (Socket)ar.AsyncState;
            int count = _socket.EndReceive(ar);
            if (count <= 0)
            {
                DebugMgr.Instance.LogError("count is 0" + count);
                return;
            }
            buffCount += count;
            OnReceiveData(_socket);

        }
        catch (Exception e)
        {
            DebugMgr.Instance.LogError("ReceiveCallBack" + e);
            throw;
        }
    }
    /// <summary>
    /// 消息发送
    /// </summary>
    /// <param name="str"></param>
    public void Send(string str)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(str);
        socket.BeginSend(bodyBytes, 0, bodyBytes.Length, SocketFlags.None, SendCallBack, socket);
    }
    /// <summary>
    /// 消息发送回调
    /// </summary>
    /// <param name="ar"></param>
    void SendCallBack(IAsyncResult ar)
    {
        Socket _socket = (Socket)ar.AsyncState;
        int count = _socket.EndSend(ar);
    }
    /// <summary>
    /// 在业务Update调用，有消息就会分发，可以 在主线程使用数据
    /// </summary>
    public void Update()
    {
        if (msgList.Count > 0)
        {
            String msgStr = msgList[0];
            //DebugMgr.Instance.LogError("Update :" + msgStr);
            msgList.RemoveAt(0);
            EventMgr.Instance.Emit(Constant.LISTERMSG, msgStr);
        }
    }
}
