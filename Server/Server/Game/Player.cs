using System;
using System.Diagnostics;
using NetMessage;
using WX.Core;
using WX.Network;
using WX.Utils;

namespace WX.Game;

class Player
{
    public string playerName { get; init; }
    public string Ip { get; private set; }
    public int Hp{ get; set; }
    private Vector2 position;
    private int colorId;
    private int playerId;

    public Player(Connection conn,Vector2 pos,int color,int _playerId)
    {
        playerName = conn.GetUserInfo.PlayerName;
        Ip = conn.GetIpEndPoint.ToString();
        Hp = 100;
        position = pos;
        colorId = color;
        playerId = _playerId;
    }

    public void UpdatePos(float x,float y)
    {
        position.Set(x,y);
    }

    public void Damage(int attack)
    {
        Hp -= attack;
    }
    
    public bool CheckDead()
    {
        Log.LogInfo($"{playerName}当前血量：{Hp}");
        return Hp <= 0;
    }
    
    public PlayerInfo ToPlayerInfo()
    {
        return new PlayerInfo
        {
            Ip = this.Ip,
            PlayerName = playerName,
            ColorId = colorId,
            Hp=this.Hp,
            PlayerId = playerId,
            PosX = position.x,
            PosY = position.y
        };
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Ip:{Ip}, PlayerName:{playerName}, ColorId:{colorId}, Hp:{this.Hp}, PlayerId = {playerId}");
    }
}