using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "MeshInfo", menuName = "Infos/meshinfo")]
public class MeshInfo : ScriptableObject
{

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    public Material material;

    [SerializeField]
    public Texture2D texture;

    public string[] tags;


    public UnityEvent Changed;

    public Vector4 scale;
    public Vector3 objectOffset, offset;

}
