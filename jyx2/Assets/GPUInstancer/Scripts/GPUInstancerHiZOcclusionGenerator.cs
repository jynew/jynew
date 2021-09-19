using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class GPUInstancerHiZOcclusionGenerator : MonoBehaviour
    {
        public bool debuggerEnabled = false;
        public bool debuggerGUIOnTop = false;
        [Range(0f, 0.1f)]
        public float debuggerOverlay = 0;
        [Range(0, 16)]
        public int debuggerMipLevel;

        [Header("For info only, don't change:")]
        //[HideInInspector]
        public RenderTexture hiZDepthTexture;
        //[HideInInspector]
        public Texture unityDepthTexture;
        //[HideInInspector]
        public Vector2 hiZTextureSize;
        [HideInInspector]
        public bool isVREnabled;

        private Camera _mainCamera;
        private bool _isInvalid;
        private GPUIOcclusionCullingType occlusionCullingType;
        private Material _depthBlitMaterial;
        private int _hiZMipLevels = 0;
        private RenderTexture[] _hiZMipLevelTextures = null;

        private RenderTexture _tempDepthTextureForTex2DArray;
        private bool _isDepthTex2DArray;

        // Debugger:
        private RawImage _hiZDebugDepthTextureGUIImage;
        private GameObject _hiZOcclusionDebuggerGUI;
#if UNITY_EDITOR
        private bool _debuggerGUIOnTopCached;
        private float _debuggerOverlayCached;
#endif

        #region MonoBehaviour Methods

        private void Awake()
        {
            hiZTextureSize = Vector2.zero;
            GPUInstancerConstants.SetupComputeTextureUtils();
            occlusionCullingType = GPUInstancerConstants.gpuiSettings.occlusionCullingType;

#if !UNITY_2018_3_OR_NEWER
            occlusionCullingType = GPUIOcclusionCullingType.Default;
#endif
        }

        private void OnEnable()
        {
#if UNITY_2018_1_OR_NEWER // The SRP classes exist only in Unity 2018 and later. 
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
#if UNITY_2019_1_OR_NEWER // In Unity 2019, the SRP classes are removed from "Experimental".
                UnityEngine.Rendering.RenderPipelineManager.endCameraRendering += OnEndCameraRenderingSRP;
#else
                // Also, the "endCameraRendering" method was added in 2019, so we use beginCameraRendering to get the depth info from the previous frame.
                UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering += OnEndCameraRendering;
#endif
            else
#endif
                if (occlusionCullingType == GPUIOcclusionCullingType.Default)
            {
                _depthBlitMaterial = new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_HIZ_OCCLUSION_GENERATOR));
                Camera.onPostRender += OnEndCameraRenderingBlit;

            }
            else
                Camera.onPostRender += OnEndCameraRendering;
        }

        private void OnDisable()
        {
#if UNITY_2018_1_OR_NEWER
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
#if UNITY_2019_1_OR_NEWER
                UnityEngine.Rendering.RenderPipelineManager.endCameraRendering -= OnEndCameraRenderingSRP;
#else
                UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering -= OnEndCameraRendering;
#endif
            else
#endif
                if (occlusionCullingType == GPUIOcclusionCullingType.Default)
                Camera.onPostRender -= OnEndCameraRenderingBlit;
            else
                Camera.onPostRender -= OnEndCameraRendering;

            if (hiZDepthTexture != null)
            {
                hiZDepthTexture.Release();
                hiZDepthTexture = null;
            }

            if (_tempDepthTextureForTex2DArray != null)
            {
                _tempDepthTextureForTex2DArray.Release();
                _tempDepthTextureForTex2DArray = null;
            }

            if (_hiZMipLevelTextures != null)
            {
                for (int i = 0; i < _hiZMipLevelTextures.Length; i++)
                {
                    if (_hiZMipLevelTextures[i] != null)
                        _hiZMipLevelTextures[i].Release();
                }
                _hiZMipLevelTextures = null;
            }
        }

        #endregion


        #region Public Methods

        public void Initialize(Camera occlusionCamera = null)
        {
            _isInvalid = false;

            _mainCamera = occlusionCamera != null ? occlusionCamera : gameObject.GetComponent<Camera>();

            if (_mainCamera == null)
            {
                Debug.LogError("GPUI Hi-Z Occlision Culling Generator failed to initialize: camera not found.");
                _isInvalid = true;
                return;
            }

#if GPUI_VR || GPUI_XR

#if UNITY_2017_2_OR_NEWER
#if GPUI_XR
            if (UnityEngine.XR.XRSettings.enabled)
#endif
#else
#if GPUI_VR
            if (UnityEngine.VR.VRSettings.enabled)
#endif
#endif
            {
                isVREnabled = true;

#if UNITY_2018_3_OR_NEWER
#if GPUI_XR
                // Set the correct vr rendering mode if it is 2018.3 or later automatically
                GPUInstancerConstants.gpuiSettings.vrRenderingMode = UnityEngine.XR.XRSettings.stereoRenderingMode == UnityEngine.XR.XRSettings.StereoRenderingMode.SinglePass ? 0 : 1;
#endif
#endif

                if (isVREnabled && _depthBlitMaterial != null)
                {
                    if (GPUInstancerConstants.gpuiSettings.vrRenderingMode == 1)
                        _depthBlitMaterial.EnableKeyword("MULTIPASS_VR_ENABLED");
                    else
                        _depthBlitMaterial.EnableKeyword("SINGLEPASS_VR_ENABLED");

                    if (GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion)
                        _depthBlitMaterial.EnableKeyword("HIZ_TEXTURE_FOR_BOTH_EYES");
                }

                if (_mainCamera.stereoTargetEye != StereoTargetEyeMask.Both)
                {
                    Debug.LogError("GPUI Hi-Z Occlision works only for cameras that render to Both eyes. Disabling Occlusion Culling.");
                    _isInvalid = true;
                    return;
                }

            }
            else
#endif
            {
                isVREnabled = false;
            }

            _mainCamera.depthTextureMode |= DepthTextureMode.Depth;

            CreateHiZDepthTexture();
        }

        #endregion


        #region Private Methods

