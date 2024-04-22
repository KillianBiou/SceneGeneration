using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    public Texture2D tex, norm;

    public MatInfo matInfo;

    public void ApplyTexture()
    {
        if (matInfo == null || matInfo.selectedId == -1)
            return;

        matInfo.materials[matInfo.selectedId].SetTexture("_Texture2D", tex);
        if (norm != null)
            matInfo.materials[matInfo.selectedId].SetTexture("_NormalMap", norm);

        matInfo.Changed.Invoke();
    }
}
