using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.Protobuf.Collections;
using NetMessage;
using NetMessage.Game;
using WX.Core;
using WX.Network;
using WX.Responder;

namespace WX.Game;

class GameManager
{
    private static Vector2[] poses = new Vector2[4] { new (-6.04f, -1.33f),new (6.04f, 1.33f),new (-2.65f, 0.67f),new (2.65f, 0.67f)};
    private Dictionary<string, Player> m_playerMap=new ();
    private int m_maxPlayerNum;
    private Room m_room;
    private GameResponder m_gameResponder;

    public GameManager(Room room,int maxNum)
    {
        m_room = room;
        m_maxPlayerNum = maxNum;
        m_gameResponder = MsgHandler.instance.GetResponder(ActionType.Game) as GameResponder;
    }

    // private void AddPlayer(string ip,Connection conn)
    // {
    //     m_playerMap.Add(ip,new Player(conn));
    // }
    //
    // private void RemovePlayer(string ip)
    // {
    //     if (m_playerMap.ContainsKey(ip))
    //     {
    //         m_playerMap.Remove(ip);
    //     }
    // }
    public RepeatedField<PlayerInfo> InitGame(Dictionary<string,Connection> connMap)
    {
        for (int i=0;i< connMap.Count;i++)
        {
            var c = connMap.ElementAt(i);
            var p = new Player(c.Value,poses[i],i,i);
            m_playerMap.Add(c.Key,p);
        }

        RepeatedField<PlayerInfo> playerInfos=new RepeatedField<PlayerInfo>();
        foreach (var p in m_playerMap)
        {
            var playerInfo = p.Value.ToPlayerInfo();
            p.Value.PrintInfo();
            playerInfos.Add(playerInfo);
        }

        return playerInfos;
    }

    public void EndGame()
    {
        m_playerMap.Clear();
    }

    public Player? GetPlayer(string srcIp)
    {
        if (m_playerMap.TryGetValue(srcIp,out var p))
        {
            return p;
        }

        return null;
    }

    public void Move(MovePack pack)
    {
        GetPlayer(pack.SrcIp)?.UpdatePos(pack.Position.X,pack.Position.Y);
    }

    public void NewBullet(RepeatedField<BulletPack> bullets)
    {
        
    }
    
    public void DelBullet(RepeatedField<BulletPack> bullets)
    {
        
    }

    public void SyncBullet(RepeatedField<BulletPack> bullets)
    {
       

    }

    public void Damage(DamagePack pack)
    {
        Player? player = GetPlayer(pack.DesIp);
        if (player is not null)
        {
            player.Damage(pack.Attack);
            if (player.Hp<=0)
            {
                var deadPack = new GamePack
                {
                    FuncCode = FuncCode.PlayerDead,
                    PlayerDeadSrcIp = player.Ip
                };
                m_room.BroadcastToAll(m_gameResponder.Encode(deadPack));
                Console.WriteLine($"玩家{player.Ip}死亡，成功发包通知所有客户端");
                //游戏结束，通知所有客户端
                if (CheckGameEnd(out var winnerIp))
                {
                    var gameEndPack = new GamePack
                    {
                        FuncCode = FuncCode.GameEnd,
                        GameEndPack = new GameEndPack
                        {
                            WinnerName = m_playerMap[winnerIp].playerName
                        }
                    };
                    m_room.BroadcastToAll(m_gameResponder.Encode(gameEndPack));
                    Console.WriteLine("游戏结束，成功发包通知所有客户端");
                    EndGame();
                }
            }
        }
    }

    private bool CheckGameEnd(out string winnerIp)
    {
        int alivePlayer = 0;
        string aliveIp="null";
        foreach (var p in m_playerMap)
        {
            Console.WriteLine($"{p.Value.Ip}  {p.Value.Hp}");
            if (!p.Value.CheckDead())
            {
                alivePlayer++;
                aliveIp = p.Key;
            }
        }
        Console.WriteLine($"场上还存活{alivePlayer}名玩家");
        winnerIp = aliveIp;
        if (alivePlayer <= 1)
        {
            return true;
        }
        else return false;
        
    }
}