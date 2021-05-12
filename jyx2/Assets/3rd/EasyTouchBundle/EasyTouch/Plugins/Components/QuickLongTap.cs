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
[AddComponentMenu("EasyTouch/Quick LongTap")]
public class QuickLongTap : QuickBase {

	#region Events
	[System.Serializable] public class OnLongTap : UnityEvent<Gesture>{}
	
	[SerializeField] 
	public OnLongTap onLongTap;
	#endregion

	#region Enumeration
	public enum ActionTriggering {Start,InProgress,End};
	#endregion

	#region Members
	public ActionTriggering actionTriggering;
	private Gesture currentGesture;
	#endregion

	public QuickLongTap(){
			quickActionName = "QuickLongTap"+ System.Guid.NewGuid().ToString().Substring(0,7);
	}

	void Update(){
		currentGesture = EasyTouch.current;

            if (currentGesture != null)
            {

                if (!is2Finger)
                {

                    if (currentGesture.type == EasyTouch.EvtType.On_TouchStart && fingerIndex == -1 && IsOverMe(currentGesture))
                    {
                        fingerIndex = currentGesture.fingerIndex;
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_LongTapStart && actionTriggering == ActionTriggering.Start)
                    {
                        if (currentGesture.fingerIndex == fingerIndex || isMultiTouch)
                        {

                            DoAction(currentGesture);
                        }
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_LongTap && actionTriggering == ActionTriggering.InProgress)
                    {
                        if (currentGesture.fingerIndex == fingerIndex || isMultiTouch)
                        {
                            DoAction(currentGesture);
                        }
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_LongTapEnd && actionTriggering == ActionTriggering.End)
                    {
                        if (currentGesture.fingerIndex == fingerIndex || isMultiTouch)
                        {
                            DoAction(currentGesture);
                            fingerIndex = -1;
                        }
                    }
                }
                else
                {
                    if (currentGesture.type == EasyTouch.EvtType.On_LongTapStart2Fingers && actionTriggering == ActionTriggering.Start)
                    {
                        DoAction(currentGesture);
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_LongTap2Fingers && actionTriggering == ActionTriggering.InProgress)
                    {
                        DoAction(currentGesture);
                    }

                    if (currentGesture.type == EasyTouch.EvtType.On_LongTapEnd2Fingers && actionTriggering == ActionTriggering.End)
                    {
                        DoAction(currentGesture);
                    }
                }
            }
	}

	void DoAction(Gesture gesture){
		if (IsOverMe(gesture)){
			onLongTap.Invoke( gesture);
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
}
}
