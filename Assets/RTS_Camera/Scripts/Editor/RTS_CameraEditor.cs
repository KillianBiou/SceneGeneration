﻿using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace RTS_Cam
{
    [CustomEditor(typeof(RTS_Camera))]
    public class RTS_CameraEditor : Editor
    {
        private RTS_Camera camera { get { return target as RTS_Camera; } }

        private TabsBlock tabs;


        private void OnEnable()
        {
            tabs = new TabsBlock(new Dictionary<string, System.Action>() 
            {
                {"Interaction", InteractionTab},
                {"Movement", MovementTab},
                {"Rotation", RotationTab},
                {"Height", HeightTab}
            });
            tabs.SetCurrentMethod(camera.lastTab);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            Undo.RecordObject(camera, "RTS_CAmera");
            tabs.Draw();
            if (GUI.changed)
                camera.lastTab = tabs.curMethodIndex;
            EditorUtility.SetDirty(camera);
            serializedObject.ApplyModifiedProperties();
        }

        private void InteractionTab()
        {
            SerializedProperty OnMoveState = serializedObject.FindProperty("OnMoveState");
            SerializedProperty OnScaleState = serializedObject.FindProperty("OnScaleState");
            SerializedProperty OnRotateState = serializedObject.FindProperty("OnRotateState");
            SerializedProperty OnGenState = serializedObject.FindProperty("OnGenState");
            SerializedProperty PopUp = serializedObject.FindProperty("popUp");

            GUILayout.Label("Short Click Trigger", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(OnMoveState);
            EditorGUILayout.PropertyField(OnScaleState);
            EditorGUILayout.PropertyField(OnRotateState);
            EditorGUILayout.PropertyField(OnGenState);

            GUILayout.Label("Interaction References", EditorStyles.boldLabel);

            camera.longClickThreshold = EditorGUILayout.FloatField("Long click threshold: ", camera.longClickThreshold);
            EditorGUILayout.PropertyField(PopUp);
        }

        private void MovementTab()
        {
            using (new HorizontalBlock())
            {
                GUILayout.Label("Use keyboard input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.useKeyboardInput = EditorGUILayout.Toggle( camera.useKeyboardInput);
            }
            if(camera.useKeyboardInput)
            {
                camera.horizontalAxis = EditorGUILayout.TextField("Horizontal axis name: ", camera.horizontalAxis);
                camera.verticalAxis = EditorGUILayout.TextField("Vertical axis name: ", camera.verticalAxis);

                using (new HorizontalBlock())
                {
                    camera.minKeyboardMovementSpeed = EditorGUILayout.FloatField("Min height movement speed: ", camera.minKeyboardMovementSpeed);
                    camera.maxKeyboardMovementSpeed = EditorGUILayout.FloatField("Max height movement speed: ", camera.maxKeyboardMovementSpeed);
                }

            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Screen edge input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.useScreenEdgeInput = EditorGUILayout.Toggle( camera.useScreenEdgeInput);
            }

            if(camera.useScreenEdgeInput)
            {
                EditorGUILayout.FloatField("Screen edge border size: ", camera.screenEdgeBorder);
                camera.screenEdgeMovementSpeed = EditorGUILayout.FloatField("Screen edge movement speed: ", camera.screenEdgeMovementSpeed);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Panning with mouse: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.usePanning = EditorGUILayout.Toggle(camera.usePanning);
            }
            if(camera.usePanning)
            {
                camera.panningKey = (KeyCode)EditorGUILayout.EnumPopup("Panning when holding: ", camera.panningKey);
                camera.panningSpeed = EditorGUILayout.FloatField("Panning speed: ", camera.panningSpeed);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Limit movement: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.limitMap = EditorGUILayout.Toggle(camera.limitMap);
            }
            if (camera.limitMap)
            {
                camera.limitX = EditorGUILayout.FloatField("Limit X: ", camera.limitX);
                camera.limitY = EditorGUILayout.FloatField("Limit Y: ", camera.limitY);
            }

            GUILayout.Label("Follow target", EditorStyles.boldLabel);
            camera.targetFollow = EditorGUILayout.ObjectField("Target to follow: ", camera.targetFollow, typeof(Transform)) as Transform;
            camera.targetOffset = EditorGUILayout.Vector3Field("Target offset: ", camera.targetOffset);
            camera.followingSpeed = EditorGUILayout.FloatField("Following speed: ", camera.followingSpeed);
        }

        private void RotationTab()
        {
            using (new HorizontalBlock())
            {
                GUILayout.Label("Keyboard input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.useKeyboardRotation = EditorGUILayout.Toggle(camera.useKeyboardRotation);
            }
            if(camera.useKeyboardRotation)
            {
                camera.rotateLeftKey = (KeyCode)EditorGUILayout.EnumPopup("Rotate left: ", camera.rotateLeftKey);
                camera.rotateRightKey = (KeyCode)EditorGUILayout.EnumPopup("Rotate right: ", camera.rotateRightKey);
                camera.rotationSped = EditorGUILayout.FloatField("Keyboard rotation speed", camera.rotationSped);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Mouse input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.useMouseRotation = EditorGUILayout.Toggle(camera.useMouseRotation);
            }
            if(camera.useMouseRotation)
            {
                camera.mouseRotationKey = (KeyCode)EditorGUILayout.EnumPopup("Mouse rotation key: ", camera.mouseRotationKey);
                camera.mouseRotationSpeed = EditorGUILayout.FloatField("Mouse rotation speed: ", camera.mouseRotationSpeed);
            }
        }

        private void HeightTab()
        {
            using (new HorizontalBlock())
            {
                GUILayout.Label("Auto height: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.autoHeight = EditorGUILayout.Toggle(camera.autoHeight);
            }
            if (camera.autoHeight)
            {
                camera.heightDampening = EditorGUILayout.FloatField("Height dampening: ", camera.heightDampening);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("groundMask"));
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Keyboard zooming: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.useKeyboardZooming = EditorGUILayout.Toggle(camera.useKeyboardZooming);
            }
            if(camera.useKeyboardZooming)
            {
                camera.zoomInKey = (KeyCode)EditorGUILayout.EnumPopup("Zoom In: ", camera.zoomInKey);
                camera.zoomOutKey = (KeyCode)EditorGUILayout.EnumPopup("Zoom Out: ", camera.zoomOutKey);
                camera.keyboardZoomingSensitivity = EditorGUILayout.FloatField("Keyboard sensitivity: ", camera.keyboardZoomingSensitivity);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Scrollwheel zooming: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.useScrollwheelZooming = EditorGUILayout.Toggle(camera.useScrollwheelZooming);
            }
            if (camera.useScrollwheelZooming)
                camera.scrollWheelZoomingSensitivity = EditorGUILayout.FloatField("Scrollwheel sensitivity: ", camera.scrollWheelZoomingSensitivity);

            if (camera.useScrollwheelZooming || camera.useKeyboardZooming)
            {
                using (new HorizontalBlock())
                {
                    camera.maxHeight = EditorGUILayout.FloatField("Max height: ", camera.maxHeight);
                    camera.minHeight = EditorGUILayout.FloatField("Min height: ", camera.minHeight);
                }
                using (new HorizontalBlock())
                {
                    camera.maxHeightCameraAngle = EditorGUILayout.FloatField("Max height camera angle: ", camera.maxHeightCameraAngle);
                    camera.minHeightCameraAngle = EditorGUILayout.FloatField("Min height camera angle: ", camera.minHeightCameraAngle);
                }
            }  
        }
    }
}