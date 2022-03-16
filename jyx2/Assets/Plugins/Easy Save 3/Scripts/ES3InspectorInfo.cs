using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 	Displays an info message in the inspector.
 * 	Only available in the Editor.
 */
public class ES3InspectorInfo : MonoBehaviour 
{
	#if UNITY_EDITOR
	public string message = "";

	public void SetMessage(string message)
	{
		this.message = message;
	}
	#endif
}