using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Name", menuName = "Objects/OD")]
public class ObjectDescription : ScriptableObject
{

    public string name;
    public string[] tags;
    public float mass;
    public float bounds;
    public Material[] materials;

}
