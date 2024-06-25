using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedData : MonoBehaviour
{
    public Transform moveTransform;
    public Transform rotateTransform;
    public Transform scaleTransform;

    private void Start()
    {
        //GetComponent<MeshFilter>().mesh.RecalculateNormals();
        //StartCoroutine(DelaySet());
    }

    private IEnumerator DelaySet()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
        GetComponent<MeshRenderer>().material = GlobalVariables.Instance.GetBaseMaterial();
        transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;
    }

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
