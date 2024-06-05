using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectPlacer : MonoBehaviour
{

    [SerializeField]
    private GameObject _cpRef;

    [SerializeField]
    private XRRayInteractor _ray;

    private bool isLoading;


    private void Start()
    {
        //ObjectManager.StartLoadingOBJ.AddListener(StartLoad);
        //ObjectManager.EndLoadingOBJ.AddListener(EndLoad);
    }


    public void Place(SelectExitEventArgs args)
    {
        if (isLoading)
            return;


        GameObject last = Instantiate(_cpRef.transform.GetChild(0).gameObject);
        _ray.TryGetHitInfo(out Vector3 pos, out Vector3 norm, out int i, out bool b);
        last.transform.position = pos;
        //last.transform.localScale = new Vector3(1, 1, 1);
    }


    public void StartLoad()
    {
        isLoading = true;
        //startFX
    }

    public void EndLoad(GameObject last)
    {
        isLoading = false;

        if (_cpRef.transform.childCount > 0)
            Destroy(_cpRef.transform.GetChild(0));

        last.transform.SetParent(_cpRef.transform, false);

        //stopFX
    }

}
