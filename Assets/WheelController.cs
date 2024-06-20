using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class WheelController : MonoBehaviour
{
    public GameObject cancelable;
    private bool wasCanceled = false;


    [SerializeField]
    private WheelUIController wheel;

    public InputActionProperty buttonHold, stick;

    [SerializeField]
    private UnityEvent<int> OutputChoiceEvent;


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
        OutputChoiceEvent.Invoke(n);
    }


    public void ShowWheel(InputAction.CallbackContext c)
    {
        bool newState = 1 == c.ReadValue<float>();

        wheel.gameObject.SetActive(newState);

        if (cancelable == null)
            return;

        if(newState && cancelable.activeSelf)
        {
            cancelable.SetActive(false);
            wasCanceled = true;
        }
        else if (newState)
        {
            cancelable.SetActive(false);
        }

        if (wasCanceled && !newState)
        {
            cancelable.SetActive(true);
            wasCanceled = false;
        }
    }
}
