using System;
using System.Linq;
using NetMessage.Enum;
using NetMessage.Conn;
using WX.DAO;
using WX.DAO.Model;
using WX.Network;
using WX.Utils;

namespace WX.Responder;

class ConnResponder : BaseResponder<ConnPack>
{
    private const ActionType m_ActionType = ActionType.Conn;

    public override void HandleRequest(Connection conn, byte[] msg_body)
    {
        var pack = Decode(msg_body);
        ConnPack returnPack;
        switch (pack.FuncCode)
        {
            case FuncCode.InitConn:
                returnPack = InitConnection(conn);
                conn.SentToClient(Encode(returnPack));
                break;
            case FuncCode.Register:
                returnPack = Register(pack);
                conn.SentToClient(Encode(returnPack));
                break;
            case FuncCode.Login:
                returnPack = Login(conn, pack);
                conn.SentToClient(Encode(returnPack));
                break;
            default:
                Log.LogError($"Could not found funcName:{pack.FuncCode}");
                break;
        }
    }


    public override byte[] Encode(ConnPack pack)
    {
        return base.Encode(m_ActionType, pack);
    }

    protected override ConnPack Decode(byte[] msg_body)
    {
        var connPack = ConnPack.Parser.ParseFrom(msg_body);
        return connPack;
    }

    #region 处理消息

    private ConnPack InitConnection(Connection conn)
    {
        //Server.Instance.AddConnetion(conn);
        if (Server.Instance.ContainsConnection(conn))
        {
            var returnPack = new ConnPack
            {
                FuncCode = FuncCode.InitConn,
                S2CInitConn = new S2C_InitConn
                {
                    ReturnCode = ReturnCode.Success,
                    ClientIp = conn.GetIpEndPoint.ToString()
                }
            };
            return returnPack;
        }
        else
        {
            var returnPack = new ConnPack
                {FuncCode = FuncCode.InitConn, S2CInitConn = new S2C_InitConn {ReturnCode = ReturnCode.Fail}};
            return returnPack;
        }
    }

    private ConnPack Register(ConnPack pack)
    {
        string username = pack.C2SRegister.Username;
        string password = pack.C2SRegister.Password;
        string playerName = pack.C2SRegister.PlayerName;
        using (var context = new GameContext())
        {
            ConnPack returnPack;
            var isExisted = context.playerAccount.Any(x => x.UserName == username);
            if (!isExisted)
            {
                var player = new PlayerAccount {UserName = username, PassWord = password, PlayName = playerName};
                context.playerAccount.Add(player);
                context.SaveChanges();
                var ScRegister = new S2C_Register {ReturnCode = ReturnCode.Success, ErrorCode = ErrorCode.ErrorNone};
                returnPack = new ConnPack {FuncCode = FuncCode.Register, S2CRegister = ScRegister};
            }
            else
            {
                var ScRegister = new S2C_Register
                    {ReturnCode = ReturnCode.Fail, ErrorCode = ErrorCode.HasUserNameRegistered};
                returnPack = new ConnPack {FuncCode = FuncCode.Register, S2CRegister = ScRegister};
            }

            return returnPack;
        }
    }

    private ConnPack Login(Connection conn, ConnPack pack)
    {
        string username = pack.C2SLogin.Username;
        string password = pack.C2SLogin.Password;
        using (var context = new GameContext())
        {
            var t = context.playerAccount.Where(
                x => x.UserName == username && x.PassWord == password
            ).ToList();

            ConnPack returnPack;
            if (t.Count == 0)
            {
                var login = new S2C_Login {ReturnCode = ReturnCode.Fail, ErrorCode = ErrorCode.LoginFail};
                returnPack = new ConnPack {FuncCode = FuncCode.Login, S2CLogin = login};
            }
            else
            {
                var p = new UserInfo {UserName = t[0].PlayName};
                var login = new S2C_Login
                    {UserInfo = p, ReturnCode = ReturnCode.Success, ErrorCode = ErrorCode.ErrorNone};
                returnPack = new ConnPack {FuncCode = FuncCode.Login, S2CLogin = login};
                conn.GetUserInfo.PlayerName = p.UserName;
            }

            return returnPack;
        }
    }

    #endregion
}