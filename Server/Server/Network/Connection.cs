using System;
using System.Net;
using KcpProject;
using WX.Game;
using WX.Utils;

namespace WX.Network;

class Connection
{
    private readonly IPEndPoint m_clientEp;
    private readonly KCP m_kcp;
    private readonly Server m_server;
    
    private Timer heartBeatTimer;
    private const float m_connectOutTime = 4000;
    public bool m_isHeartBeatAlive;

    //private Player m_player;
    private Room? m_room;

    public Room? GetRoom
    {
        get => m_room;
        set => m_room = value;
    }

    public bool IsInRoom { get; set; }
    public IPEndPoint GetIpEndPoint => m_clientEp;

    private UserInfo m_userInfo;
    public UserInfo GetUserInfo
    {
        get=>m_userInfo;
    }

    public Connection(Server server, Action<byte[], int, IPEndPoint> SendToWeb, IPEndPoint clientEp,Action<IPEndPoint> OnDisConnect)
    {
        m_server = server;
        m_clientEp = clientEp;
        m_kcp = new KCP(1, (msg, size) => SendToWeb(msg, size, m_clientEp));
        m_kcp.NoDelay(1, 10, 2, 1);
        m_kcp.WndSize(128, 128);
        m_kcp.SetStreamMode(false);
        m_userInfo = new UserInfo("???");
        heartBeatTimer = new Timer(m_connectOutTime, ()=>OnDisConnect(m_clientEp), true);
    }

    //由应用层(Server)接收消息时调用
    public void KCP_Input(byte[] buffer)
    {
        m_kcp.Input(buffer, 0, buffer.Length, true, true);
    }

    /// <summary>
    /// 由应用层(Server)发送消息时调用
    /// </summary>
    /// <param name="buffer">要传送的消息</param>
    public void SentToClient(byte[] buffer)
    {
        m_kcp.Send(buffer);
    }

    public void Update(float dtMS)
    {
        KCP_Update();
        if (m_isHeartBeatAlive)
        {
            heartBeatTimer.ReRun();
            m_isHeartBeatAlive = false;
        }
        else
        {
            heartBeatTimer.Tick(dtMS);
        }
    }

    private void KCP_Update()
    {
        // if (0 == m_nextUpdateTime || m_kcp.CurrentMS >= m_nextUpdateTime)
        // {
        //     m_kcp.Update();
        //     m_nextUpdateTime = m_kcp.Check();
        // }
        m_kcp.Update();

        //检测kcp的revqueue，判断是否有完整的消息到达，有则取出消息并处理
        for (var size = m_kcp.PeekSize(); size > 0; size = m_kcp.PeekSize())
        {
            var buffer = new byte[size];
            if (m_kcp.Recv(buffer) > 0)
            {
                m_server.HandleRequest(m_clientEp, buffer);
            }
        }
    }

    //被服务器踢出去后调用此函数
    public void DisConnect()
    {
        if (IsInRoom)
        {
            m_room?.ForceKickPlayer(this);
        }
    }
}