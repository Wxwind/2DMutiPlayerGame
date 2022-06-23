using System;
using UnityEngine;

namespace Game
{
    public class RemoteBullet : MonoBehaviour
    {
        public string srcIp;
        public int id;
        private Vector2 correctPos;

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 20);
        }

        //初始化调用
        public void Init(string _srcIp, int _id,float posX,float poxY)
        {
            srcIp = _srcIp;
            id = _id;
            transform.position = new Vector3(posX, poxY, 0);
            correctPos.Set(posX,poxY);
        }

        public void UpdatePos(float x,float y)
        {
            correctPos.Set(x,y);
        }
    }
}