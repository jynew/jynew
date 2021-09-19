using System;
using UnityEngine;

namespace GPUInstancer
{
    [Serializable]
    public class GPUInstancerCameraData
    {
        public Camera mainCamera;
        public bool renderOnlySelectedCamera = false;
        [NonSerialized]
        public GPUInstancerHiZOcclusionGenerator hiZOcclusionGenerator;
        [NonSerialized]
        public float[] mvpMatrixFloats;
        [NonSerialized]
        public float[] mvpMatrix2Floats;
        [NonSerialized]
        public Vector3 cameraPosition = Vector3.zero;
        [NonSerialized]
        public bool hasOcclusionGenerator = false;
        [NonSerialized]
        public float halfAngle;

        public GPUInstancerCameraData() : this(null) { }

        public GPUInstancerCameraData(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
            mvpMatrixFloats = new float[16];
            CalculateHalfAngle();
        }

        public void SetCamera(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
            CalculateHalfAngle();
        }

        public void CalculateCameraData()
        {
            hasOcclusionGenerator = hiZOcclusionGenerator != null && hiZOcclusionGenerator.hiZDepthTexture != null;

            Matrix4x4 mvpMatrix =
                (hasOcclusionGenerator && hiZOcclusionGenerator.isVREnabled
                ? mainCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left) : mainCamera.projectionMatrix) * mainCamera.worldToCameraMatrix;

            if (mvpMatrixFloats == null || mvpMatrixFloats.Length != 16)
                mvpMatrixFloats = new float[16];
            mvpMatrix.Matrix4x4ToFloatArray(mvpMatrixFloats);

            if (hasOcclusionGenerator && hiZOcclusionGenerator.isVREnabled && GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion)
            {
                Matrix4x4 mvpMatrix2 = mainCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right) * mainCamera.worldToCameraMatrix;
                if (mvpMatrix2Floats == null || mvpMatrix2Floats.Length != 16)
                    mvpMatrix2Floats = new float[16];
                mvpMatrix2.Matrix4x4ToFloatArray(mvpMatrix2Floats);
            }

            cameraPosition = mainCamera.transform.position;
        }

        public void CalculateHalfAngle()
        {
            if (mainCamera != null)
                halfAngle = Mathf.Tan(Mathf.Deg2Rad * mainCamera.fieldOfView * 0.25f);
        }

        public Camera GetRenderingCamera()
        {
            if (renderOnlySelectedCamera
#if UNITY_EDITOR
                || UnityEditor.EditorApplication.isPaused
#endif
                )
                return mainCamera;
            return null;
        }
    }
}