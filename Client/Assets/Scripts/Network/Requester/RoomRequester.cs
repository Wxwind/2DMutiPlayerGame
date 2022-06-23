using System;
using NetMessage.Room;
using UnityEngine;

namespace Requester
{
    class RoomRequester:BaseRequester<RoomPack>
    {
        private const ActionType m_ActionType=ActionType.Room;
        public ActionType ActionType
        {
            get => m_ActionType;
        }

        public Action<RoomPack> OnResponse_PlayerEnterRoom;
        public Action<RoomPack> OnResponse_PlayerExitRoom;
        //房主开始了游戏
        public Action<RoomPack> OnResponse_PlayerStartGame;
        public Action<RoomPack> OnResponse_ExitRoom;
        public Action<RoomPack> OnResponse_StartGame;

        public override void OnResponse(byte[] msg_body)
        {
            var pack = Decode(msg_body);
            //单发消息
            switch (pack.FuncCode)
            {
                case FuncCode.ExitRoom:
                    OnResponse_ExitRoom(pack);
                    return;
                case FuncCode.StartGame:
                    OnResponse_StartGame(pack);
                    break;
                case FuncCode.FuncNone:
                    break;
                default:
                    break;
            }
            //广播消息
            switch (pack.RoomEvent.EventCode)
            {
                case EventCode.PlayerEnterRoom:
                    OnResponse_PlayerEnterRoom?.Invoke(pack);
                    break;
                case EventCode.PlayerExitRoom:
                    OnResponse_PlayerExitRoom?.Invoke(pack);
                    break;
                case EventCode.PlayerStartGame:
                    OnResponse_PlayerStartGame?.Invoke(pack);
                    break;
                case EventCode.EventNone:
                    break;
                default:
                    Debug.Log($"Could not found funcName:{pack.RoomEvent.EventCode}");
                    break;
            };
            Debug.Log($"收到消息 消息类型funcCode={pack.FuncCode} 广播消息eventCode={pack.RoomEvent.EventCode}");
        }

        public override byte[] Encode(RoomPack pack)
        {
            return base.Encode(m_ActionType, pack);
        }

        protected override RoomPack Decode(byte[] msg_body)
        {
            var roomPack = RoomPack.Parser.ParseFrom(msg_body);
            return roomPack;
        }
    }
}