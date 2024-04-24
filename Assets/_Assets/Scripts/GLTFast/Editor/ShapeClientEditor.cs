using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ShapeClient))]
public class ShapeClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Request Generation"))
        {
            ShapeClient client = (ShapeClient)target;
            client.RequestDebugGeneration();
        }
    }
}
