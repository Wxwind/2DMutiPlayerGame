using System;
using NetMessage.Lobby;
using WX.Network;

namespace WX.Responder;

class HeatBeatResopnder:BaseResponder<HeartBeatPack>
{
    private const ActionType m_ActionType = ActionType.HeartBeat;
    
    public override void HandleRequest(Connection conn, byte[] msg_body)
    {
        Console.WriteLine($"处理心跳包{conn.GetIpEndPoint}");
        conn.m_isHeartBeatAlive = true;
        conn.SentToClient(Encode(new HeartBeatPack()));
    }

    public override byte[] Encode(HeartBeatPack pack)
    {
        return base.Encode(m_ActionType, pack);
    }

    protected override HeartBeatPack Decode(byte[] msg_body)
    {
        throw new System.NotImplementedException();
    }
}