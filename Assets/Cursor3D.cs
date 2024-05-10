using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Cursor3D : MonoBehaviour
{

    static public Cursor3D instance;

    private void Awake()
    {
        instance = this;
    }


    public void SetTransform(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
    public void SetTransform(Vector3 pos, Vector3 rot)
    {
        transform.position = pos;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rot);
    }
}
