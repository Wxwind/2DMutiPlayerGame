using System;
using Google.Protobuf;
using WX.Network;

namespace WX.Responder;

abstract class BaseResponder<T> : BaseResponder where T : IMessage, new()
{
    protected byte[] Encode(ActionType packType, T pack)
    {
        var data = pack.ToByteArray();
        var msg = new byte[data.Length + 1];
        msg[0] = (byte) packType;
        Array.Copy(data, 0, msg, 1, data.Length);
        return msg;
    }

    public abstract byte[] Encode(T pack);
    protected abstract T Decode(byte[] msg_body);
    
}

abstract class BaseResponder
{
    public abstract void HandleRequest(Connection conn, byte[] msg_body);
}