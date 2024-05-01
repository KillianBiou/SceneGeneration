using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(GenerationDatabase))]
public class GenerationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Save Database"))
        {
            GenerationDatabase client = (GenerationDatabase)target;
            client.SaveDatabase();
        }
        if (GUILayout.Button("Load Database"))
        {
            GenerationDatabase client = (GenerationDatabase)target;
            client.LoadDatabase();
        }
    }
}
#endif