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
[AddComponentMenu("EasyTouch/Quick Enter-Over-Exit")]
public class QuickEnterOverExist : QuickBase {

	#region Event
	[System.Serializable] public class OnTouchEnter : UnityEvent<Gesture>{}
	[System.Serializable] public class OnTouchOver : UnityEvent<Gesture>{}
	[System.Serializable] public class OnTouchExit : UnityEvent<Gesture>{}

	[SerializeField] 
	public OnTouchEnter onTouchEnter;
	[SerializeField] 
	public OnTouchOver onTouchOver;
	[SerializeField] 
	public OnTouchExit onTouchExit;
	#endregion

	#region Members
	private bool[] fingerOver = new bool[100];
	#endregion

	#region MonoBehaviour callback
	public QuickEnterOverExist(){
			quickActionName = "QuickEnterOverExit"+ System.Guid.NewGuid().ToString().Substring(0,7);
	}

	void Awake(){

		for (int i=0;i<100;i++){
			fingerOver[i] = false;
		}
	}

	public override void OnEnable(){
		base.OnEnable();
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_TouchUp += On_TouchUp;
	}
	
	public override void OnDisable(){
		base.OnDisable();
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
	}
	#endregion

	#region EasyTouch Event
	void On_TouchDown (Gesture gesture){

		if (realType != GameObjectType.UI){
			if ((!enablePickOverUI && gesture.GetCurrentFirstPickedUIElement() == null) || enablePickOverUI){

				if ( gesture.GetCurrentPickedObject()== gameObject){
					if (!fingerOver[gesture.fingerIndex] && ((!isOnTouch && !isMultiTouch) || isMultiTouch)){
						fingerOver[gesture.fingerIndex] = true;
						onTouchEnter.Invoke( gesture);
						isOnTouch = true;
					}
					else if (fingerOver[gesture.fingerIndex]){
						onTouchOver.Invoke(gesture);
					}
				}
				else{
					if (fingerOver[gesture.fingerIndex]){
						fingerOver[gesture.fingerIndex] = false;
						onTouchExit.Invoke(gesture);
						if (!isMultiTouch){
							isOnTouch = false;
						}
					}
				}
			}
			else{
				if ( gesture.GetCurrentPickedObject()== gameObject && (!enablePickOverUI && gesture.GetCurrentFirstPickedUIElement() != null)){
					if (fingerOver[gesture.fingerIndex]){
						fingerOver[gesture.fingerIndex] = false;
						onTouchExit.Invoke(gesture);
						if (!isMultiTouch){
							isOnTouch = false;
						}
					}
				}
			}
		}
		else{
			if ( gesture.GetCurrentFirstPickedUIElement()== gameObject){
				if (!fingerOver[gesture.fingerIndex] && ((!isOnTouch && !isMultiTouch) || isMultiTouch)){
					fingerOver[gesture.fingerIndex] = true;
					onTouchEnter.Invoke( gesture);
					isOnTouch = true;
				}
				else if (fingerOver[gesture.fingerIndex]){
					onTouchOver.Invoke(gesture);
				}
			}
			else{
				if (fingerOver[gesture.fingerIndex]){
					fingerOver[gesture.fingerIndex] = false;
					onTouchExit.Invoke(gesture);
					if (!isMultiTouch){
						isOnTouch = false;
					}
				}
			}
		}
		
	}
	
	void On_TouchUp (Gesture gesture){

		if (fingerOver[gesture.fingerIndex]){
			fingerOver[gesture.fingerIndex] = false;
			onTouchExit.Invoke(gesture);
			if (!isMultiTouch){
				isOnTouch = false;
			}
		}
	}
	#endregion
}
}
