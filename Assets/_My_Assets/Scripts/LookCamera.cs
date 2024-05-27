using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class LookCamera : MonoBehaviour
{
    public Vector3 rotationOffset = new Vector3();
    private Transform cam;


    private void Awake()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam);
        transform.Rotate(rotationOffset);
    }
}
