/*
	TileWorldCreator
	Simple camera navigation script
*/

using UnityEngine;
using System.Collections;


public class CameraNavigation : MonoBehaviour {
	
	bool screenPanning = false;
	
	public GameObject target;
	
	//panning
	public float mouseSensitivity = 0.1f;
	Vector3 lastPosition = new Vector3();
	public float maxDistance = 20f;
	
	
	//zooming
	public float scrollSpeed = 0.05f;
	public int defaultScroll = 20;
	public int minScroll = 5;
	public int maxScroll = 30;
	float distance = 0f;
	
	
	void Start()
	{
		distance = Mathf.Clamp(distance, defaultScroll, maxScroll);
	}
	
	void Update () {
		
		// PANNING
		// Hold middle Mouse Button to pan the screen in a direction
		if(Input.GetMouseButtonDown(2)) 
		{
			screenPanning = true;
			lastPosition = Input.mousePosition;	
		}
		
		//If panning, find the angle to pan based on camera angle not screen
		if(screenPanning==true) 
		{
			if(Input.GetMouseButtonUp(2))
			{
				screenPanning = false;
			}
			
			var delta = Input.mousePosition - lastPosition;
			
			
			target.transform.Translate((-(delta.x * (mouseSensitivity / 2))), 0, (-(delta.y * (mouseSensitivity / 2))));
			
			
			target.transform.localPosition = new Vector3(Mathf.Clamp(target.transform.localPosition.x, -maxDistance,maxDistance), Mathf.Clamp(target.transform.localPosition.y, -maxDistance,maxDistance+5), Mathf.Clamp(target.transform.localPosition.z, -maxDistance,maxDistance));  //without this the code works fine but I want to set boundaries for the actual map.
			
			
			lastPosition = Input.mousePosition;
				
		}
		
		//scrolling
		////////////////
		distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
		distance = Mathf.Clamp(distance, minScroll, maxScroll);
		
	
		target.transform.position = new Vector3(target.transform.position.x,distance,target.transform.position.z);	
		
		
		mouseSensitivity = (distance * 0.1f) * 0.1f;

	}
	
	
}
