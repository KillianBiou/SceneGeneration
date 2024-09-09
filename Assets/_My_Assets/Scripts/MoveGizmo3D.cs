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
        if (!rx && !ry && !rz)
            return;


        Vector2 diff = Input.mousePosition - beginMousePos;

        Vector3 difff = new Vector3(0, 0, 0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (XZplane.Raycast(ray, out enter))
            difff = ray.GetPoint(enter) - XZbeginMousePosition;

        Vector3 offset = new Vector3();

        if (rx)
        {
            offset += new Vector3(difff.x, 0, 0);
        }
        if (ry)
        {
            offset += new Vector3(0, 0.01f * diff.y, 0);
        }
        if (rz)
        {
            offset += new Vector3(0, 0, difff.z);
        }

        target.transform.position = beginTargetPosition + offset;
        gameObject.transform.position = target.transform.position;
    }
}