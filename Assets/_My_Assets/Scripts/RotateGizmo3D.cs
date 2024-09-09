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
        base.AssignTarget(t);
        target = t.GetComponent<GeneratedData>().rotateTransform.gameObject;
        gameObject.transform.position = target.transform.position;
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


        Quaternion offset = Quaternion.identity;
        Vector3 oofset = new Vector3();

        if (rx)
        {
            if (XZplane.Raycast(ray, out enter))
                difff = ray.GetPoint(enter);



            offset *= Quaternion.LookRotation(Vector3.Normalize(difff - target.transform.position), Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(Vector3.Normalize(XZbeginMousePosition - target.transform.position), Vector3.up));
            oofset = Quaternion.LookRotation(Vector3.Normalize(difff - target.transform.position), Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(Vector3.Normalize(XZbeginMousePosition - target.transform.position), Vector3.up)).eulerAngles;
        }
        if (ry)
        {
            if (XYplane.Raycast(ray, out enter))
                difff = ray.GetPoint(enter);

            offset *= Quaternion.LookRotation(Vector3.Normalize(difff - target.transform.position), Vector3.forward) * Quaternion.Inverse(Quaternion.LookRotation(Vector3.Normalize(XYbeginMousePosition - target.transform.position), Vector3.forward));
        }
        if (rz)
        {
            if (YZplane.Raycast(ray, out enter))
                difff = ray.GetPoint(enter);

            offset *= Quaternion.LookRotation(Vector3.Normalize(difff - target.transform.position), Vector3.left) * Quaternion.Inverse(Quaternion.LookRotation(Vector3.Normalize(YZbeginMousePosition - target.transform.position), Vector3.left));
        }
        
        //target.transform.eulerAngles = beginTargetRotation + oofset;
        target.transform.rotation = offset * biginTargetRotation;
        gameObject.transform.rotation = Quaternion.identity * offset;
    }
}