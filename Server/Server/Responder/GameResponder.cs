using System;
using System.Diagnostics;
using NetMessage.Game;
using WX.Game;
using WX.Network;
using WX.Utils;

namespace WX.Responder;

class GameResponder : BaseResponder<GamePack>
{
    private const ActionType m_ActionType = ActionType.Game;

    public override void HandleRequest(Connection conn, byte[] msg_body)
    {
        var pack = Decode(msg_body);
        if (pack.RoomId==""||pack.RoomId==String.Empty)
        {
            Log.LogWarning($"pack.RoomId为空");
        }
        var room = LobbyManager.Instance.GetRoom(pack.RoomId);
        if (room==null)
        {
            Log.LogWarning($"未找到房间: RoomId is {pack.RoomId}");
            return;
        }
        switch (pack.FuncCode)
        {
            case FuncCode.Move:
                room.GetGameManager.Move(pack.MovePack);
                room.BroadcastToOther(conn.GetIpEndPoint.ToString(),Encode(pack));
                break;
            case FuncCode.Damage:
                room.GetGameManager.Damage(pack.DamagePack);
                room.BroadcastToAll(Encode(pack));
                break;
            case FuncCode.NewBullet:
                room.GetGameManager.NewBullet(pack.BulletPack);
                room.BroadcastToOther(conn.GetIpEndPoint.ToString(),Encode(pack));
                break;
            case FuncCode.DelBullet:
                room.GetGameManager.DelBullet(pack.BulletPack);
                room.BroadcastToOther(conn.GetIpEndPoint.ToString(),Encode(pack));
                break;
            case FuncCode.BulletPos:
                room.GetGameManager.SyncBullet(pack.BulletPack);
                room.BroadcastToOther(conn.GetIpEndPoint.ToString(),Encode(pack));
                break;
            case FuncCode.FuncNone:
            default:
                Log.LogError($"Could not found funcName:{pack.FuncCode}");
                break;
        }
    }


    public override byte[] Encode(GamePack pack)
    {
        return base.Encode(m_ActionType, pack);
    }

    protected override GamePack Decode(byte[] msg_body)
    {
        var gamePack = GamePack.Parser.ParseFrom(msg_body);
        return gamePack;
    }
    
}