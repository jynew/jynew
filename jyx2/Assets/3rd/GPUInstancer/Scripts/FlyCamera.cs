using UnityEngine;

namespace GPUInstancer
{
    // Inspired by http://forum.unity3d.com/threads/fly-cam-simple-cam-script.67042/
    public class FlyCamera : MonoBehaviour
    {
        public float mainSpeed = 10.0f;
        public float shiftSpeed = 30.0f;
        public float rotationSpeed = 5.0f;

        private Vector3 _inputVector;
        private Vector3 _rotationEuler;

        void Start()
        {
            _inputVector = Vector3.zero;
            _rotationEuler = transform.rotation.eulerAngles;
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                _rotationEuler.x -= Input.GetAxis("Mouse Y") * rotationSpeed;
                _rotationEuler.y += Input.GetAxis("Mouse X") * rotationSpeed;
                transform.eulerAngles = _rotationEuler;
            }

            CalculateInputVector();

            transform.Translate(_inputVector);
        }

        private void CalculateInputVector()
        {
            _inputVector.x = 0;
            _inputVector.y = 0;
            _inputVector.z = 0;
            if (Input.GetKey(KeyCode.W))
                _inputVector.z += 1;
            if (Input.GetKey(KeyCode.S))
                _inputVector.z -= 1;
            if (Input.GetKey(KeyCode.A))
                _inputVector.x -= 1;
            if (Input.GetKey(KeyCode.D))
                _inputVector.x += 1;
            if (Input.GetKey(KeyCode.Q))
                _inputVector.y -= 1;
            if (Input.GetKey(KeyCode.E))
                _inputVector.y += 1;
            if (Input.GetKey(KeyCode.LeftShift))
                _inputVector *= Time.deltaTime * shiftSpeed;
            else
                _inputVector *= Time.deltaTime * mainSpeed;
        }
    }
}