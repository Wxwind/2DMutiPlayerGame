using System;
using System.Net;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Test : MonoBehaviour
{
    public LayerMask ground;
    public LayerMask remotePlayer;
    public Rigidbody2D rb;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
    }

    public void Init(float speed)
    {
        rb.velocity = new Vector2(speed, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log((int)other.gameObject.layer);
       
        if (1<<other.gameObject.layer == ground)
        {
            Destroy(gameObject);
        }
        else if (1<<other.gameObject.layer == remotePlayer)
        {
            //命中敌方玩家
       
                Destroy(gameObject);
               
                Debug.Log("发送子弹命中玩家消息");
                
        }
    }
}
