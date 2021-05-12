using UnityEngine;

namespace ThreeEyedGames.DecaliciousExample
{
    public class SwingIt : MonoBehaviour
    {
        public float MaxAngle = 0.1f;
        public float Speed = 1.0f;

        void Update()
        {
            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.z = Mathf.Sin(Time.time * Speed) * MaxAngle;
            transform.localEulerAngles = eulerAngles;
        }
    }
}