using NetMessage.Conn;
using NetMessage.Enum;
using Network;
using Requester;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Panel
{
    class MainMenuPanel : BasePanel
    {
        public Button EnterLoginBtn;
        public Button ExitGameBtn;
        public TMP_Text hintText;

        private ConnRequester m_connRequester;

        private void Start()
        {
            EnterLoginBtn.onClick.AddListener(SendInitConnRequset);
            ExitGameBtn.onClick.AddListener(ExitGame);
            m_connRequester = MsgHandler.instance.GetRequester(ActionType.Conn) as ConnRequester;
            m_connRequester.OnResponse_InitConnection = OnResponse_InitConnection;
        }

        private void OnDestroy()
        {
            m_connRequester.OnResponse_InitConnection -= OnResponse_InitConnection;
        }

        private void SendInitConnRequset()
        {
            ConnPack pack = new ConnPack {FuncCode = FuncCode.InitConn};
            m_connRequester.SendRequest(pack);
            EnterLoginBtn.interactable = false;
            SetText("正在连接服务器");
        }

        private void OnResponse_InitConnection(S2C_InitConn s2cInitConn)
        {
            EnterLoginBtn.interactable = true;
            
            switch (s2cInitConn.ReturnCode)
            {
                case ReturnCode.Success:
                    UIManeger.instance.EnterPanel(PanelType.LoginPanel);
                    Client.instance.clientIp = s2cInitConn.ClientIp;
                    break;
                case ReturnCode.Fail:
                    SetText("连接服务器失败，请稍后重试");
                    break;
                default:
                    break;
            }
        }

        public void SetText(string msg)
        {
            hintText.text = msg;
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        public override void OnRecovery()
        {
            base.OnRecovery();
            EnterLoginBtn.interactable = true;
        }
    }
}