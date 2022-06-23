using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public enum PrefabType
    {
        LocalPlayer,
        RemotePlayer,
        LocalBullet,
        RemoteBullet,
        Room,
        PlayerHealthPanel,
        playerHealthItem,
    }

    public class PrefabManager : MonoBehaviour
    {
        private Dictionary<PrefabType, string> m_prefabsPath = new Dictionary<PrefabType, string>();
        private Dictionary<PrefabType, GameObject> m_prefabsCache = new Dictionary<PrefabType, GameObject>();

        public static PrefabManager instance;

        private void Awake()
        {
            instance = this;
            m_prefabsPath.Add(PrefabType.LocalPlayer, "Prefabs/LocalPlayer");
            m_prefabsPath.Add(PrefabType.RemotePlayer, "Prefabs/RemotePlayer");
            m_prefabsPath.Add(PrefabType.LocalBullet, "Prefabs/Bullet");
            m_prefabsPath.Add(PrefabType.RemoteBullet, "Prefabs/RemoteBullet");
            m_prefabsPath.Add(PrefabType.Room, "Prefabs/Room");
            m_prefabsPath.Add(PrefabType.PlayerHealthPanel, "Prefabs/PlayerHealthPanel");
            m_prefabsPath.Add(PrefabType.playerHealthItem, "Prefabs/PlayerHealthItem");
        }

        public GameObject LoadGameobject(PrefabType prefabType, Transform parent = null)
        {
            //如果cache已经有该prefab则直接实例化
            if (m_prefabsCache.TryGetValue(prefabType,out var prefab))
            {
                var go = Instantiate(prefab, parent);
                return go;
            }
            //cache中没有，则从m_prefabsPath查找路径并加载预制体，然后加入cache
            if (m_prefabsPath.TryGetValue(prefabType, out var path))
            {
                var prefab_ = Resources.Load<GameObject>(path);
                if (prefab_ == null)
                {
                    Debug.LogError($"未找到 {prefabType} in {path}");
                    return null;
                }
                m_prefabsCache.Add(prefabType,prefab_);
                var go = Instantiate(prefab_, parent);
                return go;
            }
            else
            {
                Debug.LogError($"此预制体未绑定路径{prefabType}");
                return null;
            }
        }
    }
}