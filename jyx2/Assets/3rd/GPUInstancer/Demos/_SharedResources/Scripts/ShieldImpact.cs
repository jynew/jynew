using UnityEngine;

namespace GPUInstancer
{
    public class ShieldImpact : MonoBehaviour
    {


        private float impactTime;
        private Material impactMat;

        private void Awake()
        {
            impactMat = transform.Find("ImpactShield").GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {

            if (impactTime > 0)
            {
                impactTime -= Time.deltaTime * 1000;
                if (impactTime < 0)
                    impactTime = 0;
                impactMat.SetFloat("_ImpactTime", impactTime);
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                impactMat.SetVector("_ImpactPosition", transform.InverseTransformPoint(contact.point));
                impactTime = 500;
                impactMat.SetFloat("_ImpactTime", impactTime);
            }
        }
    }
}
