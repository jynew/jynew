using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    public abstract class GPUInstancerManager : MonoBehaviour
    {
        public List<GPUInstancerPrototype> prototypeList;

        public bool autoSelectCamera = true;
        public GPUInstancerCameraData cameraData = new GPUInstancerCameraData(null);

        public bool useFloatingOriginHandler = false;
        public Transform floatingOriginTransform;
        [NonSerialized]
        public GPUInstancerFloatingOriginHandler floatingOriginHandler;

        [NonSerialized]
        public List<GPUInstancerRuntimeData> runtimeDataList;
        [NonSerialized]
        public Bounds instancingBounds;

        public bool isFrustumCulling = true;
        public bool isOcclusionCulling = true;
        public float minCullingDistance = 0;

        protected GPUInstancerSpatialPartitioningData<GPUInstancerCell> spData;

        public static List<GPUInstancerManager> activeManagerList;
        public static bool showRenderedAmount;

        protected static ComputeShader _cameraComputeShader;
        protected static int[] _cameraComputeKernelIDs;
        protected static ComputeShader _visibilityComputeShader;
        protected static int[] _instanceVisibilityComputeKernelIDs;
        protected static ComputeShader _bufferToTextureComputeShader;
        protected static int _bufferToTextureComputeKernelID;

#if UNITY_EDITOR
        public List<GPUInstancerPrototype> selectedPrototypeList;
        [NonSerialized]
        public GPUInstancerEditorSimulator gpuiSimulator;
        public bool isPrototypeTextMode = false;

        public bool showSceneSettingsBox = true;
        public bool showPrototypeBox = true;
        public bool showAdvancedBox = false;
        public bool showHelpText = false;
        public bool showDebugBox = true;
        public bool showGlobalValuesBox = true;
        public bool showRegisteredPrefabsBox = true;
        public bool showPrototypesBox = true;

        public bool keepSimulationLive = false;
        public bool updateSimulation = true;
#endif

        public class GPUIThreadData
        {
            public Thread thread;
            public object parameter;
        }
        public static int maxThreads = 3;
        public readonly List<Thread> activeThreads = new List<Thread>();
        public readonly Queue<GPUIThreadData> threadStartQueue = new Queue<GPUIThreadData>();
        public readonly Queue<Action> threadQueue = new Queue<Action>();

        // Tree variables
        public static int lastTreePositionUpdate;
        public static GameObject treeProxyParent;
        public static Dictionary<GameObject, Transform> treeProxyList; // Dict[TreePrefab, TreeProxyGO]

        // Time management
        public static int lastDrawCallFrame;
        public static float lastDrawCallTime;
        public static float timeSinceLastDrawCall;

        // Global Wind
        protected static Vector4 _windVector = Vector4.zero;

        [NonSerialized]
        protected bool isInitial = true;

        [NonSerialized]
        public bool isInitialized = false;

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
        [NonSerialized]
        public PlayModeStateChange playModeState;
#endif
        [NonSerialized]
        public bool isQuiting = false;
        [NonSerialized]
        public Dictionary<GPUInstancerPrototype, GPUInstancerRuntimeData> runtimeDataDictionary;

        public LayerMask layerMask = ~0;
        public bool lightProbeDisabled = false;

        #region MonoBehaviour Methods

        public virtual void Awake()
        {
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();
            GPUInstancerUtility.SetPlatformDependentVariables();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                CheckPrototypeChanges();
#endif
            if (Application.isPlaying && activeManagerList == null)
                activeManagerList = new List<GPUInstancerManager>();

            if (SystemInfo.supportsComputeShaders)
            {
                if (_visibilityComputeShader == null)
                {
                    switch (GPUInstancerUtility.matrixHandlingType)
                    {
                        case GPUIMatrixHandlingType.MatrixAppend:
                            _visibilityComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.VISIBILITY_COMPUTE_RESOURCE_PATH_VULKAN);
                            GPUInstancerConstants.DETAIL_STORE_INSTANCE_DATA = true;
                            GPUInstancerConstants.COMPUTE_MAX_LOD_BUFFER = 3;
                            break;
                        case GPUIMatrixHandlingType.CopyToTexture:
                            _visibilityComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.VISIBILITY_COMPUTE_RESOURCE_PATH);
                            _bufferToTextureComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.BUFFER_TO_TEXTURE_COMPUTE_RESOURCE_PATH);
                            _bufferToTextureComputeKernelID = _bufferToTextureComputeShader.FindKernel(GPUInstancerConstants.BUFFER_TO_TEXTURE_KERNEL);
                            break;
                        default:
                            _visibilityComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.VISIBILITY_COMPUTE_RESOURCE_PATH);
                            break;
                    }

                    _instanceVisibilityComputeKernelIDs = new int[GPUInstancerConstants.VISIBILITY_COMPUTE_KERNELS.Length];
                    for (int i = 0; i < _instanceVisibilityComputeKernelIDs.Length; i++)
                        _instanceVisibilityComputeKernelIDs[i] = _visibilityComputeShader.FindKernel(GPUInstancerConstants.VISIBILITY_COMPUTE_KERNELS[i]);
                    GPUInstancerConstants.TEXTURE_MAX_SIZE = SystemInfo.maxTextureSize;

                    _cameraComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.CAMERA_COMPUTE_RESOURCE_PATH);

