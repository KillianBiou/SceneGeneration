using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationGaze : MonoBehaviour
{



    [SerializeField]
    private float angleTolerance;

    [SerializeField]
    private InputActionProperty gaze, resetToGaze;

    private bool getBack = false;

    void Update()
    {
        Quaternion gazeAngle = gaze.action.ReadValue<Quaternion>();

        if (getBack)
        {
            //lerp
            if (ClosestDiff(gazeAngle.eulerAngles.y, gameObject.transform.rotation.eulerAngles.y) < 1)
                getBack = false;
            return;
        }


        float diff = ClosestDiff(gazeAngle.eulerAngles.y, gameObject.transform.rotation.eulerAngles.y);

        if (diff > angleTolerance)
            ResetRotToGaze(gazeAngle.eulerAngles.y);//getBack = true;

        if(resetToGaze.action.ReadValue<float>() > 0)
            ResetRotToGaze(gazeAngle.eulerAngles.y);
    }

    private void ResetRotToGaze(float y)
    {
        gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.rotation.eulerAngles.x, y, gameObject.transform.rotation.eulerAngles.z);
    }


    public float ClosestDiff(float a, float b)
    {
        return Mathf.Min(Mathf.Abs(a - b), Mathf.Abs(Mathf.Max(a,b) - 360 - Mathf.Min(a, b)));
    }
}
