/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HedgehogTeam.EasyTouch{
[AddComponentMenu("EasyTouch/Quick Twist")]
public class QuickTwist : QuickBase {

	#region Events
	[System.Serializable] public class OnTwistAction : UnityEvent<Gesture>{}
	
	[SerializeField] 
	public OnTwistAction onTwistAction;
	#endregion

	#region enumeration
	public enum ActionTiggering {InProgress,End};
	public enum ActionRotationDirection {All, Clockwise, Counterclockwise};
	#endregion

	#region Members
	public bool isGestureOnMe = false;
	public ActionTiggering actionTriggering;
	public ActionRotationDirection rotationDirection;
	private float axisActionValue = 0;
	public bool enableSimpleAction = false;
	#endregion

	#region MonoBehaviour callback
	public QuickTwist(){
			quickActionName = "QuickTwist"+ System.Guid.NewGuid().ToString().Substring(0,7);
	}

	public override void OnEnable(){
		EasyTouch.On_Twist += On_Twist;
		EasyTouch.On_TwistEnd += On_TwistEnd;
	}

	public override void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyTouch.On_Twist -= On_Twist;
		EasyTouch.On_TwistEnd -= On_TwistEnd;
	}
	#endregion

	#region EasyTouch event
	void On_Twist (Gesture gesture){
		
		if (actionTriggering == ActionTiggering.InProgress){

			if (IsRightRotation(gesture)){
				DoAction( gesture);
			}
		}

	}
	
	void On_TwistEnd (Gesture gesture){
	
		if (actionTriggering == ActionTiggering.End){
			if (IsRightRotation(gesture)){
				DoAction( gesture);
			}
		}
	}
	#endregion

	#region Private method
	bool IsRightRotation(Gesture gesture){

		axisActionValue =0;
		float coef = 1;
		if ( inverseAxisValue){
			coef = -1;
		}

		switch (rotationDirection){
		case ActionRotationDirection.All:
			axisActionValue = gesture.twistAngle * sensibility * coef;
			return true;

		case ActionRotationDirection.Clockwise:
			if (gesture.twistAngle<0){
				axisActionValue = gesture.twistAngle * sensibility* coef;
				return true;
			}
			break;
		case ActionRotationDirection.Counterclockwise:
			if (gesture.twistAngle>0){
				axisActionValue = gesture.twistAngle * sensibility* coef;
				return true;
			}
			break;
		}

		return false;
	}

	void DoAction(Gesture gesture){

		if (isGestureOnMe){
			if ( realType == GameObjectType.UI){
				if (gesture.isOverGui ){
					if ((gesture.pickedUIElement == gameObject || gesture.pickedUIElement.transform.IsChildOf( transform))){
						onTwistAction.Invoke(gesture);
						if (enableSimpleAction){
							DoDirectAction( axisActionValue);
						}
					}
				}
			}
			else{

				if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI){
					if (gesture.GetCurrentPickedObject( true) == gameObject){
						onTwistAction.Invoke(gesture);
						if (enableSimpleAction){
							DoDirectAction( axisActionValue);
						}
					}
				}
			}
		}
		else{
			if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI){
				onTwistAction.Invoke(gesture);
				if (enableSimpleAction){
					DoDirectAction( axisActionValue);
				}
			}
		}

	}
	#endregion
}
}
