using UnityEngine;
using System.Collections;

public class Delay01 : MonoBehaviour {
	
	public float delayTime = 1.0f;
	
	// Use this for initialization
	void Start () {		
		gameObject.SetActiveRecursively(false);
		Invoke("DelayFunc", delayTime);
	}
	
	void DelayFunc()
	{
		gameObject.SetActiveRecursively(true);
	}
	
}
