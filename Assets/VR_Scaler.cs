using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VR_Scaler : MonoBehaviour
{

    private GameObject target;
    public GameObject lHand, rHand;
    private float handDist;
    private Vector3 beginScale;
    private bool isScalling;

    [SerializeField]
    private InputActionProperty leftScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        if(leftScale.action.ReadValue<float>() == 1.0f && !isScalling)
        {
            isScalling = true;
            handDist = Vector3.Distance(lHand.transform.position, rHand.transform.position);
            beginScale = target.transform.localScale;
        }
        else if(leftScale.action.ReadValue<float>() != 1.0f)
            isScalling = false;

        if (isScalling)
        {
            target.transform.localScale = beginScale * (Vector3.Distance(lHand.transform.position, rHand.transform.position) / handDist);
        }

    }


    public void SetTarget(SelectEnterEventArgs args)
    {
        target = args.interactableObject.transform.gameObject;
    }

    public void UnSetTarget()
    {
        target = null;
        isScalling = false;
    }


}
