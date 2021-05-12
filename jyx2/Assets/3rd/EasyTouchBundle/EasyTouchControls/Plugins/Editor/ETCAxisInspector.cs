using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ETCAxisInspector{

	public static void AxisInspector(ETCAxis axis, string label, ETCBase.ControlType type, bool turnMove = false, string[] unityAxes=null){

		EditorGUILayout.BeginHorizontal();
		//GUI.color = color;
		axis.enable = ETCGuiTools.Toggle(label +  " axis : ",axis.enable,true,125,true);
		//GUI.color = Color.white;
		axis.name =  EditorGUILayout.TextField("",axis.name,GUILayout.MinWidth(50));
		EditorGUILayout.EndHorizontal();

		if (axis.enable){

			EditorGUI.indentLevel++;
		
			#region General
			//EditorGUILayout.LabelField( "General",EditorStyles.boldLabel);
			axis.showGeneralInspector = EditorGUILayout.Foldout(axis.showGeneralInspector,"General setting");
			if (axis.showGeneralInspector){
				ETCGuiTools.BeginGroup(20);{
					EditorGUI.indentLevel--;
					axis.actionOn = (ETCAxis.ActionOn)EditorGUILayout.EnumPopup("React on",axis.actionOn );

					EditorGUILayout.Space();
					if (type == ETCBase.ControlType.Joystick ){
						axis.valueMethod = (ETCAxis.AxisValueMethod)EditorGUILayout.EnumPopup("Dead zone method",axis.valueMethod);
						switch (axis.valueMethod){
						case ETCAxis.AxisValueMethod.Classical:
							axis.deadValue = EditorGUILayout.Slider("Dead length",axis.deadValue,0f,1f);
							break;
						case ETCAxis.AxisValueMethod.Curve:
							if (axis.curveValue == null){
								axis.InitDeadCurve();
							}
							axis.curveValue = EditorGUILayout.CurveField("Sensitivity curve", axis.curveValue);
							break;
						}

					}
					EditorGUILayout.Space();

					axis.invertedAxis = ETCGuiTools.Toggle("Inverted axis",axis.invertedAxis,true);

					EditorGUILayout.Space();

					#region Button & DPAD Value over the time
					if (type == ETCBase.ControlType.Button || type == ETCBase.ControlType.DPad){
						axis.isValueOverTime = ETCGuiTools.Toggle("Value over the time",axis.isValueOverTime,true);
						if (axis.isValueOverTime){
							//EditorGUI.indentLevel--;
							ETCGuiTools.BeginGroup(5);{
								axis.overTimeStep = EditorGUILayout.FloatField("Step",axis.overTimeStep);
								axis.maxOverTimeValue = EditorGUILayout.FloatField("Max value",axis.maxOverTimeValue);
							}ETCGuiTools.EndGroup();
							//EditorGUI.indentLevel++;
						}
					}
					#endregion

					#region Joysick 
					if (type == ETCBase.ControlType.Joystick ){
						axis.axisThreshold = EditorGUILayout.Slider("On/Off Threshold",axis.axisThreshold,0f,1f);
					}
					#endregion
				
					if (!turnMove){
						string labelspeed = "Speed";
						if (type== ETCBase.ControlType.TouchPad){
							labelspeed ="Sensitivity";
						}
						axis.speed = EditorGUILayout.FloatField(labelspeed,axis.speed);
					}
					EditorGUI.indentLevel++;
				}ETCGuiTools.EndGroup();
			}
			#endregion

			if (!turnMove){
				#region Direction Action
				axis.showDirectInspector = EditorGUILayout.Foldout(axis.showDirectInspector,"Direction ation (optional)");
				if (axis.showDirectInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;

						//EditorGUILayout.BeginHorizontal();
						axis.autoLinkTagPlayer = EditorGUILayout.ToggleLeft("Auto link on tag",axis.autoLinkTagPlayer, GUILayout.Width(200));
						if (axis.autoLinkTagPlayer){
							axis.autoTag = EditorGUILayout.TagField("",axis.autoTag);
						}
						//EditorGUILayout.EndHorizontal();

						if (!axis.autoLinkTagPlayer){
							axis.directTransform = (Transform)EditorGUILayout.ObjectField("Direct action to",axis.directTransform,typeof(Transform),true);
						}
						
						axis.directAction = (ETCAxis.DirectAction ) EditorGUILayout.EnumPopup( "Action",axis.directAction);
						if (axis.directAction != ETCAxis.DirectAction.Jump){
							axis.axisInfluenced = (ETCAxis.AxisInfluenced) EditorGUILayout.EnumPopup("Affected axis",axis.axisInfluenced);
						}
						else{
							EditorGUILayout.HelpBox("Required character controller", MessageType.Info);
						}

						if ((axis.directCharacterController || axis.autoLinkTagPlayer) && (axis.directAction == ETCAxis.DirectAction.Translate || axis.directAction == ETCAxis.DirectAction.TranslateLocal)){

							axis.isLockinJump = EditorGUILayout.Toggle("Lock in jump",axis.isLockinJump);
							if (axis.autoLinkTagPlayer)
								EditorGUILayout.HelpBox("Required character controller", MessageType.Info);
						}
						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
				}
				#endregion

				#region smooth & inertia
				axis.showInertiaInspector = EditorGUILayout.Foldout(axis.showInertiaInspector,"Gravity-Inertia-smoothing...");
				if (axis.showInertiaInspector){
					ETCGuiTools.BeginGroup(20);{
						EditorGUI.indentLevel--;

						if ( axis.directCharacterController!=null || axis.autoLinkTagPlayer){
							axis.gravity = EditorGUILayout.FloatField("Gravity",axis.gravity);
						}

						// Inertia
						axis.isEnertia = ETCGuiTools.Toggle("Enable inertia", axis.isEnertia,true);
						if (axis.isEnertia){
							//EditorGUI.indentLevel--;
							ETCGuiTools.BeginGroup(5);{
								axis.inertia = EditorGUILayout.Slider("Inertia",axis.inertia,1f,500f);
								axis.inertiaThreshold = EditorGUILayout.FloatField("Threshold",axis.inertiaThreshold);
							}ETCGuiTools.EndGroup();
							//EditorGUI.indentLevel++;
						}
						
						// AutoStab & clamp rotation
						if (axis.directAction == ETCAxis.DirectAction.RotateLocal ){
							//AutoStab
							axis.isAutoStab = ETCGuiTools.Toggle("Automatic stabilization",axis.isAutoStab,true);
							if (axis.isAutoStab){
								//EditorGUI.indentLevel--;
								ETCGuiTools.BeginGroup(5);{
									axis.autoStabSpeed = EditorGUILayout.FloatField("Speed",axis.autoStabSpeed);
									axis.autoStabThreshold = EditorGUILayout.FloatField("Threshold ", axis.autoStabThreshold);
								}ETCGuiTools.EndGroup();
								//EditorGUI.indentLevel++;
							}
							
							// clamp rotation
							axis.isClampRotation = ETCGuiTools.Toggle("Clamp rotation",axis.isClampRotation,true);
							if (axis.isClampRotation){
								//EditorGUI.indentLevel--;
								ETCGuiTools.BeginGroup(5);{
									axis.maxAngle = EditorGUILayout.FloatField("Max angle",axis.maxAngle);	
									axis.minAngle = EditorGUILayout.FloatField("Min angle",axis.minAngle);
								}ETCGuiTools.EndGroup();
								//EditorGUI.indentLevel++;
							}
							
						}
			
						EditorGUI.indentLevel++;
					}ETCGuiTools.EndGroup();
				}
				#endregion
			}

			#region Unity axes
			axis.showSimulatinInspector = EditorGUILayout.Foldout(axis.showSimulatinInspector,"Unity axes");
			if (axis.showSimulatinInspector){
				ETCGuiTools.BeginGroup(20);{
					EditorGUI.indentLevel--;
					int index = System.Array.IndexOf(unityAxes,axis.unityAxis );
					int tmpIndex = EditorGUILayout.Popup(index,unityAxes);
					if (tmpIndex != index){
						axis.unityAxis = unityAxes[tmpIndex];
					}
					EditorGUI.indentLevel++;
				}ETCGuiTools.EndGroup();

			}
			#endregion

			EditorGUI.indentLevel--;
		}
	}
}
