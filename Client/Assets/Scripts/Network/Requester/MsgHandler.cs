using System;
using System.Collections.Generic;
using UnityEngine;
using WX;

namespace Requester
{
    public enum ActionType:byte
    {
        HeartBeat=0,
        Conn=1,
        Lobby=2,
        Room=3,
        Game=4,
    }

    
    /// <summary>
    /// 处理消息头部，以此决定分发给哪一个requester处理
    /// </summary>
    class MsgHandler
    {
        private readonly Dictionary<ActionType, BaseRequester> m_requesterDic=new Dictionary<ActionType, BaseRequester>();

        public static readonly MsgHandler instance=new MsgHandler();

        public MsgHandler()
        {
            m_requesterDic.Add(ActionType.Conn,new ConnRequester());
            m_requesterDic.Add(ActionType.Lobby,new LobbyRequester());
            m_requesterDic.Add(ActionType.Room,new RoomRequester());
            m_requesterDic.Add(ActionType.HeartBeat,new HeartBeatRequester());
            m_requesterDic.Add(ActionType.Game,new GameRequester());
        }

        public void HandleMsg(byte[] msg)
        {
            var head = msg[0];
            var data = new byte[msg.Length - 1];
            Array.Copy(msg, 1, data, 0,data.Length);
            if (m_requesterDic.TryGetValue((ActionType)head,out BaseRequester responder))
            {
                responder.OnResponse(data);
            }
            else
            {
                Log.LogError($"Could not found the responder! head is {head}");
            }
        }

        public BaseRequester GetRequester(ActionType actionType)
        {
            if (m_requesterDic.TryGetValue(actionType, out var panel))
            {
                return panel;
            }
            else
            {
                Debug.LogError($"{actionType} is null");
                return null;
            }
        }
    }
}