#if GPUI_VR || GPUI_XR
#if UNITY_2017_2_OR_NEWER
#if GPUI_XR
                    if (isOcclusionCulling && UnityEngine.XR.XRSettings.enabled && GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion)
#endif
#else
#if GPUI_VR
                    if (isOcclusionCulling && UnityEngine.VR.VRSettings.enabled && GPUInstancerConstants.gpuiSettings.testBothEyesForVROcclusion)
#endif
#endif
                    {
                        _cameraComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.CAMERA_VR_COMPUTE_RESOURCE_PATH);
                    }
#endif
                    _cameraComputeKernelIDs = new int[GPUInstancerConstants.CAMERA_COMPUTE_KERNELS.Length];
                    for (int i = 0; i < _cameraComputeKernelIDs.Length; i++)
                        _cameraComputeKernelIDs[i] = _cameraComputeShader.FindKernel(GPUInstancerConstants.CAMERA_COMPUTE_KERNELS[i]);
                }

                GPUInstancerConstants.SetupComputeRuntimeModification();
                GPUInstancerConstants.SetupComputeSetDataPartial();
            }
            else if (Application.isPlaying)
            {
                Debug.LogError("Target Graphics API does not support Compute Shaders. Please refer to Minimum Requirements on GPUInstancer/ReadMe.txt for detailed information.");
                this.enabled = false;
            }

            showRenderedAmount = false;

            InitializeCameraData();

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
#endif
        }

        public virtual void Start()
        {
            if (Application.isPlaying && SystemInfo.supportsComputeShaders)
            {
                SetupOcclusionCulling(cameraData);
            }
        }

        public virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (gpuiSimulator == null)
                gpuiSimulator = new GPUInstancerEditorSimulator(this);
