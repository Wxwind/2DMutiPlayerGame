using System;
using NetMessage.Conn;

namespace Requester
{
    class ConnRequester : BaseRequester<ConnPack>
    {
        private const ActionType m_ActionType = ActionType.Conn;

        public ActionType ActionType
        {
            get => m_ActionType;
        }

        public Action<S2C_Register> OnResponse_Register;
        public Action<S2C_Login> OnResponse_Login;
        public Action<S2C_InitConn> OnResponse_InitConnection;

        public override void OnResponse(byte[] msg_body)
        {
            var pack = Decode(msg_body);
            switch (pack.FuncCode)
            {
                case FuncCode.InitConn:
                    OnResponse_InitConnection?.Invoke(pack.S2CInitConn);
                    break;
                case FuncCode.Register:
                    OnResponse_Register?.Invoke(pack.S2CRegister);
                    break;
                case FuncCode.Login:
                    OnResponse_Login?.Invoke(pack.S2CLogin);
                    break;
                default:
                    Console.WriteLine($"Could not found funcName:{pack.FuncCode}");
                    break;
            }
        }

        public override byte[] Encode(ConnPack pack)
        {
            return base.Encode(m_ActionType, pack);
        }

        protected override ConnPack Decode(byte[] msg_body)
        {
            var userConnPack = ConnPack.Parser.ParseFrom(msg_body);
            return userConnPack;
        }
    }
}