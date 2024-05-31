using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class WheelController : MonoBehaviour
{
    public GameObject cancelable;


    [SerializeField]
    private WheelUIController wheel;

    public InputActionProperty buttonHold, stick;

    [SerializeField]
    private UnityEvent<int> OutpuChoiceEvent;


    void Start()
    {
        buttonHold.action.started += ShowWheel;
        buttonHold.action.performed += ShowWheel;

        wheel.ChoiceDone.AddListener(ModeToggle);

        wheel.Init();
    }



    void Update()
    {

        if(buttonHold.action.ReadValue<float>() == 1)
        {
            Vector2 dir = stick.action.ReadValue<Vector2>();
            wheel.input = dir;
        }
    }

    public void ModeToggle(int n)
    {
        OutpuChoiceEvent.Invoke(n);
    }


    public void ShowWheel(InputAction.CallbackContext c)
    {
        wheel.gameObject.SetActive(1 == c.ReadValue<float>());

        if (cancelable != null)
            cancelable.SetActive(1 != c.ReadValue<float>());
    }
}
