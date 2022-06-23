using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthItem : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text hpText;

    private void Awake()
    {
       
    }
    
    public void SetPlayerName(string _name)
    {
        playerNameText.text = _name;
    }
    public void SetHp(string hpInfo)
    {
        hpText.text = hpInfo;
    }

    public void CheckNull()
    {
        Debug.Log($"playerNameText==null {playerNameText==null}");
        Debug.Log($"hpText==null {hpText==null}");
    }
}
