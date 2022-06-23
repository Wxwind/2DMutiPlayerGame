using System;
using NetMessage.Lobby;

namespace Requester
{
    public class HeartBeatRequester:BaseRequester<HeartBeatPack>
    {
        private const ActionType m_ActionType = ActionType.HeartBeat;

        public Action OnResponse_HeartBeat;
        
        public override void OnResponse(byte[] msg_body)
        {
            OnResponse_HeartBeat?.Invoke();
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
}