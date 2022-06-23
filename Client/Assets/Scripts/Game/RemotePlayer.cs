using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Game
{
    public class RemotePlayer:BasePlayer
    {
        private Vector2 correctPos;

        protected override void Awake()
        {
            base.Awake();
            isLocalPlayer = false;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 20);
        }

        public override void Init(int _id,string _ip, string _playerName, int _hp, Vector2 respawnPoint, int _facedir,GameObject playerHealthPanel)
        {
            base.Init(_id, _ip,_playerName, _hp,  respawnPoint, _facedir, playerHealthPanel);
            correctPos = respawnPoint;
        }
        
        public void OnReceive_PlayerMove(float x,float y,int _faceDir)
        {
            transform.position.Set(1,2,1);
            correctPos.Set(x,y);
            m_spriteRenderer.flipX = _faceDir==-1;
            faceDir = _faceDir;
        }
        

        public void OnReceive_PlayerDeath()
        {
            Destroy(gameObject);
        }
    }
}