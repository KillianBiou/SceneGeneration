using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class RotateGizmo3D : Gizmo3D
{
    public override void TargetReset()
    {
        target.transform.localScale = Vector3.one;
    }

    public override void AssignTarget(GameObject t)
    {
        target = t.GetComponent<GeneratedData>().rotateTransform.gameObject;
        gameObject.transform.position = target.transform.position;
    }

    public override void Treatment()
    {
        Vector2 diff = Input.mousePosition - beginMousePos;

        if (rx)
        {
            target.transform.eulerAngles = beginTargetRotation + new Vector3(.1f * diff.x, 0, 0);
            gameObject.transform.eulerAngles = new Vector3(.1f * diff.x, 0, 0);
        }
        if (ry)
        {
            target.transform.eulerAngles = beginTargetRotation + new Vector3(0, .1f * diff.x, 0);
            gameObject.transform.eulerAngles = new Vector3(0, .1f * diff.x, 0);
        }
        if (rz)
        {
            target.transform.eulerAngles = beginTargetRotation + new Vector3(0, 0, .1f * diff.y);
            gameObject.transform.eulerAngles = new Vector3(0, 0, .1f * diff.y);
        }
    }
}