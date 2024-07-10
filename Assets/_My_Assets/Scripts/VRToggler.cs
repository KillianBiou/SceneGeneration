using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;

public class VRToggler : MonoBehaviour
{
    [SerializeField]
    public InputActionProperty isTracking;
    [SerializeField]
    private UnityEvent TurnOnVR, TurnOffVR;


    public static VRToggler Instance;
    private bool isVrOn = false;

    private void Awake()
    {
        Instance = this;
        isVrOn = isTracking.action.ReadValue<int>() == 15;
        ToggleVR(isVrOn);
    }

    private void Update()
    {
        if (isVrOn != (isTracking.action.ReadValue<int>() == 15))
        {
            ToggleVR(isTracking.action.ReadValue<int>() == 15);
        }
    }

    private void ToggleVR(bool state)
    {
        isVrOn = state;

        if(isVrOn)
        {
            TurnOnVR.Invoke();
        }
        else
        {
            TurnOffVR.Invoke();
        }
    }

}
