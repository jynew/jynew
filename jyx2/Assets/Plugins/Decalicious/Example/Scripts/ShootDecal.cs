using UnityEngine;
using System.Collections;

namespace ThreeEyedGames.DecaliciousExample
{
    public class ShootDecal : MonoBehaviour
    {
        public GameObject DecalPrefab;
        public float RemoveAfterSeconds = 10.0f;
        public float RoundsPerMin = 10.0f;
        public int TotalAmmo = -1;
        public AudioClip Sound;


        protected float _timeSinceLastShot = 0.0f;

        // Update is called once per frame
        void Update()
        {
            _timeSinceLastShot += Time.deltaTime;

            if (TotalAmmo != 0 && _timeSinceLastShot > (60.0f / RoundsPerMin) && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1)))
            {
                TotalAmmo--;
                _timeSinceLastShot = 0.0f;

                RaycastHit hit;
                if (Physics.Raycast(GetComponent<Camera>().ViewportPointToRay(Vector3.one * 0.5f), out hit))
                {
                    Transform t = (Instantiate(DecalPrefab, hit.collider.transform, true) as GameObject).transform;
                    t.position = hit.point;
                    t.up = hit.normal;
                    t.Rotate(Vector3.up, Random.Range(0, 360), Space.Self);

                    Camera.main.transform.Rotate(Vector3.right, -1 * Random.Range(0.2f, 0.4f), Space.Self);
                    Camera.main.transform.Rotate(Vector3.up, Random.Range(-0.2f, 0.2f), Space.World);

                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.AddForceAtPosition(Camera.main.transform.forward * 20.0f, hit.point, ForceMode.Impulse);

                    AudioSource.PlayClipAtPoint(Sound, transform.position);
                    StartCoroutine(RemoveDecal(t.gameObject));
                }
            }
        }

        protected IEnumerator RemoveDecal(GameObject decal)
        {
            yield return new WaitForSeconds(RemoveAfterSeconds);
            Decal d = decal.GetComponent<Decal>();
            while (d.Fade > 0)
            {
                d.Fade -= Time.deltaTime * 0.3f;
                yield return null;
            }
            d.Fade = 0;
        }
    }
}