#endif

            if (!Application.isPlaying)
                return;

            if (cameraData.mainCamera == null)
            {
                InitializeCameraData();
                if (cameraData.mainCamera == null)
                    Debug.LogWarning(GPUInstancerConstants.ERRORTEXT_cameraNotFound);
            }

            if (activeManagerList != null && !activeManagerList.Contains(this))
                activeManagerList.Add(this);

            if (SystemInfo.supportsComputeShaders)
            {
                if (GPUInstancerConstants.gpuiSettings == null || GPUInstancerConstants.gpuiSettings.shaderBindings == null)
                    Debug.LogWarning("No shader bindings file was supplied. Instancing will terminate!");

                if (runtimeDataList == null || runtimeDataList.Count == 0)
                    InitializeRuntimeDataAndBuffers();
                isInitial = true;
            }

            if (useFloatingOriginHandler && floatingOriginTransform != null)
            {
                if (floatingOriginHandler == null)
                {
                    floatingOriginHandler = floatingOriginTransform.gameObject.GetComponent<GPUInstancerFloatingOriginHandler>();
                    if (floatingOriginHandler == null)
                        floatingOriginHandler = floatingOriginTransform.gameObject.AddComponent<GPUInstancerFloatingOriginHandler>();
                }
                if (floatingOriginHandler.gPUIManagers == null)
                    floatingOriginHandler.gPUIManagers = new List<GPUInstancerManager>();
                if (!floatingOriginHandler.gPUIManagers.Contains(this))
                    floatingOriginHandler.gPUIManagers.Add(this);
            }
        }

        public virtual void Update()
        {
            ClearCompletedThreads();
            while (threadStartQueue.Count > 0 && activeThreads.Count < maxThreads)
            {
                GPUIThreadData threadData = threadStartQueue.Dequeue();
                threadData.thread.Start(threadData.parameter);
                activeThreads.Add(threadData.thread);
            }
            if (threadQueue.Count > 0)
            {
                Action action = threadQueue.Dequeue();
                if (action != null)
                    action.Invoke();
            }

            if (Application.isPlaying && treeProxyParent && lastTreePositionUpdate != Time.frameCount && cameraData.mainCamera != null)
            {
                treeProxyParent.transform.position = cameraData.mainCamera.transform.position;
                treeProxyParent.transform.rotation = cameraData.mainCamera.transform.rotation;
                lastTreePositionUpdate = Time.frameCount;
            }
        }

        public virtual void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                CheckPrototypeChanges();
            else
            {
#endif
                if (cameraData.mainCamera != null)
                {
                    UpdateTreeMPB();
                    UpdateBuffers(cameraData);
                }
#if UNITY_EDITOR
            }
#endif
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void Reset()
        {
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();
#if UNITY_EDITOR
            CheckPrototypeChanges();
#endif
        }

        public virtual void OnDisable() // could also be OnDestroy, but OnDestroy seems to be too late to prevent buffer leaks.
        {
            if (activeManagerList != null)
                activeManagerList.Remove(this);

            ClearInstancingData();
#if UNITY_EDITOR
            if (gpuiSimulator != null)
            {
                gpuiSimulator.ClearEditorUpdates();
                gpuiSimulator = null;
            }
#endif

            if (floatingOriginHandler != null && floatingOriginHandler.gPUIManagers != null && floatingOriginHandler.gPUIManagers.Contains(this))
            {
                floatingOriginHandler.gPUIManagers.Remove(this);
            }
        }

        private void OnApplicationQuit()
        {
            isQuiting = true;
        }
        #endregion MonoBehaviour Methods

        #region Virtual Methods

        public virtual void ClearInstancingData()
        {
            GPUInstancerUtility.ReleaseInstanceBuffers(runtimeDataList);
            GPUInstancerUtility.ReleaseSPBuffers(spData);
            if (runtimeDataList != null)
                runtimeDataList.Clear();
            if (runtimeDataDictionary != null)
                runtimeDataDictionary.Clear();
            spData = null;
            threadStartQueue.Clear();
            threadQueue.Clear();
            isInitialized = false;
        }

        public virtual void GeneratePrototypes(bool forceNew = false)
        {
            ClearInstancingData();

            if (forceNew || prototypeList == null)
                prototypeList = new List<GPUInstancerPrototype>();
            else
                prototypeList.RemoveAll(p => p == null);

            GPUInstancerConstants.gpuiSettings.SetDefultBindings();
        }

