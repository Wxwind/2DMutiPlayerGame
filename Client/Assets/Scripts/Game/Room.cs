using System.Collections.Generic;
using Network;
using UnityEngine;

namespace Game
{
    class Room:MonoBehaviour
    {
        private int m_maxPlayerNum;
        private int m_nowPlayerNum;
        
        private string m_roomId;
        public string RoomId
        {
            get => m_roomId;
        }

        public bool IsInRoom { get; set; } = false;
        // private RoomState m_roomState = RoomState.Invalid;
        // public RoomState GetRoomState => m_roomState;

        // internal enum RoomState
        // {
        //     //还未进入房间
        //     Invalid,
        //     Wait,
        //     Started,
        // }

        public GameManager gameManager;

        private Dictionary<string, Connection> m_playerMap = new Dictionary<string, Connection>();
        private string m_hostName;
        private string m_roomName;

        public void Init(string roomId, int maxPlayerNum, string hostName, string roomName, int nowPlayerNum)
        {
            m_maxPlayerNum = maxPlayerNum;
            m_roomId = roomId;
            m_hostName = hostName;
            m_roomName = roomName;
            m_nowPlayerNum = nowPlayerNum;
        }

        public void SetRoomId(string roomId)
        {
            m_roomId = roomId;
        }

        // public void SetRoomState(RoomState state)
        // {
        //     m_roomState = state;
        // }
        //
        // public void OnEnterRoom()
        // {
        //     m_roomState = RoomState.Wait;
        // }
        //
        // public void OnExitRoom()
        // {
        //     m_roomState = RoomState.Invalid;
        //     
        // }
    }
}