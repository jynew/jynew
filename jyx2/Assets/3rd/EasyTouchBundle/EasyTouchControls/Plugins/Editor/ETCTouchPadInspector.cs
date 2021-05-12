/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(ETCTouchPad))]
public class ETCTouchPadInspector : Editor {

	public string[] unityAxes;
	
	void OnEnable(){
		var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
		SerializedObject obj = new SerializedObject(inputManager);
		SerializedProperty axisArray = obj.FindProperty("m_Axes");
		if (axisArray.arraySize > 0){
			unityAxes = new string[axisArray.arraySize];
			for( int i = 0; i < axisArray.arraySize; ++i ){
				var axis = axisArray.GetArrayElementAtIndex(i);
				unityAxes[i] = axis.FindPropertyRelative("m_Name").stringValue;
			}
		}
		
	}

	public override void OnInspectorGUI(){

		ETCTouchPad t = (ETCTouchPad)target;

		EditorGUILayout.Space();

		t.gameObject.name = EditorGUILayout.TextField("TouchPad name",t.gameObject.name);

		t.activated = ETCGuiTools.Toggle("Activated",t.activated,true);
		t.visible = ETCGuiTools.Toggle("Visible at runtime",t.visible,true);

		EditorGUILayout.Space();
		t.useFixedUpdate = ETCGuiTools.Toggle("Use Fixed Updae",t.useFixedUpdate,true);
		t.isUnregisterAtDisable = ETCGuiTools.Toggle("Unregister at disabling time",t.isUnregisterAtDisable,true);

		#region Position & Size
		t.showPSInspector = ETCGuiTools.BeginFoldOut( "Position & Size",t.showPSInspector);
		if (t.showPSInspector){
			ETCGuiTools.BeginGroup();{
				// Anchor
				t.anchor = (ETCBase.RectAnchor)EditorGUILayout.EnumPopup( "Anchor",t.anchor);
				if (t.anchor != ETCBase.RectAnchor.UserDefined){
					t.anchorOffet = EditorGUILayout.Vector2Field("Offset",t.anchorOffet);
				}
				
				EditorGUILayout.Space();
				
				// Size
				float width = EditorGUILayout.FloatField("Width", t.rectTransform().rect.width);
				t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);

				float height = EditorGUILayout.FloatField("Height", t.rectTransform().rect.height);
				t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,height);


			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Behaviour & axes
		t.showBehaviourInspector= ETCGuiTools.BeginFoldOut( "Axes",t.showBehaviourInspector);
		if (t.showBehaviourInspector){
			ETCGuiTools.BeginGroup();{
				EditorGUILayout.Space();
				t.enableKeySimulation = ETCGuiTools.Toggle("Enable key simulation",t.enableKeySimulation,true);
				if (t.enableKeySimulation){
					t.allowSimulationStandalone = ETCGuiTools.Toggle("Allow simulation on standalone",t.allowSimulationStandalone,true);
				}
				EditorGUILayout.Space();

				t.isDPI = ETCGuiTools.Toggle("DPI",t.isDPI,true);
				t.isSwipeIn = ETCGuiTools.Toggle("Swipe in",t.isSwipeIn,true);
				t.isSwipeOut = ETCGuiTools.Toggle("Swipe out",t.isSwipeOut,true);

				EditorGUILayout.Space();

				ETCGuiTools.BeginGroup(5);{
					ETCAxisInspector.AxisInspector(t.axisX,"Horizontal",ETCBase.ControlType.TouchPad,false, unityAxes); 
				}ETCGuiTools.EndGroup();
				
				ETCGuiTools.BeginGroup(5);{
					ETCAxisInspector.AxisInspector( t.axisY,"Vertical",ETCBase.ControlType.TouchPad,false, unityAxes);
				}ETCGuiTools.EndGroup();


			}ETCGuiTools.EndGroup();
		}

		#endregion

		#region Sprite
		t.showSpriteInspector = ETCGuiTools.BeginFoldOut( "Sprites",t.showSpriteInspector);
		if (t.showSpriteInspector){
			ETCGuiTools.BeginGroup();{

				Sprite frameSprite = t.GetComponent<Image>().sprite;
				
				EditorGUILayout.BeginHorizontal();
				t.GetComponent<Image>().sprite = (Sprite)EditorGUILayout.ObjectField("Frame",t.GetComponent<Image>().sprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.GetComponent<Image>().color = EditorGUILayout.ColorField("",t.GetComponent<Image>().color,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				Rect spriteRect = new Rect( frameSprite.rect.x/ frameSprite.texture.width,
				                           frameSprite.rect.y/ frameSprite.texture.height,
				                           frameSprite.rect.width/ frameSprite.texture.width,
				                           frameSprite.rect.height/ frameSprite.texture.height);
				GUILayout.Space(8);
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.x = 20;
				lastRect.width = 100;
				lastRect.height = 100;
				
				GUILayout.Space(100);
				
				ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.GetComponent<Image>().sprite.texture,Color.white);
			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Events
		t.showEventInspector = ETCGuiTools.BeginFoldOut( "Move Events",t.showEventInspector);
		if (t.showEventInspector){
			ETCGuiTools.BeginGroup();{
				
				serializedObject.Update();
				SerializedProperty moveStartEvent = serializedObject.FindProperty("onMoveStart");
				EditorGUILayout.PropertyField(moveStartEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty moveEvent = serializedObject.FindProperty("onMove");
				EditorGUILayout.PropertyField(moveEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty moveSpeedEvent = serializedObject.FindProperty("onMoveSpeed");
				EditorGUILayout.PropertyField(moveSpeedEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty moveEndEvent = serializedObject.FindProperty("onMoveEnd");
				EditorGUILayout.PropertyField(moveEndEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
			}ETCGuiTools.EndGroup();
		}
		
		t.showTouchEventInspector = ETCGuiTools.BeginFoldOut( "Touch Events",t.showTouchEventInspector);
		if (t.showTouchEventInspector){
			ETCGuiTools.BeginGroup();{
				
				serializedObject.Update();
				SerializedProperty touchStartEvent = serializedObject.FindProperty("onTouchStart");
				EditorGUILayout.PropertyField(touchStartEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty touchUpEvent = serializedObject.FindProperty("onTouchUp");
				EditorGUILayout.PropertyField(touchUpEvent, true, null);
				serializedObject.ApplyModifiedProperties();
			}ETCGuiTools.EndGroup();
		}
		
		t.showDownEventInspector = ETCGuiTools.BeginFoldOut( "Down Events",t.showDownEventInspector);
		if (t.showDownEventInspector){
			ETCGuiTools.BeginGroup();{
				
				serializedObject.Update();
				SerializedProperty downUpEvent = serializedObject.FindProperty("OnDownUp");
				EditorGUILayout.PropertyField(downUpEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty downRightEvent = serializedObject.FindProperty("OnDownRight");
				EditorGUILayout.PropertyField(downRightEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty downDownEvent = serializedObject.FindProperty("OnDownDown");
				EditorGUILayout.PropertyField(downDownEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty downLeftEvent = serializedObject.FindProperty("OnDownLeft");
				EditorGUILayout.PropertyField(downLeftEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
			}ETCGuiTools.EndGroup();
		}
		
		t.showPressEventInspector = ETCGuiTools.BeginFoldOut( "Press Events",t.showPressEventInspector);
		if (t.showPressEventInspector){
			ETCGuiTools.BeginGroup();{
				
				serializedObject.Update();
				SerializedProperty pressUpEvent = serializedObject.FindProperty("OnPressUp");
				EditorGUILayout.PropertyField(pressUpEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty pressRightEvent = serializedObject.FindProperty("OnPressRight");
				EditorGUILayout.PropertyField(pressRightEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty pressDownEvent = serializedObject.FindProperty("OnPressDown");
				EditorGUILayout.PropertyField(pressDownEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty pressLeftEvent = serializedObject.FindProperty("OnPressLeft");
				EditorGUILayout.PropertyField(pressLeftEvent, true, null);
				serializedObject.ApplyModifiedProperties();
				
			}ETCGuiTools.EndGroup();
		}
		
		#endregion

		if (t.anchor != ETCBase.RectAnchor.UserDefined){
			t.SetAnchorPosition();
		}

		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}
		

	}
	
}
