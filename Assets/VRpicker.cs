using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPicker : MonoBehaviour
{


    public MatInfo selectedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        selectedMaterial.Changed.AddListener(Refresh);
    }


    public void Refresh()
    {
        GetComponent<Renderer>().material.SetTexture("_BaseMap", selectedMaterial.texture);
    }
}