#if UNITY_2019_1_OR_NEWER
        // SRP callback signature is different in 2019.1, but we only need the camera. 
        // Using this method to direct the callback to the main method. Unity 2018 has the same signature with Camera.onPostRender, so this is not relevant.
        private void OnEndCameraRenderingSRP(UnityEngine.Rendering.ScriptableRenderContext context, Camera camera)
        {
            OnEndCameraRendering(camera);
        }
#endif

        private void OnEndCameraRendering(Camera camera)
        {
            if (_isInvalid || _mainCamera == null || camera != _mainCamera)
                return;

            if (hiZDepthTexture == null)
            {
                Debug.LogWarning("GPUI HiZ Depth texture is null where it should not be. Recreating it.");
                CreateHiZDepthTexture();
            }

            HandleScreenSizeChange();

            if (unityDepthTexture == null)
            {
                unityDepthTexture = Shader.GetGlobalTexture("_CameraDepthTexture");

#if UNITY_2018_3_OR_NEWER
                if (unityDepthTexture != null && unityDepthTexture.dimension == UnityEngine.Rendering.TextureDimension.Tex2DArray)
                    _isDepthTex2DArray = true;
                else
                    _isDepthTex2DArray = false;
#endif
            }

#if UNITY_2018_3_OR_NEWER
            if (_isDepthTex2DArray && unityDepthTexture != null && _tempDepthTextureForTex2DArray == null)
            {
                _tempDepthTextureForTex2DArray = new RenderTexture(unityDepthTexture.width, unityDepthTexture.height, 24, unityDepthTexture.graphicsFormat);
                _tempDepthTextureForTex2DArray.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
                _tempDepthTextureForTex2DArray.Create();
            }
#endif

            if (unityDepthTexture != null) // will be null the first time this runs in Unity 2018 SRP since we have to use beginCameraRendering there.
            {
                if (isVREnabled && GPUInstancerConstants.gpuiSettings.vrRenderingMode == 1)
                {
                    if (_mainCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
                        UpdateTextureWithComputeShader(0);
                    else if (GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion)
                        UpdateTextureWithComputeShader((int)hiZTextureSize.x / 2);
                }
                else
                    UpdateTextureWithComputeShader(0);
            }

#if UNITY_EDITOR
            HandleHiZDebugger();
#endif
        }

        private void UpdateTextureWithComputeShader(int offset)
        {
#if UNITY_2018_3_OR_NEWER
            if (_isDepthTex2DArray)
            {
                Graphics.CopyTexture(unityDepthTexture, 0, 0, _tempDepthTextureForTex2DArray, 0, 0);
                GPUInstancerUtility.CopyTextureWithComputeShader(_tempDepthTextureForTex2DArray, hiZDepthTexture, offset);
            }
            else
#endif
                GPUInstancerUtility.CopyTextureWithComputeShader(unityDepthTexture, hiZDepthTexture, offset);

            for (int i = 0; i < _hiZMipLevels - 1; ++i)
            {
                RenderTexture tempRT = _hiZMipLevelTextures[i];

                if (i == 0)
                    GPUInstancerUtility.ReduceTextureWithComputeShader(hiZDepthTexture, tempRT, offset);
                else
                    GPUInstancerUtility.ReduceTextureWithComputeShader(_hiZMipLevelTextures[i - 1], tempRT, offset);

                GPUInstancerUtility.CopyTextureWithComputeShader(tempRT, hiZDepthTexture, offset, 0, i + 1, false);
            }
        }

        private void OnEndCameraRenderingBlit(Camera camera)
        {
            if (_isInvalid || _mainCamera == null || camera != _mainCamera)
                return;

            if (hiZDepthTexture == null)
            {
                Debug.LogWarning("GPUI HiZ Depth texture is null where it should not be. Recreating it.");
                CreateHiZDepthTexture();
            }

            HandleScreenSizeChange();

            Graphics.Blit(null, hiZDepthTexture, _depthBlitMaterial, 0);

            for (int i = 0; i < _hiZMipLevels; ++i)
            {
                RenderTexture tempRT = _hiZMipLevelTextures[i];

                if (i == 0)
                    Graphics.Blit(hiZDepthTexture, tempRT, _depthBlitMaterial, 1);
                else
                    Graphics.Blit(_hiZMipLevelTextures[i - 1], tempRT, _depthBlitMaterial, 1);

                Graphics.CopyTexture(tempRT, 0, 0, hiZDepthTexture, 0, i + 1);
            }

#if UNITY_EDITOR
            HandleHiZDebugger();
#endif
        }

        private Vector2 GetScreenSize()
        {
            Vector2 screenSize = Vector2.zero;
#if GPUI_VR || GPUI_XR
            if (isVREnabled)
            {
#if UNITY_2017_2_OR_NEWER
#if GPUI_XR
                screenSize.x = UnityEngine.XR.XRSettings.eyeTextureWidth;
                screenSize.y = UnityEngine.XR.XRSettings.eyeTextureHeight;
#endif
#else
#if GPUI_VR
                screenSize.x = UnityEngine.VR.VRSettings.eyeTextureWidth;
                screenSize.y = UnityEngine.VR.VRSettings.eyeTextureHeight;
#endif
#endif
                if (GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion)
                    screenSize.x *= 2;
            }
            else
#endif
            {
                screenSize.x = _mainCamera.pixelWidth;
                screenSize.y = _mainCamera.pixelHeight;
            }
#if GPUI_URP
            if (GPUInstancerConstants.gpuiSettings.isURP)
                screenSize *= (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset).renderScale;
#endif
            return screenSize;
        }

        private void CreateHiZDepthTexture()
        {
            hiZTextureSize = GetScreenSize();

            _hiZMipLevels = (int)Mathf.Floor(Mathf.Log(hiZTextureSize.x, 2f));

            if (hiZTextureSize.x <= 0 || hiZTextureSize.y <= 0 || _hiZMipLevels == 0)
            {
                if (hiZDepthTexture != null)
                {
                    hiZDepthTexture.Release();
                    hiZDepthTexture = null;
                }

                Debug.LogError("Cannot create GPUI HiZ Depth Texture for occlusion culling: Screen size is too small.");
                return;
            }

            if (hiZDepthTexture != null)
                hiZDepthTexture.Release();

            int width = (int)hiZTextureSize.x;
            int height = (int)hiZTextureSize.y;

            hiZDepthTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            hiZDepthTexture.name = "GPUIHiZDepthTexture";
            hiZDepthTexture.filterMode = FilterMode.Point;
            hiZDepthTexture.useMipMap = true;
            hiZDepthTexture.autoGenerateMips = false;
            hiZDepthTexture.enableRandomWrite = true;
            hiZDepthTexture.Create();
            hiZDepthTexture.hideFlags = HideFlags.HideAndDontSave;
            hiZDepthTexture.GenerateMips();

            if (_hiZMipLevelTextures != null)
            {
                for (int i = 0; i < _hiZMipLevelTextures.Length; i++)
                {
                    if (_hiZMipLevelTextures[i] != null)
                        _hiZMipLevelTextures[i].Release();
                }
            }

            _hiZMipLevelTextures = new RenderTexture[_hiZMipLevels];

            for (int i = 0; i < _hiZMipLevels; ++i)
            {
                width = width >> 1;

                height = height >> 1;

                if (width == 0)
                    width = 1;

                if (height == 0)
                    height = 1;

                _hiZMipLevelTextures[i] = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
                _hiZMipLevelTextures[i].name = "GPUIHiZDepthTexture_Mip_" + i;
                _hiZMipLevelTextures[i].filterMode = FilterMode.Point;
                _hiZMipLevelTextures[i].useMipMap = false;
                _hiZMipLevelTextures[i].autoGenerateMips = false;
                _hiZMipLevelTextures[i].enableRandomWrite = true;
                _hiZMipLevelTextures[i].Create();
                _hiZMipLevelTextures[i].hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private void HandleScreenSizeChange()
        {
            Vector2 newScreenSize = GetScreenSize();
            if (newScreenSize != hiZTextureSize)
            {
                CreateHiZDepthTexture();

#if UNITY_2018_3_OR_NEWER
                if (_isDepthTex2DArray && unityDepthTexture != null && _tempDepthTextureForTex2DArray != null &&
                    (unityDepthTexture.width != _tempDepthTextureForTex2DArray.width || unityDepthTexture.height != _tempDepthTextureForTex2DArray.height))
                {
                    _tempDepthTextureForTex2DArray.Release();
                    _tempDepthTextureForTex2DArray = null;
                }
#endif
            }
        }

#if UNITY_EDITOR
        private void HandleHiZDebugger()
        {
            if (debuggerEnabled && _hiZOcclusionDebuggerGUI == null)
                CreateHiZDebuggerCanvas();

            if (!debuggerEnabled && _hiZOcclusionDebuggerGUI != null)
                DestroyImmediate(_hiZOcclusionDebuggerGUI);

            if (!debuggerEnabled)
                return;

            if (debuggerGUIOnTop != _debuggerGUIOnTopCached || debuggerOverlay != _debuggerOverlayCached)
            {
                _hiZOcclusionDebuggerGUI.GetComponent<Canvas>().sortingOrder = debuggerGUIOnTop ? 10000 : -10000;
                _hiZDebugDepthTextureGUIImage.color = new Color(1, 1, 1, 1 - debuggerOverlay);

                _debuggerOverlayCached = debuggerOverlay;
                _debuggerGUIOnTopCached = debuggerGUIOnTop;
            }

            if (_hiZOcclusionDebuggerGUI != null && hiZDepthTexture != null)
            {
                _hiZDebugDepthTextureGUIImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, isVREnabled
                    && GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion ? hiZTextureSize.x * 0.5f : hiZTextureSize.x);
                _hiZDebugDepthTextureGUIImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hiZTextureSize.y);
                _hiZDebugDepthTextureGUIImage.texture = debuggerMipLevel == 0 ? hiZDepthTexture : _hiZMipLevelTextures[debuggerMipLevel >= _hiZMipLevels ? _hiZMipLevels - 1 : debuggerMipLevel];
            }
        }

        private void CreateHiZDebuggerCanvas()
        {
            _hiZOcclusionDebuggerGUI = new GameObject("GPUI HiZ Occlusion Culling Debugger");
            _debuggerGUIOnTopCached = debuggerGUIOnTop;
            _debuggerOverlayCached = debuggerOverlay;

            // Add and setup the canvas
            Canvas debuggerCanvas = _hiZOcclusionDebuggerGUI.AddComponent<Canvas>();
            debuggerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            debuggerCanvas.pixelPerfect = true; // no antialiasing
            debuggerCanvas.sortingOrder = debuggerGUIOnTop ? 10000 : -10000;
            debuggerCanvas.targetDisplay = 0;

            // Add a raw image to display the HiZ Depth RenderTexture
            GameObject hiZDepthTextureGUI = new GameObject("HiZ Depth Texture");
            hiZDepthTextureGUI.transform.parent = _hiZOcclusionDebuggerGUI.transform;
            _hiZDebugDepthTextureGUIImage = hiZDepthTextureGUI.AddComponent<RawImage>();
            _hiZDebugDepthTextureGUIImage.color = new Color(1, 1, 1, 1 - debuggerOverlay);

            // Setup the image's RectTransform for anchors, pivot and position
            Vector2 bottomRight = new Vector2(0, 0);
            _hiZDebugDepthTextureGUIImage.rectTransform.anchorMin = bottomRight;
            _hiZDebugDepthTextureGUIImage.rectTransform.anchorMax = bottomRight;
            _hiZDebugDepthTextureGUIImage.rectTransform.pivot = bottomRight;
            _hiZDebugDepthTextureGUIImage.rectTransform.position = Vector2.zero;
        }
#endif // UNITY_EDITOR
        #endregion
    }
}