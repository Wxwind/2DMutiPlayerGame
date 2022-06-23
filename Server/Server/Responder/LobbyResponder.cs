using System;
using Google.Protobuf;
using NetMessage.Lobby;
using WX.Game;
using WX.Network;
using WX.Utils;

namespace WX.Responder;

class LobbyResponder:BaseResponder<LobbyPack>
{
    private const ActionType m_ActionType=ActionType.Lobby;
    public override void HandleRequest(Connection conn, byte[] msg_body)
    {
        var pack = Decode(msg_body);
        // var methodName = pack.FuncCode.ToString();
        // var method = LobbyManager.Instance.GetType().GetMethod(methodName);
        // if (method is null)
        // {
        //     Log.LogError($"could not found method \"{methodName}\" in LobbyManager");
        //     return;
        // }
        // object[] param = {conn, pack};
        // method.Invoke(LobbyManager.Instance, param);
        LobbyPack returnPack;
        switch (pack.FuncCode)
        {
            case FuncCode.CreateRoom:
                returnPack = LobbyManager.Instance.CreateRoom(conn,pack);
                conn.SentToClient(Encode(returnPack));
                break;
            case FuncCode.EnterRoom:
                returnPack = LobbyManager.Instance.EnterRoom(conn,pack);
                conn.SentToClient(Encode(returnPack));
                break;
            // case FuncCode.FindRoom:
            //     returnPack = LobbyManager.Instance.FindRoom(pack);
            //     conn.SentToClient(Encode(returnPack));
            //     break;
            case FuncCode.RefreshRoom:
                returnPack =LobbyManager.Instance.RefreshRoom();
                conn.SentToClient(Encode(returnPack));
                break;
            default:
                Log.LogError($"Could not found funcName:{pack.FuncCode}");
                break;
        }
    }

    public override byte[] Encode(LobbyPack pack)
    {
        return base.Encode(m_ActionType, pack);
    }

    protected override LobbyPack Decode(byte[] msg_body)
    {
        var lobbyPack = LobbyPack.Parser.ParseFrom(msg_body);
        return lobbyPack;
    }
}