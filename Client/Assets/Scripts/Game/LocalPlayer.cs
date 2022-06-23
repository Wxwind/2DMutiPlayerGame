using System;
using NetMessage.Game;
using Network;
using Requester;
using UnityEngine;

namespace Game
{
    class LocalPlayer : BasePlayer
    {
        public float moveSpeed;
        public float jumpSpeed;
        public float bulletSpeed;
        public float shotInterval ;

        [Header("碰撞盒偏移")] public LayerMask groundLayer;
        public Vector2 groundBoxOffset;
        public Vector2 groundBoxSize;

        private bool isOnGround;
        private bool isCanShot = true;
        private Timer shotIntervalTimer;


        private Rigidbody2D m_rb;
        private GameRequester m_gameRequester;
        private GameManager m_gameManeger;

        protected override void Awake()
        {
            base.Awake();
            m_rb = GetComponent<Rigidbody2D>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_gameRequester = MsgHandler.instance.GetRequester(ActionType.Game) as GameRequester;
            isLocalPlayer = true;
            shotIntervalTimer = new Timer(shotInterval, () => isCanShot = true, true);

        }

        private void Update()
        {
            shotIntervalTimer.Tick(Time.deltaTime);
            CheckIsOnGround();
            Move();
            Shot();
            Jump();
        }

        public void MyFixedUpdate(int frame)
        {
            MVector2 pos = new MVector2
            {
                X = transform.position.x,
                Y = transform.position.y
            };
            var movePack = new GamePack
            {
                FuncCode = FuncCode.Move,
                RoomId = Client.instance.GetRoom.RoomId,
                MovePack = new MovePack
                {
                    Frame = frame,
                    SrcIp = Client.instance.clientIp,
                    Position = pos,
                    FaceDir = faceDir
                }
            };
            m_gameRequester.SendRequest(movePack);
        }


        private void Shot()
        {
            if (Input.GetKeyDown(KeyCode.K) && isCanShot)
            {
                isCanShot = false;
                shotIntervalTimer.ReRun();
                //创建子弹实体
                LocalBullet t =
                    m_gameManeger.bulletManeger.LoadLocalBullet(playerId, ip, bulletSpeed * faceDir);
                t.transform.position = transform.position;
                //发送创建子弹信息
                var newBulletPack = new GamePack
                {
                    FuncCode = FuncCode.NewBullet,
                    RoomId = Client.instance.GetRoom.RoomId
                };
                newBulletPack.BulletPack.Add(t.ToBulletPack());
                m_gameRequester.SendRequest(newBulletPack);
                Debug.Log("本地客户端发送创建子弹消息");
            }
        }

        private void Jump()
        {
            if (isOnGround && Input.GetKeyDown(KeyCode.J))
            {
                m_rb.velocity = new Vector2(m_rb.velocity.x, jumpSpeed);
            }
        }

        private void Move()
        {
            int x = (int) Input.GetAxisRaw("Horizontal");
            m_rb.velocity = new Vector2(x * moveSpeed, m_rb.velocity.y);
            if (x != 0)
            {
                faceDir = x ;
            }
        }

        private void CheckIsOnGround()
        {
            Vector2 position = transform.position;
            isOnGround = Physics2D.OverlapBox(position + groundBoxOffset, groundBoxSize, 0, groundLayer);
        }

        public void SetGameManeger(GameManager gameManager)
        {
            m_gameManeger = gameManager;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 position = transform.position;
            Gizmos.DrawWireCube(position + groundBoxOffset, groundBoxSize);
        }

        public void OnReceive_PlayerDeath(){
            Destroy(gameObject);
        }
    }
}