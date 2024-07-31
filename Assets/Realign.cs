using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Realign : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.tag == "3D generated")
                {
                    Align(hit.collider.gameObject, hit.normal);
                }
            }
        }
    }


    public void Align(GameObject go, Vector3 normal)
    {
        // Calculez la rotation nécessaire pour aligner la normale avec Vector3.up
        Quaternion targetRotation = Quaternion.FromToRotation(normal, Vector3.up);

        // Appliquez cette rotation à l'objet
        go.transform.rotation = targetRotation * go.transform.rotation;
    }
}
