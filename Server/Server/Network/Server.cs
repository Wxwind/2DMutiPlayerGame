using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WX.Responder;
using WX.Utils;

namespace WX.Network;

class Server:Singleton<Server>
{
    private readonly UdpClient m_udpClient;
    private readonly Dictionary<IPEndPoint, Connection> m_connMap = new();
    //private readonly ConcurrentDictionary<IPEndPoint, Connection> m_connMap = new();

    public Server(int port)
    {
        m_udpClient = new UdpClient(port);
        m_udpClient.BeginReceive(ReceiveCallback, null);
        Console.WriteLine("Udp服务器已成功启动");
    }

    public Server():this(6123)
    {
        
    }

    //接收消息并传进kcp(kcp_input)
    private void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint? ep = null;
        var buffer = m_udpClient.EndReceive(ar, ref ep);
        if (ep is not null)
        {
            //收到未知ip的消息，建立连接，如果消息来自于userConnPack&&funccode==initConn，则添加到m_connMap中
            if (!m_connMap.ContainsKey(ep))
            {
                var conn = new Connection(this, Send, ep, DisConnect);
                conn.KCP_Input(buffer);
                AddConnetion(conn);
                Log.LogInfo($"建立新连接:{ep}",false);
            }
            else
            {
                m_connMap[ep].KCP_Input(buffer);
            }
        }

        m_udpClient.BeginReceive(ReceiveCallback, null);
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

    //应用层传入kcp(kcp_send)
    public void SendToOne(IPEndPoint ep_to, byte[] buffer)
    {
        if (m_connMap.ContainsKey(ep_to))
        {
            m_connMap[ep_to].SentToClient(buffer);
        }
    }

    //应用层传入kcp(kcp_send)
    public void SendToAll(byte[] buffer)
    {
        foreach (var connection in m_connMap.Values)
        {
            connection.SentToClient(buffer);
        }
    }

    public void Update(float dt)
    {
        // foreach (var conn in m_connMap.Values)
        // {
        //     conn.Update(dt);
        // }

        for (int i = 0; i < m_connMap.Count; i++)
        {
            m_connMap.Values.ElementAt(i).Update(dt);
        }
    }

    public void AddConnetion(Connection conn)
    {
        m_connMap.Add(conn.GetIpEndPoint,conn);
    }

    public bool ContainsConnection(Connection conn)
    {
        return m_connMap.ContainsKey(conn.GetIpEndPoint);
    }
    private void DisConnect(IPEndPoint ep)
    {
        if (m_connMap.ContainsKey(ep))
        {
            m_connMap[ep].DisConnect();
            m_connMap.Remove(ep);
            Console.WriteLine($"客户端（{ep}）长时间未响应，已断开连接");
        }
    }

    public void HandleRequest(IPEndPoint ep_from, byte[] msg)
    {
        if (m_connMap.TryGetValue(ep_from, out var conn))
        {
            MsgHandler.instance.HandleMsg(conn, msg);
        }
        else
        {
            Log.LogWarning($"与该客户端的连接已经断开或该客户端未曾建立连接，无法处理来自此客户端的消息,ep_from is {ep_from}");
        }
    }
}