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


[CustomEditor(typeof(ETCJoystick))]
public class ETCJoystickInspector:Editor  {

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
		
		ETCJoystick t = (ETCJoystick)target;

			
		EditorGUILayout.Space();

		t.gameObject.name = EditorGUILayout.TextField("Joystick name",t.gameObject.name);
		t.activated = ETCGuiTools.Toggle("Activated",t.activated,true);
		t.visible = ETCGuiTools.Toggle("Visible",t.visible,true);

		EditorGUILayout.Space();
		t.useFixedUpdate = ETCGuiTools.Toggle("Use Fixed Updae",t.useFixedUpdate,true);
		t.isUnregisterAtDisable = ETCGuiTools.Toggle("Unregister at disabling time",t.isUnregisterAtDisable,true);

		EditorGUILayout.Space();

		#region Type & Size
		t.showPSInspector = ETCGuiTools.BeginFoldOut( "Position & Size",t.showPSInspector);
		if (t.showPSInspector){
			ETCGuiTools.BeginGroup();{

				// Type
				t.joystickType = (ETCJoystick.JoystickType)EditorGUILayout.EnumPopup("Type",t.joystickType);

				if (t.joystickType == ETCJoystick.JoystickType.Static){
					t.anchor = (ETCBase.RectAnchor)EditorGUILayout.EnumPopup( "Anchor",t.anchor);
					if (t.anchor != ETCBase.RectAnchor.UserDefined){
						t.anchorOffet = EditorGUILayout.Vector2Field("Offset",t.anchorOffet);
					}

					t.IsNoOffsetThumb = ETCGuiTools.Toggle("No offset thumb",t.IsNoOffsetThumb,true);

				//	if (t.isNoOffsetThumb) t.isNoReturn = false;
					t.IsNoReturnThumb = ETCGuiTools.Toggle("No return of the thumb",t.IsNoReturnThumb,true);
				}
				else if( t.joystickType == ETCJoystick.JoystickType.Dynamic){
					t.anchor = ETCBase.RectAnchor.UserDefined;
					t.allowJoystickOverTouchPad = ETCGuiTools.Toggle("Allow over touchpad",t.allowJoystickOverTouchPad,true);
					t.joystickArea = (ETCJoystick.JoystickArea)EditorGUILayout.EnumPopup( "Joystick area",t.joystickArea);
					if (t.joystickArea == ETCJoystick.JoystickArea.UserDefined){
						t.userArea = (RectTransform)EditorGUILayout.ObjectField("User area",t.userArea,typeof(RectTransform),true);
					}
				}

				EditorGUILayout.Space();

				// Area sprite ratio
				Rect rect =  t.GetComponent<Image>().sprite.rect;
				float ratio = rect.width / rect.height;
				
				// Area Size
				if (ratio>=1){
					float s = EditorGUILayout.FloatField("Background size", t.rectTransform().rect.width);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
				}
				else{
					float s = EditorGUILayout.FloatField("Background size", t.rectTransform().rect.height);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
				}
				
				// Thumb sprite ratio
				rect = t.thumb.GetComponent<Image>().sprite.rect;
				ratio = rect.width / rect.height;
				
				// Thumb size
				if (ratio>=1){
					float s = EditorGUILayout.FloatField("Thumb size", t.thumb.rectTransform().rect.width);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
				}
				else{
					float s = EditorGUILayout.FloatField("Thumb size", t.thumb.rectTransform().rect.height);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
				}
				
				EditorGUILayout.Space();

				t.radiusBase = (ETCJoystick.RadiusBase)EditorGUILayout.EnumPopup("Radius based on",t.radiusBase );
				if (t.radiusBase == ETCJoystick.RadiusBase.UserDefined){
					t.radiusBaseValue = EditorGUILayout.FloatField("Radius",t.radiusBaseValue);
				}
				EditorGUILayout.Space();

	

			}ETCGuiTools.EndGroup();

		}
		#endregion

