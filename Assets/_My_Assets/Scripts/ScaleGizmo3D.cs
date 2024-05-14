using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class ScaleGizmo3D : MonoBehaviour
{
    public GameObject target;

    [SerializeField]
    private GameObject X, Y, Z, W;
    private bool rx, ry, rz = false;
    private Vector3 mousePos, targetScale;


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


                mousePos = Input.mousePosition;
                targetScale = target.transform.localScale;
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

        Vector2 diff = Input.mousePosition - mousePos;

        if(rx && ry && rz)
        {
            target.transform.localScale = targetScale + new Vector3(.01f * -diff.x, .01f * -diff.x, .01f * -diff.x);
            return;
        }

        if (rx)
        {
            target.transform.localScale = targetScale + new Vector3(.01f * -diff.x, 0, 0);
        }
        if (ry)
        {
            target.transform.localScale = targetScale + new Vector3(0, .01f * diff.y, 0);
        }
        if (rz)
        {
            target.transform.localScale = targetScale + new Vector3(0, 0, .01f * diff.x);
        }
    }

    public void AssignTarget(GameObject t)
    {
        target = t;
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
        W.SetActive(false);
    }

    public void ShowGizmos()
    {
        X.SetActive(true);
        Y.SetActive(true);
        Z.SetActive(true);
        W.SetActive(true);
    }
}