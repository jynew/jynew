using UnityEngine;
using UnityEngine.UI;

namespace BeautifyEffect
{
	public class Demo4 : MonoBehaviour
	{
		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.T)) {
				Beautify.instance.depthOfFieldTransparencySupport = !Beautify.instance.depthOfFieldTransparencySupport;
				GameObject.Find("Beautify").GetComponent<Text>().text = Beautify.instance.depthOfFieldTransparencySupport ? "Beautify (DoF transparency support is ON)" : "Beautify (Dof transparency support is OFF)";
			}
	
		}
	}

}