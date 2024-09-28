using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using ThirdParty.Json.LitJson;
using System.Numerics;

namespace TcpServer
{
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }

    class Program
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 10086);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            //Accept
            listenfd.BeginAccept(AcceptCallback, listenfd);
            //等待
            Console.ReadLine();
        }
        //连接
        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("[服务器]Accept 连接成功");
                Socket listenfd = (Socket)ar.AsyncState;
                Socket clientfd = listenfd.EndAccept(ar);
                //clients列表
                ClientState state = new ClientState();
                state.socket = clientfd;
                clients.Add(clientfd, state);

                //接收数据BeginReceive
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

                //存储所有的玩家数据

                JsonData jsonData = new JsonData();
                jsonData[Constant.USER_ID] = clientfd.RemoteEndPoint.ToString();

                //发送初始数据到客户端
                //初始化自己玩家
                string initMsg = $"{Constant.MSG_SELF_USERINFO}|{jsonData.ToJson()}";
                SendMsg(initMsg, state);

                //把数据同步所有玩家 
                SetHumanMode(jsonData);
                //继续Accept
                listenfd.BeginAccept(AcceptCallback, listenfd);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Accept fail" +
               ex.ToString());
            }
        }
        //Receive回调 收到消息
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("[服务器]ReceiveCallback");
                ClientState state = (ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                int count = clientfd.EndReceive(ar);
                //客户端关闭
                if (count == 0)
                {
                    string usrid = clientfd.RemoteEndPoint.ToString();
                    HumanModel.RemoveHumaModeData(usrid);
                    //SyncAllPlayerInfos();
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    OnMSG_HumanLeave(usrid);
                    return;
                }
                string recvStr = Encoding.UTF8.GetString(state.readBuff, 0, count);
                Console.WriteLine("[服务器] 接收:recvStr:" + recvStr);
                //OnReceiveData(recvStr);
                SendMsg(recvStr);
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                ClientState state = (ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                string usrid = clientfd.RemoteEndPoint.ToString();
                HumanModel.RemoveHumaModeData(usrid);
                clientfd.Close();
                clients.Remove(clientfd);
                OnMSG_HumanLeave(usrid);
                Console.WriteLine("Socket Receive fail" + ex.ToString());
            }
        }

        public static void SendMsg(string str, ClientState clientState = null)
        {

            byte[] bodyBytes = Encoding.UTF8.GetBytes(str);

            #region  前两位添加长度

            short length = (short)bodyBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            byte[] sendBytes = new byte[bodyBytes.Length + 2];
            sendBytes[0] = lengthBytes[0];
            sendBytes[1] = lengthBytes[1];
            Array.Copy(bodyBytes, 0, sendBytes, 2, bodyBytes.Length);
            #endregion
            string tt = Encoding.UTF8.GetString(sendBytes, 0, sendBytes.Length);
            Console.WriteLine("sendMsg:" + tt + "  length: " + sendBytes.Length);
            if (clientState != null)
            {
                //clientState.socket.Send(sendBytes);
                clientState.socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, BeginSendCallBack, clientState.socket);
                return;
            }
            foreach (var s in clients.Values)
            {
                //s.socket.Send(sendBytes);
                s.socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, BeginSendCallBack, s.socket);
            }
        }

        public static void BeginSendCallBack(IAsyncResult ar)
        {
            Socket _socket = (Socket)ar.AsyncState;
            int count = _socket.EndSend(ar);
            Console.WriteLine("BeginSendCallBack :" + count);

        }

        static void OnReceiveData(string receiveData)
        {
            //var msgData = receiveData.Split('|');
            //string msgID = msgData[0];
            //string msgBody = msgData[1];
            //switch (msgID)
            //{
            //    case Constant.MSG_ENDMOVE:
            //        JsonData endMove = JsonMapper.ToObject(msgBody);
            //        OnMsgEndMove(endMove);
            //        break;
            //    default:
            //        break;
            //}
        }
        static void OnMsgEndMove(JsonData moveData)
        {
            HumanModel.SetHumaModeData(moveData);
        }
        static void SetHumanMode(JsonData jsonData)
        {
            HumanModel.SetHumaModeData(jsonData);
            SyncAllPlayerInfos();
        }
        static void SyncAllPlayerInfos()
        {
            var allData = HumanModel.GetHumaModeData();
            foreach (var item in allData)
            {
                SendMsg($"{Constant.MSG_OHTER_USERINFO}|{item.Value.ToJson()}");
            }
        }
        /// <summary>
        /// 有玩家离场
        /// </summary>
        /// <param name="usreId"></param>
        static void OnMSG_HumanLeave(string usreId)
        {
            JsonData jsonData = new JsonData();
            jsonData[Constant.USER_ID] = usreId;
            //发送初始数据到客户端
            //初始化自己玩家
            string initMsg = $"{Constant.MSG_USER_LEAVE}|{jsonData.ToJson()}";
            SendMsg($"{initMsg}");


        }
    }
}
