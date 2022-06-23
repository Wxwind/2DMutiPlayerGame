using System;
using System.Collections.Generic;
using NetMessage;
using NetMessage.Enum;
using NetMessage.Lobby;
using NetMessage.Room;
using WX.Network;
using WX.Responder;
using WX.Utils;
using FuncCode = NetMessage.Lobby.FuncCode;

namespace WX.Game;

//管理所有房间
class LobbyManager : Singleton<LobbyManager>
{
    private Dictionary<string, Room> m_roomDic = new();

    private RoomResponder m_roomResponder;

    public LobbyManager()
    {
        m_roomResponder = MsgHandler.instance.GetResponder(ActionType.Room) as RoomResponder;
    }

    public void RemoveRoom(Room room)
    {
        if (m_roomDic.ContainsKey(room.RoomId))
        {
            RoomIdAllocator.OnRemoveRoom(room.RoomId);
            m_roomDic.Remove(room.RoomId);
        }
        else Log.LogWarning($"roomId({room.RoomId}) 已经不存在或者被移除");
    }

    public Room? GetRoom(string roomId)
    {
        if (m_roomDic.TryGetValue(roomId, out var room))
        {
            return room;
        }
        Log.LogWarning("Room is null");
        return null;
    }

    #region 处理消息

    public LobbyPack CreateRoom(Connection conn, LobbyPack pack)
    {
        Console.WriteLine("处理加入房间消息");
        LobbyPack returnPack;
        if (conn.IsInRoom)
        {
            returnPack = new LobbyPack
            {
                FuncCode = FuncCode.CreateRoom,
                ReturnCode = ReturnCode.Fail,
                ErrorCode = ErrorCode.CreateFail
            };
            return returnPack;
        }

        string? roomId = RoomIdAllocator.AllocateId();
        //成功创建房间
        if (roomId is not null)
        {
            Room room = new Room(roomId, pack.C2SRoomInfo.MaxPlayerNum, pack.C2SRoomInfo.HostName,
                pack.C2SRoomInfo.RoomName);
            room.AddHostPlayer(conn);
            m_roomDic.Add(roomId, room);
            Console.WriteLine($"player({conn.GetIpEndPoint.ToString()})创建房间(id={roomId})");
            returnPack = new LobbyPack {FuncCode = FuncCode.CreateRoom, ReturnCode = ReturnCode.Success};
            returnPack.S2CRoomInfo.Add(room.ToS2CRoomInfo());
            foreach (var c in room.GetAllPlayer())
            {
                returnPack.S2CPlayerInfo.Add(new PlayerInfo
                    {PlayerName = c.GetUserInfo.PlayerName, Ip = c.GetIpEndPoint.ToString()});
            }
        }
        else
        {
            returnPack = new LobbyPack
            {
                FuncCode = FuncCode.CreateRoom,
                ReturnCode = ReturnCode.Fail,
                ErrorCode = ErrorCode.CreateFail
            };
        }

        return returnPack;
    }

    // public LobbyPack FindRoom(LobbyPack pack)
    // {
    //     LobbyPack returnPack;
    //     if (m_roomList.TryGetValue(pack.S2CRoomInfo[0].RoomId,out var room))
    //     {
    //         returnPack = new LobbyPack {FuncCode = FuncCode.FindRoom, ReturnCode = ReturnCode.Success};
    //         returnPack.S2CRoomInfo.Add(room);
    //     }
    //     else
    //     {
    //         returnPack = new LobbyPack
    //         {
    //             FuncCode = FuncCode.FindRoom,
    //             ReturnCode = ReturnCode.Fail,
    //             ErrorCode = ErrorCode.NotExistRoom
    //         };
    //     }
    //     return returnPack;
    // }

    public LobbyPack EnterRoom(Connection conn, LobbyPack pack)
    {
        LobbyPack returnPack;
        if (conn.IsInRoom)
        {
            returnPack = new LobbyPack
            {
                FuncCode = FuncCode.EnterRoom,
                ReturnCode = ReturnCode.Fail,
                ErrorCode = ErrorCode.ErrorNone
            };
            return returnPack;
        }

        if (m_roomDic.TryGetValue(pack.C2SRoomId, out var room))
        {
            if (room.TryAddPlayer(conn, out var errorCode) == false)
            {
                returnPack = new LobbyPack
                    {FuncCode = FuncCode.EnterRoom, ReturnCode = ReturnCode.Fail, ErrorCode = errorCode};
            }
            else
            {
                //成功加入房间，告知客户端房间信息
                returnPack = new LobbyPack
                    {FuncCode = FuncCode.EnterRoom, ReturnCode = ReturnCode.Success, ErrorCode = ErrorCode.ErrorNone};
                returnPack.S2CRoomInfo.Add(room.ToS2CRoomInfo());
                foreach (var c in room.GetAllPlayer())
                {
                    returnPack.S2CPlayerInfo.Add(new PlayerInfo
                        {PlayerName = c.GetUserInfo.PlayerName, Ip = c.GetIpEndPoint.ToString()});
                }

                //通知其他玩家有新玩家加入
                var roomPack = new RoomPack
                {
                    RoomEvent = new RoomEvent
                    {
                        EventCode = EventCode.PlayerEnterRoom,
                        SrcPlayerIp = conn.GetIpEndPoint.ToString(),
                        PlayerInfo = new PlayerInfo {Ip = conn.GetIpEndPoint.ToString(), PlayerName = conn.GetUserInfo.PlayerName}
                    },
                    RoomId = room.RoomId
                };

                room.BroadcastToOther(conn.GetIpEndPoint.ToString(), m_roomResponder.Encode(roomPack));
            }
        }
        else
        {
            returnPack = new LobbyPack
            {
                FuncCode = FuncCode.EnterRoom,
                ReturnCode = ReturnCode.Fail,
                ErrorCode = ErrorCode.NotExistRoom
            };
        }

        return returnPack;
    }

    public LobbyPack RefreshRoom()
    {
        LobbyPack returnPack;
        returnPack = new LobbyPack
            {FuncCode = FuncCode.RefreshRoom, ReturnCode = ReturnCode.Success, ErrorCode = ErrorCode.ErrorNone};
        foreach (var r in m_roomDic.Values)
        {
            returnPack.S2CRoomInfo.Add(r.ToS2CRoomInfo());
        }

        return returnPack;
    }

    // public LobbyPack ExitRoom(Connection conn,LobbyPack pack)
    // {
    //     LobbyPack returnPack;
    //     if (conn.GetRoom.TryRemovePlayer(conn))
    //     {
    //         returnPack = new LobbyPack {FuncCode = FuncCode.ExitRoom, ReturnCode = ReturnCode.Success};
    //     }
    //     else
    //     {
    //         Log.LogWarning("Player tried exit room but already not exist in roomlist");
    //         returnPack = new LobbyPack {FuncCode = FuncCode.ExitRoom, ReturnCode = ReturnCode.Fail};
    //     }
    //
    //     return returnPack;
    // }

    #endregion
}