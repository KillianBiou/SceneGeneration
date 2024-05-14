using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Panner : MonoBehaviour
{

    public float panSpeed = 10f; // Adjust the speed as needed

    private bool isPanning;
    private Vector3 lastMousePosition;

    public GameObject camera;
    private Vector3 originPos;
    private Quaternion originRot;



    private void Awake()
    {
        originPos = camera.transform.position;
        originRot = camera.transform.rotation;
    }


    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(2)) // Check for middle mouse button press
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2)) // Check for middle mouse button release
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            camera.transform.Translate(-mouseDelta * panSpeed * Time.deltaTime);
            lastMousePosition = Input.mousePosition;
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            camera.transform.position = originPos;
            camera.transform.rotation = originRot;
        }

        camera.transform.position += camera.transform.forward * Input.mouseScrollDelta.y * 1;

    }
}
