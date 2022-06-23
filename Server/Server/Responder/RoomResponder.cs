using System;
using System.Diagnostics;
using NetMessage.Room;
using WX.Game;
using WX.Network;
using WX.Utils;

namespace WX.Responder;

class RoomResponder:BaseResponder<RoomPack>
{
    private const ActionType m_ActionType = ActionType.Room;

    public override void HandleRequest(Connection conn, byte[] msg_body)
    {
        var pack = Decode(msg_body);
        var room = LobbyManager.Instance.GetRoom(pack.RoomId);
        if (room==null)
        {
            Log.LogWarning($"未找到房间: RoomId is {pack.RoomId}");
            return;
        }
        RoomPack returnPack;
        switch (pack.FuncCode)
        {
            case FuncCode.ExitRoom:
                returnPack=room.TryRemovePlayer(conn);
                conn.SentToClient(Encode(returnPack));
                break;
            case FuncCode.StartGame:
                returnPack = room.StartGame(conn,pack);
                conn.SentToClient(Encode(returnPack));
                break;
            default:
                Log.LogError($"Could not found funcName:{pack.FuncCode}");
                break;
        }
    }

    public override byte[] Encode(RoomPack pack)
    {
       return base.Encode(m_ActionType, pack);
    }

    protected override RoomPack Decode(byte[] msg_body)
    {
       var roomPack= RoomPack.Parser.ParseFrom(msg_body);
       return roomPack;
    }
}
