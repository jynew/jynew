using UnityEngine;

namespace GPUInstancer
{
    public class SpaceshipController : MonoBehaviour
    {
        // Ship rigidbody angular drag and drag simulate reverse thrusters. Recommended settings are:
        // mass: 10, drag: 0.65, angular drag: 10

        public float engineTorque = 1500f;
        public float enginePower = 4500f;

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
            GetInputs();
            Move();
            AdjustThrusterEffects();

        }

        private void GetInputs()
        {
            yawInput = Input.GetAxis("Horizontal");
            thrustInput = Input.GetAxis("Jump");
            pitchInput = Input.GetAxis("Vertical");
            rollInput = Input.GetKey(KeyCode.Q) ? 1f : Input.GetKey(KeyCode.E) ? -1f : 0f;
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

