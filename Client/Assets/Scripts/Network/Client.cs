using System;
using System.Net;
using System.Net.Sockets;
using Game;
using Panel;
using Requester;
using UnityEngine;
using WX;

namespace Network
{
    class Client : MonoBehaviour
    {
        public string serveAddress;
        public int serverPort;
        private UdpClient m_udpClient;
        private Connection m_connection;
        private IPEndPoint m_serverEP;

        public static Client instance;
        public UserInfo userInfo = new UserInfo("???");
        private Room m_room;

        public Room GetRoom
        {
            get => m_room;
            set => m_room = value;
        }

        public string clientIp;

        private void Awake()
        {
            m_serverEP = new IPEndPoint(IPAddress.Parse("192.168.1.208"), serverPort);
            m_connection = new Connection(this, m_serverEP, Send);
            m_udpClient = new UdpClient();
            m_udpClient.BeginReceive(ReceiveMsgCallback, null);
            m_room = GetComponent<Room>();
            instance = this;
            clientIp = "UnKnown";
        }

        private void Update()
        {
            m_connection.Update(Time.realtimeSinceStartup, Time.deltaTime * 1000);
        }

        private void ReceiveMsgCallback(IAsyncResult ar)
        {
            IPEndPoint ep = null;
            var buffer = m_udpClient.EndReceive(ar, ref ep);
            if (ep.Equals(m_connection.ServerEp))
                m_connection.KCP_Input(buffer);
            m_udpClient.BeginReceive(ReceiveMsgCallback, null);
        }

        //由kcp调用并发送消息给客户端(kcp_output)
        private void Send(byte[] buffer, int size, IPEndPoint ep_to)
        {
            m_udpClient.BeginSend(buffer, size, ep_to, this.SendCallback, null);
        }

        private void SendCallback(IAsyncResult ar)
        {
            m_udpClient.EndSend(ar);
        }

        public void SendToServer(byte[] buffer)
        {
            m_connection.KCP_Send(buffer);
        }

        public void HandleResponse(IPEndPoint ep_from, byte[] msg)
        {
            if (m_connection != null && ep_from.Equals(m_serverEP))
            {
                MsgHandler.instance.HandleMsg(msg);
            }
            else
            {
                Log.LogWarning($"this conn is none or not server {ep_from.Address}:{ep_from.Port}");
            }
        }

        private void Close()
        {
            m_udpClient.Close();
        }

        public void DisConnect()
        {
            Debug.Log("Disconnect");
            UIManeger.instance.Show();
            UIManeger.instance.EnterPanel(PanelType.MainMenuPanel);
            var panel = UIManeger.instance.GetPanel(PanelType.MainMenuPanel) as MainMenuPanel;
            panel.SetText("与服务器断开连接");
            if (m_room!=null)
            {
                m_room.gameManager.ForceExitGame();
            }
        }
    }
}