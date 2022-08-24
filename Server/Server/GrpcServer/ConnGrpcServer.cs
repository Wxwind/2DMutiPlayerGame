using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using NetMessage.ConnServer;
using WX.DAO;
using WX.DAO.Model;
using WX.Utils;

namespace WX.GrpcServer;

class ConnGrpcServerImpl : ConnServer.ConnServerBase
{
    public override Task<S2C_Register> Register(C2S_Register request, ServerCallContext context)
    {
        string username = request.Username;
        string password = request.Password;
        string playerName = request.PlayerName;
        using (var gameContext = new GameContext())
        {
            S2C_Register ScRegister;
            var isExisted = gameContext.playerAccount.Any(x => x.UserName == username);
            if (!isExisted)
            {
                var player = new PlayerAccount {UserName = username, PassWord = password, PlayName = playerName};
                gameContext.playerAccount.Add(player);
                gameContext.SaveChanges();
                ScRegister = new S2C_Register {ReturnCode = ReturnCode.Success, ErrorCode = ErrorCode.ErrorNone};
            }
            else
            {
                ScRegister = new S2C_Register
                    {ReturnCode = ReturnCode.Fail, ErrorCode = ErrorCode.HasUserNameRegistered};
            }
            Console.WriteLine("收到客户端的Register请求");

            return Task.FromResult(ScRegister);
        }
    }
    // public override Task<S2C_Login> Login(C2S_Login request, ServerCallContext context)
    // {
    //     string username = request.Username;
    //     string password = request.Password;
    //     using (var gameContext = new GameContext())
    //     {
    //         var t = gameContext.playerAccount.Where(
    //             x => x.UserName == username && x.PassWord == password
    //         ).ToList();
    //
    //         S2C_Login login;
    //         if (t.Count == 0)
    //         {
    //             login = new S2C_Login {ReturnCode = ReturnCode.Fail, ErrorCode = ErrorCode.LoginFail};
    //         }
    //         else
    //         {
    //             var p = new UserInfo {UserName = t[0].PlayName};
    //             login = new S2C_Login
    //                 {UserInfo = p, ReturnCode = ReturnCode.Success, ErrorCode = ErrorCode.ErrorNone};
    //             //conn.GetUserInfo.PlayerName = p.UserName;
    //         }
    //
    //         return Task.FromResult(login);
    //     }
    // }
}

public class ConnGrpcServer
{
    private Server m_server;

    public ConnGrpcServer(int port)
    {
        m_server = new Server
        {
            Services = {ConnServer.BindService(new ConnGrpcServerImpl())},
            Ports = {new ServerPort("localhost", port, ServerCredentials.Insecure)}
        };
        m_server.Start();

        Log.LogInfo($"ConnGrpc Server 已启动，端口: {port}");
    }

    public void Close()
    {
        m_server.ShutdownAsync().Wait();
    }
}