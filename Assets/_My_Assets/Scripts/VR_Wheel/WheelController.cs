using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public struct WheelVisuals
{
    [SerializeField]
    public string name;
    [SerializeField]
    public Sprite img;
}

public class WheelController : MonoBehaviour
{

    [SerializeField]
    private List<WheelVisuals> visuals;

    [SerializeField]
    private List<XRRayInteractor> cancellables;


    [SerializeField]
    private WheelUIController wheel;

    public InputActionProperty buttonHold, stick;


    [SerializeField]
    private UnityEvent<int> OutputChoiceEvent;
    [SerializeField]
    private bool floatOut;
    [SerializeField]
    [Tooltip("Retrun a value between 0 and 1")]
    private UnityEvent<float> OutputFloatEvent;


    void Start()
    {
        buttonHold.action.started += ShowWheel;
        buttonHold.action.performed += ShowWheel;

        wheel.ChoiceDone.AddListener(CallbackEvents);
        if (floatOut)
            wheel.ChoiceFloatDone.AddListener(CallbackEvents);
        if(!floatOut)
            wheel.Init(visuals);
    }



    void Update()
    {

        if(buttonHold.action.ReadValue<float>() == 1)
        {
            Vector2 dir = stick.action.ReadValue<Vector2>();
            wheel.input = dir;
        }
    }

    public void CallbackEvents(int n)
    {
        OutputChoiceEvent.Invoke(n);
    }
    public void CallbackEvents(float f)
    {
        OutputFloatEvent.Invoke(f);
    }


    public void ShowWheel(InputAction.CallbackContext c)
    {
        bool newState = 1 == c.ReadValue<float>();

        wheel.gameObject.SetActive(newState);

        foreach (XRRayInteractor ray in cancellables)
            ray.gameObject.SetActive(!newState);// = !newState;
    }
}
