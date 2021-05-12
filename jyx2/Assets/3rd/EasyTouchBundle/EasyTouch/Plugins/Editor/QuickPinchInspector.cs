using UnityEngine;
using System.Collections;
using UnityEditor;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(QuickPinch))]
public class QuickPinchInspector : Editor {

	public override void OnInspectorGUI(){
		
		QuickPinch t = (QuickPinch)target;
		
		EditorGUILayout.Space();
		
		t.quickActionName = EditorGUILayout.TextField("Quick name",t.quickActionName);
		
		EditorGUILayout.Space();
		
		t.isGestureOnMe = EditorGUILayout.ToggleLeft("Gesture over me", t.isGestureOnMe);
		t.enablePickOverUI = EditorGUILayout.ToggleLeft("Allow over UI Element",t.enablePickOverUI);
		
		EditorGUILayout.Space();
		
		t.actionTriggering = (QuickPinch.ActionTiggering)EditorGUILayout.EnumPopup("Triggering",t.actionTriggering);
		t.pinchDirection = (QuickPinch.ActionPinchDirection)EditorGUILayout.EnumPopup("Pinch direction",t.pinchDirection);
		//t.rotationDirection = (QuickTwist.ActionRotationDirection)EditorGUILayout.EnumPopup("Twist direction",t.rotationDirection);
		
		EditorGUILayout.Space();
		if (t.actionTriggering == QuickPinch.ActionTiggering.InProgress){
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
		SerializedProperty pinchAction = serializedObject.FindProperty("onPinchAction");
		EditorGUILayout.PropertyField(pinchAction, true, null);
		serializedObject.ApplyModifiedProperties();

		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}
}
