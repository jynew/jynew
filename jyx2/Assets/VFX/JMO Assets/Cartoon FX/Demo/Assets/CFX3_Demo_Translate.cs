using UnityEngine;
using System.Collections;

// Cartoon FX  - (c) 2013, Jean Moreno

public class CFX3_Demo_Translate : MonoBehaviour
{
	public float speed = 30.0f;
	public Vector3 rotation = Vector3.forward;
	public Vector3 axis = Vector3.forward;
	public bool gravity;
	private Vector3 dir;
	
	void Start ()
	{
		dir = new Vector3(Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f));
		dir.Scale(rotation);
		this.transform.localEulerAngles = dir;
	}
	
	void Update ()
	{
		this.transform.Translate(axis * speed * Time.deltaTime, Space.Self);
	}
}
