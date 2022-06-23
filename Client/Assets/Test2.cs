using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public RectTransform playerHealthTransfom;
    // Start is called before the first frame update
    void Start()
    {
        playerHealthTransfom=GameObject.Find("PlayerHealthPanel").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
