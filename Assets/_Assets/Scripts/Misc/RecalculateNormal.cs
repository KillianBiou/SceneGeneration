using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecalculateNormal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }
}
