using UnityEngine;
using System.Collections;
using UnityEditor;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(QuickDrag))]
public class QuickDragInspector : Editor {

	public override void OnInspectorGUI(){

		QuickDrag t = (QuickDrag)target;

		EditorGUILayout.Space();

		t.quickActionName = EditorGUILayout.TextField("Quick name",t.quickActionName);

		EditorGUILayout.Space();

		t.axesAction = (QuickBase.AffectedAxesAction) EditorGUILayout.EnumPopup("Allow on the axes",t.axesAction);

		EditorGUILayout.Space();

		t.enablePickOverUI = EditorGUILayout.ToggleLeft("Allow pick over UI element",t.enablePickOverUI);
		t.isStopOncollisionEnter = EditorGUILayout.ToggleLeft("Stop drag on collision enter",t.isStopOncollisionEnter);
		t.resetPhysic = EditorGUILayout.ToggleLeft("Reset physic on drag",t.resetPhysic);

		EditorGUILayout.Space();

		serializedObject.Update();
		SerializedProperty start = serializedObject.FindProperty("onDragStart");
		EditorGUILayout.PropertyField(start, true, null);
		serializedObject.ApplyModifiedProperties();
		
		serializedObject.Update();
		SerializedProperty drag = serializedObject.FindProperty("onDrag");
		EditorGUILayout.PropertyField(drag, true, null);
		serializedObject.ApplyModifiedProperties();
		
		serializedObject.Update();
		SerializedProperty end = serializedObject.FindProperty("onDragEnd");
		EditorGUILayout.PropertyField(end, true, null);
		serializedObject.ApplyModifiedProperties();
		
		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}

}
