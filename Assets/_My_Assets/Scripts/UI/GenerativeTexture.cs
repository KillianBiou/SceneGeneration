using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerativeTexture : MonoBehaviour
{

    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        materials = GetComponent<Renderer>().materials;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
