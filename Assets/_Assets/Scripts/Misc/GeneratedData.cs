using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedData : MonoBehaviour
{
    public Transform moveTransform;
    public Transform rotateTransform;
    public Transform scaleTransform;

    public void GeneratedModelSetup(Transform parent)
    {
        Debug.Log("PivotPoint Setup");

        transform.tag = "3D generated";
        GetComponent<MeshRenderer>().material = GlobalVariables.Instance.GetBaseMaterial();

        moveTransform = parent;
        rotateTransform = transform;
        scaleTransform = parent;
    }
}
