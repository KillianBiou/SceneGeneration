using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panner : MonoBehaviour
{

    public float panSpeed = 10f; // Adjust the speed as needed

    private bool isPanning;
    private Vector3 lastMousePosition;

    public GameObject camera;



    void Update()
    {
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
            // Calculate the difference in mouse position since last frame
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Adjust the camera's position based on mouse movement
            camera.transform.Translate(-mouseDelta * panSpeed * Time.deltaTime);

            // Update the last mouse position for the next frame
            lastMousePosition = Input.mousePosition;
        }
    }
}
