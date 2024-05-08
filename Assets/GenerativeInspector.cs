using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class GenerativeInspector : MonoBehaviour
{

    public GameObject container;
    public GameObject MaterialBubble;
    public MatInfo matInfo;


    public void InspectMat()
    {
        matInfo.selectedId = 0;

        for (int i = container.transform.childCount; i > 0; i--)
        {
            Destroy(container.transform.GetChild(i - 1).gameObject);
        }

        for (int i = 0; i < matInfo.materials.Length; i++)
        {
            GameObject last = Instantiate(MaterialBubble);
            last.GetComponent<MatInspector>().Init(matInfo.materials[i], matInfo, i);
            last.transform.SetParent(container.transform, false);
        }
        container.SetActive(true);
    }
}
