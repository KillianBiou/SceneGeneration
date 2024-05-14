using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaterialSetter : MonoBehaviour
{
    public RawImage texture, normal;

    public Texture2D tex, norm;

    public MatInfo matInfo;

    public void SetupButton(MatInfo m, Texture2D t, Texture2D n)
    {
        matInfo = m;
        tex = t;
        norm = n;

        texture.texture = tex;
        if(norm != null)
        {
            normal.texture = norm;
            normal.gameObject.SetActive(true);
        }
        else
            normal.gameObject.SetActive(false);
    }


    public void ApplyTexture()
    {
        if (matInfo == null || matInfo.selectedId == -1)
            return;

        matInfo.materials[matInfo.selectedId].SetTexture("_Texture2D", tex);
        if (norm != null)
        {
            matInfo.materials[matInfo.selectedId].SetTexture("_Normal_Map", norm);
            matInfo.materials[matInfo.selectedId].SetFloat("_use_normal", 1);
        }
        else
            matInfo.materials[matInfo.selectedId].SetFloat("_use_normal", 0);

        matInfo.Changed.Invoke();
    }
}
