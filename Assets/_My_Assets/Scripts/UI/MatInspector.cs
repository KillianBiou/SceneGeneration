using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatInspector : MonoBehaviour
{

    [SerializeField]
    public Material mat;

    public Slider sliderScale, sliderOffsetX, sliderOffsetY, sliderOffsetZ,
        sliderScalingX, sliderScalingY, sliderScalingZ,
        sliderRotateX, sliderRotateY, sliderRotateZ;
    public Image img;

    private MatInfo matInfo;
    private int ID;

    public void Init(Material m, MatInfo mInf, int id)
    {
        mat = m;
        matInfo = mInf;
        ID = id;

        matInfo.Changed.AddListener(Refresh);
        Refresh();
    }

    public void ChangeOffsetX(float t)
    {
        Vector4 val = mat.GetVector("_position_offset");
        val.x = t;
        mat.SetVector("_position_offset", val);
    }
    public void ChangeOffsetY(float t)
    {
        Vector4 val = mat.GetVector("_position_offset");
        val.y = t;
        mat.SetVector("_position_offset", val);
    }
    public void ChangeOffsetZ(float t)
    {
        Vector4 val = mat.GetVector("_position_offset");
        val.z = t;
        mat.SetVector("_position_offset", val);
    }



    public void ChangeScalingX(float t)
    {
        Vector4 val = mat.GetVector("_Scaling");
        val.x = t;
        mat.SetVector("_Scaling", val);
    }
    public void ChangeScalingY(float t)
    {
        Vector4 val = mat.GetVector("_Scaling");
        val.y = t;
        mat.SetVector("_Scaling", val);
    }
    public void ChangeScalingZ(float t)
    {
        Vector4 val = mat.GetVector("_Scaling");
        val.z = t;
        mat.SetVector("_Scaling", val);
    }

    public void ChangeTexScaling(float t)
    {
        Vector4 val = mat.GetVector("_Scaling");
        val.w = t;
        mat.SetVector("_Scaling", val);
    }



    public void ChangeRotationgX(float t)
    {
        Vector4 val = mat.GetVector("_Rotation");
        val.x = t;
        mat.SetVector("_Rotation", val);
    }
    public void ChangeRotationgY(float t)
    {
        Vector4 val = mat.GetVector("_Rotation");
        val.y = t;
        mat.SetVector("_Rotation", val);
    }
    public void ChangeRotationgZ(float t)
    {
        Vector4 val = mat.GetVector("_Rotation");
        val.z = t;
        mat.SetVector("_Rotation", val);
    }
    public void Refresh()
    {
        if (!mat.HasTexture("_BaseMap"))
            return;

        Texture tex = mat.GetTexture("_BaseMap");
        img.sprite = Sprite.Create((Texture2D)tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        Vector4 offsets = mat.GetVector("_position_offset");
        sliderOffsetX.value = offsets.x;
        sliderOffsetY.value = offsets.y;
        sliderOffsetZ.value = offsets.z;

        Vector4 scalings = mat.GetVector("_Scaling");
        sliderScalingX.value = scalings.x;
        sliderScalingY.value = scalings.y;
        sliderScalingZ.value = scalings.z;
        sliderScale.value = scalings.w;

        Vector4 rotations = mat.GetVector("_Rotation");
        sliderRotateX.value = rotations.x;
        sliderRotateY.value = rotations.y;
        sliderRotateZ.value = rotations.z;
    }

    private void OnDestroy()
    {
        matInfo.Changed.RemoveListener(Refresh);
    }
}