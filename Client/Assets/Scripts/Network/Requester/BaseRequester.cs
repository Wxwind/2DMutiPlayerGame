using System;
using Google.Protobuf;
using Network;

namespace Requester
{
    public abstract class BaseRequester<T>:BaseRequester where T:IMessage
    {
        protected byte[] Encode(ActionType packType, T pack)
        {
            var data = pack.ToByteArray();
            var msg = new byte[data.Length + 1];
            msg[0] = (byte) packType;
            Array.Copy(data, 0, msg, 1, data.Length);
            return msg;
        }
        
        /// <summary>
        /// 给包体加上包头并转换为字节流
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public abstract byte[] Encode(T pack);
        
        /// <summary>
        /// 将包体解码回消息类
        /// </summary>
        /// <param name="msg_body"></param>
        /// <returns></returns>
        protected abstract T Decode(byte[] msg_body);

        public void SendRequest(T pack)
        {
            var msg = Encode(pack);
            Client.instance.SendToServer(msg);
        }

    }
    
    public abstract class BaseRequester
    {
        /// <summary>
        /// 收到消息时回调
        /// </summary>
        /// <param name="msg_body">包体</param>
        public abstract void OnResponse(byte[] msg_body);
    }
}