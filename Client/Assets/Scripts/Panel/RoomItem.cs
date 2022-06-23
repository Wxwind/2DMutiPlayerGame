using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Panel
{
    public class RoomItem : MonoBehaviour,IPointerClickHandler
    {
        public TMP_Text hostName;
        public TMP_Text roomName;
        public TMP_Text playNum;
        public string roomId;
        public Color highLightColor;
        public Color unHighLightColor;
        public Action<RoomItem> OnClick;

        private Image m_image;

        private void Awake()
        {
            m_image = GetComponent<Image>();
        }

        public void SetInfo(string _hostName,string _roomName,int _playNum,int _maxNum,string _roomId)
        {
            hostName.text = _hostName;
            roomName.text = _roomName;
            playNum.text = $"{_playNum}/{_maxNum}";
            roomId = _roomId;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick(this);
        }

        public void HighLight()
        {
            m_image.color = highLightColor;
        }

        public void UnHighLight()
        {
            m_image.color = unHighLightColor;
        }

        public  bool Equals(RoomItem other)
        {
            return roomId==other.roomId;
        }
    }

}


