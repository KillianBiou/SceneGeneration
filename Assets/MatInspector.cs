using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class MatInspector : MonoBehaviour
{

    [SerializeField]
    public Material mat;

    public Slider sliderX, sliderY;
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

    public void ChangeTillingX(float t)
    {
        Vector4 val = mat.GetVector("_scale");
        val.x = t;
        mat.SetVector("_scale", val);
    }

    public void ChangeTillingY(float t)
    {
        Vector4 val = mat.GetVector("_scale");
        val.y = t;
        mat.SetVector("_scale", val);
    }

    public void Refresh()
    {
        if (!mat.HasTexture("_Texture2D"))
            return;

        Texture tex = mat.GetTexture("_Texture2D");
        img.sprite = Sprite.Create((Texture2D)tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        Vector4 val = mat.GetVector("_scale");
        sliderX.value = val.x;
        sliderY.value = val.y;
    }

    private void OnDestroy()
    {

        matInfo.Changed.RemoveListener(Refresh);
    }
}