		#region Axes properties
		t.showAxesInspector = ETCGuiTools.BeginFoldOut( "Axes properties",t.showAxesInspector);
		if (t.showAxesInspector){
			ETCGuiTools.BeginGroup();{

				EditorGUILayout.Space();

				ETCGuiTools.BeginGroup(5);{
				t.enableKeySimulation = ETCGuiTools.Toggle("Enable Unity axes",t.enableKeySimulation,true);
				if (t.enableKeySimulation){
					t.allowSimulationStandalone = ETCGuiTools.Toggle("Allow Unity axes on standalone",t.allowSimulationStandalone,true);
					t.visibleOnStandalone = ETCGuiTools.Toggle("Force visible",t.visibleOnStandalone,true);
				}
				}ETCGuiTools.EndGroup();

				EditorGUILayout.Space();

				ETCGuiTools.BeginGroup(5);{
				//t.isTurnAndMove = EditorGUILayout.ToggleLeft("Enable Turn & Move direction Action",t.isTurnAndMove);
				t.isTurnAndMove	= ETCGuiTools.Toggle("Turn & Move direction Action",t.isTurnAndMove,true,220,true);
				if (t.isTurnAndMove){
					TurnAndMove(t.axisX, t.axisY,t );
				}
				}ETCGuiTools.EndGroup();

				//EditorGUILayout.Space();

				ETCGuiTools.BeginGroup(5);{
					ETCAxisInspector.AxisInspector( t.axisX,"Horizontal",ETCBase.ControlType.Joystick,t.isTurnAndMove,unityAxes);
				}ETCGuiTools.EndGroup();

				ETCGuiTools.BeginGroup(5);{
					ETCAxisInspector.AxisInspector( t.axisY,"Vertical" ,ETCBase.ControlType.Joystick,t.isTurnAndMove,unityAxes);
				}ETCGuiTools.EndGroup();



			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Camera
		t.showCameraInspector = ETCGuiTools.BeginFoldOut( "Camera",t.showCameraInspector);
		if (t.showCameraInspector){
			ETCGuiTools.BeginGroup();{
				EditorGUILayout.Space();
				t.enableCamera = ETCGuiTools.Toggle("Enable tracking",t.enableCamera,true);
				if (t.enableCamera){

					EditorGUILayout.Space();

					// Auto link
					ETCGuiTools.BeginGroup(5);{
						t.autoLinkTagCam = EditorGUILayout.ToggleLeft("Auto link on tag",t.autoLinkTagCam);
						if (t.autoLinkTagCam){
							t.autoCamTag = EditorGUILayout.TagField("",t.autoCamTag);
						}
						else{
							t.cameraTransform = (Transform)EditorGUILayout.ObjectField("Camera",t.cameraTransform,typeof(Transform),true);
						}
					}ETCGuiTools.EndGroup();

					EditorGUILayout.Space();

					ETCGuiTools.BeginGroup(5);{
						t.cameraTargetMode = (ETCJoystick.CameraTargetMode)EditorGUILayout.EnumPopup("Target mode",t.cameraTargetMode);
						if (t.cameraTargetMode == ETCBase.CameraTargetMode.UserDefined){
							t.cameraLookAt = (Transform)EditorGUILayout.ObjectField("Camera target",t.cameraLookAt,typeof(Transform),true);
						}
						if (t.cameraTargetMode == ETCBase.CameraTargetMode.LinkOnTag){
							t.camTargetTag = EditorGUILayout.TagField("",t.camTargetTag);
						}
					}ETCGuiTools.EndGroup();

						EditorGUILayout.Space();

					ETCGuiTools.BeginGroup(5);{
						t.cameraMode = (ETCJoystick.CameraMode)EditorGUILayout.EnumPopup("Camera mode",t.cameraMode);
						switch (t.cameraMode){
						case ETCJoystick.CameraMode.Follow:
							t.followOffset = EditorGUILayout.Vector3Field("Offset",t.followOffset);
							break;
						case ETCJoystick.CameraMode.SmoothFollow:
							t.enableWallDetection = EditorGUILayout.Toggle("Wall detection", t.enableWallDetection);
							if (t.enableWallDetection){
								SerializedObject so = new SerializedObject(t);
								SerializedProperty layer = so.FindProperty("wallLayer");
								EditorGUILayout.PropertyField( layer,true);
								so.ApplyModifiedProperties();
							}
							EditorGUILayout.Space();
							t.followDistance = EditorGUILayout.FloatField("Distance",t.followDistance);
							t.followHeight = EditorGUILayout.FloatField("Height",t.followHeight);
							t.followHeightDamping = EditorGUILayout.FloatField("Height damping",t.followHeightDamping);
							t.followRotationDamping = EditorGUILayout.FloatField("Rotation dampping",t.followRotationDamping);
							break;
						}
					}ETCGuiTools.EndGroup();
				}
			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region sprites
		t.showSpriteInspector = ETCGuiTools.BeginFoldOut( "Sprites",t.showSpriteInspector);
		if (t.showSpriteInspector){
			ETCGuiTools.BeginGroup();{
				#region Background
				Sprite areaSprite = t.GetComponent<Image>().sprite;
				
				EditorGUILayout.BeginHorizontal();
				t.GetComponent<Image>().sprite = (Sprite)EditorGUILayout.ObjectField("Background",t.GetComponent<Image>().sprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.GetComponent<Image>().color = EditorGUILayout.ColorField("",t.GetComponent<Image>().color,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				Rect spriteRect = new Rect( areaSprite.rect.x/ areaSprite.texture.width,
				                           areaSprite.rect.y/ areaSprite.texture.height,
				                           areaSprite.rect.width/ areaSprite.texture.width,
				                           areaSprite.rect.height/ areaSprite.texture.height);
				GUILayout.Space(8);
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.x = 20;
				lastRect.width = 100;
				lastRect.height = 100;
				
				GUILayout.Space(100);
				
				ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.GetComponent<Image>().sprite.texture,Color.white);
				#endregion
				EditorGUILayout.Space();
				#region thumb
				Sprite thumbSprite = t.thumb.GetComponent<Image>().sprite;
				
				EditorGUILayout.BeginHorizontal();
				t.thumb.GetComponent<Image>().sprite = (Sprite)EditorGUILayout.ObjectField("Thumb",t.thumb.GetComponent<Image>().sprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.thumb.GetComponent<Image>().color = EditorGUILayout.ColorField("",t.thumb.GetComponent<Image>().color,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				
				spriteRect = new Rect( thumbSprite.rect.x/ thumbSprite.texture.width,
				                      thumbSprite.rect.y/ thumbSprite.texture.height,
				                      thumbSprite.rect.width/ thumbSprite.texture.width,
				                      thumbSprite.rect.height/ thumbSprite.texture.height);
				
				GUILayout.Space(8);
				lastRect = GUILayoutUtility.GetLastRect();
				lastRect.x = 20;
				lastRect.width = 100;
				lastRect.height = 100;
				
				GUILayout.Space(100);
				
				ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.thumb.GetComponent<Image>().sprite.texture,Color.white);
				
				#endregion
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

		if (t.anchor != ETCBase.RectAnchor.UserDefined && t.joystickType == ETCJoystick.JoystickType.Static){
			t.SetAnchorPosition();
		}

		if (GUI.changed){
			EditorUtility.SetDirty(t);
			#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			#endif
		}


	}

	private void TurnAndMove(ETCAxis X, ETCAxis Y, ETCJoystick j){

		EditorGUI.indentLevel++;
		X.autoLinkTagPlayer = EditorGUILayout.ToggleLeft("Auto link on tag",X.autoLinkTagPlayer, GUILayout.Width(200));
		if (X.autoLinkTagPlayer){
			X.autoTag = EditorGUILayout.TagField("",X.autoTag);
		}

		if (!X.autoLinkTagPlayer){
			X.directTransform = (Transform)EditorGUILayout.ObjectField("Direct action to",X.directTransform,typeof(Transform),true);
		}
		EditorGUILayout.Space();

		if (j.tmMoveCurve==null){
			j.InitTurnMoveCurve();
		}
		j.tmMoveCurve = EditorGUILayout.CurveField("Move curve",j.tmMoveCurve);

		j.tmSpeed = EditorGUILayout.FloatField("Move speed",j.tmSpeed);
		j.tmAdditionnalRotation = EditorGUILayout.FloatField("Intial rotation",j.tmAdditionnalRotation);

		j.tmLockInJump = EditorGUILayout.Toggle("Lock in jump",j.tmLockInJump);

		X.gravity = EditorGUILayout.FloatField("Gravity",X.gravity);
		EditorGUI.indentLevel--;

	}
		
}

