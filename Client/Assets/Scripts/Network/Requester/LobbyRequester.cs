using System;
using NetMessage.Lobby;

namespace Requester
{
    class LobbyRequester:BaseRequester<LobbyPack>
    {
        private const ActionType m_ActionType=ActionType.Lobby;
        public ActionType ActionType
        {
            get => m_ActionType;
        }

        public Action<LobbyPack> OnResponse_CreateRoom;
        public Action<LobbyPack> OnResponse_EnterRoom;
        public Action<LobbyPack> OnResponse_RefreshRoom;
        //public Action<LobbyPack> OnResponse_FindRoom;
        public override void OnResponse(byte[] msg_body)
        {
            var pack = Decode(msg_body);
            switch (pack.FuncCode)
            {
                case FuncCode.CreateRoom:
                    OnResponse_CreateRoom?.Invoke(pack);
                    break;
                case FuncCode.EnterRoom:
                    OnResponse_EnterRoom?.Invoke(pack);
                    break;
                case FuncCode.RefreshRoom:
                    OnResponse_RefreshRoom?.Invoke(pack);
                    break;
                // case FuncCode.FindRoom:
                //     OnResponse_FindRoom.Invoke(pack);
                //     break;
                default:
                    Console.WriteLine($"Could not found funcName:{pack.FuncCode}");
                    break;
            };
        }

        public override byte[] Encode(LobbyPack pack)
        {
            return base.Encode(m_ActionType, pack);
        }

        protected override LobbyPack Decode(byte[] msg_body)
        {
            var roomPack = LobbyPack.Parser.ParseFrom(msg_body);
            return roomPack;
        }
    }
}