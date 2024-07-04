using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LIghtPlacerVR : MonoBehaviour
{

    public EditmapMode edmod;
    public GameObject lightprefab;
    public GameObject hand;
    private TileLight preview;


    private bool placed = false;

    public InputActionProperty buttonHold, buttonConfirm;

    // Start is called before the first frame update
    void Start()
    {
        GameObject last = Instantiate(lightprefab);
        preview = last.GetComponent<TileLight>();
        preview.transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (preview == null)
            return;


        if(buttonHold.action.ReadValue<float>() == 1)
        {
            if (placed)
                return;

            //display a preview light
            preview.transform.gameObject.SetActive(true);
            preview.transform.position = new Vector3(hand.transform.position.x, 0, hand.transform.position.z);
            preview.lightObj.transform.position = new Vector3(preview.lightObj.transform.position.x, hand.transform.position.y, preview.lightObj.transform.position.z);

            //place a copy of preview light in scene
            if (buttonConfirm.action.ReadValue<float>() > 0.5 && !placed)
            {
                GameObject last = Instantiate(lightprefab);
                last.transform.position = new Vector3(hand.transform.position.x, 0, hand.transform.position.z);
                last.GetComponent<TileLight>().lightObj.transform.position = new Vector3(preview.lightObj.transform.position.x, hand.transform.position.y, preview.lightObj.transform.position.z);
                last.GetComponent<TileLight>().ActivateRemover();
                RoomMap.Instance.llights.Add(last);
                placed = true;
            }
        }
        else
        {
            preview.transform.gameObject.SetActive(false);
            placed = false;
        }
    }
}
