using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using Google.Protobuf.Collections;
using Microsoft.Win32.SafeHandles;
using NetMessage;
using NetMessage.Enum;
using NetMessage.Game;
using NetMessage.Lobby;
using NetMessage.Room;
using WX.Network;
using WX.Responder;
using WX.Utils;
using FuncCode = NetMessage.Room.FuncCode;

namespace WX.Game;

class Room
{
    private int m_maxPlayerNum;
    private int m_nowPlayerNum;

    private string m_roomId;

    public string RoomId
    {
        get => m_roomId;
    }

    private Dictionary<string, Connection> m_connMap = new();
    //private Dictionary<string,Player> m_playerMap = new();
    private string m_hostName;
    private string m_roomName;
    private RoomState m_roomState = RoomState.Wait;
    private RoomResponder m_roomResponder;

    private GameManager m_gameManager;
    public GameManager GetGameManager => m_gameManager;

    enum RoomState
    {
        Wait,
        Started,
    }

    public Room(string roomId, int maxPlayerNum, string hostName, string roomName)
    {
        m_maxPlayerNum = maxPlayerNum;
        m_roomId = roomId;
        m_hostName = hostName;
        m_roomName = roomName;
        m_nowPlayerNum = 0;
        m_gameManager = new GameManager(this,m_maxPlayerNum);
        m_roomResponder = MsgHandler.instance.GetResponder(ActionType.Room) as RoomResponder;
    }

    public bool TryAddPlayer(Connection conn, out ErrorCode errorCode)
    {
        if (m_nowPlayerNum >= m_maxPlayerNum)
        {
            errorCode = ErrorCode.MaxNum;
            return false;
        }
        else if (m_nowPlayerNum == 0)
        {
            Log.LogWarning("玩家正在尝试进入已经关闭的房间（房间里已经没有人）");
            errorCode = ErrorCode.NotExistRoom;
            return false;
        }
        else if (m_roomState == RoomState.Started)
        {
            errorCode = ErrorCode.GameStarted;
            return false;
        }
        //成功进入房间
        else if (m_connMap.ContainsKey(conn.GetIpEndPoint.ToString()))
        {
            errorCode = ErrorCode.ErrorNone;
            return false;
        }
        else
        {
            //更新服务器信息
            conn.GetRoom = this;
            conn.IsInRoom = true;
            var ip = conn.GetIpEndPoint.ToString();
            m_connMap.Add(ip, conn);
            OnPlayerNumUpate();
            errorCode = ErrorCode.ErrorNone;
            return true;
        }
    }

    public RoomPack TryRemovePlayer(Connection conn)
    {
        RoomPack returnPack;
        string ip = conn.GetIpEndPoint.ToString();
        if (m_connMap.ContainsKey(ip))
        {
            //从房间中有移除玩家，如果没有人则移除房间
            m_connMap.Remove(ip);
            OnPlayerNumUpate();

            conn.GetRoom = null;
            conn.IsInRoom = false;
            returnPack = new RoomPack
            {
                ReturnCode = ReturnCode.Success,
                FuncCode = FuncCode.ExitRoom,
            };
            //通知房间内其他玩家
            string srcIp = conn.GetIpEndPoint.ToString();
            var bcPack = new RoomPack
            {
                RoomId = m_roomId,
                RoomEvent = new RoomEvent
                {
                    SrcPlayerIp = srcIp,
                    EventCode = EventCode.PlayerExitRoom
                }
            };
            BroadcastToOther(srcIp, m_roomResponder.Encode(bcPack));
        }
        else
        {
            returnPack = new RoomPack
            {
                ReturnCode = ReturnCode.Fail,
                FuncCode = NetMessage.Room.FuncCode.ExitRoom,
            };
            Log.LogWarning("玩家已经不在房间中但仍尝试移除玩家");
        }

        return returnPack;
    }

