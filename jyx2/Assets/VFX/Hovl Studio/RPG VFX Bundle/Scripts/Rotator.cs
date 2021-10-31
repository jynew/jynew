using System.Collections;
using UnityEngine;

public class RPGRotator : MonoBehaviour
{
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;
	void OnEnable()
    {
		InvokeRepeating("Rotate", 0f, 0.0167f);
	}
	void OnDisable()
    {
		CancelInvoke();
	}
	void Rotate()
    {
		this.transform.localEulerAngles += new Vector3(x,y,z);
	}
}
