using System;
using System.Collections.Generic;
using UnityEngine;

namespace Panel
{
    enum PanelType
    {
        MainMenuPanel,
        LoginPanel,
        RegisterPanel,
        LobbyPanel,
        RoomPanel,
    }

    enum PanelState
    {
        running,
        standby
    }
    class UIManeger : MonoBehaviour
    {
        private Dictionary<PanelType, BasePanel> m_panelCacheDic = new Dictionary<PanelType, BasePanel>();
        private Dictionary<PanelType, string> m_panelsPath = new Dictionary<PanelType, string>();
        private Stack<BasePanel> m_runningPannels = new Stack<BasePanel>();
        private Transform m_canvasTrans;
        public static UIManeger instance { get; private set; }
        public Canvas canvas;
        private PanelState m_panelState = PanelState.running;

        private void Awake()
        {
            instance = this;
            m_canvasTrans = canvas.GetComponent<Transform>();
            LoadPanelPath();
        }
        

        private void LoadPanelPath()
        {
            m_panelsPath.Add(PanelType.RegisterPanel, "Prefabs/Panel/RegisterPanel");
            m_panelsPath.Add(PanelType.LoginPanel, "Prefabs/Panel/LoginPanel");
            m_panelsPath.Add(PanelType.MainMenuPanel, "Prefabs/Panel/MainMenuPanel");
            m_panelsPath.Add(PanelType.LobbyPanel, "Prefabs/Panel/LobbyPanel");
            m_panelsPath.Add(PanelType.RoomPanel, "Prefabs/Panel/RoomPanel");
        }

        /// <summary>
        /// 实例化panel并放入缓存字典中
        /// </summary>
        /// <param name="type"></param>
        private BasePanel SpawnPanel(PanelType type)
        {
            var prefab = Resources.Load<GameObject>(m_panelsPath[type]);
            if (prefab == null)
            {
                Debug.LogError($"未找到该panel:{type} in {m_panelsPath[type]}");
            }

            var panel = Instantiate(prefab, m_canvasTrans, false);
            BasePanel panelComponent = panel.GetComponent<BasePanel>();
            if (panelComponent == null)
            {
                Debug.LogError($"未找到BasePanel组件 in {panel.name}");
            }

            m_panelCacheDic.Add(type, panelComponent);
            return panelComponent;
        }

        /// <summary>
        /// 切换到其他panel
        /// </summary>
        /// <param name="panelType"></param>
        public void EnterPanel(PanelType panelType)
        {
            //Debug.Log("调用enterpanel函数");
            if (!m_panelCacheDic.ContainsKey(panelType))
            {
                if (m_runningPannels.Count > 0)
                {
                    m_runningPannels.Peek().OnHideAndFreeze();
                }
                var panel = SpawnPanel(panelType);
                m_runningPannels.Push(panel);
                panel.OnEnter();
                //Debug.Log("不在缓存中，新创建了"+m_runningPannels.Peek().GetType().Name);
            }
            //此panel已经被实例化并在stack中
            else if (m_runningPannels.Contains(m_panelCacheDic[panelType]))
            {
                while (m_runningPannels.Peek() != m_panelCacheDic[panelType])
                {
                    var p = m_runningPannels.Pop();
                    p.OnExit();
                }
                m_runningPannels.Peek().OnRecovery();
                //Debug.Log("在缓存中"+m_runningPannels.Peek().GetType().Name);
            }
            //此panel已经被实例化但未在stack中
            else
            {
                var p = m_panelCacheDic[panelType];
                p.OnRecovery();
                m_runningPannels.Peek().OnHideAndFreeze();
                m_runningPannels.Push(p);
            }
        }

        /// <summary>
        /// 退出当前panel
        /// </summary>
        public void ExitPanel()
        {
            if (m_runningPannels.Count != 0)
            {
                var p = m_runningPannels.Pop();
                p.OnExit();
            }

            BasePanel nowPanel = m_runningPannels.Peek();
            if (nowPanel != null)
            {
                nowPanel.OnRecovery();
            }
        }

        public BasePanel GetRunningPanel()
        {
            if (m_runningPannels.Count > 0)
            {
               return m_runningPannels.Peek();
            }
            else return null;
        }
        public BasePanel GetPanel(PanelType panelType)
        {
            if (m_panelCacheDic.ContainsKey(panelType))
            {
                return m_panelCacheDic[panelType];
            }
            else return null;
        }

        public void Hide()
        {
            if (m_panelState==PanelState.running&&canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(false);
                
            }

            m_panelState = PanelState.standby;
        }

        public void Show()
        {
            if (m_panelState==PanelState.standby&&!canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(true);
            }

            m_panelState = PanelState.running;
        }
    }
}