using UnityEngine;
using System.Collections;

namespace GPUInstancer
{
    public class FPS : MonoBehaviour
    {
        public float FPSCount;

        IEnumerator Start()
        {
            //GUI.depth = 2;
            while (true)
            {
                if (Time.timeScale == 1)
                {
                    yield return new WaitForSeconds(0.1f);
                    FPSCount = Mathf.Round(1 / Time.deltaTime);
                }
                else
                {
                    FPSCount = 0;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
