using System;
using System.Collections.Generic;
using System.Linq;
using NetMessage.Game;
using Network;
using Requester;
using UnityEngine;
using Utils;

namespace Game
{
    //管理和同步其他玩家的子弹
    public class BulletManeger : MonoBehaviour
    {
        private Dictionary<int, RemoteBullet> m_remoteBulletsDic = new Dictionary<int, RemoteBullet>();
        private Dictionary<int, LocalBullet> m_localBulletsDic = new Dictionary<int, LocalBullet>();
        private int nowId = 0;
        private GameRequester m_gameRequester;

        private void Start()
        {
            m_gameRequester = MsgHandler.instance.GetRequester(ActionType.Game) as GameRequester;
        }

        public LocalBullet LoadLocalBullet(int playerid, string srcIp, float speedX)
        {
            var id = GenerateGUID(playerid);
            Debug.Log($"playid:{playerid}  nowId:{nowId}");
            var bullet = PrefabManager.instance.LoadGameobject(PrefabType.LocalBullet).GetComponent<LocalBullet>();
            bullet.Init(srcIp, id, speedX, RemoveLocalBullet);
            m_localBulletsDic.Add(bullet.id, bullet);
            return bullet;
        }

        public void RemoveLocalBullet(LocalBullet localBullet)
        {
            m_localBulletsDic.Remove(localBullet.id);
        }

        public void LoadRemoteBullet(int id, string srcIp, float spawnPosX, float spawnPosY)
        {
            var bullet = PrefabManager.instance.LoadGameobject(PrefabType.RemoteBullet).GetComponent<RemoteBullet>();
            bullet.Init(srcIp, id, spawnPosX, spawnPosY);
            m_remoteBulletsDic.Add(bullet.id, bullet);
        }

        public void RemoveRemoteBullet(int id)
        {
            if (m_remoteBulletsDic.TryGetValue(id, out var bullet))
            {
                Destroy(bullet.gameObject);
                m_remoteBulletsDic.Remove(id);
            }
            // string idinfo=String.Empty;
            // foreach (var a in m_remoteBulletsDic.Values)
            // {
            //     idinfo += a.srcIp + "   id:" + a.id;
            // }
            // Debug.Log(idinfo);
        }

        public void UpdatePos(int id, float x, float y)
        {
            if (m_remoteBulletsDic.TryGetValue(id, out var bullet))
            {
                bullet.UpdatePos(x, y);
                Debug.Log($"子弹id:{id} pos:{x},{y}");
            }
        }

        
        
        public void MyFixedUpdate(int frame)
        {
            if (m_localBulletsDic.Count == 0)
            {
                return;
            }
            else
            {
                var bulletPosPack = new GamePack
                {
                    FuncCode = FuncCode.BulletPos,
                    RoomId = Client.instance.GetRoom.RoomId,
                };

                foreach (var bullet in m_localBulletsDic.Values)
                {
                    bulletPosPack.BulletPack.Add(bullet.ToBulletPack(frame));
                }
                
                m_gameRequester.SendRequest(bulletPosPack);
            }
        }

        private int GenerateGUID(int playerId)
        {
            int a = nowId++;
            int guid = playerId * 10000 + a;
            return guid;
        }

        public int GetBulletCount()
        {
            return m_remoteBulletsDic.Count;
        }

        public void OnEndGame()
        {
            foreach (var b in m_remoteBulletsDic.Values)
            {
                Destroy(b.gameObject);
            }

            m_remoteBulletsDic.Clear();

            foreach (var b in m_localBulletsDic.Values)
            {
                Destroy(b.gameObject);
            }

            m_localBulletsDic.Clear();
        }
    }
}