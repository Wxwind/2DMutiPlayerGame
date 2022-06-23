using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MapManager:MonoBehaviour
    {
        public GameObject map1;
        private Dictionary<int, GameObject> m_mapDic=new Dictionary<int, GameObject>();
        public static MapManager instance;

        private void Awake()
        {
            instance = this;
            map1 = Instantiate(Resources.Load<GameObject>("Prefabs/Map/Map1"));
            m_mapDic.Add(0,map1);
        }

        public GameObject LoadMap(int mapId)
        {
            if (m_mapDic.TryGetValue(mapId, out var map))
            {
                return map;
            }
            else
            {
                Debug.LogError($"加载地图失败 id:{mapId}");
                return null;
            }
        }
    }
}