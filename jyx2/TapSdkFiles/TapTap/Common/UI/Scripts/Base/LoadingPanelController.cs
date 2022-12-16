using UnityEngine;

namespace  TapSDK.UI
{
    public class LoadingPanelController : BasePanelController
    {
        public Transform rotater;

        public float speed = 10;
        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            rotater = transform.Find("Image");;
        }

        private void Update()
        {
            if (rotater != null)
            {
                var localEuler = rotater.localEulerAngles;
                var z = rotater.localEulerAngles.z;
                z += Time.deltaTime * speed;
                rotater.localEulerAngles = new Vector3(localEuler.x, localEuler.y, z);
            }
        }
    }
}
