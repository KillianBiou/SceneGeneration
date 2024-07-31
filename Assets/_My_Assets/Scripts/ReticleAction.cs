using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleAction : MonoBehaviour
{

    public GameObject loading;

    public void StartLoading()
    {
        loading.SetActive(true);
        
    }

    public int EndingLoad(GameObject go)
    {
        loading.SetActive(false);
        return 0;
    }
}
