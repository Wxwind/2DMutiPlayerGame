using System;
using Grpc.Core;
using NetMessage.ConnServer;
using Requester;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WX;
using C2S_Register = NetMessage.Conn.C2S_Register;
using ConnPack = NetMessage.Conn.ConnPack;
using ErrorCode = NetMessage.Conn.ErrorCode;
using FuncCode = NetMessage.Conn.FuncCode;
using ReturnCode = NetMessage.Enum.ReturnCode;
using S2C_Register = NetMessage.Conn.S2C_Register;

namespace Panel
{
    class RegisterPanel : BasePanel
    {
        public TMP_InputField userName;
        public TMP_InputField passWord;
        public TMP_InputField comfirmPassword;
        public TMP_InputField playerName;
        public TMP_Text infoLabel;
        public Button registerBtn;
        public Button switchToLoginBtn;


        private ConnRequester m_connRequester;

        private void Start()
        {
            registerBtn.onClick.AddListener(SendRequest_Register);
            switchToLoginBtn.onClick.AddListener(SwitchToLoginPanel);
            m_connRequester = MsgHandler.instance.GetRequester(ActionType.Conn) as ConnRequester;
            if (m_connRequester == null)
            {
                Log.LogError("ConnRequester is null");
            }

            m_connRequester.OnResponse_Register += OnResponse_Register;
        }

        private void OnDestroy()
        {
            m_connRequester.OnResponse_Register -= OnResponse_Register;
        }

        private void SendRequest_Register()
        {
            if (CheckInput())
            {
                SetText("正在等待服务器回应");
                RegisterByGRPC();
                // var csRegister = new C2S_Register
                //     {Username = userName.text, Password = passWord.text, PlayerName = playerName.text};
                // var pack = new ConnPack {C2SRegister = csRegister, FuncCode = FuncCode.Register};
                // m_connRequester.SendRequest(pack);
                // SetText("正在等待服务器回应");
                // registerBtn.interactable = false;
            }
            else
            {
                SetText("输入的用户名或者密码太长或者为空");
            }
        }

        private bool CheckInput()
        {
            if (comfirmPassword.text != passWord.text)
            {
                SetText("两次输入的密码不一致");
                return false;
            }

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

        private void OnResponse_Register(S2C_Register s2CRegister)
        {
            if (s2CRegister == null)
            {
                Log.LogError("s2CRegister pack is null");
                return;
            }

            var rc = s2CRegister.ReturnCode;
            var ec = s2CRegister.ErrorCode;
            switch (rc)
            {
                case ReturnCode.Success:
                    SetText("注册成功");
                    break;
                case ReturnCode.Fail:
                    if (ec == ErrorCode.HasUserNameRegistered)
                    {
                        SetText("该用户名已被注册");
                    }
                    else SetText("注册失败");

                    break;
                case ReturnCode.ReturnNone:
                    Log.LogError("未设置return code");
                    SetText("未设置return code");
                    break;
                default:
                    return;
            }

            registerBtn.interactable = true;
        }

        private void SwitchToLoginPanel()
        {
            UIManeger.instance.ExitPanel();
        }

        private void RegisterByGRPC()
        {
            Channel channel = new Channel("127.0.0.1:6124", ChannelCredentials.Insecure);
            
            var client = new ConnServer.ConnServerClient(channel);

            var s2CRegister = client.Register(new NetMessage.ConnServer.C2S_Register
            {
                Username = userName.text,
                Password = passWord.text, 
                PlayerName = playerName.text
            });
            
            
            if (s2CRegister == null)
            {
                Log.LogError("s2CRegister pack is null");
                return;
            }
            Log.LogInfo("Received c2SRegister");
            var rc = s2CRegister.ReturnCode;
            var ec = s2CRegister.ErrorCode;
            switch (rc)
            {
                case NetMessage.ConnServer.ReturnCode.Success:
                    SetText("注册成功");
                    break;
                case NetMessage.ConnServer.ReturnCode.Fail:
                    if (ec == NetMessage.ConnServer.ErrorCode.HasUserNameRegistered)
                    {
                        SetText("该用户名已被注册");
                    }
                    else SetText("注册失败");
            
                    break;
                case NetMessage.ConnServer.ReturnCode.ReturnNone:
                    Log.LogWarning("未设置return code");
                    SetText("未设置return code");
                    break;
                default:
                    return;
            }
            
            registerBtn.interactable = true;
            
            channel.ShutdownAsync().Wait();
        }
    }
}