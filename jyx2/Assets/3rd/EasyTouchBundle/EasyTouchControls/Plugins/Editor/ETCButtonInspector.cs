using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(ETCButton))]
public class ETCButtonInspector : Editor {

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
		
		ETCButton t = (ETCButton)target;
		
		
		EditorGUILayout.Space();
		

		t.gameObject.name = EditorGUILayout.TextField("Button name",t.gameObject.name);
		t.axis.name = t.gameObject.name;

		t.activated = ETCGuiTools.Toggle("Activated",t.activated,true);
		t.visible = ETCGuiTools.Toggle("Visible",t.visible,true);

		EditorGUILayout.Space();
		t.useFixedUpdate = ETCGuiTools.Toggle("Use Fixed Update",t.useFixedUpdate,true);
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
				
				// Area sprite ratio
				if (t.GetComponent<Image>().sprite != null){
					Rect rect =  t.GetComponent<Image>().sprite.rect;
					float ratio = rect.width / rect.height;
					
					// Area Size
					if (ratio>=1){
						float s = EditorGUILayout.FloatField("Size", t.rectTransform().rect.width);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
					}
					else{
						float s = EditorGUILayout.FloatField("Size", t.rectTransform().rect.height);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
					}
				}
			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Behaviour
		t.showBehaviourInspector = ETCGuiTools.BeginFoldOut( "Behaviour",t.showBehaviourInspector);
		if (t.showBehaviourInspector){
			ETCGuiTools.BeginGroup();{

				EditorGUILayout.Space();
				ETCGuiTools.BeginGroup(5);{
					t.enableKeySimulation = ETCGuiTools.Toggle("Enable Unity axes",t.enableKeySimulation,true);
					if (t.enableKeySimulation){
						t.allowSimulationStandalone = ETCGuiTools.Toggle("Allow Unity axes on standalone",t.allowSimulationStandalone,true);
						t.visibleOnStandalone = ETCGuiTools.Toggle("Force visible",t.visibleOnStandalone,true);
					}
				}ETCGuiTools.EndGroup();

				#region General propertie
				EditorGUI.indentLevel++;
				t.axis.showGeneralInspector = EditorGUILayout.Foldout(t.axis.showGeneralInspector,"General setting");
				if (t.axis.showGeneralInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;

						t.isSwipeIn = ETCGuiTools.Toggle("Swipe in",t.isSwipeIn,true);
						t.isSwipeOut = ETCGuiTools.Toggle("Swipe out",t.isSwipeOut,true);

						t.axis.isValueOverTime = ETCGuiTools.Toggle("Value over the time",t.axis.isValueOverTime,true);
						if (t.axis.isValueOverTime){

							ETCGuiTools.BeginGroup(5);{
								t.axis.overTimeStep = EditorGUILayout.FloatField("Step",t.axis.overTimeStep);
								t.axis.maxOverTimeValue = EditorGUILayout.FloatField("Max value",t.axis.maxOverTimeValue);
							}ETCGuiTools.EndGroup();

						}
						t.axis.speed = EditorGUILayout.FloatField("Value",t.axis.speed);

						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
				}
				EditorGUI.indentLevel--;
				#endregion

				#region Direct Action
				EditorGUI.indentLevel++;
				t.axis.showDirectInspector = EditorGUILayout.Foldout(t.axis.showDirectInspector,"Direction ation");
				if (t.axis.showDirectInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;

						t.axis.autoLinkTagPlayer = EditorGUILayout.ToggleLeft("Auto link on tag",t.axis.autoLinkTagPlayer, GUILayout.Width(200));
						if (t.axis.autoLinkTagPlayer){
							t.axis.autoTag = EditorGUILayout.TagField("",t.axis.autoTag);
						}
						else{
							t.axis.directTransform = (Transform)EditorGUILayout.ObjectField("Direct action to",t.axis.directTransform,typeof(Transform),true);
						}

						EditorGUILayout.Space();
	
						t.axis.actionOn = (ETCAxis.ActionOn)EditorGUILayout.EnumPopup("Action on",t.axis.actionOn);

						t.axis.directAction = (ETCAxis.DirectAction ) EditorGUILayout.EnumPopup( "Action",t.axis.directAction);

						if (t.axis.directAction != ETCAxis.DirectAction.Jump){
							t.axis.axisInfluenced = (ETCAxis.AxisInfluenced) EditorGUILayout.EnumPopup("Affected axis",t.axis.axisInfluenced);
						}
						else{
							EditorGUILayout.HelpBox("Required character controller", MessageType.Info);
							t.axis.gravity = EditorGUILayout.FloatField("Gravity",t.axis.gravity);
						}
						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
				}
				EditorGUI.indentLevel--;
				#endregion

				#region Unity axis
				EditorGUI.indentLevel++;
				t.axis.showSimulatinInspector = EditorGUILayout.Foldout(t.axis.showSimulatinInspector,"Unity axes");
				if (t.axis.showSimulatinInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;
						int index = System.Array.IndexOf(unityAxes,t.axis.unityAxis );
						int tmpIndex = EditorGUILayout.Popup(index,unityAxes);
						if (tmpIndex != index){
							t.axis.unityAxis = unityAxes[tmpIndex];
						}
						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
					
				}

				EditorGUI.indentLevel--;
				#endregion

			}ETCGuiTools.EndGroup();


		}
		#endregion

