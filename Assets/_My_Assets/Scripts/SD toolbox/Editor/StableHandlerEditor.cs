using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StableHandler))]
public class StableHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StableHandler myComponent = (StableHandler)target;

        // Draw the drop-down list for the Samplers list
        myComponent.selectedSampler = EditorGUILayout.Popup("Sampler", myComponent.selectedSampler, myComponent.samplersList);

        // Draw the drop-down list for the Models list
        myComponent.selectedModel = EditorGUILayout.Popup("Model", myComponent.selectedModel, myComponent.modelsList);

        // Apply the changes to the serialized object
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate"))
            myComponent.Generate();
    }
}