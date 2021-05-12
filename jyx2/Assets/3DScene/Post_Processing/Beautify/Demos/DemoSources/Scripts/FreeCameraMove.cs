using UnityEngine;
using System.Collections;

namespace BeautifyEffect
{
	public class FreeCameraMove : MonoBehaviour
	{
		public float cameraSensitivity = 150;
		public float climbSpeed = 20;
		public float normalMoveSpeed = 20;
		public float slowMoveFactor = 0.25f;
		public float fastMoveFactor = 3;

		float rotationX = 0.0f;
		float rotationY = 0.0f;
		Quaternion originalRotation;

		void Start ()
		{
			Cursor.lockState = CursorLockMode.Locked;
			originalRotation = Camera.main.transform.rotation;
		}

		void Update ()
		{
			Vector2 mousePos = Input.mousePosition;
			if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
				return;

			rotationX += Input.GetAxis ("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis ("Mouse Y") * cameraSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
			
			transform.localRotation = Quaternion.AngleAxis (rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis (rotationY, Vector3.left);
			transform.localRotation *= originalRotation;
			
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis ("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis ("Horizontal") * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis ("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis ("Horizontal") * Time.deltaTime;
			} else {
				transform.position += transform.forward * normalMoveSpeed * Input.GetAxis ("Vertical") * Time.deltaTime;
				transform.position += transform.right * normalMoveSpeed * Input.GetAxis ("Horizontal") * Time.deltaTime;
			}
			
			
			if (Input.GetKey (KeyCode.Q)) {
				transform.position -= transform.up * climbSpeed * Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.E)) {
				transform.position += transform.up * climbSpeed * Time.deltaTime;
			}

			if (transform.position.y<1) transform.position += Vector3.up * (1- transform.position.y);
			
		}

	}
}