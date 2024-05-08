using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[Serializable]
public struct MaterialInformations
{
    [SerializeField]
    public int selectedId;
    [SerializeField]
    public Material[] materials;
    [SerializeField]
    public Texture2D texture;
}




[CreateAssetMenu(fileName = "MatInfo", menuName = "Infos/matinfo")]
public class MatInfo : ScriptableObject
{

    [SerializeField]
    public MaterialInformations[] matsInfos;

    [SerializeField]
    public int selectedId =-1;
    [SerializeField]
    public Material[] materials;
    [SerializeField]
    public Texture2D texture;

    public UnityEvent Changed;

    public Vector4 scale;
    public Vector3 objectOffset, offset;


    public Vector3 GetOffset()
    {
        return materials[selectedId].GetVector("_position_offset");
    }

    public Vector3 GetScale()
    {
        return materials[selectedId].GetVector("_scale");
    }

}
