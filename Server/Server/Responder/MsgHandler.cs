using System;
using System.Collections.Generic;
using WX.Network;
using WX.Utils;

namespace WX.Responder;

internal enum ActionType:byte
{
    HeartBeat=0,
    Conn=1,
    Lobby=2,
    Room=3,
    Game=4,
}

class MsgHandler
{
    private readonly Dictionary<ActionType, BaseResponder> responderMap=new();
    
    public static readonly MsgHandler instance=new ();

    public MsgHandler()
    {
        responderMap.Add(ActionType.Conn,new ConnResponder());
        responderMap.Add(ActionType.Lobby,new LobbyResponder());
        responderMap.Add(ActionType.Room,new RoomResponder());
        responderMap.Add(ActionType.HeartBeat,new HeatBeatResopnder());
        responderMap.Add(ActionType.Game,new GameResponder());
    }

    public void HandleMsg(Connection conn,byte[] msg)
    {
        var head = msg[0];
        var data = new byte[msg.Length - 1];
        Array.Copy(msg, 1, data, 0,data.Length);
        if (responderMap.TryGetValue((ActionType)head,out var responder))
        {
            responder.HandleRequest(conn,data);
        }
        else
        {
            Log.LogError($"Could not found the responder! head is {head}");
        }
    }

    public BaseResponder? GetResponder(ActionType actionType)
    {
        if (responderMap.TryGetValue(actionType,out var responder))
        {
            return responder;
        }

        return null;
    }
}