    public RoomPack StartGame(Connection conn,RoomPack pack)
    {
        RoomPack returnPack;
        switch (m_roomState)
        {
            case RoomState.Wait:
                //初始化游戏
               RepeatedField<PlayerInfo> playerInfos= m_gameManager.InitGame(m_connMap);

                //单发消息
                returnPack = new RoomPack
                {
                    FuncCode = FuncCode.StartGame,
                    ReturnCode = ReturnCode.Success,
                    RoomId = m_roomId,
                    RoomEvent = new RoomEvent
                    {
                        Players = {playerInfos}
                    },
                    RoomSettings = pack.RoomSettings,
                };
                
                //广播消息
                var bcPack = new RoomPack
                {
                    FuncCode = FuncCode.StartGame,
                    RoomId = m_roomId,
                    RoomEvent = new RoomEvent
                    {
                        EventCode = EventCode.PlayerStartGame,
                        RoomSettings = pack.RoomSettings,
                        Players = {playerInfos}
                    },
                };
                // foreach (var p in m_playerMap)
                // {
                //     var playerInfo = p.Value.ToPlayerInfo();
                //     p.Value.PrintInfo();
                //     returnPack.RoomEvent.Players.Add(playerInfo);
                //     bcPack.RoomEvent.Players.Add(playerInfo);
                // }
                
                BroadcastToOther(conn.GetIpEndPoint.ToString(),m_roomResponder.Encode(bcPack));
                break;
            case RoomState.Started:
                returnPack = new RoomPack
                {
                    FuncCode = FuncCode.StartGame,
                    ReturnCode = ReturnCode.Fail,
                };
                break;
            default:
                returnPack = new RoomPack
                {
                    FuncCode = FuncCode.StartGame,
                    ReturnCode = ReturnCode.Fail,
                };
                break;
        }

        return returnPack;
    }

    public Connection? GetPlayer(string ipEndPoint)
    {
        if (m_connMap.ContainsKey(ipEndPoint))
        {
            return m_connMap[ipEndPoint];
        }

        return null;
    }
    
    public Connection[] GetAllPlayer()
    {
        return m_connMap.Values.ToArray();
    }

    /// <summary>
    /// 发送消息通知房间内的其他玩家
    /// </summary>
    /// <param name="srcIp">发送者</param>
    /// <param name="msg">消息</param>
    public void BroadcastToOther(string srcIp, byte[] msg)
    {
        foreach (var pair in m_connMap)
        {
            if (pair.Key.Equals(srcIp))
            {
                continue;
            }

            pair.Value.SentToClient(msg);
        }
    }
    
    public void BroadcastToAll(byte[] msg)
    {
        foreach (var pair in m_connMap)
        {
            pair.Value.SentToClient(msg);
        }
    }

    //初始化房间并添加房主时调用
    public void AddHostPlayer(Connection conn)
    {
        conn.GetRoom = this;
        conn.IsInRoom = true;
        m_connMap.Add(conn.GetIpEndPoint.ToString(), conn);
        OnPlayerNumUpate();
    }

    public void ForceKickPlayer(Connection conn)
    {
        var a = conn.GetIpEndPoint.ToString();
        if (m_connMap.ContainsKey(a))
        {
            m_connMap.Remove(a);
            OnPlayerNumUpate();
            Console.WriteLine("连接断开后已经将玩家从房间中移除");
        }
    }

    private void OnPlayerNumUpate()
    {
        m_nowPlayerNum = m_connMap.Count;
        if (m_nowPlayerNum == 0)
        {
            LobbyManager.Instance.RemoveRoom(this);
        }
    }

    public S2CRoomInfo ToS2CRoomInfo()
    {
        S2CRoomInfo roomInfo = new S2CRoomInfo
        {
            HostName = m_hostName,
            MaxPlayerNum = m_maxPlayerNum,
            PlayerNum = m_nowPlayerNum,
            RoomId = RoomId,
            RoomName = m_roomName
        };
        return roomInfo;
    }
}