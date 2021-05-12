/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

namespace HedgehogTeam.EasyTouch{
// This is the class that simulate touches with the mouse.
// Internal use only, DO NOT USE IT
public class EasyTouchInput{
	
	#region private members
	private Vector2[] oldMousePosition = new Vector2[2];
	private int[] tapCount = new int[2];
	private float[] startActionTime = new float[2];
	private float[] deltaTime = new float[2];
	private float[] tapeTime = new float[2];
	
	// Complexe 2 fingers simulation
	private bool bComplex=false;
	private Vector2 deltaFingerPosition;
	private Vector2 oldFinger2Position;
	private Vector2 complexCenter;
	#endregion
	
	#region Public methods
	// Return the number of touch
	public int TouchCount(){
		
			#if ((UNITY_ANDROID || UNITY_IOS || UNITY_BLACKBERRY || UNITY_TVOS || UNITY_PSP2) && !UNITY_EDITOR) 
			return getTouchCount(true);
		#else
			return getTouchCount(false);
		#endif
		
	}
	
	private int getTouchCount(bool realTouch){
		
		int count=0;
		
		if (realTouch || EasyTouch.instance.enableRemote ){
			count = Input.touchCount;
		}
		else{
			if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)){
				count=1;
				if (EasyTouch.GetSecondeFingerSimulation()){
					if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(EasyTouch.instance.twistKey)|| Input.GetKey(KeyCode.LeftControl) ||Input.GetKey(EasyTouch.instance.swipeKey ))
						count=2;
					if (Input.GetKeyUp(KeyCode.LeftAlt)|| Input.GetKeyUp(EasyTouch.instance.twistKey)|| Input.GetKeyUp(KeyCode.LeftControl)|| Input.GetKeyUp(EasyTouch.instance.swipeKey))
						count=2;
				}
				if (count ==0){
					complexCenter = Vector2.zero;
					oldMousePosition[0] = new Vector2(-1,-1);
					oldMousePosition[1] = new Vector2(-1,-1);
				}
			}		

		}



		return count;
	}
	
	// return in Finger structure all informations on an touch
	public Finger GetMouseTouch(int fingerIndex,Finger myFinger){

		Finger finger;
	
		if (myFinger!=null){
			finger = myFinger;
		}
		else{
			finger = new Finger();
			finger.gesture = EasyTouch.GestureType.None;
		}
		
		
		if (fingerIndex==1 && (Input.GetKeyUp(KeyCode.LeftAlt)|| Input.GetKeyUp(EasyTouch.instance.twistKey)|| Input.GetKeyUp(KeyCode.LeftControl)|| Input.GetKeyUp(EasyTouch.instance.swipeKey))){
			finger.fingerIndex = fingerIndex;
			finger.position = oldFinger2Position; 
			finger.deltaPosition = finger.position - oldFinger2Position;
			finger.tapCount = tapCount[fingerIndex];
			finger.deltaTime = Time.realtimeSinceStartup-deltaTime[fingerIndex];
			finger.phase = TouchPhase.Ended;
			
			return finger;			
		}
		
		if (Input.GetMouseButton(0)){
			
			finger.fingerIndex = fingerIndex;
			finger.position = GetPointerPosition(fingerIndex);
			
			if (Time.realtimeSinceStartup-tapeTime[fingerIndex]>0.5){
				tapCount[fingerIndex]=0;
			}
			
			if (Input.GetMouseButtonDown(0) || (fingerIndex==1 && (Input.GetKeyDown(KeyCode.LeftAlt)|| Input.GetKeyDown(EasyTouch.instance.twistKey)|| Input.GetKeyDown(KeyCode.LeftControl)|| Input.GetKeyDown(EasyTouch.instance.swipeKey)))){

				// Began						
				finger.position = GetPointerPosition(fingerIndex);
				finger.deltaPosition = Vector2.zero;
				tapCount[fingerIndex]=tapCount[fingerIndex]+1;
				finger.tapCount = tapCount[fingerIndex];
				startActionTime[fingerIndex] = Time.realtimeSinceStartup;
				deltaTime[fingerIndex] = startActionTime[fingerIndex];
				finger.deltaTime = 0;
				finger.phase = TouchPhase.Began;
				
				
				if (fingerIndex==1){
					oldFinger2Position = finger.position;
					oldMousePosition[fingerIndex] = finger.position;
				}
				else{
					oldMousePosition[fingerIndex] = finger.position;
				}
				
				if (tapCount[fingerIndex]==1){
					tapeTime[fingerIndex] = Time.realtimeSinceStartup;
				}
				
				
				return finger;
			}	

			finger.deltaPosition = finger.position - oldMousePosition[fingerIndex];

			finger.tapCount = tapCount[fingerIndex];
			finger.deltaTime = Time.realtimeSinceStartup-deltaTime[fingerIndex];
			if (finger.deltaPosition.sqrMagnitude <1){
				finger.phase = TouchPhase.Stationary;
			}
			else{
				finger.phase = TouchPhase.Moved;
			}
			
			oldMousePosition[fingerIndex] = finger.position;
			deltaTime[fingerIndex] = Time.realtimeSinceStartup;
			
			return finger;
		}
		
		else if (Input.GetMouseButtonUp(0)){
			finger.fingerIndex = fingerIndex;
			finger.position = GetPointerPosition(fingerIndex);
			finger.deltaPosition = finger.position - oldMousePosition[fingerIndex];
			finger.tapCount = tapCount[fingerIndex];
			finger.deltaTime = Time.realtimeSinceStartup-deltaTime[fingerIndex];
			finger.phase = TouchPhase.Ended;
			oldMousePosition[fingerIndex] = finger.position;
			
			return finger;
		}

		return null;
	}
	
	// Get the position of the simulate second finger
	public Vector2 GetSecondFingerPosition(){

		Vector2 pos = new Vector2(-1,-1);

		if ((Input.GetKey(KeyCode.LeftAlt)|| Input.GetKey(EasyTouch.instance.twistKey)) && (Input.GetKey(KeyCode.LeftControl)|| Input.GetKey(EasyTouch.instance.swipeKey))){
			if (!bComplex){
				bComplex=true;
				deltaFingerPosition = (Vector2)Input.mousePosition - oldFinger2Position;
			}
			pos = GetComplex2finger();
			return pos;
		}
		else if (Input.GetKey(KeyCode.LeftAlt)|| Input.GetKey(EasyTouch.instance.twistKey) ){	
			pos =  GetPinchTwist2Finger();
			bComplex = false;
			return pos;
		}else if (Input.GetKey(KeyCode.LeftControl)|| Input.GetKey(EasyTouch.instance.swipeKey) ){	
			
			pos =GetComplex2finger();
			bComplex = false;
			return pos;
		}
		
		return pos;		
	}
	#endregion
	
	#region Private methods
	// Get the postion of simulate finger
	private Vector2 GetPointerPosition(int index){
		
		Vector2 pos;
		
		if (index==0){
			pos= Input.mousePosition;
			return pos;
		}
		else{
			return GetSecondFingerPosition();
			
		}
	}
	
	// Simulate for a twist or pinc
	private Vector2 GetPinchTwist2Finger(bool newSim=false){
		
		Vector2 position;

		if (complexCenter==Vector2.zero){
			position.x = (Screen.width/2.0f) - (Input.mousePosition.x - (Screen.width/2.0f)) ;
			position.y = (Screen.height/2.0f) - (Input.mousePosition.y - (Screen.height/2.0f));
		}
		else{
			position.x = (complexCenter.x) - (Input.mousePosition.x - (complexCenter.x)) ;
			position.y = (complexCenter.y) - (Input.mousePosition.y - (complexCenter.y));
		}
		oldFinger2Position = position;
		
		return position;
	}
	
	// complexe Alt + Ctr
	private Vector2 GetComplex2finger(){
		
		Vector2 position;
		
		position.x = Input.mousePosition.x - deltaFingerPosition.x;
		position.y = Input.mousePosition.y - deltaFingerPosition.y;
		
		complexCenter = new Vector2((Input.mousePosition.x+position.x)/2f, (Input.mousePosition.y+position.y)/2f);
		oldFinger2Position = position;
		
		return position;
	}
	#endregion
}
}

