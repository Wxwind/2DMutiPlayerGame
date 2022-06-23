using NetMessage.Enum;
using NetMessage.Conn;
using Network;
using Requester;
using TMPro;
using UnityEngine.UI;
using WX;

namespace Panel
{
    class LoginPanel : BasePanel
    {
        public TMP_InputField userName;
        public TMP_InputField passWord;
        public TMP_Text infoLabel;
        public Button loginBtn;
        public Button switchToRegisterBtn;

        private ConnRequester m_connRequester;

        private void Start()
        {
            loginBtn.onClick.AddListener(SendRequest_Login);
            switchToRegisterBtn.onClick.AddListener(SwitchToRegisterPanel);
            m_connRequester = MsgHandler.instance.GetRequester(ActionType.Conn) as ConnRequester;
            m_connRequester.OnResponse_Login = OnResponse_Login;
        }
        
        private void OnDestroy()
        {
            m_connRequester.OnResponse_Login -= OnResponse_Login;
        }

        private void SendRequest_Login()
        {
            if (CheckInput())
            {
                var csLogin = new C2S_Login {Username = userName.text, Password = passWord.text};
                var pack = new ConnPack {C2SLogin = csLogin, FuncCode = FuncCode.Login};
                m_connRequester.SendRequest(pack);
                SetText("正在等待服务器回应");
                loginBtn.interactable = false;
            }
        }

        private bool CheckInput()
        {
            if (userName.text.Length > 20 || userName.text.Length == 0 || passWord.text.Length > 20 ||
                passWord.text.Length == 0
               )
            {
                SetText("输入的账号密码太长或为空");
                return false;
            }

            return true;
        }

        private void SetText(string info)
        {
            infoLabel.text = info;
        }


        private void OnResponse_Login(S2C_Login s2CLogin)
        {

            var rc = s2CLogin.ReturnCode;
            var pi = s2CLogin.UserInfo;
            var ec = s2CLogin.ErrorCode;
            switch (rc)
            {
                case ReturnCode.Success:
                    SetText("登陆成功，正在进入游戏");
                    UIManeger.instance.EnterPanel(PanelType.LobbyPanel);
                    Client.instance.userInfo.UserName = pi.UserName;
                    break;
                case ReturnCode.Fail:
                    if (ec == ErrorCode.LoginFail)
                    {
                        SetText("输入的用户名或者密码错误");
                    }
                    else SetText("未知的登陆错误");
                    break;
                case ReturnCode.ReturnNone:
                    Log.LogError("未设置return code");
                    SetText("未设置return code");
                    break;
                default:
                    break;
            }

            loginBtn.interactable = true;
        }

        private void SwitchToRegisterPanel()
        {
            UIManeger.instance.EnterPanel(PanelType.RegisterPanel);
        }

        public override void OnRecovery()
        {
            base.OnRecovery();
            userName.text = "";
            passWord.text = "";
        }
    }
}