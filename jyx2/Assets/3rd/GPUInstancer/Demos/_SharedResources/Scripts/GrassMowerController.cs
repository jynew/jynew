using UnityEngine;

namespace GPUInstancer
{
    public class GrassMowerController : MonoBehaviour
    {
        public float engineTorque = 3500;
        public float enginePower = 4000;

        private Rigidbody grassMowerRigidbody;

        // Control Inputs
        private float thrustInput;
        private float yawInput;

        private void Awake()
        {
            grassMowerRigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            GetInputs();
            Move();
        }

        private void GetInputs()
        {
            yawInput = Input.GetAxis("Horizontal");
            thrustInput = Input.GetAxis("Jump");
        }

        private void Move()
        {
            grassMowerRigidbody.AddRelativeTorque(Vector3.up * yawInput * engineTorque * Time.deltaTime);
             grassMowerRigidbody.AddRelativeForce(Vector3.forward * thrustInput * enginePower * Time.deltaTime);
        }
    }
}

