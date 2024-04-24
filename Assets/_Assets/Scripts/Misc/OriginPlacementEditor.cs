using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OriginPlacement))]
public class OriginPlacementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Origin Replacement"))
        {
            OriginPlacement client = (OriginPlacement)target;
            client.ReplaceOrigin();
        }
    }
}
