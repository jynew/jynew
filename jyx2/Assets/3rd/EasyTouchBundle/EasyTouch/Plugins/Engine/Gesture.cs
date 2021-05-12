/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using System;

namespace HedgehogTeam.EasyTouch{
/// <summary>
/// This is the class passed as parameter by EasyTouch events, that containing all informations about the touch that raise the event,
/// or by the tow fingers gesture that raise the event.
/// </summary>
public class Gesture : BaseFinger,ICloneable{
	
	/// <summary>
	/// The siwpe or drag  type ( None, Left, Right, Up, Down, Other => look at EayTouch.SwipeType enumeration).
	/// </summary>
	public EasyTouch.SwipeDirection swipe;	
	/// <summary>
	/// The length of the swipe.
	/// </summary>
	public float swipeLength;				
	/// <summary>
	/// The swipe vector direction.
	/// </summary>
	public Vector2 swipeVector;			

	/// <summary>
	/// The pinch length delta since last change.
	/// </summary>
	public float deltaPinch;	
	/// <summary>
	/// The angle of the twist.
	/// </summary>
	public float twistAngle;		
	/// <summary>
	/// The distance between two finger for a two finger gesture.
	/// </summary>
	public float twoFingerDistance;

	public EasyTouch.EvtType type = EasyTouch.EvtType.None;
	
	#region public method
	public object Clone(){
		return this.MemberwiseClone();
	}

	
	/// <summary>
	/// Transforms touch position into world space, or the center position between the two touches for a two fingers gesture.
	/// </summary>
	/// <returns>
	/// The touch to wordl point.
	/// </returns>
	/// <param name='z'>
	/// The z position in world units from the camera or in world depending on worldZ value
	/// </param>
	/// <param name='worldZ'>
	/// true = r
	/// </param>
	/// 
	public Vector3 GetTouchToWorldPoint(float z){

		return  Camera.main.ScreenToWorldPoint( new Vector3( position.x, position.y,z));	

	}
	
	public Vector3 GetTouchToWorldPoint( Vector3 position3D){

		return  Camera.main.ScreenToWorldPoint( new Vector3( position.x, position.y,Camera.main.transform.InverseTransformPoint(position3D).z));	
	}


	/// <summary>
	/// Gets the swipe or drag angle. (calculate from swipe Vector)
	/// </summary>
	/// <returns>
	/// Float : The swipe or drag angle.
	/// </returns>
	public float GetSwipeOrDragAngle(){
		return Mathf.Atan2( swipeVector.normalized.y,swipeVector.normalized.x) * Mathf.Rad2Deg;	
	}

	/// <summary>
	/// Normalizeds the position.
	/// </summary>
	/// <returns>
	/// The position.
	/// </returns>
	public Vector2 NormalizedPosition(){
		return new Vector2(100f/Screen.width*position.x/100f,100f/Screen.height*position.y/100f);	
	}

	/// <summary>
	/// Determines whether this instance is over user interface element.
	/// </summary>
	/// <returns><c>true</c> if this instance is over user interface element; otherwise, <c>false</c>.</returns>
	public bool IsOverUIElement(){
		return EasyTouch.IsFingerOverUIElement( fingerIndex);
	}

	/// <summary>
	/// Determines whether this instance is over rect transform the specified tr camera.
	/// </summary>
	/// <returns><c>true</c> if this instance is over rect transform the specified tr camera; otherwise, <c>false</c>.</returns>
	/// <param name="tr">Tr.</param>
	/// <param name="camera">Camera.</param>
	public bool IsOverRectTransform(RectTransform tr,Camera camera=null){

		if (camera == null){
			return RectTransformUtility.RectangleContainsScreenPoint( tr,position,null);
		}
		else{
			return RectTransformUtility.RectangleContainsScreenPoint( tr,position,camera);
		}

	}

	/// <summary>
	/// Gets the first picked user interface element.
	/// </summary>
	/// <returns>The first picked user interface element.</returns>
	public GameObject GetCurrentFirstPickedUIElement(bool isTwoFinger=false){
		return EasyTouch.GetCurrentPickedUIElement( fingerIndex,isTwoFinger);
	}

	/// <summary>
	/// Gets the current picked object.
	/// </summary>
	/// <returns>The current picked object.</returns>
	public GameObject GetCurrentPickedObject(bool isTwoFinger=false){
		return EasyTouch.GetCurrentPickedObject( fingerIndex,isTwoFinger);
	}
	#endregion
}
}
