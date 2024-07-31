using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public abstract class Gizmo3D : MonoBehaviour
{
    public GameObject target;
    public Plane XZplane;

    [SerializeField]
    protected GameObject X, Y, Z, W, reset;
    protected bool rx, ry, rz = false;
    protected Vector3 beginMousePos, beginTargetPosition, beginTargetRotation, beginTargetScale;
    protected Vector3 XZbeginMousePosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            int layerMask = 1 << 5;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if (hit.collider.gameObject == X)
                    rx = true;
                
                if (hit.collider.gameObject == Y)
                    ry = true;

                if (hit.collider.gameObject == Z)
                    rz = true;

                if (hit.collider.gameObject == W)
                {
                    rx = true;
                    ry = true;
                    rz = true;
                }

                if (hit.collider.gameObject == reset)
                {
                    TargetReset();
                }

                beginMousePos = Input.mousePosition;
                beginTargetPosition = target.transform.position;
                beginTargetRotation = target.transform.rotation.eulerAngles;
                beginTargetScale = target.transform.localScale;

                float enter;
                if(XZplane.Raycast(ray, out enter))
                    XZbeginMousePosition = ray.GetPoint(enter);
            }
            else
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.tag == "3D generated")
                    {
                        if(target != hit.collider.gameObject)
                            AssignTarget(hit.collider.gameObject);
                        ShowGizmos();
                    }
                    else
                        HideGizmos();

                    return;
                }
            }
        }

        if(target == null)
            return;

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            rx = false;
            ry = false;
            rz = false;
        }

        Treatment();
    }

    
    public virtual void AssignTarget(GameObject t)
    {
        target = t;
        gameObject.transform.position = target.transform.position;
        XZplane = new Plane(Vector3.up, target.transform.position);
    }

    private void OnDisable()
    {
        target = null;
    }

    public void HideGizmos()
    {
        X.SetActive(false);
        Y.SetActive(false);
        Z.SetActive(false);
        if (W != null)
            W.SetActive(false);
        if (reset != null)
            reset.SetActive(false);
    }

    public void ShowGizmos()
    {
        X.SetActive(true);
        Y.SetActive(true);
        Z.SetActive(true);
        if(W != null)
            W.SetActive(true);
        if (reset != null)
            reset.SetActive(true);
    }

    public abstract void TargetReset();

    public abstract void Treatment();
}