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



    void Update()
    {
        Quaternion gazeAngle = gaze.action.ReadValue<Quaternion>();

        float diff = Mathf.Abs(gazeAngle.eulerAngles.y - gameObject.transform.rotation.eulerAngles.y);

        if (diff > angleTolerance)
            ResetRotToGaze(gazeAngle.eulerAngles.y);

        if(resetToGaze.action.ReadValue<float>() > 0)
            ResetRotToGaze(gazeAngle.eulerAngles.y);
    }

    private void ResetRotToGaze(float y)
    {
        gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.rotation.eulerAngles.x, y, gameObject.transform.rotation.eulerAngles.z);
    }
}
