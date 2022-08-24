using System;
using WX.GrpcServer;
using WX.Network;
using WX.Utils;

class Program
{
    public static bool quit = false;

    public const float dt = 16; //16ms per frame,62.5fps

    public static int frame = 0; //

    private static void Main(string[] args)
    {
        Log.SetLevel(Log.LogLevel.Info);
        Log.SetOuputToFile(false);
        ConnGrpcServer connGrpcServer = new ConnGrpcServer(6124);

        double runningTime = 0;
        DateTime currentTime = GetTime();
        double accumulator = 0.0;

        while (!quit)
        {
            var newTime = GetTime();
            var frameTime = (newTime - currentTime).TotalMilliseconds;
            currentTime = newTime;

            accumulator += frameTime;

            while (accumulator >= dt)
            {
                accumulator -= dt;
                frame++;
                runningTime += dt;
                //执行每帧的逻辑
                Server.Instance.Update(dt);
            }
        }
        
        connGrpcServer.Close();
        Log.LogError("服务器已关闭");
        Console.ReadKey();
    }

    private static DateTime GetTime()
    {
        return DateTime.Now;
    }
}