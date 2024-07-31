using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VR_LightEdit : MonoBehaviour
{
    public TileLight selectedTileLightRef;


    [SerializeField]
    private InputActionProperty buttonHold;
    private Transform interractorT;
    private bool isDraging;
    private float heightOffset;

    [SerializeField]
    private GameObject vrLightInfo;


    void Update()
    {
        if(!selectedTileLightRef)
            return;

        if(isDraging)
        {
            selectedTileLightRef.SetLightHeight(interractorT.position.y - heightOffset);
        }
    }

    public void SetLightIntensity(float f)
    {
        selectedTileLightRef.SetLightIntensity(f);
    }

    public void SetLightColor(Color c)
    {
        selectedTileLightRef.SetLightColor(c);
    }

    public void LightBeginDrag(SelectEnterEventArgs args)
    {
        interractorT = args.interactorObject.transform;
        isDraging = true;
        heightOffset = interractorT.position.y - selectedTileLightRef.GetLightHeight();
    }

    public void LightEndDrag(SelectExitEventArgs args)
    {
        isDraging = false;
    }

    public void SetLightRange(float f)
    {
        selectedTileLightRef.SetLightRange(f);
    }

    public void LightSelect(SelectEnterEventArgs args)
    {
        if(selectedTileLightRef != args.interactableObject.transform.parent.parent.GetComponent<TileLight>())
        {
            selectedTileLightRef = args.interactableObject.transform.parent.parent.GetComponent<TileLight>();
            VR_LightInfo.Instance.gameObject.SetActive(true);
            VR_LightInfo.Instance.transform.position = args.interactableObject.transform.position;
            VR_LightInfo.Instance.Init(selectedTileLightRef);
        }
        else
        {
            LightBeginDrag(args);
        }
    }

    private void OnDisable()
    {
        selectedTileLightRef = null;
        vrLightInfo.SetActive(false);
    }
}
