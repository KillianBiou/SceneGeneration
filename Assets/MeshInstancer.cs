using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInstancer : MonoBehaviour
{

    public object spawnObj;
    public GameObject spawnPos;


    public void SpawnObject()
    {
        if(spawnObj == null || !spawnPos)
            return;

        //Object last = Instantiate(spawnObj, spawnPos.transform.position, spawnPos.transform.rotation);
    }
}
