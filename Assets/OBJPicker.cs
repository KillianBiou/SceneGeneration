using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBJPicker : MonoBehaviour
{
    public void Pick()
    {
        GenerationDatabase.Instance.GetObjectEvent(transform.name);
    }
}
