using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationPopUp : MonoBehaviour
{
    public Vector3 point;
    public GameObject modelPrompt;

    public void RequestGeneration()
    {
        modelPrompt.SetActive(true);
        Cursor3D.instance.blocked = true;
        Cursor3D.instance.toggleLoadFX(true);
        Player.Instance.libAdd = false;
        //modelPrompt.GetComponent<GenerativeChoice>().generationPos = point;
        gameObject.SetActive(false);
    }
}
