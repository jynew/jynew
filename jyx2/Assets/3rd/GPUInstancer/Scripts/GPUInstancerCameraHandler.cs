using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// GPUInstancerCameraHandler component can be used to handle camera switching automatically. Add the GPUInstancerCameraHandler to your GameObjects with the Camera 
    /// component to let it make the SetCamera calls automatically. Only one of the Cameras with GPUInstancerCameraHandler components should be active in the scene at any given time
    /// for the handler to work as intended.
    /// Note: If you want to manage the Cameras manually, please use GPUInstancer.GPUInstancerAPI.SetCamera API method.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class GPUInstancerCameraHandler : MonoBehaviour
    {
        private Camera _camera;
        private bool _isCameraSet;

        private void OnEnable()
        {
            if (!_camera)
                _camera = GetComponent<Camera>();
            _isCameraSet = false;
        }

        private void OnDisable()
        {
            _isCameraSet = false;
        }

        private void Update()
        {
            if (_camera.isActiveAndEnabled && !_isCameraSet)
            {
                GPUInstancerAPI.SetCamera(_camera);
                _isCameraSet = true;
            }
            else if (!_camera.isActiveAndEnabled && _isCameraSet)
            {
                _isCameraSet = false;
            }
        }
    }
}