#if UNITY_EDITOR
        public virtual void CheckPrototypeChanges()
        {
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();

            if (prototypeList == null)
                GeneratePrototypes();
            else
                prototypeList.RemoveAll(p => p == null);

            if (GPUInstancerConstants.gpuiSettings != null && GPUInstancerConstants.gpuiSettings.shaderBindings != null)
            {
                GPUInstancerConstants.gpuiSettings.shaderBindings.ClearEmptyShaderInstances();
                foreach (GPUInstancerPrototype prototype in prototypeList)
                {
                    if (prototype.prefabObject != null)
                    {
                        GPUInstancerUtility.GenerateInstancedShadersForGameObject(prototype);
                        if (string.IsNullOrEmpty(prototype.warningText))
                        {
                            if (prototype.prefabObject.GetComponentInChildren<MeshRenderer>() == null)
                            {
                                prototype.warningText = "Prefab object does not contain any Mesh Renderers.";
                            }
                        }
                    }
                    else
                    {
                        if (GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                            GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_FOLIAGE);
                        else if (GPUInstancerConstants.gpuiSettings.isURP)
                        {
                            if (Shader.Find(GPUInstancerConstants.SHADER_GPUI_FOLIAGE_URP) != null)
                                GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_FOLIAGE_URP);
                        }
                        else if (GPUInstancerConstants.gpuiSettings.isLWRP)
                        {
                            if (Shader.Find(GPUInstancerConstants.SHADER_GPUI_FOLIAGE_LWRP) != null)
                                GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_FOLIAGE_LWRP);
                        }
                        else if (GPUInstancerConstants.gpuiSettings.isHDRP)
                        {
                            if (Shader.Find(GPUInstancerConstants.SHADER_GPUI_FOLIAGE_HDRP) != null)
                                GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_FOLIAGE_HDRP);
                        }
                    }
                }
            }
            if (GPUInstancerConstants.gpuiSettings != null && GPUInstancerConstants.gpuiSettings.billboardAtlasBindings != null)
            {
                GPUInstancerConstants.gpuiSettings.billboardAtlasBindings.ClearEmptyBillboardAtlases();
                //foreach (GPUInstancerPrototype prototype in prototypeList)
                //{
                //    if (prototype.prefabObject != null && prototype.useGeneratedBillboard && 
                //        (prototype.billboard == null || prototype.billboard.albedoAtlasTexture == null || prototype.billboard.normalAtlasTexture == null))
                //        GPUInstancerUtility.GeneratePrototypeBillboard(prototype, billboardAtlasBindings);
                //}
            }
        }
#endif
        public virtual void InitializeRuntimeDataAndBuffers(bool forceNew = true)
        {
            GPUInstancerUtility.SetPlatformDependentVariables();
            if (forceNew || !isInitialized)
            {
                instancingBounds = new Bounds(Vector3.zero, Vector3.one * GPUInstancerConstants.gpuiSettings.instancingBoundsSize);

                GPUInstancerUtility.ReleaseInstanceBuffers(runtimeDataList);
                GPUInstancerUtility.ReleaseSPBuffers(spData);
                if (runtimeDataList != null)
                    runtimeDataList.Clear();
                else
                    runtimeDataList = new List<GPUInstancerRuntimeData>();

                if (runtimeDataDictionary != null)
                    runtimeDataDictionary.Clear();
                else
                    runtimeDataDictionary = new Dictionary<GPUInstancerPrototype, GPUInstancerRuntimeData>();

                if (prototypeList == null)
                    prototypeList = new List<GPUInstancerPrototype>();
            }
        }

        public virtual void InitializeSpatialPartitioning()
        {

        }

        public virtual void UpdateSpatialPartitioningCells(GPUInstancerCameraData renderingCameraData)
        {

        }

        public virtual void DeletePrototype(GPUInstancerPrototype prototype, bool removeSO = true)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Delete prototype");
