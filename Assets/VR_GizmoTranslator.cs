using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VR_GizmoTranslator : MonoBehaviour
{

    public bool X, Y, Z;

    public VR_Translator translator;

    public void StartTranslate()
    {
        translator.controller =  gameObject.GetComponent<XRSimpleInteractable>().interactorsSelecting[0].transform.gameObject;
        translator.Grabbing(X, Y, Z);
    }
}
