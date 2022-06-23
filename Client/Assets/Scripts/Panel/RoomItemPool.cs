using System.Collections.Generic;
using UnityEngine;

namespace Panel
{
    public class RoomItemPool:MonoBehaviour
    {
        public GameObject roomItemPre;
        public Transform scrollViewContent;
        public int initSize;
        private Stack<RoomItem> m_roomItemPool = new Stack<RoomItem>();
        public static RoomItemPool Instance;
    
        private void Awake()
        {
            Instance=this;
            InitPool(initSize);
        }

        public RoomItem GetFromPool()
        {
            if (m_roomItemPool.Count==0)
            {
                ExpandPool();
            }
            var item=m_roomItemPool.Pop();
            item.gameObject.SetActive(true);
            return item;
        }
    
        public void ReturnPool(RoomItem roomItem)
        {
            roomItem.gameObject.SetActive(false);
            m_roomItemPool.Push(roomItem);
        }
    
        private void InitPool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject go = Instantiate(roomItemPre, scrollViewContent);
                go.SetActive(false);
                m_roomItemPool.Push(go.GetComponent<RoomItem>());
            }
        }
    
        private void ExpandPool()
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject go = Instantiate(roomItemPre, scrollViewContent);
                go.SetActive(false);
                m_roomItemPool.Push(go.GetComponent<RoomItem>());
            }
        }
    }
}