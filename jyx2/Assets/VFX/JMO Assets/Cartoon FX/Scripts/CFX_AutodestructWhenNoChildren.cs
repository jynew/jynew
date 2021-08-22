using UnityEngine;
using System.Collections;

// Cartoon FX  - (c) 2013, Jean Moreno

// Automatically destroys the GameObject when there are no children left.

public class CFX_AutodestructWhenNoChildren : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
		if( transform.childCount == 0)
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}
