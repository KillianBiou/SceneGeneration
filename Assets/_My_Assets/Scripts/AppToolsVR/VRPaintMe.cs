using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPaintMe : MonoBehaviour
{

    [SerializeField]
    private MatInfo _mat;



    public void PaintMe()
    {
        GetComponent<Renderer>().material.SetTexture("_BaseMap", _mat.texture);
    }
}
