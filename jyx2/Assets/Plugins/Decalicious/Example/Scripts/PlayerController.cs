using UnityEngine;

namespace ThreeEyedGames.DecaliciousExample
{
    public class PlayerController : MonoBehaviour
    {
        public float MouseSensitivity = 1.0f;
        public float MoveSpeed = 1.0f;

        public AudioClip InteractSuccess;
        public AudioClip InteractError;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            Vector3 forward = transform.forward;
            forward.y = 0.0f;
            forward.Normalize();

            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * MouseSensitivity, 0), Space.World);
            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * MouseSensitivity, 0, 0), Space.Self);

            float moveSpeed = MoveSpeed;
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                moveSpeed *= 3.0f;
#endif

            if (Input.GetKey(KeyCode.W))
                transform.Translate(forward * Time.deltaTime * moveSpeed, Space.World);
            if (Input.GetKey(KeyCode.S))
                transform.Translate(-forward * Time.deltaTime * moveSpeed, Space.World);

            if (Input.GetKey(KeyCode.A))
                transform.Translate(-transform.right * Time.deltaTime * moveSpeed, Space.World);
            if (Input.GetKey(KeyCode.D))
                transform.Translate(transform.right * Time.deltaTime * moveSpeed, Space.World);

            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(Vector3.one * 0.5f), out hit))
                {
                    bool success = false;
                    if (Vector3.SqrMagnitude(transform.position - hit.point) < 2.25f)
                    {
                        if (hit.collider.GetComponent<IInteract>() != null)
                        {
                            hit.collider.GetComponent<IInteract>().Interact();
                            success = true;
                        }
                    }

                    AudioSource.PlayClipAtPoint(success ? InteractSuccess : InteractError, transform.position + transform.forward * 0.5f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DecaliciousRenderer r = Camera.main.GetComponent<DecaliciousRenderer>();
                r.enabled = !r.enabled;
            }
        }
    }
}
