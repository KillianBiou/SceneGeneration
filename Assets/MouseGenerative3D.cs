using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseGenerative3D : MonoBehaviour
{

    public bool positionLocked;

    private void OnEnable()
    {
        positionLocked = false;
    }


    // Update is called once per frame
    void Update()
    {

        Ray rayOne = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitOne;
        bool onUI = false;
        if (Physics.Raycast(rayOne, out hitOne, 100))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                onUI = true;
            }
        }

        if (onUI)
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                {
                    Cursor3D.instance.SetTransform(hit.point, hit.normal);
                }
            }
            positionLocked = true;
        }

        if (positionLocked)
            return;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.gameObject.GetComponent<Renderer>() != null)
            {
                Cursor3D.instance.SetTransform(hit.point, hit.normal);
            }
        }
    }
}