#endif
            prototypeList.Remove(prototype);

            if (removeSO && prototype.useGeneratedBillboard && prototype.billboard != null)
            {
                if (GPUInstancerConstants.gpuiSettings.billboardAtlasBindings.DeleteBillboardTextures(prototype))
                    prototype.billboard = null;
            }
        }

        public virtual void RemoveInstancesInsideBounds(Bounds bounds, float offset, List<GPUInstancerPrototype> prototypeFilter = null)
        {
            if (runtimeDataList != null)
            {
                foreach (GPUInstancerRuntimeData rd in runtimeDataList)
                {
                    if (prototypeFilter != null && !prototypeFilter.Contains(rd.prototype))
                        continue;
                    GPUInstancerUtility.RemoveInstancesInsideBounds(rd.transformationMatrixVisibilityBuffer, bounds.center, bounds.extents, offset);
                }
            }
        }
        public virtual void RemoveInstancesInsideCollider(Collider collider, float offset, List<GPUInstancerPrototype> prototypeFilter = null)
        {
            if (runtimeDataList != null)
            {
                foreach (GPUInstancerRuntimeData rd in runtimeDataList)
                {
                    if (prototypeFilter != null && !prototypeFilter.Contains(rd.prototype))
                        continue;
                    if (collider is BoxCollider)
                        GPUInstancerUtility.RemoveInstancesInsideBoxCollider(rd.transformationMatrixVisibilityBuffer, (BoxCollider)collider, offset);
                    else if (collider is SphereCollider)
                        GPUInstancerUtility.RemoveInstancesInsideSphereCollider(rd.transformationMatrixVisibilityBuffer, (SphereCollider)collider, offset);
                    else if (collider is CapsuleCollider)
                        GPUInstancerUtility.RemoveInstancesInsideCapsuleCollider(rd.transformationMatrixVisibilityBuffer, (CapsuleCollider)collider, offset);
                    else
                        GPUInstancerUtility.RemoveInstancesInsideBounds(rd.transformationMatrixVisibilityBuffer, collider.bounds.center, collider.bounds.extents, offset);
                }
            }
        }

        public virtual void SetGlobalPositionOffset(Vector3 offsetPosition)
        {
        }
        #endregion Virtual Methods

        #region Public Methods

        public void ClearCompletedThreads()
        {
            if (activeThreads.Count > 0)
            {
                for (int i = activeThreads.Count - 1; i >= 0; i--)
                {
                    if (!activeThreads[i].IsAlive)
                        activeThreads.RemoveAt(i);
                }
            }
        }

        public void InitializeCameraData()
        {
            if (autoSelectCamera || cameraData.mainCamera == null)
            {
                cameraData.SetCamera(Camera.main);
            }
        }

        public void SetupOcclusionCulling(GPUInstancerCameraData renderingCameraData)
        {
            if (renderingCameraData == null || renderingCameraData.mainCamera == null)
                return;

#if UNITY_EDITOR
            if (renderingCameraData.mainCamera.name == GPUInstancerEditorSimulator.sceneViewCameraName || !Application.isPlaying)
                return;
#endif

            if (isOcclusionCulling)
            {
                if (renderingCameraData.hiZOcclusionGenerator == null)
                {
                    GPUInstancerHiZOcclusionGenerator hiZOcclusionGenerator =
                        renderingCameraData.mainCamera.GetComponent<GPUInstancerHiZOcclusionGenerator>();

                    if (hiZOcclusionGenerator == null)
                        hiZOcclusionGenerator = renderingCameraData.mainCamera.gameObject.AddComponent<GPUInstancerHiZOcclusionGenerator>();

                    renderingCameraData.hiZOcclusionGenerator = hiZOcclusionGenerator;
                    renderingCameraData.hiZOcclusionGenerator.Initialize(renderingCameraData.mainCamera);
                }
            }
        }

        public void UpdateBuffers()
        {
            UpdateBuffers(cameraData);
        }

        public void UpdateBuffers(GPUInstancerCameraData renderingCameraData)
        {
            if (renderingCameraData != null && renderingCameraData.mainCamera != null && SystemInfo.supportsComputeShaders)
            {
                if (isOcclusionCulling && renderingCameraData.hiZOcclusionGenerator == null)
                    SetupOcclusionCulling(renderingCameraData);

                renderingCameraData.CalculateCameraData();

                instancingBounds.center = renderingCameraData.mainCamera.transform.position;

                if (lastDrawCallFrame != Time.frameCount)
                {
                    lastDrawCallFrame = Time.frameCount;
                    timeSinceLastDrawCall = Time.realtimeSinceStartup - lastDrawCallTime;
                    lastDrawCallTime = Time.realtimeSinceStartup;
                }

                UpdateSpatialPartitioningCells(renderingCameraData);

                GPUInstancerUtility.UpdateGPUBuffers(_cameraComputeShader, _cameraComputeKernelIDs, _visibilityComputeShader, _instanceVisibilityComputeKernelIDs, runtimeDataList, renderingCameraData, isFrustumCulling,
                    isOcclusionCulling, showRenderedAmount, isInitial);
                isInitial = false;

                if (GPUInstancerUtility.matrixHandlingType == GPUIMatrixHandlingType.CopyToTexture)
                    GPUInstancerUtility.DispatchBufferToTexture(runtimeDataList, _bufferToTextureComputeShader, _bufferToTextureComputeKernelID);

                GPUInstancerUtility.GPUIDrawMeshInstancedIndirect(runtimeDataList, instancingBounds, renderingCameraData, layerMask, lightProbeDisabled);
            }
        }

        public void SetCamera(Camera camera)
        {
            if (cameraData == null)
                cameraData = new GPUInstancerCameraData(camera);
            else
                cameraData.SetCamera(camera);

            if (cameraData.hiZOcclusionGenerator != null)
                DestroyImmediate(cameraData.hiZOcclusionGenerator);

            if (isOcclusionCulling)
            {
                cameraData.hiZOcclusionGenerator = cameraData.mainCamera.GetComponent<GPUInstancerHiZOcclusionGenerator>();
                if (cameraData.hiZOcclusionGenerator == null)
                {
                    cameraData.hiZOcclusionGenerator = cameraData.mainCamera.gameObject.AddComponent<GPUInstancerHiZOcclusionGenerator>();
                }
                cameraData.hiZOcclusionGenerator.Initialize(cameraData.mainCamera);
            }
        }

        public ComputeBuffer GetTransformDataBuffer(GPUInstancerPrototype prototype)
        {
            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
            if (runtimeData != null)
                return runtimeData.transformationMatrixVisibilityBuffer;
            return null;
        }

        public void SetLODBias(float newLODBias, List<GPUInstancerPrototype> prototypeFilter)
        {
            if (runtimeDataList == null || newLODBias <= 0)
                return;

            foreach (GPUInstancerRuntimeData runtimeData in runtimeDataList)
            {
                if (runtimeData == null || runtimeData.lodBiasApplied <= 0 || runtimeData.lodBiasApplied == newLODBias || runtimeData.instanceLODs == null
                    || runtimeData.instanceLODs.Count == 0 || (prototypeFilter != null && !prototypeFilter.Contains(runtimeData.prototype)))
                    continue;

                for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                {
                    if (i < 4)
                        runtimeData.lodSizes[i * 4] *= runtimeData.lodBiasApplied / newLODBias;
                    else
                        runtimeData.lodSizes[(i - 4) * 4 + 1] *= runtimeData.lodBiasApplied / newLODBias;
                }
                runtimeData.lodBiasApplied = newLODBias;
            }
        }

        #region Tree Instancing Methods
        public void UpdateTreeMPB()
        {
            if (treeProxyList != null && treeProxyList.Count > 0)
            {
                GPUInstancerPrototypeLOD rdLOD;
                GPUInstancerRenderer rdRenderer;
                MeshRenderer meshRenderer;
                Transform proxyTransform;
                foreach (GPUInstancerRuntimeData runtimeData in runtimeDataList)
                {
                    // Do not set append buffers if there is no instance of this tree prototype on the terrain
                    if (runtimeData.bufferSize == 0)
                        continue;

                    if (runtimeData.prototype.treeType != GPUInstancerTreeType.SpeedTree && runtimeData.prototype.treeType != GPUInstancerTreeType.SpeedTree8)
                        continue;

                    proxyTransform = treeProxyList[runtimeData.prototype.prefabObject];

                    if (!proxyTransform)
                        continue;

                    for (int lod = 0; lod < runtimeData.instanceLODs.Count; lod++)
                    {
                        if (proxyTransform.childCount <= lod)
                            continue;

                        rdLOD = runtimeData.instanceLODs[lod];
                        meshRenderer = proxyTransform.GetChild(lod).GetComponent<MeshRenderer>();

                        for (int r = 0; r < rdLOD.renderers.Count; r++)
                        {
                            rdRenderer = rdLOD.renderers[r];
                            //if (treeType == GPUInstancerTreeType.SoftOcclusionTree)
                            //{
                            //    // Soft occlusion shader wind parameters here.
                            //    // rdRenderer.mpb.SetFloat("_ShakeDisplacement", 0.8f);
                            //    continue;
                            //}

                            meshRenderer.GetPropertyBlock(rdRenderer.mpb);
                            if (rdRenderer.shadowMPB != null)
                                meshRenderer.GetPropertyBlock(rdRenderer.shadowMPB);
                        }
                    }

                    GPUInstancerUtility.SetAppendBuffers(runtimeData);
                }
            }
        }

        // Wind workaround:
        public static void AddTreeProxy(GPUInstancerPrototype treePrototype, GPUInstancerRuntimeData runtimeData)
        {
            switch (treePrototype.treeType)
            {
                case GPUInstancerTreeType.SpeedTree:
                case GPUInstancerTreeType.SpeedTree8:

                    if (treeProxyParent == null)
                    {
                        treeProxyParent = new GameObject("GPUI Tree Manager Proxy");
                        if (treeProxyList != null)
                            treeProxyList.Clear();
                    }

                    if (treeProxyList == null)
                    {
                        treeProxyList = new Dictionary<GameObject, Transform>(); // Dict[TreePrefab, TreeProxyGO]
                    }
                    else if (treeProxyList.ContainsKey(treePrototype.prefabObject))
                    {
                        if (treeProxyList[treePrototype.prefabObject] == null)
                            treeProxyList.Remove(treePrototype.prefabObject);
                        else
                            return;
                    }

                    Mesh treeProxyMesh = new Mesh();
                    treeProxyMesh.name = "TreeProxyMesh";

                    GameObject treeProxyObjectParent = new GameObject(treeProxyList.Count + "_" + treePrototype.name);
                    treeProxyObjectParent.transform.SetParent(treeProxyParent.transform);
                    treeProxyObjectParent.transform.localPosition = Vector3.zero;
                    treeProxyObjectParent.transform.localRotation = Quaternion.identity;
                    treeProxyList.Add(treePrototype.prefabObject, treeProxyObjectParent.transform);

                    if (treePrototype.prefabObject.GetComponent<LODGroup>() != null)
                    {
                        LOD[] speedTreeLODs = treePrototype.prefabObject.GetComponent<LODGroup>().GetLODs();
                        for (int lod = 0; lod < speedTreeLODs.Length; lod++)
                        {
                            if (speedTreeLODs[lod].renderers[0].GetComponent<BillboardRenderer>())
                                continue;

                            Material[] treeProxyMaterial = new Material[1] { new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_TREE_PROXY)) };

                            InstantiateTreeProxyObject(speedTreeLODs[lod].renderers[0].gameObject, treeProxyObjectParent, treeProxyMaterial, treeProxyMesh, lod == 0);
                        }
                    }
                    else
                    {
                        Material[] treeProxyMaterial = new Material[1] { new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_TREE_PROXY)) };

                        InstantiateTreeProxyObject(treePrototype.prefabObject, treeProxyObjectParent, treeProxyMaterial, treeProxyMesh, true);
                    }
                    break;

                case GPUInstancerTreeType.TreeCreatorTree:

                    // no need to create a TreeCreator proxy - setting the global wind vector instead
                    Shader.SetGlobalVector("_Wind", GetWindVector());

                    //Material[] treeCreatorProxyMaterials = new Material[2];
                    //treeCreatorProxyMaterials[0] = new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_TREE_PROXY));
                    //treeCreatorProxyMaterials[1] = new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_TREE_PROXY));
                    //InstantiateTreeProxyObject(treePrototype.prefabObject, treeProxyObjectParent, treeCreatorProxyMaterials, treeProxyMesh, true);
                    break;

            }
        }

        public static void InstantiateTreeProxyObject(GameObject treePrefab, GameObject proxyObjectParent, Material[] proxyMaterials, Mesh proxyMesh, bool setBounds)
        {
            GameObject treeProxyObject = Instantiate(treePrefab, proxyObjectParent.transform);
            treeProxyObject.name = treePrefab.name;

            if (setBounds)
                proxyMesh.bounds = treeProxyObject.GetComponent<MeshFilter>().sharedMesh.bounds;

            // Setup Tree Proxy object mesh renderer.
            MeshRenderer treeProxyObjectMR = treeProxyObject.GetComponent<MeshRenderer>();
            treeProxyObjectMR.shadowCastingMode = ShadowCastingMode.Off;
            treeProxyObjectMR.receiveShadows = false;
            treeProxyObjectMR.lightProbeUsage = LightProbeUsage.Off;

            for (int i = 0; i < proxyMaterials.Length; i++)
            {
                proxyMaterials[i].CopyPropertiesFromMaterial(treeProxyObjectMR.materials[i]);
                proxyMaterials[i].enableInstancing = true;
            }

            treeProxyObjectMR.sharedMaterials = proxyMaterials;
            treeProxyObjectMR.GetComponent<MeshFilter>().sharedMesh = proxyMesh;

            // Strip all unwanted components potentially on the tree prefab:
            Component[] allComponents = treeProxyObject.GetComponents(typeof(Component));
            for (int i = 0; i < allComponents.Length; i++)
            {
                if (allComponents[i] is Transform || allComponents[i] is MeshFilter ||
                    allComponents[i] is MeshRenderer || allComponents[i] is Tree)
                    continue;

                Destroy(allComponents[i]);
            }
        }
        #endregion Tree Instancing Methods

        #region Global Wind Methods

        public static Vector4 GetWindVector()
        {
            if (_windVector != Vector4.zero)
                return _windVector;

            UpdateSceneWind();

            return _windVector;
        }

        public static void UpdateSceneWind()
        {
            WindZone[] sceneWindZones = FindObjectsOfType<WindZone>();

            for (int i = 0; i < sceneWindZones.Length; i++)
            {
                if (sceneWindZones[i].mode == WindZoneMode.Directional)
                {
                    _windVector = new Vector4(sceneWindZones[i].windTurbulence, sceneWindZones[i].windPulseMagnitude, sceneWindZones[i].windPulseFrequency, sceneWindZones[i].windMain);
                    break;
                }
            }
        }

        #endregion Wind Methods

        public Exception threadException;
        public void LogThreadException()
        {
            Debug.LogException(threadException);
        }

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
        public void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            playModeState = state;
        }
#endif

        public GPUInstancerRuntimeData GetRuntimeData(GPUInstancerPrototype prototype, bool logError = false)
        {
            GPUInstancerRuntimeData runtimeData = null;
            if (!runtimeDataDictionary.TryGetValue(prototype, out runtimeData) && logError)
                Debug.LogError("Can not find runtime data for prototype: " + prototype + ". Please check if the prototype was added to the Manager and the initialize method was called.");
            return runtimeData;
        }
        #endregion Public Methods
    }

}