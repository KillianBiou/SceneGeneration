using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParentCheck : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            SetupChild();
        }
    }

    void SetupChild()
    {
        transform.GetChild(0).AddComponent<GeneratedData>();
        Destroy(this);
    }
}
