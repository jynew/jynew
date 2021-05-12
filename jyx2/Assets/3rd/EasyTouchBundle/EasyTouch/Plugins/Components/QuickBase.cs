/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

namespace HedgehogTeam.EasyTouch{
public class QuickBase : MonoBehaviour {

	#region enumeration
	protected enum GameObjectType { Auto,Obj_3D,Obj_2D,UI};

	public enum DirectAction {None,Rotate, RotateLocal,Translate, TranslateLocal, Scale};
	public enum AffectedAxesAction {X,Y,Z,XY,XZ,YZ,XYZ};
	#endregion

	#region Members
	public string quickActionName;

	// Touch management
	public bool isMultiTouch = false;
	public bool is2Finger = false;
	public bool isOnTouch=false;
	public bool enablePickOverUI = false;
	public bool resetPhysic = false;

	// simple Action
	public DirectAction directAction;
	public AffectedAxesAction axesAction;
	public float sensibility = 1;
	public CharacterController directCharacterController;
	public bool inverseAxisValue = false;

		
	protected Rigidbody cachedRigidBody;
	protected bool isKinematic;
	
	protected Rigidbody2D cachedRigidBody2D;
	protected bool isKinematic2D;

	// internal management
	protected GameObjectType realType;
	protected int fingerIndex =-1;
	#endregion

	#region Monobehavior Callback
	void Awake(){
		cachedRigidBody = GetComponent<Rigidbody>();
		if (cachedRigidBody){
			isKinematic = cachedRigidBody.isKinematic;
		}
		
		cachedRigidBody2D = GetComponent<Rigidbody2D>();
		if (cachedRigidBody2D){
			isKinematic2D = cachedRigidBody2D.isKinematic;
		}

	}

	public virtual void Start(){

		EasyTouch.SetEnableAutoSelect( true);

		realType = GameObjectType.Obj_3D;

		if (GetComponent<Collider>()){
			realType = GameObjectType.Obj_3D;
		}
		else if (GetComponent<Collider2D>()){
			realType = GameObjectType.Obj_2D;
		}
		else if (GetComponent<CanvasRenderer>()){
			realType = GameObjectType.UI;
		}


		switch (realType){

		case GameObjectType.Obj_3D:
			LayerMask mask = EasyTouch.Get3DPickableLayer();
			mask = mask | 1<<gameObject.layer;
			EasyTouch.Set3DPickableLayer( mask);
			break;
			//2D
		case GameObjectType.Obj_2D:
			EasyTouch.SetEnable2DCollider( true);
			mask = EasyTouch.Get2DPickableLayer();
			mask = mask | 1<<gameObject.layer;
			EasyTouch.Set2DPickableLayer( mask);
			break;
			// UI
		case GameObjectType.UI:
			EasyTouch.instance.enableUIMode = true;
			EasyTouch.SetUICompatibily( false);
			break;
		}
		
		if (enablePickOverUI){
			EasyTouch.instance.enableUIMode = true;
			EasyTouch.SetUICompatibily( false);
		}
		
	}

	public virtual void OnEnable(){
		//QuickTouchManager.instance.RegisterQuickAction( this);
	}

	public virtual void OnDisable(){
		//if (QuickTouchManager._instance){
		//	QuickTouchManager.instance.UnregisterQuickAction( this);
		//}
	}
	 #endregion                      

	#region Protected Methods
	protected Vector3 GetInfluencedAxis(){
		
		Vector3 axis = Vector3.zero;
		
		switch(axesAction){
		case AffectedAxesAction.X:
			axis = new Vector3(1,0,0);
			break;
		case AffectedAxesAction.Y:
			axis = new Vector3(0,1,0);
			break;
		case AffectedAxesAction.Z:
			axis = new Vector3(0,0,1);
			break;
		case AffectedAxesAction.XY:
			axis = new Vector3(1,1,0);
			break;
		case AffectedAxesAction.XYZ:
			axis = new Vector3(1,1,1);
			break;
		case AffectedAxesAction.XZ:
			axis = new Vector3(1,0,1);
			break;
		case AffectedAxesAction.YZ:
			axis = new Vector3(0,1,1);
			break;
		}	
		
		return axis;
	}

	protected void DoDirectAction(float value){
		
	
		Vector3 localAxis = GetInfluencedAxis();
		
		switch (  directAction){
		// Rotate
		case DirectAction.Rotate:
			transform.Rotate( localAxis *  value, Space.World);
			break;
		// Rotate Local
		case DirectAction.RotateLocal:
			transform.Rotate( localAxis *  value,Space.Self);
			break;
		// Translate
		case DirectAction.Translate:
			if ( directCharacterController==null){
				transform.Translate(localAxis *  value,Space.World);
			}
			else{
				Vector3 direction = localAxis *  value;
				directCharacterController.Move( direction  );
			}
			break;
			
		// Translate local
		case DirectAction.TranslateLocal:
			if ( directCharacterController==null){
				transform.Translate(localAxis *  value,Space.Self);
			}
			else{
				Vector3 direction =  directCharacterController.transform.TransformDirection(localAxis) *  value;
				directCharacterController.Move( direction );
			}
			break;	
		// Scale
		case DirectAction.Scale:
			transform.localScale +=  localAxis *  value;
			break;

		/*
		// Force
		case DirectAction.Force:
			if (directRigidBody!=null){
				directRigidBody.AddForce( localAxis * axisValue * speed);
			}
			break;
		// Relative force
		case DirectAction.RelativeForce:
			if (directRigidBody!=null){
				directRigidBody.AddRelativeForce( localAxis * axisValue * speed);
			}
			break;
		// Torque
		case DirectAction.Torque:
			if (directRigidBody!=null){
				directRigidBody.AddTorque(localAxis * axisValue * speed);
			}

			break;
		// Relative torque
		case DirectAction.RelativeTorque:
			if (directRigidBody!=null){
				directRigidBody.AddRelativeTorque(localAxis * axisValue * speed);
			}
			break;*/
		}

	}
	#endregion

	#region Public Methods
	public void EnabledQuickComponent(string quickActionName){

		QuickBase[] quickBases = GetComponents<QuickBase>();
		foreach( QuickBase qb in quickBases){
			if (qb.quickActionName == quickActionName){
				qb.enabled = true;
			}
		}

	}

	public void DisabledQuickComponent(string quickActionName){

		QuickBase[] quickBases = GetComponents<QuickBase>();
		foreach( QuickBase qb in quickBases){
			if (qb.quickActionName == quickActionName){
				qb.enabled = false;
			}
		}	
			
	}

	public void DisabledAllSwipeExcepted(string quickActionName){

		QuickSwipe[] swipes = FindObjectsOfType(typeof(QuickSwipe)) as QuickSwipe[];
		foreach( QuickSwipe swipe in swipes){
			if (swipe.quickActionName != quickActionName || ( swipe.quickActionName == quickActionName && swipe.gameObject != gameObject)){
				swipe.enabled = false;
			}
		}
	}	
	#endregion
}
}
