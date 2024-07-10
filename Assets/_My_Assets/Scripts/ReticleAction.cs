using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleAction : MonoBehaviour
{

    public GameObject loading;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


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
