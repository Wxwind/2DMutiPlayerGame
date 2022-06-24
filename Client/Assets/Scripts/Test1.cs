using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class Test1 : BasePlayer
{
    public float moveSpeed;
        public float jumpSpeed;
        public float bulletSpeed;
        public float shotInterval;
        public GameObject bulletPrefab;

        [Header("碰撞盒偏移")] public LayerMask groundLayer;
        public Vector2 groundBoxOffset;
        public Vector2 groundBoxSize;

        public bool isOnGround;
        public bool isCanShot = true;
        private Timer shotIntervalTimer;


        private Rigidbody2D m_rb;

        protected override void Awake()
        {
            base.Awake();
            m_rb = GetComponent<Rigidbody2D>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
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

        private void FixedUpdate()
        {

        }


        private void Shot()
        {
            if (Input.GetKeyDown(KeyCode.K) && isCanShot)
            {
                isCanShot = false;
                shotIntervalTimer.ReRun();
                //创建子弹实体
                GameObject bullet = Instantiate(bulletPrefab,transform.position,transform.rotation);
                bullet.GetComponent<Test>().Init(bulletSpeed*faceDir);
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
                faceDir = x;
            }
        }

        private void CheckIsOnGround()
        {
            Vector2 position = transform.position;
            isOnGround = Physics2D.OverlapBox(position + groundBoxOffset, groundBoxSize, 0, groundLayer);
        }
        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 position = transform.position;
            Gizmos.DrawWireCube(position + groundBoxOffset, groundBoxSize);
        }
}
