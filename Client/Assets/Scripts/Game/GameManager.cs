using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using NetMessage;
using NetMessage.Game;
using NetMessage.Room;
using Network;
using Panel;
using UnityEngine;
using Utils;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static Color[] _colors = new Color[4]
        {
            new Color(1, 0, 0, 1),
            new Color(1, 1, 0, 1),
            new Color(0, 0, 1, 1),
            new Color(0, 1, 0, 1)
        };

        private Dictionary<string, RemotePlayer> m_remotePlayerMap = new Dictionary<string, RemotePlayer>();
        private LocalPlayer m_localPlayer;
        private Dictionary<string, BasePlayer> m_alivePlayersMap = new Dictionary<string, BasePlayer>();
        public BulletManeger bulletManeger;
        public GameEndPanel gameEndPanel;
        private GameObject m_map;
        private GameObject m_playerHealthPanel;
        private RectTransform m_canvas;

        private void Awake()
        {
            m_canvas = GameObject.Find("GameCanvas").GetComponent<RectTransform>();
        }
        
        private const float dt = 50; //16ms per frame,62.5fps
        double accumulator = 0.0;
        private int frame = 0;
        
        private void Update()
        {
            accumulator += Time.deltaTime*1000;
            while (accumulator >= dt)
            {
                frame++;
                accumulator -= dt;
                //执行每固定帧的逻辑
                bulletManeger.MyFixedUpdate(frame);
                if (m_localPlayer!=null)
                {
                    m_localPlayer.MyFixedUpdate(frame);
                }
            }
        }

        public void InitGame(RepeatedField<PlayerInfo> players, RoomSettings setting)
        {
            m_playerHealthPanel = PrefabManager.instance.LoadGameobject(PrefabType.PlayerHealthPanel,m_canvas);
            m_playerHealthPanel.SetActive(true);
            m_map = MapManager.instance.LoadMap(setting.MapId);
            if (m_map != null)
            {
                m_map.SetActive(true);
            }
            UIManeger.instance.Hide();
            bulletManeger.enabled = true;
            //初始化玩家实体
            for (int i = 0; i < players.Count; i++)
            {
                var pi = players[i];
                bool isLocalPlayer = pi.Ip == Client.instance.clientIp;
                if (isLocalPlayer)
                {
                    var player = PrefabManager.instance.LoadGameobject(PrefabType.LocalPlayer).GetComponent<LocalPlayer>();
                    Debug.Log("LocalPlayer Add");
                    player.Init(pi.PlayerId, pi.Ip, pi.PlayerName, pi.Hp, new Vector2(pi.PosX, pi.PosY), 1,m_playerHealthPanel);
                    player.SetColor(_colors[pi.ColorId]);
                    player.SetGameManeger(this);
                    m_localPlayer = player;
                    m_alivePlayersMap.Add(pi.Ip, player);
                }
                else
                {
                    var player = PrefabManager.instance.LoadGameobject(PrefabType.RemotePlayer)
                        .GetComponent<RemotePlayer>();
                    Debug.Log("RemotePlayer Add");
                    player.Init(pi.PlayerId, pi.Ip, pi.PlayerName, pi.Hp, new Vector2(pi.PosX, pi.PosY), 1,m_playerHealthPanel);
                    player.SetColor(_colors[pi.ColorId]);
                    m_remotePlayerMap.Add(pi.Ip, player);
                    m_alivePlayersMap.Add(pi.Ip, player);
                }
            }
        }

        #region 处理游戏内消息

        public void PlayerMove(MovePack pack)
        {
            var p = GetRemotePlayer(pack.SrcIp);
            if (p != null)
            {
                p.OnReceive_PlayerMove(pack.Position.X, pack.Position.Y, pack.FaceDir);
            }
        }

        public void PlayerDamgage(DamagePack pack)
        {
            var p = GetPlayer(pack.DesIp);
            if (p != null)
            {
                p.OnReceive_PlayerDamege(pack.Attack);
            }
        }

        public void NewBullet(RepeatedField<BulletPack> pack)
        {
            foreach (var bullet in pack)
            {
                bulletManeger.LoadRemoteBullet(bullet.Id, bullet.SrcIp,bullet.Position.X,bullet.Position.Y);
                Debug.Log($"加载了其他玩家的子弹场上还有{bulletManeger.GetBulletCount()}个其他玩家的子弹");
            }
        }

        public void DelBullet(RepeatedField<BulletPack> pack)
        {
            foreach (var bullet in pack)
            {
                bulletManeger.RemoveRemoteBullet(bullet.Id);
                Debug.Log($"删除了其他玩家的子弹,场上还有{bulletManeger.GetBulletCount()}个其他玩家的子弹");
            }
        }

        public void SyncBulletPos(RepeatedField<BulletPack> pack)
        {
            Debug.Log("收到子弹更新包");
            foreach (var bullet in pack)
            {
                bulletManeger.UpdatePos(bullet.Id, bullet.Position.X, bullet.Position.Y);
            }
        }

        public void PlayerDead(string srcIp)
        {
            //是本地玩家死亡
            if (srcIp == m_localPlayer.GetIp)
            {
                m_localPlayer.OnReceive_PlayerDeath();
                m_localPlayer = null;
                m_alivePlayersMap.Remove(srcIp);
            }
            //是其他玩家死亡 
            var p = GetRemotePlayer(srcIp);
            if (p != null)
            {
                p.OnReceive_PlayerDeath();
                m_remotePlayerMap.Remove(srcIp);
                m_alivePlayersMap.Remove(srcIp);
            }
        }

        public void EndGame(GameEndPack pack)
        {
            gameEndPanel.Show();
            gameEndPanel.SetWinner(pack.WinnerName);
            bulletManeger.enabled = false;
            //销毁游戏场景
            if (m_map != null)
            {
                m_map.SetActive(false);
            }

            if (m_playerHealthPanel != null)
            {
                Destroy(m_playerHealthPanel);
            }
            bulletManeger.OnEndGame();
            foreach (var p in m_alivePlayersMap.Values)
            {
                Destroy(p.gameObject);
            }
            m_alivePlayersMap.Clear();
            m_localPlayer = null;
            m_remotePlayerMap.Clear();
        }

        #endregion
        
        public void ForceExitGame()
        {
            bulletManeger.enabled = false;
            //销毁游戏场景
            if (m_map != null)
            {
                Destroy(m_map);
            }

            if (m_playerHealthPanel != null)
            {
                Destroy(m_playerHealthPanel);
            }
            bulletManeger.OnEndGame();
            foreach (var p in m_alivePlayersMap.Values)
            {
                Destroy(p.gameObject);
            }
            m_alivePlayersMap.Clear();
        }

        private RemotePlayer GetRemotePlayer(string ip)
        {
            if (m_remotePlayerMap.TryGetValue(ip, out var player))
            {
                return player;
            }

            return null;
        }

        private BasePlayer GetPlayer(string ip)
        {
            if (m_alivePlayersMap.TryGetValue(ip, out var player))
            {
                return player;
            }

            return null;
        }
    }
}