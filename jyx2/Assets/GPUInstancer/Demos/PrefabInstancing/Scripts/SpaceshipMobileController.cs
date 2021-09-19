using UnityEngine;

namespace GPUInstancer
{
    public class SpaceshipMobileController : MonoBehaviour
    {
        // Ship rigidbody angular drag and drag simulate reverse thrusters. Recommended settings are:
        // mass: 10, drag: 0.65, angular drag: 10

        public float engineTorque = 1500f;
        public float enginePower = 4500f;
        public SpaceshipMobileJoystick spaceShipJoystick;

        private Rigidbody shipRigidbody;

        // Control Inputs
        private float rollInput;
        private float thrustInput;
        private float pitchInput;
        private float yawInput;

        // Engine Particle effects
        private ParticleSystem.EmissionModule engineThrusterEmission;
        private ParticleSystem.EmissionModule engineGlowEmission;
        private Light engineGlowLight;

        private float originalThrusterEmissionRate;
        private float originalGlowEmissionRate;
        


        private void Awake()
        {
            shipRigidbody = GetComponent<Rigidbody>();

            engineThrusterEmission = transform.GetChild(0).GetComponent<ParticleSystem>().emission;
            originalThrusterEmissionRate = engineThrusterEmission.rateOverTime.constant;

            engineGlowEmission = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().emission;
            originalGlowEmissionRate = engineGlowEmission.rateOverTime.constant;

            Transform lightObject = transform.Find("EngineGlowLight");

            if (lightObject)
                engineGlowLight = lightObject.GetComponent<Light>();
        }

        void FixedUpdate()
        {
            GetJoystickInput();
            Move();
            AdjustThrusterEffects();

        }

        private void GetJoystickInput()
        {
            //yawInput = CrossPlatformInputManager.GetAxis("Horizontal");
            //thrustInput = CrossPlatformInputManager.GetButton("Jump") ? 1 : 0;
            //pitchInput = CrossPlatformInputManager.GetAxis("Vertical");
            //rollInput = CrossPlatformInputManager.GetButton("RollLeft") ? 1f : CrossPlatformInputManager.GetButton("RollRight") ? -1f : 0f;

            yawInput = spaceShipJoystick.inputDirection.x;
            pitchInput = spaceShipJoystick.inputDirection.z;
        }

        public void SetRollInput(float rollInput) // 1 is left, -1 is right, 0 is none
        {
            this.rollInput = rollInput;
        }

        public void SetThrustInput(bool isThrusting) // -1 is left, 1 is right, 0 is none
        {
            thrustInput = isThrusting ? 1f : 0f;
        }

        private void Move()
        {
            shipRigidbody.AddRelativeTorque(Vector3.up * yawInput * engineTorque * Time.deltaTime);
            shipRigidbody.AddRelativeTorque(Vector3.right * pitchInput * engineTorque * Time.deltaTime);
            shipRigidbody.AddRelativeTorque(Vector3.forward * rollInput * engineTorque * Time.deltaTime);

            shipRigidbody.AddRelativeForce(Vector3.forward * thrustInput * enginePower * Time.deltaTime);
        }

        private void AdjustThrusterEffects()
        {
            engineThrusterEmission.rateOverTime = originalThrusterEmissionRate * thrustInput;
            engineGlowEmission.rateOverTime = Mathf.Lerp(0.5f * originalGlowEmissionRate, originalGlowEmissionRate, thrustInput);
            if (engineGlowLight)
                engineGlowLight.intensity = Mathf.Clamp01(0.5f + thrustInput);
        }
    }
}

