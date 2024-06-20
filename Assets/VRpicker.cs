using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPicker : MonoBehaviour
{

    public GameObject pannelpick;
    public MatInfo selectedMaterial;
    public InteractionLayerMask imask;

    public GameObject toDisable;

    // Start is called before the first frame update
    void Start()
    {
        selectedMaterial.Changed.AddListener(Refresh);
    }


    public void Refresh()
    {
        pannelpick.GetComponent<Renderer>().material.SetTexture("_BaseMap", selectedMaterial.texture);
    }

    private void OnEnable()
    {
        toDisable.SetActive(false);
    }

    private void OnDisable()
    {
        toDisable.SetActive(true);
    }

    public void Paint(SelectEnterEventArgs args)
    {
        if (!(args.interactableObject.interactionLayers == imask))
            return;

        args.interactableObject.transform.gameObject.GetComponent<Renderer>().material.SetTexture("_Texture2D", selectedMaterial.texture);
    }
}
