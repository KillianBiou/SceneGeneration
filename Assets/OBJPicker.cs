using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPicker : MonoBehaviour
{
    public void Pick()
    {
        GenerationDatabase.Instance.SpawnObject(transform.name, transform.position, transform.rotation, transform.localScale);
    }
}
