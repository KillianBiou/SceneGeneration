using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class MoveGizmo3D : Gizmo3D
{

    public override void TargetReset()
    {
        target.transform.localScale = Vector3.one;
    }

    public override void AssignTarget(GameObject t)
    {
        base.AssignTarget(t);
        target = t.GetComponent<GeneratedData>().rotateTransform.gameObject;
    }

    public override void Treatment()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector2 diff = Input.mousePosition - beginMousePos;

        Vector3 difff = new Vector3(0, 0, 0);

        float enter;
        if (XZplane.Raycast(ray, out enter))
            difff = ray.GetPoint(enter) - XZbeginMousePosition;


        if (rx)
        {
            target.transform.position = beginTargetPosition + new Vector3(difff.x, 0, 0);
            gameObject.transform.position = target.transform.position;
        }
        if (ry)
        {
            target.transform.position = beginTargetPosition + new Vector3(0,0.01f * diff.y, 0);
            gameObject.transform.position = target.transform.position;
        }
        if (rz)
        {
            target.transform.position = beginTargetPosition + new Vector3(0, 0, difff.z);
            gameObject.transform.position = target.transform.position;
        }
    }
}