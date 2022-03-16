using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using ES3Internal;

[CustomEditor(typeof(ES3Prefab))]
[System.Serializable]
public class ES3PrefabEditor : Editor
{
	bool showAdvanced = false;
    bool openLocalRefs = false;

	public override void OnInspectorGUI()
	{
		var es3Prefab = (ES3Prefab)serializedObject.targetObject;
		EditorGUILayout.HelpBox("Easy Save is enabled for this prefab, and can be saved and loaded with the ES3 methods.", MessageType.None);


		showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Settings");
		if(showAdvanced)
		{
			EditorGUI.indentLevel++;
			es3Prefab.prefabId =  EditorGUILayout.LongField("Prefab ID", es3Prefab.prefabId);
			EditorGUILayout.LabelField("Reference count", es3Prefab.localRefs.Count.ToString());
			EditorGUI.indentLevel--;

            openLocalRefs = EditorGUILayout.Foldout(openLocalRefs, "localRefs");
            if (openLocalRefs)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("It is not recommended to manually modify these.");

                foreach (var kvp in es3Prefab.localRefs)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(kvp.Key, typeof(UnityEngine.Object), false);
                    EditorGUILayout.LongField(kvp.Value);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}