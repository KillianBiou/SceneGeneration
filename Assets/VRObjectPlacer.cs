using AsImpL;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectPlacer : MonoBehaviour
{
    public Material matRef;

    public LayerMask mask;
    public InteractionLayerMask imask;

    [SerializeField]
    private GameObject _cpRef;

    [SerializeField]
    private XRRayInteractor _ray;

    private bool isLoading;


    private void Start()
    {
        GenerationDatabase.Instance.StartLoadingOBJ.AddListener(StartLoad);
    }


    public void Place(SelectExitEventArgs args)
    {
        if (isLoading)
            return;


        GameObject last = Instantiate(_cpRef.transform.GetChild(0).gameObject);
        _ray.TryGetHitInfo(out Vector3 pos, out Vector3 norm, out int i, out bool b);
        last.transform.position = pos;
        //last.transform.parent = 

        Rigidbody rb = last.transform.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;

        XRGrabInteractable grab = last.transform.AddComponent<XRGrabInteractable>();
        grab.interactionLayers = imask;
        grab.interactionLayerMask = mask;
        grab.trackRotation = false;
        //grab.colliders = new List<Collider>();
    }


    public void StartLoad(GameObject go)
    {
        ObjectImporter.Instance.ImportingComplete += EndLoad;
        isLoading = true;

        if (_cpRef.transform.childCount > 0)
            Destroy(_cpRef.transform.GetChild(0).gameObject);

        go.transform.SetParent(_cpRef.transform, false);

        //startFX
    }

    public void EndLoad()
    {
        ObjectImporter.Instance.ImportingComplete -= EndLoad;

        isLoading = false;

        _cpRef.transform.GetChild(0).transform.SetParent(_cpRef.transform, false);
        _cpRef.transform.GetChild(0).transform.localPosition = Vector3.zero;
        _cpRef.transform.GetChild(0).transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = matRef;
        //stopFX
    }

}
