using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteText : MonoBehaviour
{
    void Start()
    {
        var parent = transform.parent;
        var parentRenderer = parent.GetComponent<Renderer>();
        var r = GetComponent<Renderer>();
        r.sortingLayerID = parentRenderer.sortingLayerID;
        r.sortingOrder = parentRenderer.sortingOrder;
    }
}
