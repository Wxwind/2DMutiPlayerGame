using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using NetMessage.Game;
using Network;
using Requester;
using UnityEngine;

public class LocalBullet : MonoBehaviour
{
    public string srcIp;
    public int id;
    public LayerMask ground;
    public LayerMask remotePlayer;
    public int attatck;
    private GameRequester m_gameRequester;

    private Rigidbody2D m_rb;
    private Timer lifeTimer;
    public float lifeTime;
    public Action OnDelSelf;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        m_gameRequester = MsgHandler.instance.GetRequester(ActionType.Game) as GameRequester;
    }

    private void FixedUpdate()
    {
        lifeTimer.Tick(Time.fixedDeltaTime);
    }

    public BulletPack ToBulletPack(int frame)
    {
        BulletPack bulletPack = new BulletPack
        {
            Frame = frame,
            Id = id,
            SrcIp = srcIp,
            Position = new MVector2
            {
                X = transform.position.x,
                Y = transform.position.y
            }
        };
        return bulletPack;
    }
    
    public BulletPack ToBulletPack()
    {
        BulletPack bulletPack = new BulletPack
        {
            Id = id,
            SrcIp = srcIp,
            Position = new MVector2
            {
                X = transform.position.x,
                Y = transform.position.y
            }
        };
        return bulletPack;
    }

    private void DelSelf()
    {
        Destroy(gameObject);
        GamePack delBulletPack = new GamePack
        {
            FuncCode = FuncCode.DelBullet,
            RoomId = Client.instance.GetRoom.RoomId,
        };
        BulletPack bulletPack = new BulletPack
        {
            Id = id,
            SrcIp = srcIp
        };
        delBulletPack.BulletPack.Add(bulletPack);
        m_gameRequester.SendRequest(delBulletPack);
        Debug.Log("发送删除子弹消息");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == ground)
        {
            OnDelSelf?.Invoke();
        }
        else if (1 << other.gameObject.layer == remotePlayer)
        {
            var p = other.gameObject.GetComponent<BasePlayer>();
            //命中敌方玩家
            if (!p.isLocalPlayer)
            {
                OnDelSelf?.Invoke();
                DamagePack damagePack = new DamagePack
                {
                    Attack = attatck,
                    SrcIp = srcIp,
                    DesIp = p.GetIp
                };
                GamePack bulletPack2 = new GamePack
                {
                    FuncCode = FuncCode.Damage,
                    RoomId = Client.instance.GetRoom.RoomId,
                    DamagePack = damagePack
                };
                Debug.Log("发送子弹命中玩家消息");
                m_gameRequester.SendRequest(bulletPack2);
            }
        }
    }

    //初始化调用
    public void Init(string _srcIp, int _id, float speedX, Action<LocalBullet> _OnDelself)
    {
        srcIp = _srcIp;
        id = _id;
        m_rb.velocity = new Vector2(speedX, m_rb.velocity.y);
        OnDelSelf = () =>
        {
            _OnDelself(this);
            DelSelf();
        };
        lifeTimer = new Timer(lifeTime,OnDelSelf , true);
    }
}