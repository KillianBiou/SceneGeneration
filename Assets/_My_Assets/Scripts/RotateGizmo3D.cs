using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class RotateGizmo3D : MonoBehaviour
{
    public GameObject target;

    [SerializeField]
    private GameObject X, Y, Z;
    private bool rx, ry, rz = false;
    private Vector3 mousePos, targetRot;


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
                {
                    rx = true;
                    mousePos = Input.mousePosition;
                    targetRot = target.transform.eulerAngles;
                }
                if (hit.collider.gameObject == Y)
                {
                    ry = true;
                    mousePos = Input.mousePosition;
                    targetRot = target.transform.eulerAngles;
                }
                if (hit.collider.gameObject == Z)
                {
                    rz = true;
                    mousePos = Input.mousePosition;
                    targetRot = target.transform.eulerAngles;
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.tag == "3D generated")
                    {
                        if (target != hit.collider.gameObject)
                            AssignTarget(hit.collider.gameObject);
                        ShowGizmos();
                    }
                    else
                        HideGizmos();

                    return;
                }
            }
        }

        if (target == null)
            return;

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            rx = false;
            ry = false;
            rz = false;
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        Vector2 diff = Input.mousePosition - mousePos;
        if (rx)
        {
            target.transform.eulerAngles = targetRot + new Vector3(.1f * diff.x, 0, 0);
            gameObject.transform.eulerAngles = new Vector3(.1f * diff.x, 0, 0);
        }
        if (ry)
        {
            target.transform.eulerAngles = targetRot + new Vector3(0, .1f * diff.x, 0);
            gameObject.transform.eulerAngles = new Vector3(0, .1f * diff.x, 0);
        }
        if (rz)
        {
            target.transform.eulerAngles = targetRot + new Vector3(0, 0, .1f * diff.y);
            gameObject.transform.eulerAngles = new Vector3(0, 0, .1f * diff.y);
        }
    }

    public void AssignTarget(GameObject t)
    {
        target = t.GetComponent<GeneratedData>().rotateTransform.gameObject;
        gameObject.transform.position = target.transform.position;
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
    }

    public void ShowGizmos()
    {
        X.SetActive(true);
        Y.SetActive(true);
        Z.SetActive(true);
    }
}