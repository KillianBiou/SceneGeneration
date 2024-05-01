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
        modelPrompt.GetComponent<GenerativeChoice>().generationPos = point;
        gameObject.SetActive(false);
    }
}
