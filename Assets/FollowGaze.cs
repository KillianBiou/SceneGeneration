using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowGaze : MonoBehaviour
{

    public InputActionProperty gazeRot, gazePos;
    public Quaternion offsetRot, offsetPos;

    void Update()
    {
        transform.position = gazePos.action.ReadValue<Vector3>();
        transform.rotation = gazeRot.action.ReadValue<Quaternion>();
    }

}
