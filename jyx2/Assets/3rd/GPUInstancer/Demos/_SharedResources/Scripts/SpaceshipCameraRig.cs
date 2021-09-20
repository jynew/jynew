using UnityEngine;

namespace GPUInstancer
{
    // Note: Modified version of the Unity Standard Assets script.

    public class SpaceshipCameraRig : MonoBehaviour
    {

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        public Transform m_Target;                      // The target object to follow
        public float m_MoveSpeed = 3;                   // How fast the rig will move to keep up with target's position
        public float m_TurnSpeed = 1;                   // How fast the rig will turn to keep up with target's rotation
        public float m_RollSpeed = 0.2f;                // How fast the rig will roll (around Z axis) to match target's roll.
        public bool m_FollowVelocity = false;           // Whether the rig will rotate in the direction of the target's velocity.
        public bool m_FollowTilt = true;                // Whether the rig will tilt (around X axis) with the target.
        public float m_SpinTurnLimit = 90;              // The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        public float m_TargetVelocityLowerLimit = 4f;   // the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        public float m_SmoothTurnTime = 0.2f;           // the smoothing for the camera's rotation

        private Vector3 m_LastTargetPosition;
        private Rigidbody targetRigidbody;
        private float m_LastFlatAngle;                  // The relative angle of the target and the rig from the previous frame.
        private float m_CurrentTurnAmount;              // How much to turn the camera
        private float m_TurnSpeedVelocityChange;        // The change in the turn speed velocity
        private Vector3 m_RollUp = Vector3.up;          // The roll of the camera around the z axis ( generally this will always just be up )

        private void Start()
        {
            if (m_Target == null)
                return;
            targetRigidbody = m_Target.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            FollowTarget(Time.deltaTime);
        }

        private void FollowTarget(float deltaTime)
        {
            if (!(deltaTime > 0) || m_Target == null)
            {
                return;
            }

            var targetForward = m_Target.forward;
            var targetUp = m_Target.up;

            if (m_FollowVelocity && Application.isPlaying)
            {
                // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
                // but only if the object is traveling faster than a given threshold.

                if (targetRigidbody.velocity.magnitude > m_TargetVelocityLowerLimit)
                {
                    // velocity is high enough, so we'll use the target's velocty
                    targetForward = targetRigidbody.velocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);
            }
            else
            {
                // we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

                // This section allows the camera to stop following the target's rotation when the target is spinning too fast.
                // eg when a car has been knocked into a spin. The camera will resume following the rotation
                // of the target when the target's angular velocity slows below the threshold.
                var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
                if (m_SpinTurnLimit > 0)
                {
                    var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle)) / deltaTime;
                    var desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit * 0.75f, targetSpinSpeed);
                    var turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
                    if (Application.isPlaying)
                    {
                        m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount,
                                                             ref m_TurnSpeedVelocityChange, turnReactSpeed);
                    }
                    else
                    {
                        // for editor mode, smoothdamp won't work because it uses deltaTime internally
                        m_CurrentTurnAmount = desiredTurnAmount;
                    }
                }
                else
                {
                    m_CurrentTurnAmount = 1;
                }
                m_LastFlatAngle = currentFlatAngle;
            }

            // camera position moves towards target position:
            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')
            if (!m_FollowTilt)
            {
                targetForward.y = 0;
                if (targetForward.sqrMagnitude < float.Epsilon)
                {
                    targetForward = transform.forward;
                }
            }
            var rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);

            // and aligning with the target object's up direction (i.e. its 'roll')
            m_RollUp = m_RollSpeed > 0 ? Vector3.Slerp(m_RollUp, targetUp, m_RollSpeed * deltaTime) : Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, m_TurnSpeed * m_CurrentTurnAmount * deltaTime);
        }
    }
}