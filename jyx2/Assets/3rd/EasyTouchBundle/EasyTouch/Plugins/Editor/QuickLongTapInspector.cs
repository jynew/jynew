using UnityEngine;
using System.Collections;
using UnityEditor;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(QuickLongTap))]
public class QuickLongTapInspector : Editor {

	public override void OnInspectorGUI(){
		
		QuickLongTap t = (QuickLongTap)target;
		
		EditorGUILayout.Space();
		
		t.quickActionName = EditorGUILayout.TextField("Name",t.quickActionName);
		
		EditorGUILayout.Space();
		
		t.is2Finger = EditorGUILayout.Toggle("2 fingers gesture",t.is2Finger);
		t.actionTriggering = (QuickLongTap.ActionTriggering)EditorGUILayout.EnumPopup("Action triggering",t.actionTriggering);
		
		EditorGUILayout.Space();
		
		if (!t.is2Finger){
			t.isMultiTouch = EditorGUILayout.ToggleLeft("Allow multi-touch",t.isMultiTouch);
		}
		t.enablePickOverUI = EditorGUILayout.ToggleLeft("Allow over UI Element",t.enablePickOverUI);
		
		serializedObject.Update();
		SerializedProperty touch = serializedObject.FindProperty("onLongTap");
		EditorGUILayout.PropertyField(touch, true, null);
		serializedObject.ApplyModifiedProperties();
		
		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}
}
