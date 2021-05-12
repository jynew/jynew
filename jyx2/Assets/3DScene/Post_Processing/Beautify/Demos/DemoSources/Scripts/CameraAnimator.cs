using UnityEngine;
using System.Collections;

namespace BeautifyEffect
{
	public class CameraAnimator : MonoBehaviour
	{

	
		// Update is called once per frame
		void Update ()
		{
			transform.Rotate(new Vector3(0,0, Time.deltaTime * 10f));
	
		}
	}
}