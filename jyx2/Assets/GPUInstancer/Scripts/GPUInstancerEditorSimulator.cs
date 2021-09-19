#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// Simulate GPU Instancing while game is not running or when editor is paused
    /// </summary>
    public class GPUInstancerEditorSimulator
    {
        public GPUInstancerManager gpuiManager;
        public bool simulateAtEditor;
        public bool initializingInstances;
        public static GPUInstancerCameraData sceneViewCameraData = new GPUInstancerCameraData(null);

        public static readonly string sceneViewCameraName = "SceneCamera";

        public GPUInstancerEditorSimulator(GPUInstancerManager gpuiManager)
        {
            this.gpuiManager = gpuiManager;

            if (sceneViewCameraData == null)
                sceneViewCameraData = new GPUInstancerCameraData(null);
            sceneViewCameraData.renderOnlySelectedCamera = true;

            if (gpuiManager != null)
            {
                EditorApplication.update -= FindSceneViewCamera;
                EditorApplication.update += FindSceneViewCamera;
#if UNITY_2017_2_OR_NEWER
                EditorApplication.pauseStateChanged -= HandlePauseStateChanged;
                EditorApplication.pauseStateChanged += HandlePauseStateChanged;
#else
                EditorApplication.playmodeStateChanged = HandlePlayModeStateChanged;
#endif
            }
        }

        public void StartSimulation()
        {
            if ((Application.isPlaying && !EditorApplication.isPaused) || gpuiManager == null)
                return;

            initializingInstances = true;

            simulateAtEditor = true;
            EditorApplication.update -= FindSceneViewCamera;
            EditorApplication.update += FindSceneViewCamera;
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged = HandlePlayModeStateChanged;
#endif
        }

        public void StopSimulation()
        {
            if (!Application.isPlaying)
                gpuiManager.ClearInstancingData();

            simulateAtEditor = false;

#if UNITY_2018_1_OR_NEWER
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
#if UNITY_2019_1_OR_NEWER
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= CameraOnBeginRenderingSRP;
#else
                UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering -= CameraOnPreCull;
#endif
            else
#endif
                Camera.onPreCull -= CameraOnPreCull;

            EditorApplication.update -= EditorUpdate;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
#endif
        }

        public void ClearEditorUpdates()
        {
            simulateAtEditor = false;

#if UNITY_2018_1_OR_NEWER
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
#if UNITY_2019_1_OR_NEWER
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= CameraOnBeginRenderingSRP;
#else
                UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering -= CameraOnPreCull;
#endif
            else
#endif
                Camera.onPreCull -= CameraOnPreCull;


            EditorApplication.update -= FindSceneViewCamera;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.pauseStateChanged -= HandlePauseStateChanged;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged = null;
#endif
        }

        private void FindSceneViewCamera()
        {
            if (sceneViewCameraData.mainCamera == null || sceneViewCameraData.mainCamera.name != sceneViewCameraName)
            {
                Camera currentCam = Camera.current;
                if (currentCam != null && currentCam.name == sceneViewCameraName)
                    sceneViewCameraData.SetCamera(currentCam);
                else
                    return;
            }
            EditorApplication.update -= FindSceneViewCamera;
        }

        private void EditorUpdate()
        {
            if (sceneViewCameraData.mainCamera != null && sceneViewCameraData.mainCamera.name == sceneViewCameraName && gpuiManager != null)
            {
                if (initializingInstances)
                {
                    if (!gpuiManager.isInitialized)
                    {
                        gpuiManager.Awake();
                        gpuiManager.InitializeRuntimeDataAndBuffers();
                        if (gpuiManager.GetComponent<GPUInstancerLODColorDebugger>())
                            gpuiManager.GetComponent<GPUInstancerLODColorDebugger>().ChangeLODColors();
                    }
                    initializingInstances = false;
                    return;
                }

#if UNITY_2018_1_OR_NEWER
                if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                {
#if UNITY_2019_1_OR_NEWER
                    UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= CameraOnBeginRenderingSRP;
                    UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering += CameraOnBeginRenderingSRP;
#else
                    UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering -= CameraOnPreCull;
                    UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering += CameraOnPreCull;
#endif
                }
                else
                {
#endif
                    Camera.onPreCull -= CameraOnPreCull;
                    Camera.onPreCull += CameraOnPreCull;
#if UNITY_2018_1_OR_NEWER
                }
#endif
                EditorApplication.update -= EditorUpdate;
            }
        }

#if UNITY_2019_1_OR_NEWER
        private void CameraOnBeginRenderingSRP(UnityEngine.Rendering.ScriptableRenderContext context, Camera[] cams)
        {
            if (!gpuiManager.isInitialized)
            {
                StopSimulation();
                StartSimulation();
                return;
            }
            foreach (Camera cam in cams)
            {
                if (sceneViewCameraData.mainCamera == cam)
                {
                    gpuiManager.Update();
                    gpuiManager.UpdateBuffers(sceneViewCameraData);
                }
                else if (gpuiManager.cameraData.mainCamera == cam)
                {
                    gpuiManager.Update();
                    gpuiManager.UpdateBuffers(gpuiManager.cameraData);
                }
            }
        }
#endif

        private void CameraOnPreCull(Camera cam)
        {
            if (!gpuiManager.isInitialized)
            {
                StopSimulation();
                StartSimulation();
                return;
            }
            if (sceneViewCameraData.mainCamera == cam)
            {
                gpuiManager.Update();
                gpuiManager.UpdateBuffers(sceneViewCameraData);
            }
            else if (gpuiManager.cameraData.mainCamera == cam)
            {
                gpuiManager.Update();
                gpuiManager.UpdateBuffers(gpuiManager.cameraData);
            }
        }




#if UNITY_2017_2_OR_NEWER
        public void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            StopSimulation();
        }

        public void HandlePauseStateChanged(PauseState state)
        {
            if (gpuiManager == null)
            {
                EditorApplication.pauseStateChanged -= HandlePauseStateChanged;
                return;
            }
            if (Application.isPlaying)
            {
                switch (state)
                {
                    case PauseState.Paused:
                        StartSimulation();
                        break;
                    case PauseState.Unpaused:
                        StopSimulation();
                        break;
                }
            }
        }
#else
        public void HandlePlayModeStateChanged()
        {
            if (Application.isPlaying && EditorApplication.isPaused)
                StartSimulation();
            else
                StopSimulation();
        }
#endif // UNITY_2017_2_OR_NEWER
    }
}
#endif // UNITY_EDITOR