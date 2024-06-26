using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LIghtPlacerVR : MonoBehaviour
{

    public EditmapMode edmod;
    public GameObject lightprefab;
    public GameObject hand;

    private bool placed = false;

    public InputActionProperty buttonHold;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
            return;


        if(edmod.state == EDIT_STATE.LIGHT && buttonHold.action.ReadValue<float>() == 1)
        {
            GameObject last = Instantiate(lightprefab);
            last.transform.position = hand.transform.position;
            placed = true;
        }
    }

    public void PlaceLight(SelectEnterEventArgs args)
    {
        if (edmod.state == EDIT_STATE.LIGHT)
        {
            GameObject last = Instantiate(lightprefab);
            last.transform.position = hand.transform.position;
        }
    }
}
