using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public abstract class Gizmo3D : MonoBehaviour
{
    public GameObject target;
    public Plane XZplane, XYplane, YZplane;

    [SerializeField]
    protected GameObject X, Y, Z, W, reset, XZ;
    protected bool rx, ry, rz = false;
    protected Vector3 beginMousePos, beginTargetPosition, beginTargetRotation, beginTargetScale;
    protected Quaternion biginTargetRotation;
    protected Vector3 XZbeginMousePosition, XYbeginMousePosition, YZbeginMousePosition;

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

                if(hit.collider.gameObject == XZ)
                {
                    rx = true;
                    rz = true;
                }

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

                XZplane = new Plane(Vector3.up, target.transform.position);
                XYplane = new Plane(Vector3.forward * (Camera.main.transform.position.z > target.transform.position.z ? 1 : -1), target.transform.position);
                YZplane = new Plane(Vector3.left * (Camera.main.transform.position.x > target.transform.position.x ? 1 : -1), target.transform.position);

                beginMousePos = Input.mousePosition;
                beginTargetPosition = target.transform.position;
                biginTargetRotation = target.transform.rotation;
                beginTargetRotation = target.transform.rotation.eulerAngles;
                beginTargetScale = target.transform.localScale;

                float enter;
                if (XZplane.Raycast(ray, out enter))
                    XZbeginMousePosition = ray.GetPoint(enter);

                if (XYplane.Raycast(ray, out enter))
                    XYbeginMousePosition = ray.GetPoint(enter);

                if (YZplane.Raycast(ray, out enter))
                    YZbeginMousePosition = ray.GetPoint(enter);
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
            transform.rotation = Quaternion.identity;
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
        XYplane = new Plane(Vector3.forward, target.transform.position);
        YZplane = new Plane(Vector3.left, target.transform.position);
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
        if (XZ != null)
            XZ.SetActive(false);
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
        if(XZ != null)
            XZ.SetActive(true);
        if (reset != null)
            reset.SetActive(true);
    }

    public abstract void TargetReset();

    public abstract void Treatment();
}