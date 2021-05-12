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
[AddComponentMenu("EasyTouch/Quick Touch")]
public class QuickTouch : QuickBase {

	#region Events
	[System.Serializable] public class OnTouch : UnityEvent<Gesture>{}
	[System.Serializable] public class OnTouchNotOverMe : UnityEvent<Gesture>{}

	[SerializeField] 
	public OnTouch onTouch;
	public OnTouchNotOverMe onTouchNotOverMe;
	#endregion

	#region Enumeration
	public enum ActionTriggering {Start,Down,Up};
	#endregion

	#region Members
	public ActionTriggering actionTriggering;
	private Gesture currentGesture;
	#endregion

	#region Monobehavior CallBack
	public QuickTouch(){
			quickActionName = "QuickTouch"+ System.Guid.NewGuid().ToString().Substring(0,7);
	}
	#endregion		

	void Update(){
		currentGesture = EasyTouch.current;
		
		if (!is2Finger && currentGesture!=null)
            {
			
			// GetIndex at touch start
			if (currentGesture.type == EasyTouch.EvtType.On_TouchStart && fingerIndex == -1 && IsOverMe(currentGesture)){
				fingerIndex = currentGesture.fingerIndex;
			}
			
			// start
			if (currentGesture.type == EasyTouch.EvtType.On_TouchStart && actionTriggering == ActionTriggering.Start){
				if (currentGesture.fingerIndex == fingerIndex || isMultiTouch){
					DoAction( currentGesture);
				}
			}
			
			// Down
			if (currentGesture.type == EasyTouch.EvtType.On_TouchDown  && actionTriggering == ActionTriggering.Down){
				if (currentGesture.fingerIndex == fingerIndex || isMultiTouch){
					DoAction( currentGesture);
				}
			}
			
			// Up
			if (currentGesture.type == EasyTouch.EvtType.On_TouchUp){
			    if ( actionTriggering == ActionTriggering.Up){
					if (currentGesture.fingerIndex == fingerIndex || isMultiTouch){
						if (IsOverMe(currentGesture)){
							onTouch.Invoke( currentGesture);
						}
						else{
							onTouchNotOverMe.Invoke( currentGesture);
						}
					}
				}
				if (currentGesture.fingerIndex == fingerIndex){ fingerIndex =-1;}
			}
		}
		else{
                if (currentGesture != null){
                    if (currentGesture.type == EasyTouch.EvtType.On_TouchStart2Fingers && actionTriggering == ActionTriggering.Start) {
                        DoAction(currentGesture);
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_TouchDown2Fingers && actionTriggering == ActionTriggering.Down) {
                        DoAction(currentGesture);
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_TouchUp2Fingers && actionTriggering == ActionTriggering.Up) {
                        DoAction(currentGesture);
                    }
                }
		}
	}

	#region Private method
	void DoAction(Gesture gesture){
		if (IsOverMe(gesture)){
			onTouch.Invoke( gesture);
		}

	}

	private bool IsOverMe(Gesture gesture){
		bool returnValue = false;

		if ( realType == GameObjectType.UI){
			if (gesture.isOverGui ){
				if ((gesture.pickedUIElement == gameObject || gesture.pickedUIElement.transform.IsChildOf( transform))){
					returnValue = true;
				}
			}
		}
		else{
			if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI){
				if (EasyTouch.GetGameObjectAt( gesture.position,is2Finger) == gameObject){

					returnValue = true;
				}
			}
		}

		return returnValue;
	}
	#endregion
}
}
	
