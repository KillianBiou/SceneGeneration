using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class ScaleGizmo3D : Gizmo3D
{
    public override void TargetReset()
    {
        target.transform.localScale = Vector3.one;
    }

    public override void AssignTarget(GameObject t)
    {
        target = t.GetComponent<GeneratedData>().scaleTransform.gameObject;
        gameObject.transform.position = target.transform.position;
    }

    public override void Treatment()
    {
        Vector2 diff = Input.mousePosition - beginMousePos;

        if (rx && ry && rz)
        {
            target.transform.localScale = beginTargetScale + new Vector3(.01f * -diff.x, .01f * -diff.x, .01f * -diff.x);
            return;
        }

        if (rx)
        {
            target.transform.localScale = beginTargetScale + new Vector3(.01f * -diff.x, 0, 0);
        }
        if (ry)
        {
            target.transform.localScale = beginTargetScale + new Vector3(0, .01f * diff.y, 0);
        }
        if (rz)
        {
            target.transform.localScale = beginTargetScale + new Vector3(0, 0, .01f * diff.x);
        }
    }
}