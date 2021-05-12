using UnityEngine;
using System.Collections;
using UnityEditor;
using HedgehogTeam.EasyTouch;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(EasyTouch))]
public class EasyTouchInspector : Editor {

	
	public override void OnInspectorGUI(){
			
		EasyTouch t = (EasyTouch)target;

		#region General properties
		EditorGUILayout.Space();
		t.enable = HTGuiTools.Toggle("Enable EasyTouch", t.enable,true);

		t.enableRemote = HTGuiTools.Toggle("Enable Unity Remote", t.enableRemote,true);


		EditorGUILayout.Space();

		#endregion

		#region Gui propertie
		t.showGuiInspector = HTGuiTools.BeginFoldOut( "GUI compatibilty",t.showGuiInspector);
		if (t.showGuiInspector){
			HTGuiTools.BeginGroup();{
				// UGUI
	
				EditorGUILayout.Space();
				t.allowUIDetection = HTGuiTools.Toggle("Enable Unity UI detection",t.allowUIDetection,true);
				if (t.allowUIDetection ){
					EditorGUI.indentLevel++;
					t.enableUIMode = HTGuiTools.Toggle("Unity UI compatibilty", t.enableUIMode,true);
					t.autoUpdatePickedUI = HTGuiTools.Toggle("Auto update picked Unity UI", t.autoUpdatePickedUI,true);
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Space();

				// NGUI
				t.enabledNGuiMode = HTGuiTools.Toggle("Enable NGUI compatibilty", t.enabledNGuiMode,true);

				if (t.enabledNGuiMode){

					//EditorGUI.indentLevel++;
					HTGuiTools.BeginGroup(5);
					{
					// layers
					serializedObject.Update();
					SerializedProperty layers = serializedObject.FindProperty("nGUILayers");
					EditorGUILayout.PropertyField( layers,false);
					serializedObject.ApplyModifiedProperties();
	
					// Camera

						if (HTGuiTools.Button("Add camera",Color.green,100, false)){
							t.nGUICameras.Add( null);
						}

						for (int i=0;i< t.nGUICameras.Count;i++){
							EditorGUILayout.BeginHorizontal();
							if (HTGuiTools.Button("X",Color.red,19)){	
								t.nGUICameras.RemoveAt(i);
								i--;
							}
							else{
								t.nGUICameras[i] = (Camera)EditorGUILayout.ObjectField("",t.nGUICameras[i],typeof(Camera),true);
								EditorGUILayout.EndHorizontal();
							}
						}	
					}HTGuiTools.EndGroup();
					//EditorGUI.indentLevel--;
				}
				
			}HTGuiTools.EndGroup();
		}
		

		#endregion

		#region Auto selection properties
		t.showSelectInspector = HTGuiTools.BeginFoldOut( "Automatic selection",t.showSelectInspector);
		if (t.showSelectInspector){
			HTGuiTools.BeginGroup();{
				t.autoSelect = HTGuiTools.Toggle("Enable auto-select",t.autoSelect,true);
				if (t.autoSelect){

					EditorGUILayout.Space();

					// 3d layer
					serializedObject.Update();
					SerializedProperty layers = serializedObject.FindProperty("pickableLayers3D");
					EditorGUILayout.PropertyField( layers,true);
					serializedObject.ApplyModifiedProperties();


					t.autoUpdatePickedObject = HTGuiTools.Toggle( "Auto update picked gameobject",t.autoUpdatePickedObject,true);
					EditorGUILayout.Space();

					//2D
					t.enable2D = HTGuiTools.Toggle("Enable 2D collider",t.enable2D,true);
					if (t.enable2D){
						serializedObject.Update();
						layers = serializedObject.FindProperty("pickableLayers2D");
						EditorGUILayout.PropertyField( layers,true);
						serializedObject.ApplyModifiedProperties();
					}


					// Camera
					GUILayout.Space(5f);
					HTGuiTools.BeginGroup(5);
					{
						if (HTGuiTools.Button( "Add Camera",Color.green,100)){
							t.touchCameras.Add(new ECamera(null,false));
						}
						for (int i=0;i< t.touchCameras.Count;i++){
							EditorGUILayout.BeginHorizontal();
							if (HTGuiTools.Button("X",Color.red,19)){	
								t.touchCameras.RemoveAt(i);
								i--;
							}
							if (i>=0){						
								t.touchCameras[i].camera = (Camera)EditorGUILayout.ObjectField("",t.touchCameras[i].camera,typeof(Camera),true,GUILayout.MinWidth(150));
								t.touchCameras[i].guiCamera = EditorGUILayout.ToggleLeft("Gui",t.touchCameras[i].guiCamera,GUILayout.Width(50));
								EditorGUILayout.EndHorizontal();
							}
						}
					}HTGuiTools.EndGroup();
				}
			}HTGuiTools.EndGroup();
		}

		#endregion

		#region General gesture properties
		t.showGestureInspector = HTGuiTools.BeginFoldOut("General gesture properties",t.showGestureInspector);
		if (t.showGestureInspector){
			HTGuiTools.BeginGroup();{
				t.gesturePriority =(EasyTouch.GesturePriority) EditorGUILayout.EnumPopup("Priority to",t.gesturePriority );
				t.StationaryTolerance = EditorGUILayout.FloatField("Stationary tolerance",t.StationaryTolerance);
				t.longTapTime = EditorGUILayout.FloatField("Long tap time",t.longTapTime);

				EditorGUILayout.Space ();

				t.doubleTapDetection = (EasyTouch.DoubleTapDetection) EditorGUILayout.EnumPopup("Double tap detection",t.doubleTapDetection);
				if (t.doubleTapDetection == EasyTouch.DoubleTapDetection.ByTime){
					t.doubleTapTime = EditorGUILayout.Slider("Double tap time",t.doubleTapTime,0.15f,0.4f);
				}

				EditorGUILayout.Space ();

				t.swipeTolerance = EditorGUILayout.FloatField("Swipe tolerance",t.swipeTolerance);
				t.alwaysSendSwipe = EditorGUILayout.Toggle("always sent swipe event",t.alwaysSendSwipe);

			}HTGuiTools.EndGroup();
		}

		#endregion

		#region 2 fingers gesture
		t.showTwoFingerInspector =  HTGuiTools.BeginFoldOut("Two fingers gesture properties",t.showTwoFingerInspector);
		if (t.showTwoFingerInspector){
			HTGuiTools.BeginGroup();{
				t.enable2FingersGesture = HTGuiTools.Toggle("2 fingers gesture",t.enable2FingersGesture,true);

				if (t.enable2FingersGesture){
					EditorGUI.indentLevel++;

					t.twoFingerPickMethod = (EasyTouch.TwoFingerPickMethod)EditorGUILayout.EnumPopup("Pick method",t.twoFingerPickMethod);

					EditorGUILayout.Separator();

					t.enable2FingersSwipe = HTGuiTools.Toggle("Enable swipe & drag",t.enable2FingersSwipe,true);

					EditorGUILayout.Separator();

					t.enablePinch = HTGuiTools.Toggle("Enable Pinch",t.enablePinch,true);
					if (t.enablePinch){
						t.minPinchLength = EditorGUILayout.FloatField("Min pinch length",t.minPinchLength);
					}

					EditorGUILayout.Separator();

					t.enableTwist = HTGuiTools.Toggle("Enable twist",t.enableTwist,true);
					if (t.enableTwist){
						t.minTwistAngle = EditorGUILayout.FloatField("Min twist angle",t.minTwistAngle);
					}
					
					EditorGUI.indentLevel--;
				}
			}HTGuiTools.EndGroup();
		}
	
		#endregion

		#region Second Finger simulation
		t.showSecondFingerInspector = HTGuiTools.BeginFoldOut("Second finger simulation", t.showSecondFingerInspector);
		if (t.showSecondFingerInspector){
			HTGuiTools.BeginGroup();{
				t.enableSimulation = HTGuiTools.Toggle("Enable simulation",t.enableSimulation,true );

				EditorGUILayout.Space();

				if (t.enableSimulation){
					EditorGUI.indentLevel++;

					if (t.secondFingerTexture==null){
						t.secondFingerTexture =Resources.Load("secondFinger") as Texture;
					}
					
					t.secondFingerTexture = (Texture)EditorGUILayout.ObjectField("Texture",t.secondFingerTexture,typeof(Texture),true);
					EditorGUILayout.HelpBox("Change the keys settings for a fash compilation, or if you want to change the keys",MessageType.Info);
					t.twistKey = (KeyCode)EditorGUILayout.EnumPopup( "Twist & pinch key", t.twistKey);	
					t.swipeKey = (KeyCode)EditorGUILayout.EnumPopup( "Swipe key", t.swipeKey);

					EditorGUI.indentLevel--;
				}
			}HTGuiTools.EndGroup();
		}

		#endregion

		if (GUI.changed){
			EditorUtility.SetDirty(target);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
	}
}

