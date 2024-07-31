using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VR_CubeCreator : MonoBehaviour
{

    [SerializeField]
    private InputActionProperty buttonHold, confirm;
    public GameObject hand;

    private Vector3 originPos, targetPos;
    private GameObject last;

    // Update is called once per frame
    void Update()
    {
        if(buttonHold.action.ReadValue<float>() == 1.0f)
        {
            if (last != null)
            {
                targetPos = hand.transform.position;
                last.transform.position = (originPos + targetPos)/2;
                last.transform.localScale = targetPos - originPos;

                if (confirm.action.ReadValue<float>() == 1.0f)
                {

                    last = null;
                }
            }
            else
            {
                last = GameObject.CreatePrimitive(PrimitiveType.Cube);
                originPos = hand.transform.position;
            }
        }
        else
        {
            if(last != null)
            {
                Destroy(last);
                last = null;
            }
        }
    }
}
