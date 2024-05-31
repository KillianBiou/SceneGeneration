using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelUiButton : MonoBehaviour
{
    public GameObject visual;



    public void ResetRot()
    {
        visual.transform.localEulerAngles = new Vector3(0,0,0);
    }
}
