using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class VR_Translator : MonoBehaviour
{

    public GameObject target, controller;

    public bool X, Y, Z;


    private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        if ((!X && !Y) && !Z)
            return;



        Vector3 newPos = target.transform.position;

        if (X)
        {
            newPos.x = controller.transform.position.x + offset.x;
        }

        if (Y)
        {
            newPos.y = controller.transform.position.y + offset.y;
        }

        if (Z)
        {
            newPos.z = controller.transform.position.z + offset.z;
        }


        target.transform.position = newPos;
    }


    public void Grabbing(bool x, bool y, bool z)
    {
        X = x;
        Y = y;
        Z = z;

        offset = target.transform.position - controller.transform.position;
    }

    public void Release()
    {
        X = false;
        Y = false;
        Z = false;
    }


}
