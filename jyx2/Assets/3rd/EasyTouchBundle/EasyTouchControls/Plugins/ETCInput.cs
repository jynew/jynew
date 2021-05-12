/***********************************************
				EasyTouch Controls
	Copyright © 2016 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//ETCSingleton<ETCInput>
public class ETCInput : MonoBehaviour{

	public static ETCInput _instance = null;
	public static ETCInput instance{
		get{
			if( !_instance ){
				
				// check if an ObjectPoolManager is already available in the scene graph
				_instance = FindObjectOfType( typeof( ETCInput ) ) as ETCInput;
				
				// nope, create a new one
				if( !_instance ){
					GameObject obj = new GameObject( "InputManager" );
					_instance = obj.AddComponent<ETCInput>();
				}
			}
			
			return _instance;
		}
	}
	
	private  Dictionary<string,ETCAxis> axes = new Dictionary<string,ETCAxis>();
	private  Dictionary<string, ETCBase> controls = new Dictionary<string, ETCBase>();
	
	private static  ETCBase control;
	private static ETCAxis axis;

	#region Control
	public void RegisterControl(ETCBase ctrl){

		if (controls.ContainsKey( ctrl.name)){
			Debug.LogWarning("ETCInput control : " + ctrl.name + " already exists");
		}
		else{
			controls.Add( ctrl.name, ctrl);
			
			if (ctrl.GetType() == typeof(ETCJoystick) ){
				RegisterAxis( (ctrl as ETCJoystick).axisX );
				RegisterAxis( (ctrl as ETCJoystick).axisY );
			}
			else if (ctrl.GetType() == typeof(ETCTouchPad) ){
				RegisterAxis( (ctrl as ETCTouchPad).axisX );
				RegisterAxis( (ctrl as ETCTouchPad).axisY );
			}
			else if (ctrl.GetType() == typeof(ETCDPad) ){
				RegisterAxis( (ctrl as ETCDPad).axisX );
				RegisterAxis( (ctrl as ETCDPad).axisY );
			}
			else if (ctrl.GetType() == typeof(ETCButton)){
				RegisterAxis( (ctrl as ETCButton).axis );
			}
		}
	}
	
	public void UnRegisterControl(ETCBase ctrl){
		if (controls.ContainsKey( ctrl.name) && ctrl.enabled ){

			controls.Remove( ctrl.name);
			
			if (ctrl.GetType() == typeof(ETCJoystick) ){
				UnRegisterAxis( (ctrl as ETCJoystick).axisX );
				UnRegisterAxis( (ctrl as ETCJoystick).axisY );
			}
			else if (ctrl.GetType() == typeof(ETCTouchPad) ){
				UnRegisterAxis( (ctrl as ETCTouchPad).axisX );
				UnRegisterAxis( (ctrl as ETCTouchPad).axisY );
			}
			else if (ctrl.GetType() == typeof(ETCDPad) ){
				UnRegisterAxis( (ctrl as ETCDPad).axisX );
				UnRegisterAxis( (ctrl as ETCDPad).axisY );
			}
			else if (ctrl.GetType() == typeof(ETCButton)){
				UnRegisterAxis( (ctrl as ETCButton).axis );
			}
		}
	}


	public  void Create(){
	
	}

	public static void Register(ETCBase ctrl){
		ETCInput.instance.RegisterControl( ctrl);
	}

	public static void UnRegister(ETCBase ctrl){
		ETCInput.instance.UnRegisterControl( ctrl);
	}


	public static void SetControlVisible(string ctrlName,bool value){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			control.visible = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}
	
	public static bool GetControlVisible(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			return control.visible;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
			return false;
		}
	}
	
	
	public static void SetControlActivated(string ctrlName,bool value){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			control.activated = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}
	
	public static bool GetControlActivated(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			return control.activated;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
			return false;
		}
	}


	public static void SetControlSwipeIn(string ctrlName,bool value){

		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){

			control.isSwipeIn = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static bool GetControlSwipeIn(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			return control.isSwipeIn;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
			return false;
		}
	}


	public static void SetControlSwipeOut(string ctrlName,bool value){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			control.isSwipeOut = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static bool GetControlSwipeOut(string ctrlName,bool value){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			return control.isSwipeOut ;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
			return false;
		}
	}


	public static void SetDPadAxesCount(string ctrlName, ETCBase.DPadAxis value){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			control.dPadAxisCount = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static ETCBase.DPadAxis GetDPadAxesCount(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			return control.dPadAxisCount;
		}
		else{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
			return ETCBase.DPadAxis.Two_Axis;
		}
	}
	#endregion

	#region New 2.0
	// Control
	public static ETCJoystick GetControlJoystick(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			if (control.GetType() == typeof(ETCJoystick)){
				ETCJoystick tmpJoy = (ETCJoystick)control;
				return tmpJoy;
			}
		}
		
		return null;
	}
	
	public static ETCDPad GetControlDPad(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			if (control.GetType() == typeof(ETCDPad)){
				ETCDPad tmpctrl = (ETCDPad)control;
				return tmpctrl;
			}
		}

		return null;
	}
	
	public static ETCTouchPad GetControlTouchPad(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			if (control.GetType() == typeof(ETCTouchPad)){
				ETCTouchPad tmpctrl = (ETCTouchPad)control;
				return tmpctrl;
			}
		}
		
		return null;
	}
	
	public static ETCButton GetControlButton(string ctrlName){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			if (control.GetType() == typeof(ETCJoystick)){
				ETCButton tmpctrl = (ETCButton)control;
				return tmpctrl;
			}
		}
		
		return null;
	}
	

	//Image
	public static void SetControlSprite(string ctrlName,Sprite spr,Color color = default(Color)){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			Image img = control.GetComponent<Image>();
			
			if (img){
				img.sprite = spr;
				img.color = color;
			}
		}
	}
	
	public static void SetJoystickThumbSprite(string ctrlName,Sprite spr,Color color = default(Color)){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			
			if (control.GetType() == typeof(ETCJoystick)){
				ETCJoystick tmpJoy = (ETCJoystick)control;
				if (tmpJoy){
					Image img = tmpJoy.thumb.GetComponent<Image>();
					
					if (img){
						img.sprite = spr;
						img.color = color;
					}
				}
			}
		}
	}
	
	public static void SetButtonSprite(string ctrlName, Sprite sprNormal,Sprite sprPress,Color color = default(Color)){
		if (ETCInput.instance.controls.TryGetValue( ctrlName, out control)){
			ETCButton btn = control.GetComponent<ETCButton>();
			btn.normalSprite = sprNormal;
			btn.normalColor = color;
			btn.pressedColor = color;
			btn.pressedSprite = sprPress;

			SetControlSprite( ctrlName,sprNormal,color);
		}
	}

	// Axes
	public static void SetAxisSpeed(string axisName, float speed){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.speed = speed;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisGravity(string axisName, float gravity){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.gravity = gravity;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetTurnMoveSpeed(string ctrlName, float speed){
		ETCJoystick joy = GetControlJoystick( ctrlName);
		if (joy){
			joy.tmSpeed = speed;
		}
	}
	#endregion

	#region Axes
	public static void ResetAxis(string axisName ){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.axisValue = 0;
			axis.axisSpeedValue =0;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisEnabled(string axisName,bool value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.enable = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}
	
	public static bool GetAxisEnabled(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.enable;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return false;
		}
	}


	public static void SetAxisInverted(string axisName, bool value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.invertedAxis = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisInverted(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.invertedAxis;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return false;
		}
	}


	public static void SetAxisDeadValue(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.deadValue = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisDeadValue(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.deadValue;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisSensitivity(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.speed = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisSensitivity(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.speed;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisThreshold(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.axisThreshold = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisThreshold(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.axisThreshold;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisInertia(string axisName, bool value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.isEnertia = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisInertia(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.isEnertia;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return false;
		}
	}


	public static void SetAxisInertiaSpeed(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.inertia = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisInertiaSpeed(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.inertia;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}



	public static void SetAxisInertiaThreshold( string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.inertiaThreshold = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisInertiaThreshold( string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.inertiaThreshold;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisAutoStabilization( string axisName, bool value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.isAutoStab = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisAutoStabilization(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.isAutoStab;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return false;
		}
	}


	public static void SetAxisAutoStabilizationSpeed( string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.autoStabSpeed = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisAutoStabilizationSpeed(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.autoStabSpeed;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisAutoStabilizationThreshold( string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.autoStabThreshold = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}
	
	public static float GetAxisAutoStabilizationThreshold(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.autoStabThreshold;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisClampRotation(string axisName, bool value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.isClampRotation = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisClampRotation(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.isClampRotation;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return false;
		}
	}


	public static void SetAxisClampRotationValue(string axisName, float min, float max){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.minAngle = min;
			axis.maxAngle = max;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisClampRotationMinValue(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.minAngle = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisClampRotationMaxValue(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.maxAngle = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisClampRotationMinValue(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.minAngle;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}

	public static float GetAxisClampRotationMaxValue(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.maxAngle;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisDirecTransform(string axisName, Transform value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.directTransform = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static Transform GetAxisDirectTransform( string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.directTransform;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return null;
		}
	}


	public static void SetAxisDirectAction(string axisName, ETCAxis.DirectAction value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.directAction = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static ETCAxis.DirectAction GetAxisDirectAction(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.directAction;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return ETCAxis.DirectAction.Rotate;
		}
	}


	public static void SetAxisAffectedAxis(string axisName, ETCAxis.AxisInfluenced value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.axisInfluenced = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static ETCAxis.AxisInfluenced GetAxisAffectedAxis(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.axisInfluenced;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return ETCAxis.AxisInfluenced.X;
		}
	}
		

	public static void SetAxisOverTime(string axisName, bool value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.isValueOverTime = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisOverTime(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.isValueOverTime;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return false;
		}
	}


	public static void SetAxisOverTimeStep(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.overTimeStep = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisOverTimeStep(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.overTimeStep;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}


	public static void SetAxisOverTimeMaxValue(string axisName, float value){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			axis.maxOverTimeValue = value;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisOverTimeMaxValue(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.maxOverTimeValue;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return -1;
		}
	}



	public static float GetAxis(string axisName){
		
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.axisValue;
		}
		else{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
			return 0;
		}
	}
	
	public static float GetAxisSpeed(string axisName){
		
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			return axis.axisSpeedValue;
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return 0;
		}
		
	}
	
	
	public static bool GetAxisDownUp(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.DownUp){
				return true;
			}
			else{
				return false;
			}
			
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetAxisDownDown(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.DownDown){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetAxisDownRight(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.DownRight){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetAxisDownLeft(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.DownLeft){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	
	public static bool GetAxisPressedUp(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.PressUp){
				return true;
			}
			else{
				return false;
			}
			
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetAxisPressedDown(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.PressDown){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}


	public static bool GetAxisPressedRight(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.PressRight){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetAxisPressedLeft(string axisName){
		if (ETCInput.instance.axes.TryGetValue( axisName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.PressLeft){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(axisName + " doesn't exist");
			return false;
		}
	}
	
	
	public static bool GetButtonDown(string buttonName){
		if (ETCInput.instance.axes.TryGetValue( buttonName, out axis)){
			
			if (axis.axisState == ETCAxis.AxisState.Down ){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(buttonName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetButton(string buttonName){
		if (ETCInput.instance.axes.TryGetValue( buttonName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.Down || axis.axisState == ETCAxis.AxisState.Press){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(buttonName + " doesn't exist");
			return false;
		}
	}
	
	public static bool GetButtonUp(string buttonName){
		if (ETCInput.instance.axes.TryGetValue( buttonName, out axis)){
			if (axis.axisState == ETCAxis.AxisState.Up ){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			Debug.LogWarning(buttonName + " doesn't exist");
			return false;
		}
	}

	public static float GetButtonValue(string buttonName){

		if (ETCInput.instance.axes.TryGetValue( buttonName, out axis)){
			return axis.axisValue;
		}
		else{
			Debug.LogWarning(buttonName + " doesn't exist");
			return -1;
		}
	}
	#endregion

	#region private Method
	private void RegisterAxis(ETCAxis axis){
		
		if (ETCInput.instance.axes.ContainsKey( axis.name)){
			Debug.LogWarning("ETCInput axis : " + axis.name + " already exists");
		}
		else{
			axes.Add( axis.name,axis);
		}
		
	}

	private void UnRegisterAxis(ETCAxis axis){
		
		if (ETCInput.instance.axes.ContainsKey( axis.name)){
			axes.Remove( axis.name);
		}
		
	}
	#endregion


}
