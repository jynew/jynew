using UnityEngine;
using System.Collections;

public class CFX_Demo_RandomDir : MonoBehaviour
{
	public Vector3 min = new Vector3(0,0,0);
	public Vector3 max = new Vector3(0,360,0);
	
	void Awake ()
	{
		this.transform.eulerAngles = new Vector3(Random.Range(min.x,max.x),Random.Range(min.y,max.y),Random.Range(min.z,max.z));
	}
	
}
