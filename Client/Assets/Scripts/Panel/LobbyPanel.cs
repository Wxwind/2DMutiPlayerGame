using System;
using NetMessage.Lobby;
using UnityEngine.UI;
using System.Collections.Generic;
using Game;
using NetMessage.Enum;
using Network;
using Requester;
using TMPro;
using UnityEngine;
using Utils;

namespace Panel
{
    class LobbyPanel : BasePanel
    {
        public TMP_InputField roomNameTxt;
        public Button createRoomBtn;
        public Button refreshBtn;
        public Button enterRoomBtn;
        public TMP_Text hintTxt;


        private List<RoomItem> m_roomItemList = new List<RoomItem>();
        private RoomItem m_selectRoom;
        private LobbyRequester m_lobbyRequester;

        private void Awake()
        {
            createRoomBtn.onClick.AddListener(SendRequest_CreateRoom);
            refreshBtn.onClick.AddListener(SendRequest_RefreshRoom);
            enterRoomBtn.onClick.AddListener(SendRequest_EnterRoom);
        }

        private void Start()
        {
            var a = MsgHandler.instance.GetRequester(ActionType.Lobby);
            m_lobbyRequester = MsgHandler.instance.GetRequester(ActionType.Lobby) as LobbyRequester;
            m_lobbyRequester.OnResponse_CreateRoom = OnResponse_CreateRoom;
            m_lobbyRequester.OnResponse_EnterRoom = OnResponse_EnterRoom;
            m_lobbyRequester.OnResponse_RefreshRoom = OnResponse_RefreshRoom;
            SendRequest_RefreshRoom();
        }
        
        private void OnDestroy()
        {
            m_lobbyRequester.OnResponse_CreateRoom -= OnResponse_CreateRoom;
            m_lobbyRequester.OnResponse_EnterRoom -= OnResponse_EnterRoom;
            m_lobbyRequester.OnResponse_RefreshRoom -= OnResponse_RefreshRoom;
        }

        private void SendRequest_CreateRoom()
        {
            string roomName = roomNameTxt.text;
            if (roomName == "")
            {
                roomName = $"{Client.instance.userInfo.UserName}的房间";
            }

            C2SRoomInfo c2SRoomInfo = new C2SRoomInfo
            {
                HostName = Client.instance.userInfo.UserName,
                MaxPlayerNum = 4,
                RoomName = roomName
            };
            LobbyPack pack = new LobbyPack
            {
                FuncCode = FuncCode.CreateRoom,
                C2SRoomInfo = c2SRoomInfo
            };
            m_lobbyRequester.SendRequest(pack);
        }

        private void SendRequest_EnterRoom()
        {
            if (m_selectRoom == null)
            {
                SetText("还未选择房间");
                return;
            }

            LobbyPack pack = new LobbyPack
            {
                FuncCode = FuncCode.EnterRoom,
                C2SRoomId = m_selectRoom.roomId
            };
            m_lobbyRequester.SendRequest(pack);
        }

        private void SendRequest_RefreshRoom()
        {
            Debug.Log("发送刷新房间请求");
            LobbyPack pack = new LobbyPack
            {
                FuncCode = FuncCode.RefreshRoom,
            };
            m_lobbyRequester.SendRequest(pack);
        }

        private void OnResponse_CreateRoom(LobbyPack pack)
        {
            if (pack.ReturnCode == ReturnCode.Fail)
            {
                switch (pack.ErrorCode)
                {
                    case ErrorCode.CreateFail:
                        SetText("玩家已经在房间中");
                        break;
                        ;
                    default:
                        SetText("创建房间失败");
                        break;
                }

                return;
            }

            else if (pack.ReturnCode == ReturnCode.Success)
            {
                if (Client.instance.GetRoom == null)
                {
                    InitRoom(pack);
                    UIManeger.instance.EnterPanel(PanelType.RoomPanel);
                    var panel = UIManeger.instance.GetPanel(PanelType.RoomPanel) as RoomPanel;
                    panel.SetRoomId(pack.S2CRoomInfo[0].RoomId);
                    panel.GetHostPrivilege();
                    foreach (var pi in pack.S2CPlayerInfo)
                    {
                        panel.AddPlayerInfo(pi);
                    }
                }
                else Debug.LogWarning("收到重复的成功加入房间消息");
            }
        }

        private void OnResponse_EnterRoom(LobbyPack pack)
        {
            if (pack.ReturnCode == ReturnCode.Fail)
            {
                switch (pack.ErrorCode)
                {
                    case ErrorCode.MaxNum:
                        SetText("人数房间已满");
                        break;
                    case ErrorCode.GameStarted:
                        SetText("游戏已经开始");
                        break;
                    case ErrorCode.NotExistRoom:
                        SetText("房间不存在");
                        break;
                    default:
                        SetText("进入房间失败");
                        break;
                }
                return;
            }

            if (pack.ReturnCode == ReturnCode.Success)
            {
                if (Client.instance.GetRoom==null)
                {
                    Debug.Log("成功进入房间");
                    UIManeger.instance.EnterPanel(PanelType.RoomPanel);
                    //初始化房间内玩家信息
                    var panel = UIManeger.instance.GetPanel(PanelType.RoomPanel) as RoomPanel;
                    panel.SetRoomId(pack.S2CRoomInfo[0].RoomId);
                    foreach (var pi in pack.S2CPlayerInfo)
                    {
                        panel.AddPlayerInfo(pi);
                    }
                    
                    InitRoom(pack);
                }
            }
        }

        private void OnResponse_RefreshRoom(LobbyPack pack)
        {
            if (pack.ReturnCode == ReturnCode.Fail || pack.ReturnCode == ReturnCode.ReturnNone)
            {
                SetText("刷新失败");
                return;
            }

            ClearRoomInfo();
            var roomInfos = pack.S2CRoomInfo;
            foreach (var r in roomInfos)
            {
                //AddRoomItem
                RoomItem roomItem = RoomItemPool.Instance.GetFromPool();
                roomItem.SetInfo(r.HostName, r.RoomName, r.PlayerNum, r.MaxPlayerNum, r.RoomId);
                roomItem.OnClick = HightLightSelectRoom;
                m_roomItemList.Add(roomItem);
            }
        }

        private void HightLightSelectRoom(RoomItem roomItem)
        {
            if (m_selectRoom != null)
            {
                m_selectRoom.UnHighLight();
            }

            roomItem.HighLight();
            m_selectRoom = roomItem;
        }

        private void ClearRoomInfo()
        {
            foreach (var roomItem in m_roomItemList)
            {
                RoomItemPool.Instance.ReturnPool(roomItem);
            }

            m_roomItemList.Clear();
        }

        private void SetText(string info)
        {
            hintTxt.text = info;
        }
        
        private void InitRoom(LobbyPack pack)
        {
            Client.instance.GetRoom = PrefabManager.instance.LoadGameobject(PrefabType.Room).GetComponent<Room>();
            Client.instance.GetRoom.SetRoomId(pack.S2CRoomInfo[0].RoomId);
        }

        public override void OnEnter()
        {
            base.OnEnter();

        }

        public override void OnRecovery()
        {
            base.OnRecovery();
            SendRequest_RefreshRoom();
        }

        public override void OnHideAndFreeze()
        {
            base.OnHideAndFreeze();
            ClearRoomInfo();
        }

        public override void OnExit()
        {
            base.OnExit();
            ClearRoomInfo();
        }
    }
}