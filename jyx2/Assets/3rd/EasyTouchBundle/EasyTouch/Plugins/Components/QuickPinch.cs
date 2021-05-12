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
[AddComponentMenu("EasyTouch/Quick Pinch")]
public class QuickPinch : QuickBase {

	#region Events
	[System.Serializable] public class OnPinchAction : UnityEvent<Gesture>{}
	
	[SerializeField] 
	public OnPinchAction onPinchAction;
	#endregion
	
	#region enumeration
	public enum ActionTiggering {InProgress,End};
	public enum ActionPinchDirection {All, PinchIn, PinchOut};
	#endregion
	
	#region Members
	public bool isGestureOnMe = false;
	public ActionTiggering actionTriggering;
	public ActionPinchDirection pinchDirection;
	private float axisActionValue = 0;
	public bool enableSimpleAction = false;
	#endregion
	
	#region MonoBehaviour callback
	public QuickPinch(){
			quickActionName = "QuickPinch"+ System.Guid.NewGuid().ToString().Substring(0,7);
	}
	
	public override void OnEnable(){
		EasyTouch.On_Pinch += On_Pinch;
		EasyTouch.On_PinchIn += On_PinchIn;
		EasyTouch.On_PinchOut += On_PinchOut;
		EasyTouch.On_PinchEnd += On_PichEnd;
	}
	
	public override void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyTouch.On_Pinch -= On_Pinch;
		EasyTouch.On_PinchIn -= On_PinchIn;
		EasyTouch.On_PinchOut -= On_PinchOut;
		EasyTouch.On_PinchEnd -= On_PichEnd;
	}
	#endregion
	
	#region EasyTouch event
	void On_Pinch (Gesture gesture){
		
		if (actionTriggering == ActionTiggering.InProgress && pinchDirection == ActionPinchDirection.All){
			DoAction( gesture);
		}
		
	}

	void On_PinchIn (Gesture gesture){
		
		if (actionTriggering == ActionTiggering.InProgress & pinchDirection == ActionPinchDirection.PinchIn){
			DoAction( gesture);
		}
		
	}

	void On_PinchOut (Gesture gesture){
		
		if (actionTriggering == ActionTiggering.InProgress & pinchDirection == ActionPinchDirection.PinchOut){
			DoAction( gesture);
		}
		
	}

	void On_PichEnd (Gesture gesture){

		if (actionTriggering == ActionTiggering.End){
			DoAction( gesture);
		}
	}

	#endregion
	
	#region Private method
	void DoAction(Gesture gesture){

		axisActionValue = gesture.deltaPinch * sensibility * Time.deltaTime;

		if (isGestureOnMe){
			if ( realType == GameObjectType.UI){
				if (gesture.isOverGui ){
					if ((gesture.pickedUIElement == gameObject || gesture.pickedUIElement.transform.IsChildOf( transform))){
						onPinchAction.Invoke(gesture);
						if (enableSimpleAction){
							DoDirectAction( axisActionValue);
						}
					}
				}
			}
			else{
				if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI){
					if (gesture.GetCurrentPickedObject(true) == gameObject){
						onPinchAction.Invoke(gesture);
						if (enableSimpleAction){
							DoDirectAction( axisActionValue);
						}
					}
				}
			}
		}
		else{
			if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI){
				onPinchAction.Invoke(gesture);
				if (enableSimpleAction){
					DoDirectAction( axisActionValue);
				}
			}
		}
		
	}
	#endregion
}
}
