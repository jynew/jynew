using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshCombineStudio
{
    [CustomEditor(typeof(CombinedLODManager))]
    public class CombinedLODManagerEditor : Editor
    {
        SerializedProperty drawGizmos, distances, lodMode, lodDistanceMode, showLod, lodCulled, lodCullDistance;
        SerializedProperty[] distanceElements;
        
        float editorSkinMulti;

        private void OnEnable()
        {
            editorSkinMulti = EditorGUIUtility.isProSkin ? 1 : 0.35f;

            drawGizmos = serializedObject.FindProperty("drawGizmos");
            lodMode = serializedObject.FindProperty("lodMode");
            lodDistanceMode = serializedObject.FindProperty("lodDistanceMode");
            showLod = serializedObject.FindProperty("showLod");
            distances = serializedObject.FindProperty("distances");
            lodCulled = serializedObject.FindProperty("lodCulled");
            lodCullDistance = serializedObject.FindProperty("lodCullDistance");
        }

        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();
            CombinedLODManager combinedLODManager = (CombinedLODManager)target;
            serializedObject.Update();

            GUI.color = Color.yellow * editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUIDraw.LabelWidthUnderline(new GUIContent("LOD Settings", ""), 14);

            EditorGUILayout.PropertyField(drawGizmos);
            EditorGUILayout.PropertyField(lodMode);
            GUI.changed = false;
            EditorGUILayout.PropertyField(lodDistanceMode);
            if (GUI.changed)
            {
                MeshCombiner meshCombiner = combinedLODManager.GetComponent<MeshCombiner>();
                if (meshCombiner != null)
                {
                    serializedObject.ApplyModifiedProperties();
                    combinedLODManager.UpdateDistances(meshCombiner);
                    return;
                }
            }

            EditorGUILayout.PropertyField(lodCulled);
            if (lodCulled.boolValue)
            {
                EditorGUILayout.PropertyField(lodCullDistance);
                if (lodCullDistance.floatValue < 0) lodCullDistance.floatValue = 0;
            }

            if (lodMode.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(showLod);
                if (showLod.intValue < 0) showLod.intValue = 0;
                if (showLod.intValue >= distances.arraySize) showLod.intValue = distances.arraySize - 1;
            }
            else
            {
                GUI.changed = false;
                GUIDraw.PropertyArray(distances, new GUIContent(""), new GUIContent(""), false, false);
                if (GUI.changed) lodDistanceMode.enumValueIndex = 1;

                if (distanceElements == null || distanceElements.Length != distances.arraySize) distanceElements = new SerializedProperty[distances.arraySize];

                for (int i = 0; i < distances.arraySize; i++)
                {
                    distanceElements[i] = distances.GetArrayElementAtIndex(i);
                    if (i == 0) distanceElements[i].floatValue = 0;
                    else if (distanceElements[i].floatValue < distanceElements[i - 1].floatValue) distanceElements[i].floatValue = distanceElements[i - 1].floatValue + 0.1f;
                }
            }
            

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
