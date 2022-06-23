using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game
{
    public class BasePlayer : MonoBehaviour
    {
        //public GameObject playerHealthItemPre;

        protected string ip;
        protected string playerName;

        protected int hp;

        //1==right -1==left
        public int faceDir;
        public bool isLocalPlayer;
        public int playerId;
        public string GetIp => ip;

        protected TextMesh m_textMesh;
        protected SpriteRenderer m_spriteRenderer;
        private PlayerHealthItem m_playerHealthItem;
        private GameObject m_playerHealthPanel;



        protected virtual void Awake()
        {
            m_textMesh = GetComponentInChildren<TextMesh>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {

        }

        public virtual void Init(int _id,string _ip, string _playerName, int _hp, Vector2 respawnPoint, int _facedir,GameObject playerHealthPanel)
        {
            playerId = _id;
            ip = _ip;
            playerName = _playerName;
            hp = _hp;
            transform.position = respawnPoint;
            faceDir = _facedir;
            m_playerHealthPanel = playerHealthPanel;
            Transform parent = m_playerHealthPanel.GetComponent<RectTransform>();
            var go=PrefabManager.instance.LoadGameobject(PrefabType.playerHealthItem,parent);
            m_playerHealthItem = go.GetComponent<PlayerHealthItem>();
            SetName(playerName);
            m_playerHealthItem.SetPlayerName(playerName);
            m_playerHealthItem.SetHp($"生命值：{hp}");
        }

        public void SetName(string _name)
        {
            m_textMesh.text = playerName;
        }

        public void SetColor(Color color)
        {
            m_spriteRenderer.color = color;
        }
        
        public void OnReceive_PlayerDamege(int attack)
        {
            hp -= attack;
            m_playerHealthItem.SetHp($"生命值：{hp}");
        }
    }
}