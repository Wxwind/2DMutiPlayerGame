using System.Collections.Generic;
using System.Linq;
using NetMessage;
using NetMessage.Enum;
using NetMessage.Room;
using Network;
using Requester;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Game;

namespace Panel
{
    class RoomPanel : BasePanel
    {
        public Button exitRoomBtn;
        public TMP_Text roomIdTxt;
        public List<TMP_Text> playShowUIList;
        public Button startGameBtn;
        private string m_roomId;
        
        private Dictionary<string,PlayerUI> m_playerDic = new Dictionary<string, PlayerUI>();

        private RoomRequester m_roomRequester;

        private void Start()
        {
            exitRoomBtn.onClick.AddListener(SendRequest_ExitRoom);
            startGameBtn.onClick.AddListener(SendRequest_StartGame);
            m_roomRequester = MsgHandler.instance.GetRequester(ActionType.Room) as RoomRequester;
            m_roomRequester.OnResponse_PlayerEnterRoom = OnResponse_PlayerEnterRoom;
            m_roomRequester.OnResponse_PlayerExitRoom = OnResponse_PlayerExitRoom;
            m_roomRequester.OnResponse_PlayerStartGame = OnResponse_PlayerStartGame;
            m_roomRequester.OnResponse_ExitRoom = OnResponse_ExitRoom;
            m_roomRequester.OnResponse_StartGame = OnResponse_StartGame;
            startGameBtn.interactable = false;
        }

        private void OnDestroy()
        {
            m_roomRequester.OnResponse_PlayerEnterRoom -= OnResponse_PlayerEnterRoom;
            m_roomRequester.OnResponse_PlayerExitRoom -= OnResponse_PlayerExitRoom;
            m_roomRequester.OnResponse_PlayerStartGame = OnResponse_PlayerStartGame;
            m_roomRequester.OnResponse_ExitRoom -= OnResponse_ExitRoom;
            m_roomRequester.OnResponse_StartGame -= OnResponse_PlayerStartGame;
        }

        private void SendRequest_ExitRoom()
        {
            var pack = new RoomPack
            {
                FuncCode = FuncCode.ExitRoom,
                RoomId = Client.instance.GetRoom.RoomId
            };
            m_roomRequester.SendRequest(pack);
            exitRoomBtn.interactable = false;
        }

        private void SendRequest_StartGame()
        {
            var pack = new RoomPack
            {
                FuncCode = FuncCode.StartGame,
                RoomId = Client.instance.GetRoom.RoomId,
                RoomSettings = new RoomSettings
                {
                    MapId = 0
                }
            };
            m_roomRequester.SendRequest(pack);
            startGameBtn.interactable = false;
        }

        #region 收到广播消息

        //有其他玩家进入
        private void OnResponse_PlayerEnterRoom(RoomPack pack)
        {
            var p = pack.RoomEvent.PlayerInfo;
            //Debug.Log($"{{pack.RoomEvent.SrcPlayerIp}} ,{p.Ip} {p.PlayerName}");
            m_playerDic.Add(pack.RoomEvent.SrcPlayerIp, new PlayerUI(p.Ip, p.PlayerName));
            UpdateUI();
        }
        
        //有其他玩家退出
        private void OnResponse_PlayerExitRoom(RoomPack pack)
        {
            if (m_playerDic.ContainsKey(pack.RoomEvent.SrcPlayerIp))
            {
                m_playerDic.Remove(pack.RoomEvent.SrcPlayerIp);
                UpdateUI();
            }
        }

        private void OnResponse_PlayerStartGame(RoomPack pack)
        {
            startGameBtn.interactable = true;
            Client.instance.GetRoom.gameManager.InitGame(pack.RoomEvent.Players,pack.RoomEvent.RoomSettings);
        }

        #endregion


        #region 收到单发消息

        private void OnResponse_StartGame(RoomPack pack)
        {
            startGameBtn.interactable = true;
            switch(pack.ReturnCode)
            {
                case ReturnCode.Success:
                    Client.instance.GetRoom.gameManager.InitGame(pack.RoomEvent.Players,pack.RoomSettings);
                    break;
                case ReturnCode.Fail:
                    Debug.Log("开始游戏失败");
                    break;
                default:
                    break;
            }
        }

        private void OnResponse_ExitRoom(RoomPack pack)
        {
            exitRoomBtn.interactable = true;
            switch (pack.ReturnCode )
            {
                case ReturnCode.Success:
                    Destroy(Client.instance.GetRoom.gameObject);
                    Client.instance.GetRoom = null;
                    UIManeger.instance.ExitPanel();
                    Debug.Log(UIManeger.instance.GetRunningPanel().GetType().Name);
                    break;
                case ReturnCode.Fail:
                    exitRoomBtn.interactable = true;
                    break;
                default:
                    exitRoomBtn.interactable = true;
                    break;
            }

        }

        #endregion
        
        
        private void UpdateUI()
        {
            for (int i = 0; i < playShowUIList.Count; i++)
            {
                playShowUIList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < m_playerDic.Count; i++)
            {
                playShowUIList[i].gameObject.SetActive(true);
                playShowUIList[i].SetText(m_playerDic.ElementAt(i).Value.PlayerName);
            }

            if (m_playerDic.Count <= 1)
            {
                startGameBtn.interactable = false;
            }
            else startGameBtn.interactable = true;
        }

        public void SetRoomId(string id)
        {
            m_roomId = id;
            roomIdTxt.text = id;
        }

        //本地玩家进入房间时初始化房间内现有玩家的信息
        public void AddPlayerInfo(PlayerInfo pi)
        {
            m_playerDic.Add(pi.Ip,new PlayerUI(pi.Ip,pi.PlayerName));
            UpdateUI();
        }

        public override void OnHideAndFreeze()
        {
            base.OnHideAndFreeze();
            m_playerDic.Clear();
        }

        public override void OnExit()
        {
            base.OnExit();
            m_playerDic.Clear();
        }

        public override void OnRecovery()
        {
            base.OnRecovery();
            m_playerDic.Clear();
            startGameBtn.interactable = false;
            startGameBtn.gameObject.SetActive(false);
        }

        public void GetHostPrivilege()
        {
            startGameBtn.gameObject.SetActive(true);
        }
        
    }
}