		#region Sprite
		t.showSpriteInspector = ETCGuiTools.BeginFoldOut( "Sprites",t.showSpriteInspector);
		if (t.showSpriteInspector){
			ETCGuiTools.BeginGroup();{

				// Normal state				
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck ();
				t.normalSprite = (Sprite)EditorGUILayout.ObjectField("Normal",t.normalSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.normalColor = EditorGUILayout.ColorField("",t.normalColor,GUILayout.Width(50));
				if (EditorGUI.EndChangeCheck ()) {
					t.GetComponent<Image>().sprite = t.normalSprite;
					t.GetComponent<Image>().color = t.normalColor;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if ( t.normalSprite){
					Rect spriteRect = new Rect( t.normalSprite.rect.x/ t.normalSprite.texture.width,
					                           t.normalSprite.rect.y/ t.normalSprite.texture.height,
					                           t.normalSprite.rect.width/ t.normalSprite.texture.width,
					                           t.normalSprite.rect.height/ t.normalSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.normalSprite.texture,Color.white);
				}		

				// Press state
				EditorGUILayout.BeginHorizontal();
				t.pressedSprite = (Sprite)EditorGUILayout.ObjectField("Pressed",t.pressedSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.pressedColor = EditorGUILayout.ColorField("",t.pressedColor,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (t.pressedSprite){
					Rect spriteRect = new Rect( t.pressedSprite.rect.x/ t.pressedSprite.texture.width,
					                      t.pressedSprite.rect.y/ t.pressedSprite.texture.height,
					                      t.pressedSprite.rect.width/ t.pressedSprite.texture.width,
					                      t.pressedSprite.rect.height/ t.pressedSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.pressedSprite.texture,Color.white);
				}

			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Events
		t.showEventInspector = ETCGuiTools.BeginFoldOut( "Events",t.showEventInspector);
		if (t.showEventInspector){
			ETCGuiTools.BeginGroup();{
				
				serializedObject.Update();
				SerializedProperty down = serializedObject.FindProperty("onDown");
				EditorGUILayout.PropertyField(down, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty press = serializedObject.FindProperty("onPressed");
				EditorGUILayout.PropertyField(press, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty pressTime = serializedObject.FindProperty("onPressedValue");
				EditorGUILayout.PropertyField(pressTime, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty up = serializedObject.FindProperty("onUp");
				EditorGUILayout.PropertyField(up, true, null);
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
