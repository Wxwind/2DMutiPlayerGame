using System;
using System.Net;
using KcpProject;
using NetMessage.Lobby;
using Requester;
using UnityEngine;

namespace Network
{
    class Connection
    {
        public IPEndPoint ServerEp { get; }

        private KCP m_kcp;
        private uint m_realTime;
        private Client m_client;
        private Timer m_sendHeartBeatTimer;
        private const float m_sendHeartBeatInterval = 2000;
        private HeartBeatRequester m_heartBeatRequester;
        private Timer m_heartBeatTimer;
        private const float m_connectOutTime = 4000;

        public Connection(Client client, IPEndPoint serverEp, Action<byte[], int, IPEndPoint> SendToWeb)
        {
            ServerEp = serverEp;
            m_client = client;
            m_realTime = 0;

            m_kcp = new KCP(1, (byte[] msg, int size) => { SendToWeb(msg, size, this.ServerEp); });
            m_kcp.NoDelay(1, 10, 2, 1);
            m_kcp.WndSize(128, 128);
            m_sendHeartBeatTimer = new Timer(m_sendHeartBeatInterval, SendHeartBeat, true);
            m_heartBeatTimer = new Timer(m_connectOutTime, DisConnect, true);
            m_heartBeatRequester = MsgHandler.instance.GetRequester(ActionType.HeartBeat) as HeartBeatRequester;
            m_heartBeatRequester.OnResponse_HeartBeat += OnResponse_HeartBeat;
        }

        private void SendHeartBeat()
        {
            m_heartBeatRequester.SendRequest(new HeartBeatPack());
            m_sendHeartBeatTimer.ReRun();
        }

        private void OnResponse_HeartBeat()
        {
            //单纯刷新m_heartBeatTimer
            m_heartBeatTimer.ReRun();
        }

        private void DisConnect()
        {
            Client.instance.DisConnect();
        }

        public void Update(float realTime,float dtMS)
        {
            KCP_Update(realTime);
            m_sendHeartBeatTimer.Tick(dtMS);
            m_heartBeatTimer.Tick(dtMS);
        }
        
        private void KCP_Update(float realTime)
        {
            m_realTime = ToKcpTimeMS(realTime);
            m_kcp.Update();

            //检测kcp的revqueue，判断是否有完整的消息到达，有则取出消息并处理
            for (var size = m_kcp.PeekSize(); size > 0; size = m_kcp.PeekSize())
            {
                var buffer = new byte[size];
                if (m_kcp.Recv(buffer) > 0)
                {
                    m_client.HandleResponse(ServerEp, buffer);
                }
            }
        }

        //由应用层(Server)接收消息时调用
        public void KCP_Input(byte[] buffer)
        {
            m_kcp.Input(buffer, 0, buffer.Length, true, true);
        }

        //由应用层(Client)发送消息时调用
        public void KCP_Send(byte[] buffer)
        {
            m_kcp.Send(buffer);
        }

        //Second to MilliSecond
        private uint ToKcpTimeMS(float s)
        {
            return (uint) Mathf.FloorToInt(s) * 1000;
        }
    }
}