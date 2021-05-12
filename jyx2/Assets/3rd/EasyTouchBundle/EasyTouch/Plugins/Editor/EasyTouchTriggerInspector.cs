using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(EasyTouchTrigger))]
public class EasyTouchTriggerInspector : Editor {

	public override void OnInspectorGUI(){

		EasyTouchTrigger t = (EasyTouchTrigger)target;

		string[] eventNames = Enum.GetNames( typeof(EasyTouch.EvtType) ) ;
		eventNames[0] = "Add new event";

		#region Event properties
		GUILayout.Space(5f);
		for (int i=1;i<eventNames.Length;i++){

			EasyTouch.EvtType ev = (EasyTouch.EvtType)Enum.Parse( typeof(EasyTouch.EvtType), eventNames[i]);
			int result = t.receivers.FindIndex(
				delegate(EasyTouchTrigger.EasyTouchReceiver e){
				return  e.eventName == ev;
				}
			);

			if (result>-1){
				TriggerInspector( ev,t);
			}
		}
		#endregion

		#region Add Event
		GUI.backgroundColor = Color.green;
		int index = EditorGUILayout.Popup("", 0, eventNames,"Button");
		GUI.backgroundColor = Color.white;

		if (index!=0){
			//AddEvent((EasyTouch.EventName)Enum.Parse( typeof(EasyTouch.EventName), eventNames[index]),t );
			t.AddTrigger( (EasyTouch.EvtType)Enum.Parse( typeof(EasyTouch.EvtType), eventNames[index]));
			EditorPrefs.SetBool( eventNames[index], true); 
		}
		#endregion

		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}

	private void TriggerInspector(EasyTouch.EvtType ev, EasyTouchTrigger t){

		bool folding = EditorPrefs.GetBool( ev.ToString() );
		folding = HTGuiTools.BeginFoldOut( ev.ToString(),folding,false);
		EditorPrefs.SetBool(  ev.ToString(), folding); 

		if (folding){
			HTGuiTools.BeginGroup();

			int i=0;
			while (i<t.receivers.Count){
			
				if (t.receivers[i].eventName == ev){
					GUI.color = new Color(0.8f,0.8f,0.8f,1);
					HTGuiTools.BeginGroup(5);
					GUI.color = Color.white;
				

					EditorGUILayout.BeginHorizontal();
					t.receivers[i].enable = HTGuiTools.Toggle("Enable",t.receivers[i].enable,55,true);
					t.receivers[i].name = EditorGUILayout.TextField("",t.receivers[i].name, GUILayout.MinWidth(130));

					// Delete
					GUILayout.FlexibleSpace();
					if (HTGuiTools.Button("X",Color.red,19)){
						t.receivers[i] = null;
						t.receivers.RemoveAt( i );
						EditorGUILayout.EndHorizontal();
						return;
					}
					EditorGUILayout.EndHorizontal();
				

					EditorGUILayout.Space();

					// Restriced
					//t.receivers[i].restricted = HTGuiTools.Toggle("Restricted to gameobject",t.receivers[i].restricted,true);

					t.receivers[i].triggerType = (EasyTouchTrigger.ETTType)EditorGUILayout.EnumPopup("Testing on",t.receivers[i].triggerType );

					EditorGUILayout.BeginHorizontal();
					t.receivers[i].restricted = EditorGUILayout.Toggle("",t.receivers[i].restricted ,(GUIStyle)"Radio" ,GUILayout.Width(15));
					EditorGUILayout.LabelField("Only if on me (requiered a collider)");
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					t.receivers[i].restricted = !EditorGUILayout.Toggle("",!t.receivers[i].restricted ,(GUIStyle)"Radio",GUILayout.Width(15));				
					EditorGUILayout.LabelField("All the time, or other object");
					EditorGUILayout.EndHorizontal();

					if (!t.receivers[i].restricted){
						t.receivers[i].gameObject = (GameObject)EditorGUILayout.ObjectField("Other object",t.receivers[i].gameObject,typeof(GameObject),true);
					}
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					t.receivers[i].otherReceiver = HTGuiTools.Toggle("Other receiver",t.receivers[i].otherReceiver,true);
					if (t.receivers[i].otherReceiver){
						t.receivers[i].gameObjectReceiver = (GameObject)EditorGUILayout.ObjectField("Receiver",t.receivers[i].gameObjectReceiver,typeof(GameObject),true);
					}

					// Method Name
					EditorGUILayout.BeginHorizontal();
					t.receivers[i].methodName = EditorGUILayout.TextField("Method name",t.receivers[i].methodName);

					// Methode helper
					string[] mNames = null;
					if (!t.receivers[i].otherReceiver || (t.receivers[i].otherReceiver && t.receivers[i].gameObjectReceiver == null) ){
						mNames = GetMethod( t.gameObject);
					}
					else if ( t.receivers[i].otherReceiver && t.receivers[i].gameObjectReceiver != null){
						mNames = GetMethod( t.receivers[i].gameObjectReceiver);
					}

					int index = EditorGUILayout.Popup("", -1, mNames,"Button",GUILayout.Width(20));
					if (index>-1){
						t.receivers[i].methodName = mNames[index];
					}
					EditorGUILayout.EndHorizontal();


					// Parameter
					t.receivers[i].parameter = (EasyTouchTrigger.ETTParameter) EditorGUILayout.EnumPopup("Parameter to send",t.receivers[i].parameter);

					HTGuiTools.EndGroup();
				}
				i++;
			}
		}
		else{
			HTGuiTools.BeginGroup();
		}
		HTGuiTools.EndGroup(false);

		if (!GUILayout.Toggle(true,"+","ObjectPickerTab")){
			t.AddTrigger( ev);
		}

		GUILayout.Space(5f);

	}

	private void AddEvent(EasyTouch.EvtType ev, EasyTouchTrigger t){
		EasyTouchTrigger.EasyTouchReceiver r = new EasyTouchTrigger.EasyTouchReceiver();
		r.enable = true;
		r.restricted = true;
		r.eventName = ev;
		r.gameObject = t.gameObject;
		t.receivers.Add( r );

	}

	private string[] GetMethod(GameObject obj){

		List<string> methodName = new List<string>();

		Component[] allComponents = obj.GetComponents<Component>();

		if (allComponents.Length>0){
			foreach( Component comp in allComponents){
				if (comp!=null){
				if (comp.GetType().IsSubclassOf( typeof(MonoBehaviour))){
					MethodInfo[] methodInfos = comp.GetType().GetMethods();
					foreach( MethodInfo methodInfo in methodInfos){
						if ((methodInfo.DeclaringType.Namespace == null) || (!methodInfo.DeclaringType.Namespace.Contains("Unity") && !methodInfo.DeclaringType.Namespace.Contains("System"))){
							if (methodInfo.IsPublic){
								methodName.Add( methodInfo.Name );
							}
						}
						
					}
				}
				}
			}
		}
		//
		return methodName.ToArray();
	}

}

