using UnityEngine;
using System.Collections;
using UnityEditor;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(QuickEnterOverExist))]
public class QuickEnterExitInspector : Editor {

	public override void OnInspectorGUI(){
		
		QuickEnterOverExist t = (QuickEnterOverExist)target;

		EditorGUILayout.Space();

		t.quickActionName = EditorGUILayout.TextField("Quick name",t.quickActionName);

		EditorGUILayout.Space();

		t.isMultiTouch = EditorGUILayout.ToggleLeft("Allow multi-touches",t.isMultiTouch);
		t.enablePickOverUI = EditorGUILayout.ToggleLeft("Allow over UI element",t.enablePickOverUI);

		EditorGUILayout.Space();
		
		serializedObject.Update();
		SerializedProperty enter = serializedObject.FindProperty("onTouchEnter");
		EditorGUILayout.PropertyField(enter, true, null);
		serializedObject.ApplyModifiedProperties();
		
		serializedObject.Update();
		SerializedProperty over = serializedObject.FindProperty("onTouchOver");
		EditorGUILayout.PropertyField(over, true, null);
		serializedObject.ApplyModifiedProperties();
		
		serializedObject.Update();
		SerializedProperty exit = serializedObject.FindProperty("onTouchExit");
		EditorGUILayout.PropertyField(exit, true, null);
		serializedObject.ApplyModifiedProperties();
		
		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}
}
