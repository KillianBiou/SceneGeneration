using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Moveable : MonoBehaviour
{

    VR_GizmoTranslator translator;



    // Start is called before the first frame update
    void Start()
    {
        translator = FindFirstObjectByType<VR_GizmoTranslator>();
    }


    public void StartInterract()
    {
        translator.gameObject.SetActive(true);
        translator.gameObject.transform.position = transform.position;
    }

}
