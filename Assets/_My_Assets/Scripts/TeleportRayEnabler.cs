using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportRayEnabler : MonoBehaviour
{


    public XRRayInteractor teleportator;
    public List<XRRayInteractor> interactors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool ok = true;
        foreach (var interactor in interactors)
        {
            if (interactor.TryGetHitInfo(out Vector3 pos, out Vector3 norm, out int n, out bool b) && b)
                ok = false;
        }

        teleportator.enabled = ok;
    }
}
