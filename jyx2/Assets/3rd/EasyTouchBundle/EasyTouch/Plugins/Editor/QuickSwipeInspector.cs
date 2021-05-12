using UnityEngine;
using System.Collections;
using UnityEditor;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(QuickSwipe))]
public class QuickSwipeInspector : Editor {

	public override void OnInspectorGUI(){
		
		QuickSwipe t = (QuickSwipe)target;

		EditorGUILayout.Space();

		t.quickActionName = EditorGUILayout.TextField("Quick name",t.quickActionName);

		EditorGUILayout.Space();

		t.allowSwipeStartOverMe = EditorGUILayout.ToggleLeft("Allow swipe start over me",t.allowSwipeStartOverMe);
		t.enablePickOverUI = EditorGUILayout.ToggleLeft("Allow over UI Element",t.enablePickOverUI);

		EditorGUILayout.Space();

		t.actionTriggering = (QuickSwipe.ActionTriggering)EditorGUILayout.EnumPopup("Triggering",t.actionTriggering);
		t.swipeDirection = (QuickSwipe.SwipeDirection)EditorGUILayout.EnumPopup("Swipe direction",t.swipeDirection);
		
		EditorGUILayout.Space();
		if (t.actionTriggering == QuickSwipe.ActionTriggering.InProgress){
			t.enableSimpleAction = EditorGUILayout.Toggle("Enable simple action",t.enableSimpleAction);
			if (t.enableSimpleAction){
				EditorGUI.indentLevel++;
				t.directAction = (QuickSwipe.DirectAction) EditorGUILayout.EnumPopup("Action",t.directAction);
				t.axesAction = (QuickSwipe.AffectedAxesAction)EditorGUILayout.EnumPopup("Affected axes",t.axesAction);
				t.sensibility = EditorGUILayout.FloatField("Sensibility",t.sensibility);
				t.inverseAxisValue = EditorGUILayout.Toggle("Inverse axis",t.inverseAxisValue);
				EditorGUI.indentLevel--;
			}
		}

		EditorGUILayout.Space();

		serializedObject.Update();
		SerializedProperty swipeAction = serializedObject.FindProperty("onSwipeAction");
		EditorGUILayout.PropertyField(swipeAction, true, null);
		serializedObject.ApplyModifiedProperties();

		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}
}
