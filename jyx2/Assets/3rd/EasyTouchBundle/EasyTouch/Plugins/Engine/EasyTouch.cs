/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace HedgehogTeam.EasyTouch{
/// <summary>
/// This is the main class of Easytouch engine. 
/// 
/// For add Easy Touch to your scene
/// It is a fake singleton, so you can simply access their settings via a script with all static methods or with the inspector.<br>
/// </summary>
public class EasyTouch : MonoBehaviour {

	#region private classes
	[System.Serializable]
	private class DoubleTap{
		public bool inDoubleTap = false;
		public bool inWait = false;
		public float time=0;
		public int count=0;
		public Finger finger;

		public void Stop(){
			inDoubleTap = false;
			inWait = false;
			time=0;
			count=0;
		}
	}	

	private class PickedObject{
		public GameObject pickedObj;
		public Camera pickedCamera;
		public bool isGUI;
	}
	#endregion

	#region Delegate
	public delegate void TouchCancelHandler(Gesture gesture);
	public delegate void Cancel2FingersHandler(Gesture gesture);
	public delegate void TouchStartHandler(Gesture gesture);
	public delegate void TouchDownHandler(Gesture gesture);
	public delegate void TouchUpHandler(Gesture gesture);
	public delegate void SimpleTapHandler(Gesture gesture);
	public delegate void DoubleTapHandler(Gesture gesture);
	public delegate void LongTapStartHandler(Gesture gesture);
	public delegate void LongTapHandler(Gesture gesture);
	public delegate void LongTapEndHandler(Gesture gesture);
	public delegate void DragStartHandler(Gesture gesture);
	public delegate void DragHandler(Gesture gesture);
	public delegate void DragEndHandler(Gesture gesture);
	public delegate void SwipeStartHandler(Gesture gesture);
	public delegate void SwipeHandler(Gesture gesture);
	public delegate void SwipeEndHandler(Gesture gesture);
	public delegate void TouchStart2FingersHandler(Gesture gesture);
	public delegate void TouchDown2FingersHandler(Gesture gesture);
	public delegate void TouchUp2FingersHandler(Gesture gesture);
	public delegate void SimpleTap2FingersHandler(Gesture gesture);
	public delegate void DoubleTap2FingersHandler(Gesture gesture);
	public delegate void LongTapStart2FingersHandler(Gesture gesture);
	public delegate void LongTap2FingersHandler(Gesture gesture);
	public delegate void LongTapEnd2FingersHandler(Gesture gesture);
	public delegate void TwistHandler(Gesture gesture);
	public delegate void TwistEndHandler(Gesture gesture);
	public delegate void PinchInHandler(Gesture gesture);
	public delegate void PinchOutHandler(Gesture gesture);
	public delegate void PinchEndHandler(Gesture gesture);
	public delegate void PinchHandler(Gesture gesture);
	public delegate void DragStart2FingersHandler(Gesture gesture);
	public delegate void Drag2FingersHandler(Gesture gesture);
	public delegate void DragEnd2FingersHandler(Gesture gesture);
	public delegate void SwipeStart2FingersHandler(Gesture gesture);
	public delegate void Swipe2FingersHandler(Gesture gesture);
	public delegate void SwipeEnd2FingersHandler(Gesture gesture);
	public delegate void EasyTouchIsReadyHandler();

	public delegate void OverUIElementHandler( Gesture gesture);
	public delegate void UIElementTouchUpHandler( Gesture gesture);
	#endregion
	
	#region Events
	/// <summary>
	/// Occurs when The system cancelled tracking for the touch, as when (for example) the user puts the device to her face.
	/// </summary>
	public static event TouchCancelHandler On_Cancel;
	/// <summary>
	/// Occurs when the touch count is no longer egal to 2 and different to 0, after the begining of a two fingers gesture.
	/// </summary>
	public static event Cancel2FingersHandler On_Cancel2Fingers;
	/// <summary>
	/// Occurs when a finger touched the screen.
	/// </summary>
	public static event TouchStartHandler On_TouchStart;
	/// <summary>
	/// Occurs as the touch is active.
	/// </summary>
	public static event TouchDownHandler On_TouchDown;
	/// <summary>
	/// Occurs when a finger was lifted from the screen.
	/// </summary>
	public static event TouchUpHandler On_TouchUp;
	/// <summary>
	/// Occurs when a finger was lifted from the screen, and the time elapsed since the beginning of the touch is less than the time required for the detection of a long tap.
	/// </summary>
	public static event SimpleTapHandler On_SimpleTap;
	/// <summary>
	/// Occurs when the number of taps is egal to 2 in a short time.
	/// </summary>
	public static event DoubleTapHandler On_DoubleTap;
	/// <summary>
	/// Occurs when a finger is touching the screen,  but hasn't moved  since the time required for the detection of a long tap.
	/// </summary>
	public static event LongTapStartHandler On_LongTapStart;
	/// <summary>
	/// Occurs as the touch is active after a LongTapStart
	/// </summary>
	public static event LongTapHandler On_LongTap;
	/// <summary>
	/// Occurs when a finger was lifted from the screen, and the time elapsed since the beginning of the touch is more than the time required for the detection of a long tap.
	/// </summary>
	public static event LongTapEndHandler On_LongTapEnd;
	/// <summary>
	/// Occurs when a drag start. A drag is a swipe on a pickable object
	/// </summary>
	public static event DragStartHandler On_DragStart;
	/// <summary>
	/// Occurs as the drag is active.
	/// </summary>
	public static event DragHandler On_Drag;
	/// <summary>
	/// Occurs when a finger that raise the drag event , is lifted from the screen.
	/// </summary>/
	public static event DragEndHandler On_DragEnd;
	/// <summary>
	/// Occurs when swipe start.
	/// </summary>
	public static event SwipeStartHandler On_SwipeStart;
	/// <summary>
	/// Occurs as the  swipe is active.
	/// </summary>
	public static event SwipeHandler On_Swipe;
	/// <summary>
	/// Occurs when a finger that raise the swipe event , is lifted from the screen.
	/// </summary>
	public static event SwipeEndHandler On_SwipeEnd;
	/// <summary>
	/// Like On_TouchStart but for a 2 fingers gesture.
	/// </summary>
	public static event TouchStart2FingersHandler On_TouchStart2Fingers;
	/// <summary>
	/// Like On_TouchDown but for a 2 fingers gesture.
	/// </summary>
	public static event TouchDown2FingersHandler On_TouchDown2Fingers;
	/// <summary>
	/// Like On_TouchUp but for a 2 fingers gesture.
	/// </summary>
	public static event TouchUp2FingersHandler On_TouchUp2Fingers;
	/// <summary>
	/// Like On_SimpleTap but for a 2 fingers gesture.
	/// </summary>
	public static event SimpleTap2FingersHandler On_SimpleTap2Fingers;
	/// <summary>
	/// Like On_DoubleTap but for a 2 fingers gesture.
	/// </summary>
	public static event DoubleTap2FingersHandler On_DoubleTap2Fingers;
	/// <summary>
	/// Like On_LongTapStart but for a 2 fingers gesture.
	/// </summary>
	public static event LongTapStart2FingersHandler On_LongTapStart2Fingers;
	/// <summary>
	/// Like On_LongTap but for a 2 fingers gesture.
	/// </summary>
	public static event LongTap2FingersHandler On_LongTap2Fingers;
	/// <summary>
	/// Like On_LongTapEnd but for a 2 fingers gesture.
	/// </summary>
	public static event LongTapEnd2FingersHandler On_LongTapEnd2Fingers;
	/// <summary>
	/// Occurs when a twist gesture start
	/// </summary>
	public static event TwistHandler On_Twist;
	/// <summary>
	/// Occurs as the twist gesture is active.
	/// </summary>
	public static event TwistEndHandler On_TwistEnd;
	/// <summary>
	/// Occurs as the pinch  gesture is active.
	/// </summary>
	public static event PinchHandler On_Pinch;
	/// <summary>
	/// Occurs as the pinch in gesture is active.
	/// </summary>
	public static event PinchInHandler On_PinchIn;
	/// <summary>
	/// Occurs as the pinch out gesture is active.
	/// </summary>
	public static event PinchOutHandler On_PinchOut;
	/// <summary>
	/// Occurs when the 2 fingers that raise the pinch event , are lifted from the screen.
	/// </summary>
	public static event PinchEndHandler On_PinchEnd;
	/// <summary>
	/// Like On_DragStart but for a 2 fingers gesture.
	/// </summary>
	public static event DragStart2FingersHandler On_DragStart2Fingers;
	/// <summary>
	/// Like On_Drag but for a 2 fingers gesture.
	/// </summary>
	public static event Drag2FingersHandler On_Drag2Fingers;
	/// <summary>
	/// Like On_DragEnd2Fingers but for a 2 fingers gesture.
	/// </summary>
	public static event DragEnd2FingersHandler On_DragEnd2Fingers;
	/// <summary>
	/// Like On_SwipeStart but for a 2 fingers gesture.
	/// </summary>
	public static event SwipeStart2FingersHandler On_SwipeStart2Fingers;
	/// <summary>
	/// Like On_Swipe but for a 2 fingers gesture.
	/// </summary>
	public static event Swipe2FingersHandler On_Swipe2Fingers;
	/// <summary>
	/// Like On_SwipeEnd but for a 2 fingers gesture.
	/// </summary>
	public static event SwipeEnd2FingersHandler On_SwipeEnd2Fingers;
	/// <summary>
	/// Occurs when  easy touch is ready.
	/// </summary>
	public static event EasyTouchIsReadyHandler On_EasyTouchIsReady;
	/// <summary>
	/// Occurs when current touch is over user interface element.
	/// </summary>
	public static event OverUIElementHandler On_OverUIElement;

	public static event UIElementTouchUpHandler On_UIElementTouchUp;
	#endregion

	#region Enumerations

	public enum GesturePriority{ Tap, Slips};

	public enum DoubleTapDetection { BySystem, ByTime}

	public enum GestureType{ Tap, Drag, Swipe, None, LongTap, Pinch, Twist, Cancel, Acquisition };
	/// <summary>
	/// Represents the different directions for a swipe or drag gesture (Left, Right, Up, Down, Other)
	/// 
	/// The direction is influenced by the swipe Tolerance parameter Look at SetSwipeTolerance( float tolerance)
	/// <br><br>
	/// This enumeration is used on Gesture class
	/// </summary>
	public enum SwipeDirection{ None, Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight,Other,All};
		
	public enum TwoFingerPickMethod{ Finger, Average};
	
	public enum EvtType{ None,On_TouchStart,On_TouchDown,On_TouchUp,On_SimpleTap,On_DoubleTap,On_LongTapStart,On_LongTap,
	On_LongTapEnd,On_DragStart,On_Drag,On_DragEnd,On_SwipeStart,On_Swipe,On_SwipeEnd,On_TouchStart2Fingers,On_TouchDown2Fingers,On_TouchUp2Fingers,On_SimpleTap2Fingers,
	On_DoubleTap2Fingers,On_LongTapStart2Fingers,On_LongTap2Fingers,On_LongTapEnd2Fingers,On_Twist,On_TwistEnd,On_Pinch,On_PinchIn,On_PinchOut,On_PinchEnd,On_DragStart2Fingers,
		On_Drag2Fingers,On_DragEnd2Fingers,On_SwipeStart2Fingers,On_Swipe2Fingers,On_SwipeEnd2Fingers, On_EasyTouchIsReady ,On_Cancel, On_Cancel2Fingers,On_OverUIElement, On_UIElementTouchUp}

	#endregion
	
	#region Public members
	private static EasyTouch _instance;
	public static EasyTouch instance{
		get{
			if( !_instance ){
				
				// check if an ObjectPoolManager is already available in the scene graph
				_instance = FindObjectOfType( typeof( EasyTouch ) ) as EasyTouch;
				
				// nope, create a new one
				if( !_instance ){
					GameObject obj = new GameObject( "Easytouch" );
					_instance = obj.AddComponent<EasyTouch>();
				}
			}
			
			return _instance;
		}
	}

	private Gesture _currentGesture = new Gesture();
	public static Gesture current{
		get{
			return EasyTouch.instance._currentGesture;
		}
	}

	private List<Gesture> _currentGestures = new List<Gesture>();

	public bool enable;				// Enables or disables Easy Touch
	public bool enableRemote;			// Enables or disables Unity remote
		
	// General gesture properties
	public GesturePriority gesturePriority; 
	public float StationaryTolerance;// 
	public float longTapTime ;			// The time required for the detection of a long tap.
	public float swipeTolerance;		// Determines the accuracy of detecting a drag movement 0 => no precision 1=> high precision.
	public float minPinchLength;			// The minimum length for a pinch detection.
	public float minTwistAngle;			// The minimum angle for a twist detection.
	public DoubleTapDetection doubleTapDetection;
	public float doubleTapTime;
	public bool alwaysSendSwipe;
	//public bool isDpi;

	// Two finger gesture
	public bool enable2FingersGesture; // Enables 2 fingers gesture.
	public bool enableTwist;			// Enables or disables recognition of the twist
	public bool enablePinch;			// Enables or disables recognition of the Pinch
	public bool enable2FingersSwipe; 	// Enables or disables recognition of 2 fingers swipe
	public TwoFingerPickMethod twoFingerPickMethod;

	// Auto selection
	public List<ECamera> touchCameras;	// The  cameras
	public bool autoSelect;  							// Enables or disables auto select
	public LayerMask pickableLayers3D;							// Layer detectable by default
	public bool enable2D;								// Enables or disables auto select on 2D
	public LayerMask pickableLayers2D;		
	public bool autoUpdatePickedObject;

	// Unity UI
	//public EasyTouchRaycaster uiRaycaster;
	public bool allowUIDetection;
	public bool enableUIMode;
	public bool autoUpdatePickedUI;

	// NGUI
	public bool enabledNGuiMode;	// True = no events are send when touch is hover an NGui panel
	public LayerMask nGUILayers;
	public List<Camera> nGUICameras;
		
	// Second Finger
	public bool enableSimulation;
	public KeyCode twistKey;
	public KeyCode swipeKey;

	// Inspector
	public bool showGuiInspector = false;
	public bool showSelectInspector = false;
	public bool showGestureInspector = false;
	public bool showTwoFingerInspector = false;
	public bool showSecondFingerInspector = false;
	#endregion
	
	#region Private members	
	private EasyTouchInput input = new EasyTouchInput();
	private Finger[] fingers=new Finger[100];					// The informations of the touch for finger 1.
	public Texture secondFingerTexture;							// The texture to display the simulation of the second finger.
	private TwoFingerGesture twoFinger = new TwoFingerGesture();
	private int oldTouchCount=0;
	private DoubleTap[] singleDoubleTap = new DoubleTap[100];
	private Finger[] tmpArray = new Finger[100];
	private PickedObject pickedObject = new PickedObject();

	// Unity UI
	private List<RaycastResult> uiRaycastResultCache= new List<RaycastResult>();
	private PointerEventData uiPointerEventData;
	private EventSystem uiEventSystem;

	#endregion
	
	#region Constructor
	public EasyTouch(){
		enable = true;		

		allowUIDetection = true;
		enableUIMode = true;
		autoUpdatePickedUI = false;

		enabledNGuiMode = false;
		nGUICameras = new List<Camera>();

		autoSelect = true; 
		touchCameras = new List<ECamera>();
		pickableLayers3D = 1<<0;
		enable2D = false;
		pickableLayers2D = 1<<0;

		gesturePriority = GesturePriority.Tap;
		StationaryTolerance = 15;
		longTapTime =1;
		doubleTapDetection = DoubleTapDetection.BySystem;
		doubleTapTime = 0.3f;
		swipeTolerance = 0.85f;
		alwaysSendSwipe = false;

		enable2FingersGesture=true; 
		twoFingerPickMethod = TwoFingerPickMethod.Finger;
		enable2FingersSwipe = true;
		enablePinch = true;
		minPinchLength = 0f;
		enableTwist = true;
		minTwistAngle = 0f;

		enableSimulation = true;
		twistKey = KeyCode.LeftAlt;
		swipeKey = KeyCode.LeftControl;

		
	}
	#endregion
	
	#region MonoBehaviour Callback
	void OnEnable(){
		if (Application.isPlaying && Application.isEditor){
			Init();	
		}
	}

	void Awake(){
		Init();	
	}

	void Start(){

		for (int i=0;i<100;i++){
			singleDoubleTap[i] = new DoubleTap();
		}
		int index = touchCameras.FindIndex( 
			delegate(ECamera c){
				return c.camera == Camera.main;
			}
		);
		
		if (index<0)
			touchCameras.Add(new ECamera(Camera.main,false));

		// Fire ready event
		if (On_EasyTouchIsReady!=null){
			On_EasyTouchIsReady();	
		}

		// Current gesture
		_currentGestures.Add( new Gesture());
	}
	
	void Init(){

		// The texture to display the simulation of the second finger.
		#if ((!UNITY_ANDROID && !UNITY_IOS &&  !UNITY_TVOS && !UNITY_WINRT && !UNITY_BLACKBERRY) || UNITY_EDITOR)
			if (secondFingerTexture==null && enableSimulation){
				secondFingerTexture =Resources.Load("secondFinger") as Texture;
			}
		#endif	
	}
	
	// Display the simulation of the second finger
	#if ((!UNITY_ANDROID && !UNITY_IOS && !UNITY_TVOS && !UNITY_WINRT && !UNITY_BLACKBERRY) || UNITY_EDITOR) 
	void OnGUI(){
		if (enableSimulation && !enableRemote){
			Vector2 finger = input.GetSecondFingerPosition();
			if (finger!=new Vector2(-1,-1)){		
				GUI.DrawTexture( new Rect(finger.x-16,Screen.height-finger.y-16,32,32),secondFingerTexture);
			}
		}
	}
	#endif
	
	void OnDrawGizmos(){
	}

	// Non comments.
	void Update(){
	

		if (enable && EasyTouch.instance==this){

			//#if (UNITY_EDITOR )
			if (Application.isPlaying && Input.touchCount>0){
				enableRemote = true;
			}
			
			if (Application.isPlaying && Input.touchCount==0){
				enableRemote = false;
			}
			//#endif

			int i;
			
			// How many finger do we have ?
			int count = input.TouchCount();
		
			// Reset after two finger gesture;
			if (oldTouchCount==2 && count!=2 && count>0){
				CreateGesture2Finger(EvtType.On_Cancel2Fingers,Vector2.zero,Vector2.zero,Vector2.zero,0,SwipeDirection.None,0,Vector2.zero,0,0,0);
			}

			// Get touches		
			//#if (((UNITY_ANDROID || UNITY_IOS || UNITY_WINRT || UNITY_BLACKBERRY || UNITY_TVOS) && !UNITY_EDITOR))
				#if (((UNITY_ANDROID || UNITY_IOS || UNITY_BLACKBERRY || UNITY_TVOS || UNITY_PSP2) && !UNITY_EDITOR))
				UpdateTouches(true, count);
			#else
				UpdateTouches(false, count);
			#endif				
		
			// two fingers gesture
			twoFinger.oldPickedObject = twoFinger.pickedObject;
			if (enable2FingersGesture){
				if (count==2){
					TwoFinger();
				}
			}

			// Other fingers gesture
			for (i=0;i<100;i++){
				if (fingers[i]!=null){
					OneFinger(i);
				}
			}

			oldTouchCount = count;
		}
	}


	void LateUpdate(){

		// single gesture
		if (_currentGestures.Count>1){
			_currentGestures.RemoveAt(0);	
		}
		else{
                _currentGestures[0] = null;// new Gesture();
		}
		_currentGesture = _currentGestures[0];


	}

		
 
	void UpdateTouches(bool realTouch, int touchCount){
		 
		fingers.CopyTo( tmpArray,0);
		
		
		if (realTouch || enableRemote){
			ResetTouches();
			for (var i = 0; i < touchCount; ++i) {
				Touch touch = Input.GetTouch(i);
				
				int t=0;
				while (t < 100 && fingers[i]==null){	
					if (tmpArray[t] != null){
						if ( tmpArray[t].fingerIndex == touch.fingerId){								
							fingers[i] = tmpArray[t];
						}
					}
					t++;	
				}
				
				if (fingers[i]==null){
					fingers[i]= new Finger();
					fingers[i].fingerIndex = touch.fingerId;
					fingers[i].gesture = GestureType.None;
					fingers[i].phase = TouchPhase.Began;
				}
				else{
					fingers[i].phase = touch.phase;
				}

				if ( fingers[i].phase!= TouchPhase.Began){
					fingers[i].deltaPosition = touch.position - fingers[i].position;
				}
				else{
					fingers[i].deltaPosition = Vector2.zero;
				}

				fingers[i].position = touch.position;
				//fingers[i].deltaPosition = touch.deltaPosition;
				fingers[i].tapCount = touch.tapCount;
				fingers[i].deltaTime = touch.deltaTime;
				
				fingers[i].touchCount = touchCount;		

				fingers[i].altitudeAngle = touch.altitudeAngle;
				fingers[i].azimuthAngle = touch.azimuthAngle;
				fingers[i].maximumPossiblePressure = touch.maximumPossiblePressure;
				fingers[i].pressure = touch.pressure;
				fingers[i].radius = touch.radius;
				fingers[i].radiusVariance = touch.radiusVariance;
				fingers[i].touchType = touch.type;

			}
		}
		else{
			int i=0;
			while (i<touchCount){
				fingers[i] = input.GetMouseTouch(i,fingers[i]) as Finger;
				fingers[i].touchCount = touchCount;
				i++;
			}			
		}
		
	}
	
	void ResetTouches(){
		for (int i=0;i<100;i++){
			fingers[i] = null;
		}	
	}	
	#endregion
	
	#region One finger Private methods
	private void OneFinger(int fingerIndex){

		// A tap starts ?
		if ( fingers[fingerIndex].gesture==GestureType.None){

			if (!singleDoubleTap[fingerIndex].inDoubleTap){
				singleDoubleTap[fingerIndex].inDoubleTap = true;
				singleDoubleTap[fingerIndex].time = 0;
				singleDoubleTap[fingerIndex].count = 1;
			}

			fingers[fingerIndex].startTimeAction = Time.realtimeSinceStartup;
			fingers[fingerIndex].gesture=GestureType.Acquisition;
			fingers[fingerIndex].startPosition = fingers[fingerIndex].position;
			
			// do we touch a pickable gameobject ?
			if (autoSelect){
				if (GetPickedGameObject(fingers[fingerIndex])){
					fingers[fingerIndex].pickedObject = pickedObject.pickedObj;
					fingers[fingerIndex].isGuiCamera = pickedObject.isGUI;
					fingers[fingerIndex].pickedCamera = pickedObject.pickedCamera;
				}
			}

			// UnityGUI
			if (allowUIDetection){
				fingers[fingerIndex].isOverGui = IsScreenPositionOverUI(  fingers[fingerIndex].position );
				fingers[fingerIndex].pickedUIElement = GetFirstUIElementFromCache();
			}

			// we notify a touch
			CreateGesture(fingerIndex, EvtType.On_TouchStart,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);
		}

		if (singleDoubleTap[fingerIndex].inDoubleTap) singleDoubleTap[fingerIndex].time += Time.deltaTime;
		
		// Calculates the time since the beginning of the action.
		fingers[fingerIndex].actionTime =  Time.realtimeSinceStartup - fingers[fingerIndex].startTimeAction;
		
		
		// touch canceled?
		if (fingers[fingerIndex].phase == TouchPhase.Canceled){
			fingers[fingerIndex].gesture = GestureType.Cancel;
		}
		
		if (fingers[fingerIndex].phase != TouchPhase.Ended && fingers[fingerIndex].phase != TouchPhase.Canceled){
		
			// Are we stationary  for a long tap
			if (fingers[fingerIndex].phase == TouchPhase.Stationary  && 
			    fingers[fingerIndex].actionTime >= longTapTime && fingers[fingerIndex].gesture == GestureType.Acquisition){

				fingers[fingerIndex].gesture = GestureType.LongTap;				
				CreateGesture(fingerIndex, EvtType.On_LongTapStart,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);	
			}
			
			// Let's move us?
			if (( (fingers[fingerIndex].gesture == GestureType.Acquisition ||fingers[fingerIndex].gesture == GestureType.LongTap) && fingers[fingerIndex].phase == TouchPhase.Moved && gesturePriority == GesturePriority.Slips)
				|| ((fingers[fingerIndex].gesture == GestureType.Acquisition ||fingers[fingerIndex].gesture == GestureType.LongTap) && (FingerInTolerance(fingers[fingerIndex])==false) && gesturePriority == GesturePriority.Tap ))
			{

				//  long touch => cancel
				if (fingers[fingerIndex].gesture == GestureType.LongTap){
					fingers[fingerIndex].gesture = GestureType.Cancel;
					CreateGesture(fingerIndex, EvtType.On_LongTapEnd,fingers[fingerIndex],SwipeDirection.None,0,Vector2.zero);
					// Init the touch to start
					fingers[fingerIndex].gesture=GestureType.Acquisition;	
				}
				else{
					fingers[fingerIndex].oldSwipeType = SwipeDirection.None;

					// If an object is selected we drag
					if (fingers[fingerIndex].pickedObject){
						fingers[fingerIndex].gesture = GestureType.Drag;
						CreateGesture(fingerIndex, EvtType.On_DragStart,fingers[fingerIndex],SwipeDirection.None,0, Vector2.zero);

						if (alwaysSendSwipe){
							CreateGesture(fingerIndex, EvtType.On_SwipeStart,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);
						}
					}
					// If not swipe
					else{
						fingers[fingerIndex].gesture = GestureType.Swipe;
						CreateGesture(fingerIndex, EvtType.On_SwipeStart,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);
					}
					
				}
			}
			
			// Gesture update
			EvtType message = EvtType.None;
			
			switch (fingers[fingerIndex].gesture){
				case GestureType.LongTap:
					message=EvtType.On_LongTap;
					break;
				case GestureType.Drag:
					message=EvtType.On_Drag;
					break;
				case GestureType.Swipe:
					message=EvtType.On_Swipe;
					break;
			}
			
			// Send gesture
			SwipeDirection currentSwipe = SwipeDirection.None;
			currentSwipe = GetSwipe(new Vector2(0,0),fingers[fingerIndex].deltaPosition);
			if (message!=EvtType.None){

				fingers[fingerIndex].oldSwipeType = currentSwipe;
				CreateGesture(fingerIndex, message,fingers[fingerIndex], currentSwipe ,0,fingers[fingerIndex].deltaPosition);

				if (message ==  EvtType.On_Drag && alwaysSendSwipe){
					CreateGesture(fingerIndex, EvtType.On_Swipe,fingers[fingerIndex], currentSwipe ,0,fingers[fingerIndex].deltaPosition);
				}
			}
			
			// TouchDown
			CreateGesture(fingerIndex, EvtType.On_TouchDown,fingers[fingerIndex], currentSwipe,0,fingers[fingerIndex].deltaPosition);
		}
		else{
			// End of the touch		
			switch (fingers[fingerIndex].gesture){
				// tap
				case GestureType.Acquisition:
					
					if (doubleTapDetection == DoubleTapDetection.BySystem){
						if (FingerInTolerance(fingers[fingerIndex])){
							if (fingers[fingerIndex].tapCount<2){
								CreateGesture( fingerIndex, EvtType.On_SimpleTap,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);
							}
							else{
								CreateGesture( fingerIndex, EvtType.On_DoubleTap,fingers[fingerIndex],  SwipeDirection.None,0,Vector2.zero);
							}
							
						}
					}
					else{
						if (!singleDoubleTap[fingerIndex].inWait){
							singleDoubleTap[fingerIndex].finger = fingers[fingerIndex];
							StartCoroutine(SingleOrDouble(fingerIndex) );
						}
						else{
							singleDoubleTap[fingerIndex].count++;
						}
					}
	
					break;
				// long tap
				case GestureType.LongTap:
					CreateGesture( fingerIndex, EvtType.On_LongTapEnd,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);
					break;
				// drag
				case GestureType.Drag:
					CreateGesture(fingerIndex,  EvtType.On_DragEnd,fingers[fingerIndex], GetSwipe(fingers[fingerIndex].startPosition,fingers[fingerIndex].position), (fingers[fingerIndex].startPosition-fingers[fingerIndex].position).magnitude,fingers[fingerIndex].position-fingers[fingerIndex].startPosition);
					if (alwaysSendSwipe){
						CreateGesture( fingerIndex, EvtType.On_SwipeEnd,fingers[fingerIndex], GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].position-fingers[fingerIndex].startPosition).magnitude,fingers[fingerIndex].position-fingers[fingerIndex].startPosition); 
					}
					break;
				// swipe
				case GestureType.Swipe:
					CreateGesture( fingerIndex, EvtType.On_SwipeEnd,fingers[fingerIndex], GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].position-fingers[fingerIndex].startPosition).magnitude,fingers[fingerIndex].position-fingers[fingerIndex].startPosition); 
					break;

				// cancel
				case GestureType.Cancel:
					CreateGesture(fingerIndex, EvtType.On_Cancel,fingers[fingerIndex],SwipeDirection.None,0,Vector2.zero);
					break;

			}
			CreateGesture( fingerIndex, EvtType.On_TouchUp,fingers[fingerIndex], SwipeDirection.None,0,Vector2.zero);
			fingers[fingerIndex]=null;		

		}
	
	}

	IEnumerator SingleOrDouble(int fingerIndex){
		singleDoubleTap[fingerIndex].inWait = true;
		float time2Wait = doubleTapTime-singleDoubleTap[fingerIndex].finger.actionTime;
		if (time2Wait<0) time2Wait =0;
		yield return new WaitForSeconds(time2Wait);


		if (singleDoubleTap[fingerIndex].count <2){
			//CreateGesture( fingerIndex, EvtType.On_SimpleTap,singleDoubleTap[fingerIndex].finger, SwipeDirection.None,0,Vector2.zero);
			CreateGesture( fingerIndex, EvtType.On_SimpleTap,singleDoubleTap[fingerIndex].finger, SwipeDirection.None,0,singleDoubleTap[fingerIndex].finger.deltaPosition);
		}
		else{
			//CreateGesture( fingerIndex, EvtType.On_DoubleTap,singleDoubleTap[fingerIndex].finger, SwipeDirection.None,0,Vector2.zero);
			CreateGesture( fingerIndex, EvtType.On_DoubleTap,singleDoubleTap[fingerIndex].finger, SwipeDirection.None,0,singleDoubleTap[fingerIndex].finger.deltaPosition);
		}

		//fingers[fingerIndex]=null;
		singleDoubleTap[fingerIndex].Stop();
		StopCoroutine( "SingleOrDouble");
	}

	private void CreateGesture(int touchIndex,EvtType message,Finger finger, SwipeDirection swipe, float swipeLength, Vector2 swipeVector){

		bool firingEvent = true;

		if (autoUpdatePickedUI && allowUIDetection){
			finger.isOverGui = IsScreenPositionOverUI( finger.position );
			finger.pickedUIElement = GetFirstUIElementFromCache();
		}

		// NGui
		if (enabledNGuiMode  && message == EvtType.On_TouchStart){
			finger.isOverGui = finger.isOverGui || IsTouchOverNGui(finger.position);
		}

		// firing event ?
		if ((enableUIMode || enabledNGuiMode)){
			firingEvent = !finger.isOverGui;
		}

		// The new gesture
		Gesture gesture = finger.GetGesture();

		// Auto update picked object
		if (autoUpdatePickedObject && autoSelect){
			if (message != EvtType.On_Drag && message != EvtType.On_DragEnd && message != EvtType.On_DragStart){
				if (GetPickedGameObject(finger)){
					gesture.pickedObject = pickedObject.pickedObj;
					gesture.pickedCamera = pickedObject.pickedCamera;
					gesture.isGuiCamera = pickedObject.isGUI;
				}
				else{
					gesture.pickedObject = null;
					gesture.pickedCamera = null;
					gesture.isGuiCamera = false;
				}
			}
		}

		gesture.swipe = swipe;
		gesture.swipeLength = swipeLength;
		gesture.swipeVector = swipeVector;

		gesture.deltaPinch = 0;
		gesture.twistAngle = 0;


		// Firing event
		if ( firingEvent){
			RaiseEvent(message, gesture);
		}
		else if (finger.isOverGui){
			if (message == EvtType.On_TouchUp){
				RaiseEvent(EvtType.On_UIElementTouchUp, gesture);
			}
			else{
				RaiseEvent(EvtType.On_OverUIElement, gesture);
			}
		}

	}	
	#endregion

	#region Two finger private methods
	private void TwoFinger(){

		bool move=false;
				
		// A touch starts
		if ( twoFinger.currentGesture==GestureType.None){

			if (!singleDoubleTap[99].inDoubleTap){
				singleDoubleTap[99].inDoubleTap = true;
				singleDoubleTap[99].time = 0;
				singleDoubleTap[99].count = 1;
			}

			twoFinger.finger0 = GetTwoFinger(-1);
			twoFinger.finger1 = GetTwoFinger(twoFinger.finger0);
			
			twoFinger.startTimeAction = Time.realtimeSinceStartup;
			twoFinger.currentGesture=GestureType.Acquisition;			

			fingers[twoFinger.finger0].startPosition = fingers[twoFinger.finger0].position;
			fingers[twoFinger.finger1].startPosition = fingers[twoFinger.finger1].position;

			fingers[twoFinger.finger0].oldPosition = fingers[twoFinger.finger0].position;
			fingers[twoFinger.finger1].oldPosition = fingers[twoFinger.finger1].position;
			
			
			twoFinger.oldFingerDistance = Mathf.Abs( Vector2.Distance(fingers[twoFinger.finger0].position, fingers[twoFinger.finger1].position));
			twoFinger.startPosition = new Vector2((fingers[twoFinger.finger0].position.x+fingers[twoFinger.finger1].position.x)/2, (fingers[twoFinger.finger0].position.y+fingers[twoFinger.finger1].position.y)/2);
			twoFinger.position = twoFinger.startPosition;
			twoFinger.oldStartPosition = twoFinger.startPosition;
			twoFinger.deltaPosition = Vector2.zero;
			twoFinger.startDistance = twoFinger.oldFingerDistance;

			// do we touch a pickable gameobject ?
			if (autoSelect){
				if (GetTwoFingerPickedObject()){
					twoFinger.pickedObject = pickedObject.pickedObj;
					twoFinger.pickedCamera = pickedObject.pickedCamera;
					twoFinger.isGuiCamera = pickedObject.isGUI;
				}
				else{
					twoFinger.ClearPickedObjectData();
				}
			}

			// UnityGUI
			if (allowUIDetection){
				if (GetTwoFingerPickedUIElement()){
					twoFinger.pickedUIElement = pickedObject.pickedObj;
					twoFinger.isOverGui = true;
				}
				else{
					twoFinger.ClearPickedUIData();
				}
			}

			// we notify the touch
			CreateGesture2Finger(EvtType.On_TouchStart2Fingers,twoFinger.startPosition,twoFinger.startPosition,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.oldFingerDistance);				
		}

		if (singleDoubleTap[99].inDoubleTap) singleDoubleTap[99].time += Time.deltaTime;

		// Calculates the time since the beginning of the action.
		twoFinger.timeSinceStartAction =  Time.realtimeSinceStartup -twoFinger.startTimeAction;
		
		// Position & deltaPosition
		twoFinger.position = new  Vector2((fingers[twoFinger.finger0].position.x+fingers[twoFinger.finger1].position.x)/2, (fingers[twoFinger.finger0].position.y+fingers[twoFinger.finger1].position.y)/2);
		twoFinger.deltaPosition = twoFinger.position - twoFinger.oldStartPosition;
		twoFinger.fingerDistance = Mathf.Abs(Vector2.Distance(fingers[twoFinger.finger0].position, fingers[twoFinger.finger1].position));
		
		// Cancel
		if (fingers[twoFinger.finger0].phase == TouchPhase.Canceled ||fingers[twoFinger.finger1].phase == TouchPhase.Canceled){
			twoFinger.currentGesture = GestureType.Cancel;
		}

		// Let's go
		if (fingers[twoFinger.finger0].phase != TouchPhase.Ended && fingers[twoFinger.finger1].phase != TouchPhase.Ended && twoFinger.currentGesture != GestureType.Cancel){


			// Are we stationary ?
			if (twoFinger.currentGesture == GestureType.Acquisition && twoFinger.timeSinceStartAction >= longTapTime && FingerInTolerance(fingers[twoFinger.finger0]) && FingerInTolerance(fingers[twoFinger.finger1])){	
				twoFinger.currentGesture = GestureType.LongTap;				
				// we notify the beginning of a longtouch
				CreateGesture2Finger(EvtType.On_LongTapStart2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);				
			}	
			
			// Let's move us ?
			if ( ((FingerInTolerance(fingers[twoFinger.finger0])==false || FingerInTolerance(fingers[twoFinger.finger1])==false) && gesturePriority == GesturePriority.Tap ) 
			|| ((fingers[twoFinger.finger0].phase == TouchPhase.Moved || fingers[twoFinger.finger1].phase == TouchPhase.Moved) && gesturePriority == GesturePriority.Slips) ) {
				move=true;
			}

			// we move
			if (move && twoFinger.currentGesture != GestureType.Tap){
				Vector2 currentDistance = fingers[twoFinger.finger0].position - fingers[twoFinger.finger1].position;
				Vector2 previousDistance = fingers[twoFinger.finger0].oldPosition - fingers[twoFinger.finger1].oldPosition ;
				float currentDelta = currentDistance.magnitude - previousDistance.magnitude;


				#region drag & swipe
				if (enable2FingersSwipe){
					float dot = Vector2.Dot(fingers[twoFinger.finger0].deltaPosition.normalized, fingers[twoFinger.finger1].deltaPosition.normalized);

					if (dot>0 ){

						if (twoFinger.oldGesture == GestureType.LongTap){
							CreateStateEnd2Fingers(twoFinger.currentGesture,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction,false,twoFinger.fingerDistance);
							twoFinger.startTimeAction = Time.realtimeSinceStartup;
						}

						if (twoFinger.pickedObject && !twoFinger.dragStart && !alwaysSendSwipe){

							twoFinger.currentGesture = GestureType.Drag;

							CreateGesture2Finger(EvtType.On_DragStart2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);	
							CreateGesture2Finger(EvtType.On_SwipeStart2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);	
							twoFinger.dragStart = true; 
						}
						else if (!twoFinger.pickedObject && !twoFinger.swipeStart){

							twoFinger.currentGesture = GestureType.Swipe;

							CreateGesture2Finger(EvtType.On_SwipeStart2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);
							twoFinger.swipeStart=true;
						}

					} 
					else{
						if (dot<0){
							twoFinger.dragStart=false; 
							twoFinger.swipeStart=false;
						}
					}

					//
					if (twoFinger.dragStart){
						CreateGesture2Finger(EvtType.On_Drag2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition,twoFinger.position),0,twoFinger.deltaPosition,0,0,twoFinger.fingerDistance);
						CreateGesture2Finger(EvtType.On_Swipe2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition,twoFinger.position),0,twoFinger.deltaPosition,0,0,twoFinger.fingerDistance);
					}
					
					if (twoFinger.swipeStart){
						CreateGesture2Finger(EvtType.On_Swipe2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition,twoFinger.position),0,twoFinger.deltaPosition,0,0,twoFinger.fingerDistance);
					}

				}
				#endregion

				DetectPinch(currentDelta);
				DetecTwist( previousDistance, currentDistance,currentDelta);
			}
			else{
				// Long tap update
				if (twoFinger.currentGesture == GestureType.LongTap){
					CreateGesture2Finger(EvtType.On_LongTap2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);
				}	
			}

			CreateGesture2Finger(EvtType.On_TouchDown2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition,twoFinger.position),0,twoFinger.deltaPosition,0,0,twoFinger.fingerDistance);

			fingers[twoFinger.finger0].oldPosition = fingers[twoFinger.finger0].position;
			fingers[twoFinger.finger1].oldPosition = fingers[twoFinger.finger1].position;

			twoFinger.oldFingerDistance = twoFinger.fingerDistance;
			twoFinger.oldStartPosition = twoFinger.position;
			twoFinger.oldGesture = twoFinger.currentGesture;

		}
		else{			

			if (twoFinger.currentGesture != GestureType.Acquisition && twoFinger.currentGesture!= GestureType.Tap){
				CreateStateEnd2Fingers(twoFinger.currentGesture,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction,true,twoFinger.fingerDistance);
				twoFinger.currentGesture = GestureType.None;
				twoFinger.pickedObject=null;
				twoFinger.swipeStart = false;
				twoFinger.dragStart = false;

			}
			else{
				twoFinger.currentGesture = GestureType.Tap;
				CreateStateEnd2Fingers(twoFinger.currentGesture,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction,true,twoFinger.fingerDistance);
			}
		}	
	}




	private void DetectPinch(float currentDelta){
		#region Pinch
		if (enablePinch){

			if ((Mathf.Abs(twoFinger.fingerDistance - twoFinger.startDistance)>= minPinchLength && twoFinger.currentGesture != GestureType.Pinch) || twoFinger.currentGesture== GestureType.Pinch ){

				if (currentDelta !=0 && twoFinger.oldGesture == GestureType.LongTap){
					CreateStateEnd2Fingers(twoFinger.currentGesture,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction,false,twoFinger.fingerDistance);
					twoFinger.startTimeAction = Time.realtimeSinceStartup;
				}
				
				twoFinger.currentGesture = GestureType.Pinch;
				
				if (currentDelta>0){
					CreateGesture2Finger(EvtType.On_PinchOut,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.startPosition,twoFinger.position),0,Vector2.zero,0,Mathf.Abs(twoFinger.fingerDistance-twoFinger.oldFingerDistance),twoFinger.fingerDistance);
				}
				
				if (currentDelta<0){
					CreateGesture2Finger(EvtType.On_PinchIn,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.startPosition,twoFinger.position),0,Vector2.zero,0,Mathf.Abs(twoFinger.fingerDistance-twoFinger.oldFingerDistance),twoFinger.fingerDistance);
				}
				
				if (currentDelta<0 || currentDelta>0){
					CreateGesture2Finger(EvtType.On_Pinch,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, GetSwipe(twoFinger.startPosition,twoFinger.position),0,Vector2.zero,0,currentDelta,twoFinger.fingerDistance);
				}

			}

			twoFinger.lastPinch = currentDelta>0?currentDelta:twoFinger.lastPinch;
		}
		#endregion
	}
	
	private void DetecTwist(Vector2 previousDistance, Vector2 currentDistance, float currentDelta){
		#region Twist
		if (enableTwist){

			float twistAngle = Vector2.Angle( previousDistance, currentDistance );

			//Debug.Log( twistAngle);
			if (previousDistance == currentDistance)
				twistAngle =0;

			if ( Mathf.Abs(twistAngle)>=minTwistAngle && (twoFinger.currentGesture != GestureType.Twist ) || twoFinger.currentGesture== GestureType.Twist ){
				
				if ( twoFinger.oldGesture == GestureType.LongTap){
					CreateStateEnd2Fingers(twoFinger.currentGesture,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction,false,twoFinger.fingerDistance);
					twoFinger.startTimeAction = Time.realtimeSinceStartup;
				}
				
				twoFinger.currentGesture = GestureType.Twist;
				
				if (twistAngle!=0){
					twistAngle *= Mathf.Sign( Vector3.Cross( previousDistance,  currentDistance).z);
				}
				
				CreateGesture2Finger(EvtType.On_Twist,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,twistAngle,0,twoFinger.fingerDistance);
			}

			twoFinger.lastTwistAngle = twistAngle!=0?twistAngle:twoFinger.lastTwistAngle;
		}			
		#endregion
	}


	private void CreateStateEnd2Fingers(GestureType gesture, Vector2 startPosition, Vector2 position, Vector2 deltaPosition,float time, bool realEnd,float fingerDistance,float twist=0, float pinch=0){


		switch (gesture){
			// Tap
		case GestureType.Tap:
		case GestureType.Acquisition:

			if (doubleTapDetection == DoubleTapDetection.BySystem){

				if (fingers[twoFinger.finger0].tapCount<2 && fingers[twoFinger.finger1].tapCount<2){
					CreateGesture2Finger(EvtType.On_SimpleTap2Fingers,startPosition,position,deltaPosition,
					                     time, SwipeDirection.None,0,Vector2.zero,0,0,fingerDistance);				
				}
				else{
					CreateGesture2Finger(EvtType.On_DoubleTap2Fingers,startPosition,position,deltaPosition,
					                     time, SwipeDirection.None,0,Vector2.zero,0,0,fingerDistance);
				}
				twoFinger.currentGesture = GestureType.None;
				twoFinger.pickedObject=null;
				twoFinger.swipeStart = false;
				twoFinger.dragStart = false;
				singleDoubleTap[99].Stop();
				StopCoroutine( "SingleOrDouble2Fingers");

			}
			else{
				if (!singleDoubleTap[99].inWait){
					StartCoroutine("SingleOrDouble2Fingers" );
				}
				else{
					singleDoubleTap[99].count++;
				}
			}
			break;
			
			// Long tap
		case GestureType.LongTap:
			CreateGesture2Finger(EvtType.On_LongTapEnd2Fingers,startPosition,position,deltaPosition,
			                     time, SwipeDirection.None,0,Vector2.zero,0,0,fingerDistance);
			break;
			
			// Pinch 
		case GestureType.Pinch:
			CreateGesture2Finger(EvtType.On_PinchEnd,startPosition,position,deltaPosition,
			                     time, SwipeDirection.None,0,Vector2.zero,0,twoFinger.lastPinch,fingerDistance);
			break;
			
			// twist
		case GestureType.Twist:
			CreateGesture2Finger(EvtType.On_TwistEnd,startPosition,position,deltaPosition,
			                     time, SwipeDirection.None,0,Vector2.zero,twoFinger.lastTwistAngle,0,fingerDistance);
			break;	
		}
		
		if (realEnd){
			// Drag
			if ( twoFinger.dragStart){
				CreateGesture2Finger(EvtType.On_DragEnd2Fingers,startPosition,position,deltaPosition,
				                     time, GetSwipe( startPosition, position),( position-startPosition).magnitude,position-startPosition,0,0,fingerDistance);
			};
			
			// Swipe
			if ( twoFinger.swipeStart){
				CreateGesture2Finger(EvtType.On_SwipeEnd2Fingers,startPosition,position,deltaPosition,
				                     time, GetSwipe( startPosition, position),( position-startPosition).magnitude,position-startPosition,0,0,fingerDistance);
			}
			
			CreateGesture2Finger(EvtType.On_TouchUp2Fingers,startPosition,position,deltaPosition,time, SwipeDirection.None,0,Vector2.zero,0,0,fingerDistance);
		}
	}

	IEnumerator SingleOrDouble2Fingers(){
		singleDoubleTap[99].inWait = true;

		yield return new WaitForSeconds(doubleTapTime);

		if (singleDoubleTap[99].count <2){

			CreateGesture2Finger(EvtType.On_SimpleTap2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,
			                     twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);

		}
		else{
			CreateGesture2Finger(EvtType.On_DoubleTap2Fingers,twoFinger.startPosition,twoFinger.position,twoFinger.deltaPosition,
			                     twoFinger.timeSinceStartAction, SwipeDirection.None,0,Vector2.zero,0,0,twoFinger.fingerDistance);
		}

		twoFinger.currentGesture = GestureType.None;
		twoFinger.pickedObject=null;
		twoFinger.swipeStart = false;
		twoFinger.dragStart = false;
		singleDoubleTap[99].Stop();
		StopCoroutine( "SingleOrDouble2Fingers");
	}

	private void  CreateGesture2Finger(EvtType message,Vector2 startPosition,Vector2 position,Vector2 deltaPosition,
	                                   float actionTime, SwipeDirection swipe, float swipeLength,Vector2 swipeVector,float twist,float pinch, float twoDistance){

		bool firingEvent = true;
		Gesture gesture = new Gesture();
		gesture.isOverGui = false;

		// NGui
		if (enabledNGuiMode && message == EvtType.On_TouchStart2Fingers){
			gesture.isOverGui = gesture.isOverGui || ( IsTouchOverNGui(twoFinger.position) && IsTouchOverNGui(twoFinger.position));
		}

		gesture.touchCount=2;
		gesture.fingerIndex=-1;
		gesture.startPosition = startPosition;	
		gesture.position = position;
		gesture.deltaPosition = deltaPosition;
		
		gesture.actionTime = actionTime;
		gesture.deltaTime=Time.deltaTime;
		
		gesture.swipe = swipe;
		gesture.swipeLength = swipeLength;
		gesture.swipeVector = swipeVector;
		
		gesture.deltaPinch = pinch;
		gesture.twistAngle = twist;
		gesture.twoFingerDistance = twoDistance;

		gesture.pickedObject = twoFinger.pickedObject;
		gesture.pickedCamera = twoFinger.pickedCamera;
		gesture.isGuiCamera= twoFinger.isGuiCamera;

		if (autoUpdatePickedObject){
			if (message != EvtType.On_Drag && message != EvtType.On_DragEnd && message != EvtType.On_Twist && message != EvtType.On_TwistEnd && message != EvtType.On_Pinch && message != EvtType.On_PinchEnd
			 && message != EvtType.On_PinchIn && message != EvtType.On_PinchOut){

				if (GetTwoFingerPickedObject()){
					gesture.pickedObject = pickedObject.pickedObj;
					gesture.pickedCamera = pickedObject.pickedCamera;
					gesture.isGuiCamera= pickedObject.isGUI;
				}
				else{
					twoFinger.ClearPickedObjectData();
				}
			}
		}

		gesture.pickedUIElement = twoFinger.pickedUIElement;
		gesture.isOverGui = twoFinger.isOverGui;


		if (allowUIDetection && autoUpdatePickedUI){
			if (message != EvtType.On_Drag && message != EvtType.On_DragEnd && message != EvtType.On_Twist && message != EvtType.On_TwistEnd && message != EvtType.On_Pinch && message != EvtType.On_PinchEnd
			    && message != EvtType.On_PinchIn && message != EvtType.On_PinchOut){
				if (message == EvtType.On_SimpleTap2Fingers)

				if (GetTwoFingerPickedUIElement()){
					gesture.pickedUIElement = pickedObject.pickedObj;
					gesture.isOverGui = true;
				}
				else{
					twoFinger.ClearPickedUIData();
				}
			}
		}



		// Firing event ?
		if ((enableUIMode || (enabledNGuiMode && allowUIDetection) ) ){
			firingEvent = !gesture.isOverGui;
		}

		// Firing event
		if ( firingEvent){
			RaiseEvent(message, gesture);
		}
		else if (gesture.isOverGui){
			if (message == EvtType.On_TouchUp2Fingers){
				RaiseEvent(EvtType.On_UIElementTouchUp, gesture);
			}
			else{
				RaiseEvent(EvtType.On_OverUIElement, gesture);
			}
		}
	}

	private int GetTwoFinger( int index){
		
		int i=index+1;
		bool find=false;
		
		while (i<10 && !find){
			if (fingers[i]!=null ){
				if( i>=index){
					find=true;
				}
			}
			i++;
		}
		i--;
		
		return i;
	}

	private bool GetTwoFingerPickedObject(){

		bool returnValue = false;

		if (twoFingerPickMethod == TwoFingerPickMethod.Finger){
			if (GetPickedGameObject(fingers[twoFinger.finger0],false)){
				GameObject tmp = pickedObject.pickedObj;
				if (GetPickedGameObject(fingers[twoFinger.finger1],false)){
					if (tmp == pickedObject.pickedObj){
						returnValue = true;
					}
				}
			}
		}

		else{
			if (GetPickedGameObject(fingers[twoFinger.finger0],true)){
				returnValue = true;
			}
		}

		return returnValue;
	}

	private bool GetTwoFingerPickedUIElement(){

		bool returnValue = false;

		if (fingers[twoFinger.finger0] == null){
			return false;
		}

		if (twoFingerPickMethod == TwoFingerPickMethod.Finger){
			if (IsScreenPositionOverUI( fingers[twoFinger.finger0].position )){
				GameObject tmp = GetFirstUIElementFromCache();

				if (IsScreenPositionOverUI( fingers[twoFinger.finger1].position )){
					GameObject tmp2 = GetFirstUIElementFromCache();

					if (tmp2 == tmp || tmp2.transform.IsChildOf( tmp.transform) || tmp.transform.IsChildOf( tmp2.transform)){
						pickedObject.pickedObj = tmp;
						pickedObject.isGUI = true;
						returnValue = true;
					}
				}
			}
		}
		else{
			if (IsScreenPositionOverUI( twoFinger.position )){
				pickedObject.pickedObj = GetFirstUIElementFromCache();
				pickedObject.isGUI = true;
				returnValue = true;
			}
		}

		return returnValue;
	}

	#endregion

	#region General private methods
	private void RaiseEvent(EvtType evnt, Gesture gesture){
				
		gesture.type = evnt;

		switch(evnt){
			case EvtType.On_Cancel:
				if (On_Cancel!=null)
					On_Cancel( gesture);
				break;
			case EvtType.On_Cancel2Fingers:
				if (On_Cancel2Fingers!=null)
					On_Cancel2Fingers( gesture );
				break;
			case EvtType.On_TouchStart:
				if (On_TouchStart!=null)
					On_TouchStart( gesture);
				break;
			case EvtType.On_TouchDown:
				if (On_TouchDown!=null)
					On_TouchDown( gesture);
				break;
			case EvtType.On_TouchUp:
				if (On_TouchUp!=null)
					On_TouchUp( gesture );
				break;
			case EvtType.On_SimpleTap:
				if (On_SimpleTap!=null)
					On_SimpleTap( gesture);
				break;
			case EvtType.On_DoubleTap:
				if (On_DoubleTap!=null)
					On_DoubleTap(gesture);
				break;
			case EvtType.On_LongTapStart:
				if (On_LongTapStart!=null)
					On_LongTapStart(gesture);
				break;
			case EvtType.On_LongTap:
				if (On_LongTap!=null)
					On_LongTap(gesture);
				break;
			case EvtType.On_LongTapEnd:
				if (On_LongTapEnd!=null)
					On_LongTapEnd(gesture);
				break;
			case EvtType.On_DragStart:
				if (On_DragStart!=null)
					On_DragStart(gesture);
				break;
			case EvtType.On_Drag:
				if (On_Drag!=null)
					On_Drag(gesture);
				break;
			case EvtType.On_DragEnd:
				if (On_DragEnd!=null)
					On_DragEnd(gesture);
				break;
			case EvtType.On_SwipeStart:
				if (On_SwipeStart!=null)
					On_SwipeStart( gesture);
				break;
			case EvtType.On_Swipe:
				if (On_Swipe!=null)
					On_Swipe( gesture);
				break;
			case EvtType.On_SwipeEnd:
				if (On_SwipeEnd!=null)
					On_SwipeEnd(gesture);
				break;
			case EvtType.On_TouchStart2Fingers:
				if (On_TouchStart2Fingers!=null)
					On_TouchStart2Fingers( gesture);
				break;
			case EvtType.On_TouchDown2Fingers:
				if (On_TouchDown2Fingers!=null)
					On_TouchDown2Fingers(gesture);
				break;
			case EvtType.On_TouchUp2Fingers:
				if (On_TouchUp2Fingers!=null)
					On_TouchUp2Fingers(gesture);
				break;
			case EvtType.On_SimpleTap2Fingers:
				if (On_SimpleTap2Fingers!=null)
					On_SimpleTap2Fingers(gesture);
				break;
			case EvtType.On_DoubleTap2Fingers:
				if (On_DoubleTap2Fingers!=null)
					On_DoubleTap2Fingers(gesture);
				break;
			case EvtType.On_LongTapStart2Fingers:
				if (On_LongTapStart2Fingers!=null)
					On_LongTapStart2Fingers(gesture);
				break;
			case EvtType.On_LongTap2Fingers:
				if (On_LongTap2Fingers!=null)
					On_LongTap2Fingers(gesture);
				break;
			case EvtType.On_LongTapEnd2Fingers:
				if (On_LongTapEnd2Fingers!=null)
					On_LongTapEnd2Fingers(gesture);
				break;
			case EvtType.On_Twist:
				if (On_Twist!=null)
					On_Twist(gesture);
				break;
			case EvtType.On_TwistEnd:
				if (On_TwistEnd!=null)
					On_TwistEnd(gesture);
				break;
			case EvtType.On_Pinch:
				if (On_Pinch!=null)
					On_Pinch(gesture);
				break;
			case EvtType.On_PinchIn:
				if (On_PinchIn!=null)
					On_PinchIn(gesture);
				break;
			case EvtType.On_PinchOut:
				if (On_PinchOut!=null)
					On_PinchOut(gesture);
				break;
			case EvtType.On_PinchEnd:
				if (On_PinchEnd!=null)
					On_PinchEnd(gesture);
				break;
			case EvtType.On_DragStart2Fingers:
				if (On_DragStart2Fingers!=null)
					On_DragStart2Fingers(gesture);
				break;
			case EvtType.On_Drag2Fingers:
				if (On_Drag2Fingers!=null)
					On_Drag2Fingers(gesture);
				break;
			case EvtType.On_DragEnd2Fingers:
				if (On_DragEnd2Fingers!=null)
					On_DragEnd2Fingers(gesture);
				break;
			case EvtType.On_SwipeStart2Fingers:
				if (On_SwipeStart2Fingers!=null)
					On_SwipeStart2Fingers(gesture);
				break;
			case EvtType.On_Swipe2Fingers:
				if (On_Swipe2Fingers!=null)
					On_Swipe2Fingers(gesture);
				break;
			case EvtType.On_SwipeEnd2Fingers:
				if (On_SwipeEnd2Fingers!=null)
					On_SwipeEnd2Fingers(gesture);
				break;
			case EvtType.On_OverUIElement:
				if (On_OverUIElement!=null){
					On_OverUIElement(gesture);
				}
				break;
			case EvtType.On_UIElementTouchUp:
				if (On_UIElementTouchUp!=null){
					On_UIElementTouchUp( gesture);
				}
				break;
		}

		// Direct Acces 
		int result = _currentGestures.FindIndex( delegate(Gesture obj) {
			return obj!=null && obj.type == gesture.type && obj.fingerIndex == gesture.fingerIndex;
		}
		);

		if (result>-1){
			_currentGestures[result].touchCount = gesture.touchCount;
			_currentGestures[result].position = gesture.position;
			_currentGestures[result].actionTime = gesture.actionTime;
			_currentGestures[result].pickedCamera = gesture.pickedCamera;
			_currentGestures[result].pickedObject = gesture.pickedObject;
			_currentGestures[result].pickedUIElement = gesture.pickedUIElement;
			_currentGestures[result].isOverGui = gesture.isOverGui;
			_currentGestures[result].isGuiCamera = gesture.isGuiCamera;

			// Update delta from current
			_currentGestures[result].deltaPinch += gesture.deltaPinch;
			_currentGestures[result].deltaPosition += gesture.deltaPosition;

			_currentGestures[result].deltaTime += gesture.deltaTime;
			_currentGestures[result].twistAngle += gesture.twistAngle;
		}

		if (result==-1 ){
			_currentGestures.Add( (Gesture)gesture.Clone());
			if (_currentGestures.Count>0){
				_currentGesture = _currentGestures[0];
			}
		}


	}

	private bool GetPickedGameObject(Finger finger, bool isTowFinger=false){

		if (finger == null && !isTowFinger){
			return false;
		}

		pickedObject.isGUI = false;
		pickedObject.pickedObj = null;
		pickedObject.pickedCamera = null;
		
		if (touchCameras.Count>0){
			for (int i=0;i<touchCameras.Count;i++){
				if (touchCameras[i].camera!=null && touchCameras[i].camera.enabled){

					Vector2 pos=Vector2.zero;
					if (!isTowFinger){
						pos = finger.position;
					}
					else{
						pos = twoFinger.position;
					}

					
					if (GetGameObjectAt( pos, touchCameras[i].camera,touchCameras[i].guiCamera)){
							return true;
						}
					/*
			        Ray ray = touchCameras[i].camera.ScreenPointToRay( pos );
			        RaycastHit hit;

					if (enable2D){

						LayerMask mask2d = pickableLayers2D;
						RaycastHit2D[] hit2D = new RaycastHit2D[1];
						if (Physics2D.GetRayIntersectionNonAlloc( ray,hit2D,float.PositiveInfinity,mask2d)>0){
							pickedObject.pickedCamera = touchCameras[i].camera;
							pickedObject.isGUI = touchCameras[i].guiCamera;
							pickedObject.pickedObj = hit2D[0].collider.gameObject;
							return true;
						}
					}

					LayerMask mask = pickableLayers3D;
						
			        if( Physics.Raycast( ray, out hit,float.MaxValue,mask ) ){
						pickedObject.pickedCamera = touchCameras[i].camera;
						pickedObject.isGUI = touchCameras[i].guiCamera;
						pickedObject.pickedObj = hit.collider.gameObject;
			            return true;
					}*/
				}
			}
		}
		else{
			Debug.LogWarning("No camera is assigned to EasyTouch");	
		}
        return false;     
	}

	private bool GetGameObjectAt(Vector2 position, Camera cam, bool isGuiCam){

		Ray ray = cam.ScreenPointToRay( position );
		RaycastHit hit;

		if (enable2D){
			
			LayerMask mask2d = pickableLayers2D;
			RaycastHit2D[] hit2D = new RaycastHit2D[1];
			if (Physics2D.GetRayIntersectionNonAlloc( ray,hit2D,float.PositiveInfinity,mask2d)>0){
				pickedObject.pickedCamera = cam;
				pickedObject.isGUI = isGuiCam;
				pickedObject.pickedObj = hit2D[0].collider.gameObject;
				return true;
			}
		}
		
		LayerMask mask = pickableLayers3D;
		
		if( Physics.Raycast( ray, out hit,float.MaxValue,mask ) ){
			pickedObject.pickedCamera = cam;
			pickedObject.isGUI =isGuiCam;
			pickedObject.pickedObj = hit.collider.gameObject;
			return true;
		}

		return false;
	}

	private SwipeDirection GetSwipe(Vector2 start, Vector2 end){
		
		Vector2 linear;
		linear = (end - start).normalized;

		if ( Vector2.Dot( linear, Vector2.up) >= swipeTolerance)
			return SwipeDirection.Up;

		if ( Vector2.Dot( linear, -Vector2.up) >= swipeTolerance)
			return SwipeDirection.Down;

		if ( Vector2.Dot( linear,  Vector2.right) >= swipeTolerance)
			return SwipeDirection.Right;

		if ( Vector2.Dot( linear,  -Vector2.right) >= swipeTolerance)
			return SwipeDirection.Left;

		if ( Vector2.Dot( linear, new Vector2(0.5f,0.5f).normalized) >= swipeTolerance)
			return SwipeDirection.UpRight;

		if ( Vector2.Dot( linear, new Vector2(0.5f,-0.5f).normalized) >= swipeTolerance)
			return SwipeDirection.DownRight;
			
		if ( Vector2.Dot( linear, new Vector2(-0.5f,0.5f).normalized) >= swipeTolerance)
			return SwipeDirection.UpLeft;

		if ( Vector2.Dot( linear, new Vector2(-0.5f,-0.5f).normalized) >= swipeTolerance)
			return SwipeDirection.DownLeft;

		return SwipeDirection.Other;			
	}

	private bool FingerInTolerance(Finger finger ){

		if ((finger.position-finger.startPosition).sqrMagnitude <= (StationaryTolerance*StationaryTolerance)){
			return true;
		}
		else{
			return false;
		}
	}

	private bool IsTouchOverNGui(Vector2 position, bool isTwoFingers=false){
		
		bool returnValue = false;
		
		if (enabledNGuiMode){
			
			LayerMask mask= nGUILayers;
			RaycastHit hit;
								
			int i=0;
			while (!returnValue && i<nGUICameras.Count){
				Vector2 pos = Vector2.zero;
				if (!isTwoFingers){
					pos = position;//fingers[touchIndex].position;
				}
				else{
					pos = twoFinger.position;
				}
				Ray ray = nGUICameras[i].ScreenPointToRay( pos );

				returnValue =  Physics.Raycast( ray, out hit,float.MaxValue,mask );
				i++;
			}

		}

		return returnValue;
	
	}

	private Finger GetFinger(int finderId){
		int t=0;
		
		Finger fing=null;
		
		while (t < 10 && fing==null){	
			if (fingers[t] != null ){
				if ( fingers[t].fingerIndex == finderId){				
					fing = fingers[t];		
				}
			}
			t++;	
		}	
		
		return fing;
	}
	#endregion

	#region Unity UI
	private bool IsScreenPositionOverUI( Vector2 position){

		uiEventSystem = EventSystem.current;
		if (uiEventSystem != null){

			uiPointerEventData = new PointerEventData( uiEventSystem);
			uiPointerEventData.position = position;

			uiEventSystem.RaycastAll( uiPointerEventData, uiRaycastResultCache);
			if (uiRaycastResultCache.Count>0){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			return false;
		}
	}

	private GameObject GetFirstUIElementFromCache(){

		if (uiRaycastResultCache.Count>0){
			return uiRaycastResultCache[0].gameObject;
		}
		else{
			return null;
		}
	}
	                                               
	private GameObject GetFirstUIElement( Vector2 position){

		if (IsScreenPositionOverUI( position)){
			return GetFirstUIElementFromCache();
		}
		else{
			return null;
		}
	}
	#endregion

	#region Static Method
	// Unity UI compatibility
	public static bool IsFingerOverUIElement( int fingerIndex){
		if (EasyTouch.instance!=null){
			Finger finger = EasyTouch.instance.GetFinger(fingerIndex);
			if (finger != null){
				return EasyTouch.instance.IsScreenPositionOverUI( finger.position);
			}
			else{
				return false;
			}
		}
		else{
			return false;
		}
	}
	
	public static GameObject GetCurrentPickedUIElement( int fingerIndex,bool isTwoFinger){
		if (EasyTouch.instance!=null){
			Finger finger = EasyTouch.instance.GetFinger(fingerIndex);
			if (finger != null || isTwoFinger){
				Vector2 pos = Vector2.zero;
				if (!isTwoFinger){
					pos = finger.position;
				}
				else{
					pos = EasyTouch.instance.twoFinger.position;
				}
				return EasyTouch.instance.GetFirstUIElement( pos);
			}
			else{
				return null;
			}
		}
		else{
			return null;
		}
	}

	public static GameObject GetCurrentPickedObject(int fingerIndex, bool isTwoFinger){

		if (EasyTouch.instance!=null){
			Finger finger = EasyTouch.instance.GetFinger(fingerIndex);

			if ((finger!=null || isTwoFinger) && EasyTouch.instance.GetPickedGameObject(finger,isTwoFinger)){
				return EasyTouch.instance.pickedObject.pickedObj;
			}
			else{
				return null;
			}
		}
		else{
			return null;
		}
		
	}

	public static GameObject GetGameObjectAt( Vector2 position, bool isTwoFinger = false){


		if (EasyTouch.instance!=null){

			if (isTwoFinger) position = EasyTouch.instance.twoFinger.position;
			if (EasyTouch.instance.touchCameras.Count>0){
				for (int i=0;i<EasyTouch.instance.touchCameras.Count;i++){
					if (EasyTouch.instance.touchCameras[i].camera!=null && EasyTouch.instance.touchCameras[i].camera.enabled){
						if( EasyTouch.instance.GetGameObjectAt( position,EasyTouch.instance.touchCameras[i].camera,EasyTouch.instance.touchCameras[i].guiCamera)){
							return EasyTouch.instance.pickedObject.pickedObj;
						}
						else{
							return null;
						}
					}
				}
			}
		}

		return null;
	}


	public static int GetTouchCount(){
		if (EasyTouch.instance){
			return EasyTouch.instance.input.TouchCount();
		}
		else{
			return 0;
		}
	}

	public static void ResetTouch(int fingerIndex){
		if (EasyTouch.instance)
			EasyTouch.instance.GetFinger(fingerIndex).gesture=GestureType.None;
	}
	

	public static void SetEnabled( bool enable){
		EasyTouch.instance.enable = enable;
		if (enable){
			EasyTouch.instance.ResetTouches();	
		}
	}
	public static bool GetEnabled(){
		if (EasyTouch.instance)
			return EasyTouch.instance.enable;
		else
			return false;
	}

	public static void SetEnableUIDetection( bool enable){
		if (EasyTouch.instance != null){
			EasyTouch.instance.allowUIDetection = enable;
		}
	}
	public static bool GetEnableUIDetection(){
		if (EasyTouch.instance){
			return EasyTouch.instance.allowUIDetection;
		}
		else{
			return false;
		}
	}

	public static void SetUICompatibily(bool value){
		if (EasyTouch.instance != null){
			EasyTouch.instance.enableUIMode = value;
		}
	}
	public static bool GetUIComptability(){
		if (EasyTouch.instance != null){
			return EasyTouch.instance.enableUIMode;
		}
		else{
			return false;
		}
	}
	
	public static void SetAutoUpdateUI(bool value){
		if (EasyTouch.instance)
			EasyTouch.instance.autoUpdatePickedUI = value;
	}
	public static bool GetAutoUpdateUI(){
		if (EasyTouch.instance)
			return EasyTouch.instance.autoUpdatePickedUI;
		else
			return false;
	}
	
	public static void SetNGUICompatibility(bool value){
		if (EasyTouch.instance)
			EasyTouch.instance.enabledNGuiMode = value;
	}
	public static bool GetNGUICompatibility(){
		if (EasyTouch.instance)
			return EasyTouch.instance.enabledNGuiMode;
		else
			return false;
	}
	

	public static void SetEnableAutoSelect( bool value){
		if (EasyTouch.instance)
			EasyTouch.instance.autoSelect = value;
	}
	public static bool GetEnableAutoSelect(){
		if (EasyTouch.instance)
			return EasyTouch.instance.autoSelect;
		else
			return false;
	}

	public static void SetAutoUpdatePickedObject(bool value){
		if (EasyTouch.instance)
			EasyTouch.instance.autoUpdatePickedObject = value;
	}
	public static bool GetAutoUpdatePickedObject(){
		if (EasyTouch.instance)
			return EasyTouch.instance.autoUpdatePickedObject;
		else
			return false;
	}

	public static void Set3DPickableLayer(LayerMask mask){
		if (EasyTouch.instance)
			EasyTouch.instance.pickableLayers3D = mask;	
	}
	public static LayerMask Get3DPickableLayer(){
		if (EasyTouch.instance)
			return EasyTouch.instance.pickableLayers3D;	
		else
			return LayerMask.GetMask("Default");
	}

	public static void AddCamera(Camera cam,bool guiCam=false){
		if (EasyTouch.instance)
			EasyTouch.instance.touchCameras.Add(new ECamera(cam,guiCam));
	}
	public static void RemoveCamera( Camera cam){
		if (EasyTouch.instance){

			int result = EasyTouch.instance.touchCameras.FindIndex(
				delegate( ECamera c){
				return c.camera == cam;
			}
			);

			if (result>-1){
				EasyTouch.instance.touchCameras[result]=null;
				EasyTouch.instance.touchCameras.RemoveAt( result);

			}

		}

	}

	public static Camera GetCamera(int index=0){
		if (EasyTouch.instance){
			if (index< EasyTouch.instance.touchCameras.Count){
				return EasyTouch.instance.touchCameras[index].camera;	
			}
			else{
				return null;	
			}
		}
		else{
			return null;
		}
	}

	public static void SetEnable2DCollider(bool value){
		if (EasyTouch.instance)
			EasyTouch.instance.enable2D = value;
	}
	public static bool GetEnable2DCollider(){
		if (EasyTouch.instance)
			return EasyTouch.instance.enable2D;
		else
			return false;
	}

	public static void Set2DPickableLayer(LayerMask mask){
		if (EasyTouch.instance)
			EasyTouch.instance.pickableLayers2D = mask;	
	}
	public static LayerMask Get2DPickableLayer(){
		if (EasyTouch.instance)
			return EasyTouch.instance.pickableLayers2D;	
		else
			return LayerMask.GetMask("Default");
	}


	public static void SetGesturePriority( GesturePriority value){
		if (EasyTouch.instance)
			EasyTouch.instance.gesturePriority = value;
	}
	public static GesturePriority GetGesturePriority(){
		if (EasyTouch.instance)
			return EasyTouch.instance.gesturePriority;
		else
			return GesturePriority.Tap;
	}

	public static void SetStationaryTolerance(float tolerance){
		if (EasyTouch.instance)
			EasyTouch.instance.StationaryTolerance = tolerance;
	}
	public static float GetStationaryTolerance(){
		if (EasyTouch.instance)
			return EasyTouch.instance.StationaryTolerance;
		else
			return -1;
	}
	
	public static void SetLongTapTime(float time){
		if (EasyTouch.instance)
			EasyTouch.instance.longTapTime = time;
	}
	public static float GetlongTapTime(){
		if (EasyTouch.instance)
			return EasyTouch.instance.longTapTime;
		else
			return -1;
	}

	public static void SetDoubleTapTime(float time){
		if (EasyTouch.instance)
			EasyTouch.instance.doubleTapTime = time;
	}
	public static float GetDoubleTapTime(){
		if (EasyTouch.instance)
			return EasyTouch.instance.doubleTapTime;
		else
			return -1;
	}

	public static void SetDoubleTapMethod(DoubleTapDetection detection){
		if (EasyTouch.instance)
			EasyTouch.instance.doubleTapDetection = detection;
	}
	public static EasyTouch.DoubleTapDetection GetDoubleTapMethod(){
		if (EasyTouch.instance)
			return EasyTouch.instance.doubleTapDetection;
		else
			return EasyTouch.DoubleTapDetection.BySystem;
	}

	public static void SetSwipeTolerance( float tolerance){
		if (EasyTouch.instance)
			EasyTouch.instance.swipeTolerance = tolerance;
	}
	public static float GetSwipeTolerance(){
		if (EasyTouch.instance)
			return EasyTouch.instance.swipeTolerance;
		else
			return -1;
	}
	

	public static void SetEnable2FingersGesture( bool enable){
		if (EasyTouch.instance)
			EasyTouch.instance.enable2FingersGesture = enable;
	}
	public static bool GetEnable2FingersGesture(){
		if (EasyTouch.instance)
			return EasyTouch.instance.enable2FingersGesture;
		else
			return false;
	}

	public static void SetTwoFingerPickMethod( EasyTouch.TwoFingerPickMethod pickMethod){
		if (EasyTouch.instance)
			EasyTouch.instance.twoFingerPickMethod = pickMethod;
	}
	public static EasyTouch.TwoFingerPickMethod GetTwoFingerPickMethod(){
		if (EasyTouch.instance)
			return EasyTouch.instance.twoFingerPickMethod;
		else
			return EasyTouch.TwoFingerPickMethod.Finger;
	}

	public static void SetEnablePinch( bool enable){
		if (EasyTouch.instance)
			EasyTouch.instance.enablePinch = enable;
	}
	public static bool GetEnablePinch(){
		if (EasyTouch.instance)
			return EasyTouch.instance.enablePinch;
		else
			return false;
	}

	public static void SetMinPinchLength(float length){
		if (EasyTouch.instance)
			EasyTouch.instance.minPinchLength=length;
	}
	public static float GetMinPinchLength(){
		if (EasyTouch.instance)
			return EasyTouch.instance.minPinchLength;
		else
			return -1;
	}

	public static void SetEnableTwist( bool enable){
		if (EasyTouch.instance)
			EasyTouch.instance.enableTwist = enable;
	}
	public static bool GetEnableTwist(){
		if (EasyTouch.instance)
			return EasyTouch.instance.enableTwist;
		else
			return false;
	}

	public static void SetMinTwistAngle(float angle){
		if (EasyTouch.instance)
			EasyTouch.instance.minTwistAngle = angle;
	}
	public static float GetMinTwistAngle(){
		if (EasyTouch.instance)
			return EasyTouch.instance.minTwistAngle;
		else 
			return -1;
	}

	public static bool GetSecondeFingerSimulation(){
		
		if (EasyTouch.instance != null){
			return EasyTouch.instance.enableSimulation;
		}
		else{
			return false;
		}
	}
	public static void SetSecondFingerSimulation(bool value){
		if (EasyTouch.instance != null){
			EasyTouch.instance.enableSimulation = value;
		}
	}
	#endregion
	
}
}