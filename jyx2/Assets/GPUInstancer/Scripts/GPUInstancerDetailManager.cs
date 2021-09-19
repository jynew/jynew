using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    /// <summary>
    /// Add this to a Unity terrain for GPU Instancing details at runtime.
    /// </summary>
    [ExecuteInEditMode]
    public class GPUInstancerDetailManager : GPUInstancerTerrainManager
    {
        public int detailLayer;

        public bool runInThreads = false;

        private static ComputeShader _grassInstantiationComputeShader;

        private ComputeBuffer _generatingVisibilityBuffer;
#if !UNITY_2017_1_OR_NEWER || UNITY_ANDROID
        private ComputeBuffer _managedBuffer;
        private Matrix4x4[] _managedData;
#endif
        private bool _triggerEvent;
        private ComputeBuffer counterBuffer;
        private int[] counterData = new int[1];

        #region MonoBehaviour Methods
        public override void Awake()
        {
            base.Awake();

            if (_grassInstantiationComputeShader == null)
                _grassInstantiationComputeShader = (ComputeShader)Resources.Load(GPUInstancerConstants.GRASS_INSTANTIATION_RESOURCE_PATH);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            if (_generatingVisibilityBuffer != null)
            {
                _generatingVisibilityBuffer.Release();
            }
        }
        #endregion MonoBehaviour Methods

        #region Override Methods

        public override void ClearInstancingData()
        {
            base.ClearInstancingData();

            if (terrain != null && terrain.detailObjectDistance == 0)
            {
                terrain.detailObjectDistance = terrainSettings.maxDetailDistance > 250 ? 250 : terrainSettings.maxDetailDistance;
            }

#if !UNITY_2017_1_OR_NEWER || UNITY_ANDROID
            if (_managedBuffer != null)
                _managedBuffer.Release();
            _managedBuffer = null;
#endif

            if (counterBuffer != null)
                counterBuffer.Release();
            counterBuffer = null;
        }

        public override void GeneratePrototypes(bool forceNew = false)
        {
            base.GeneratePrototypes(forceNew);

            if (terrainSettings != null && terrain != null && terrain.terrainData != null)
            {
                GPUInstancerUtility.SetDetailInstancePrototypes(gameObject, prototypeList, terrain.terrainData.detailPrototypes, 2, terrainSettings, forceNew);
            }
        }

#if UNITY_EDITOR
        public override void CheckPrototypeChanges()
        {
            base.CheckPrototypeChanges();

            if (!Application.isPlaying && terrainSettings != null && terrain != null && terrain.terrainData != null)
            {
                if (prototypeList.Count != terrain.terrainData.detailPrototypes.Length)
                {
                    GeneratePrototypes();
                }

                int index = 0;
                foreach (GPUInstancerDetailPrototype prototype in prototypeList)
                {
                    prototype.prototypeIndex = index;
                    index++;
                }

                if (prototypeList.Count != terrain.terrainData.detailPrototypes.Length)
                    terrainSettings.warningText = "Detail Prototypes on the Unity Terrain do not match the Detail Prototypes in this Detail Manager. Adding and removing Detail Prototypes should be done from the Detail Manager and not from the Unity Terrain. To fix this error, you can press the Generate Prototypes button below. This will reset the prototypes in this manager with the default settings.";
                else
                    terrainSettings.warningText = null;

                AddProxyToTerrain();
            }
        }
#endif
        public override void InitializeRuntimeDataAndBuffers(bool forceNew = true)
        {
            base.InitializeRuntimeDataAndBuffers(forceNew);

            if (!forceNew && isInitialized)
                return;

            if (terrainSettings == null)
                return;

            if (!string.IsNullOrEmpty(terrainSettings.warningText))
            {
                Debug.LogError("A GPU Instancer Detail Manager currently has errors. Please refer to the error description in the Detail manager.");
                return;
            }

            replacingInstances = false;
            initalizingInstances = true;

            if (prototypeList != null && prototypeList.Count > 0)
            {
                GPUInstancerUtility.AddDetailInstanceRuntimeDataToList(runtimeDataList, prototypeList, terrainSettings, detailLayer);
            }

            terrain.detailObjectDistance = 0;

            InitializeSpatialPartitioning();

            isInitialized = true;
        }

        public override void UpdateSpatialPartitioningCells(GPUInstancerCameraData renderingCameraData)
        {
            base.UpdateSpatialPartitioningCells(renderingCameraData);

            if (terrainSettings == null || spData == null)
                return;

            if (!initalizingInstances && !replacingInstances && spData.IsActiveCellUpdateRequired(renderingCameraData.mainCamera.transform.position))
            {
                replacingInstances = true;
                if (GPUInstancerConstants.DETAIL_STORE_INSTANCE_DATA)
                    StartCoroutineAlt(GenerateVisibilityBufferFromActiveCellsCoroutine());
                else
                    StartCoroutineAlt(MergeVisibilityBufferFromActiveCellsCoroutine());
            }
        }

        public override void DeletePrototype(GPUInstancerPrototype prototype, bool removeSO = true)
        {
            if (terrainSettings != null && terrain != null && terrain.terrainData != null)
            {
                int detailPrototypeIndex = prototypeList.IndexOf(prototype);

                DetailPrototype[] detailPrototypes = terrain.terrainData.detailPrototypes;
                List<DetailPrototype> newDetailPrototypes = new List<DetailPrototype>();
                List<int[,]> newDetailLayers = new List<int[,]>();

                for (int i = 0; i < detailPrototypes.Length; i++)
                {
                    if (i != detailPrototypeIndex)
                    {
                        newDetailPrototypes.Add(detailPrototypes[i]);
                        newDetailLayers.Add(terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailResolution, terrain.terrainData.detailResolution, i));
                    }
                    terrain.terrainData.SetDetailLayer(0, 0, i, new int[terrain.terrainData.detailResolution, terrain.terrainData.detailResolution]);
                }

                terrain.terrainData.detailPrototypes = newDetailPrototypes.ToArray();
                for (int i = 0; i < newDetailLayers.Count; i++)
                {
                    terrain.terrainData.SetDetailLayer(0, 0, i, newDetailLayers[i]);
                }
                terrain.terrainData.RefreshPrototypes();

                if (removeSO)
                    base.DeletePrototype(prototype, removeSO);
                GeneratePrototypes(false);
                if (!removeSO)
                    base.DeletePrototype(prototype, removeSO);
            }
            else
                base.DeletePrototype(prototype, removeSO);
        }

        public override void RemoveInstancesInsideBounds(Bounds bounds, float offset, List<GPUInstancerPrototype> prototypeFilter = null)
        {
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.RemoveInstancesInsideBounds");
//#endif
            base.RemoveInstancesInsideBounds(bounds, offset, prototypeFilter);
            if (spData != null && !initalizingInstances)
            {
                int detailMapSize = terrain.terrainData.detailResolution / spData.cellRowAndCollumnCountPerTerrain;
                float unitSizeX = terrain.terrainData.size.x / spData.cellRowAndCollumnCountPerTerrain / detailMapSize;
                float unitSizeZ = terrain.terrainData.size.z / spData.cellRowAndCollumnCountPerTerrain / detailMapSize;
                int sizeX = Mathf.CeilToInt((bounds.extents.x * 2 + offset) / unitSizeX);
                int sizeZ = Mathf.CeilToInt((bounds.extents.z * 2 + offset) / unitSizeZ);

                foreach (GPUInstancerDetailCell detailCell in spData.GetCellList())
                {
                    if (detailCell.cellInnerBounds.Intersects(bounds))
                    {
                        if (detailCell.isActive && detailCell.detailInstanceBuffers != null)
                        {
                            foreach (int i in detailCell.detailInstanceBuffers.Keys)
                            {
                                if (prototypeFilter != null && !prototypeFilter.Contains(prototypeList[i]))
                                    continue;
                                GPUInstancerUtility.RemoveInstancesInsideBounds(detailCell.detailInstanceBuffers[i], bounds.center, bounds.extents, offset);
                            }
                        }
                        int startX = Mathf.FloorToInt((bounds.center.x - bounds.extents.x - detailCell.instanceStartPosition.x - offset) / unitSizeX);
                        int startZ = Mathf.FloorToInt((bounds.center.z - bounds.extents.z - detailCell.instanceStartPosition.z - offset) / unitSizeZ);

                        for (int i = 0; i < detailCell.detailMapData.Count; i++)
                        {
                            if (prototypeFilter != null && !prototypeFilter.Contains(prototypeList[i]))
                                continue;
                            for (int y = startZ; y < sizeZ + startZ; y++)
                            {
                                if (y < 0 || y >= detailMapSize)
                                    continue;
                                for (int x = startX; x < sizeX + startX; x++)
                                {
                                    if (x < 0 || x >= detailMapSize)
                                        continue;
                                    detailCell.detailMapData[i][x + y * detailMapSize] = 0;
                                }
                            }
                        }
                    }
                }
            }
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.EndSample();
//#endif
        }

        public override void RemoveInstancesInsideCollider(Collider collider, float offset, List<GPUInstancerPrototype> prototypeFilter = null)
        {
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.RemoveInstancesInsideCollider");
//#endif
            base.RemoveInstancesInsideCollider(collider, offset, prototypeFilter);
            if (spData != null && !initalizingInstances)
            {
                Bounds bounds = collider.bounds;
                int detailMapSize = terrain.terrainData.detailResolution / spData.cellRowAndCollumnCountPerTerrain;
                float unitSizeX = terrain.terrainData.size.x / spData.cellRowAndCollumnCountPerTerrain / detailMapSize;
                float unitSizeZ = terrain.terrainData.size.z / spData.cellRowAndCollumnCountPerTerrain / detailMapSize;
                int sizeX = Mathf.CeilToInt((bounds.extents.x * 2 + offset) / unitSizeX);
                int sizeZ = Mathf.CeilToInt((bounds.extents.z * 2 + offset) / unitSizeZ);

                Vector3 testCenter = Vector3.zero;
                Vector3 closestPoint = Vector3.zero;

                foreach (GPUInstancerDetailCell detailCell in spData.GetCellList())
                {
                    if (detailCell.cellInnerBounds.Intersects(bounds))
                    {
                        if (detailCell.isActive && detailCell.detailInstanceBuffers != null)
                        {
                            foreach (int i in detailCell.detailInstanceBuffers.Keys)
                            {
                                if (prototypeFilter != null && !prototypeFilter.Contains(prototypeList[i]))
                                    continue;

                                if (collider is BoxCollider)
                                    GPUInstancerUtility.RemoveInstancesInsideBoxCollider(detailCell.detailInstanceBuffers[i], (BoxCollider)collider, offset);
                                else if (collider is SphereCollider)
                                    GPUInstancerUtility.RemoveInstancesInsideSphereCollider(detailCell.detailInstanceBuffers[i], (SphereCollider)collider, offset);
                                else if (collider is CapsuleCollider)
                                    GPUInstancerUtility.RemoveInstancesInsideCapsuleCollider(detailCell.detailInstanceBuffers[i], (CapsuleCollider)collider, offset);
                                else
                                    GPUInstancerUtility.RemoveInstancesInsideBounds(detailCell.detailInstanceBuffers[i], collider.bounds.center, collider.bounds.extents, offset);
                            }
                        }
                        int startX = Mathf.FloorToInt((bounds.center.x - bounds.extents.x - detailCell.instanceStartPosition.x - offset) / unitSizeX);
                        int startZ = Mathf.FloorToInt((bounds.center.z - bounds.extents.z - detailCell.instanceStartPosition.z - offset) / unitSizeZ);

                        for (int y = startZ; y < sizeZ + startZ; y++)
                        {
                            if (y < 0 || y >= detailMapSize)
                                continue;
                            for (int x = startX; x < sizeX + startX; x++)
                            {
                                if (x < 0 || x >= detailMapSize)
                                    continue;

                                testCenter.x = detailCell.instanceStartPosition.x + x * unitSizeX;
                                testCenter.z = detailCell.instanceStartPosition.z + y * unitSizeZ;
                                testCenter.y = terrain.SampleHeight(testCenter);
                                closestPoint = collider.ClosestPoint(testCenter);
                                if (Vector3.Distance(closestPoint, testCenter) > unitSizeX + offset)
                                    continue;
                                for (int i = 0; i < detailCell.detailMapData.Count; i++)
                                {
                                    if (prototypeFilter != null && !prototypeFilter.Contains(prototypeList[i]))
                                        continue;
                                    detailCell.detailMapData[i][x + y * detailMapSize] = 0;
                                }
                            }
                        }
                    }
                }
            }
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.EndSample();
//#endif
        }

        public override void SetGlobalPositionOffset(Vector3 offsetPosition)
        {
            base.SetGlobalPositionOffset(offsetPosition);
            if (spData != null)
            {
                foreach (GPUInstancerDetailCell cell in spData.GetCellList())
                {
                    if (cell == null)
                        continue;
                    cell.instanceStartPosition += offsetPosition;
                    cell.cellBounds.center += offsetPosition;
                    if (cell.detailInstanceBuffers != null)
                    {
                        foreach (ComputeBuffer detailBuffer in cell.detailInstanceBuffers.Values)
                        {
                            if (detailBuffer != null)
                            {
                                GPUInstancerConstants.computeRuntimeModification.SetBuffer(GPUInstancerConstants.computeBufferTransformOffsetId,
                                    GPUInstancerConstants.VisibilityKernelPoperties.INSTANCE_DATA_BUFFER, detailBuffer);
                                GPUInstancerConstants.computeRuntimeModification.SetInt(
                                    GPUInstancerConstants.VisibilityKernelPoperties.BUFFER_PARAMETER_BUFFER_SIZE, detailBuffer.count);
                                GPUInstancerConstants.computeRuntimeModification.SetVector(
                                    GPUInstancerConstants.RuntimeModificationKernelProperties.BUFFER_PARAMETER_POSITION_OFFSET, offsetPosition);

                                GPUInstancerConstants.computeRuntimeModification.Dispatch(GPUInstancerConstants.computeBufferTransformOffsetId,
                                    Mathf.CeilToInt(detailBuffer.count / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT), 1, 1);
                            }
                        }
                    }
                    if (GPUInstancerConstants.DETAIL_STORE_INSTANCE_DATA && cell.detailInstanceList != null)
                    {
                        foreach (Matrix4x4[] instanceDataArray in cell.detailInstanceList.Values)
                        {
                            for (int i = 0; i < instanceDataArray.Length; i++)
                            {
                                instanceDataArray[i].SetColumn(3, instanceDataArray[i].GetColumn(3) + new Vector4(offsetPosition.x, offsetPosition.y, offsetPosition.z, 0));
                            }
                        }
                    }
                }
            }
        }
        #endregion Override Methods

        #region Helper Methods
        private static int FixBounds(int value, int max, int failValue)
        {
            if (value >= max)
                return failValue;
            return value;
        }

        public static Matrix4x4[] GetInstanceDataForDetailPrototype(GPUInstancerDetailPrototype detailPrototype, int[] detailMap, float[] heightMapData,
                                                                int detailMapSize, int heightMapSize,
                                                                int detailResolution, int heightResolution,
                                                                Vector3 startPosition, Vector3 terrainSize,
                                                                int instanceCount)
        {
            Matrix4x4[] result = new Matrix4x4[instanceCount];

            if (instanceCount == 0)
                return result;

            System.Random randomNumberGenerator = new System.Random();
            float detailHeightMapScale = (heightResolution - 1.0f) / detailResolution;
            int heightDataSize = heightMapSize * heightMapSize;
            float sizeDetailXScale = terrainSize.x / detailResolution;
            float sizeDetailZScale = terrainSize.z / detailResolution;
            float normalScale = heightResolution / (terrainSize.x / terrainSize.y);

            float px, py, leftBottomH, leftTopH, rightBottomH, rightTopH;
            int heightIndex;
            Vector3 position;
            Vector3 scale = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            Vector3 P = Vector3.zero;
            Vector3 Q = new Vector3(0, 0, 1);
            Vector3 R = new Vector3(1, 0, 0);
            Vector3 terrainPointNormal = Vector3.zero;

            int counter = 0;
            for (int y = 0; y < detailMapSize; y++)
            {
                for (int x = 0; x < detailMapSize; x++)
                {
                    for (int j = 0; j < detailMap[y * detailMapSize + x]; j++) // for the amount of detail at this point and for this prototype
                    {
                        position.x = x + randomNumberGenerator.Range(0f, 0.99f);
                        position.y = 0;
                        position.z = y + randomNumberGenerator.Range(0f, 0.99f);

                        // set height
                        px = position.x * detailHeightMapScale;
                        py = position.z * detailHeightMapScale;
                        heightIndex = Mathf.FloorToInt(px) + Mathf.FloorToInt(py) * heightMapSize;
                        leftBottomH = heightMapData[heightIndex];
                        leftTopH = heightMapData[FixBounds(heightIndex + heightMapSize, heightDataSize, heightIndex)];
                        rightBottomH = heightMapData[heightIndex + 1];
                        rightTopH = heightMapData[FixBounds(heightIndex + heightMapSize + 1, heightDataSize, heightIndex)];

                        position.x *= sizeDetailXScale;
                        position.y = GPUInstancerUtility.SampleTerrainHeight(px - Mathf.Floor(px), py - Mathf.Floor(py), leftBottomH, leftTopH, rightBottomH, rightTopH) * terrainSize.y;
                        position.z *= sizeDetailZScale;
                        position += startPosition;

                        // get normal
                        //Vector3 terrainPointNormal = GPUInstancerUtility.ComputeTerrainNormal(leftBottomH, leftTopH, rightBottomH, normalScale);
                        P.y = leftBottomH * normalScale;
                        Q.y = leftTopH * normalScale;
                        P.y = rightBottomH * normalScale;
                        terrainPointNormal = Vector3.Cross(Q - R, R - P).normalized;

                        rotation.SetFromToRotation(Vector3.up, terrainPointNormal);
                        rotation *= Quaternion.AngleAxis(randomNumberGenerator.Range(0.0f, 360.0f), Vector3.up);

                        float randomScale = randomNumberGenerator.Range(0.0f, 1.0f);

                        float xzScale = detailPrototype.detailScale.x + (detailPrototype.detailScale.y - detailPrototype.detailScale.x) * randomScale;
                        float yScale = detailPrototype.detailScale.z + (detailPrototype.detailScale.w - detailPrototype.detailScale.z) * randomScale;

                        scale.x = xzScale;
                        scale.y = yScale;
                        scale.z = xzScale;

                        result[counter].SetTRS(position, rotation, scale);
                        counter++;
                    }
                }
            }

            return result;
        }

        private static Matrix4x4[] GetInstanceDataForDetailPrototypeWithComputeShader(GPUInstancerDetailPrototype detailPrototype, int[] detailMap, float[] heightMapData,
                                                                int detailMapSize, int heightMapSize,
                                                                int detailResolution, int heightResolution,
                                                                Vector3 startPosition, Vector3 terrainSize,
                                                                int instanceCount,
                                                                ComputeShader grassInstantiationComputeShader, GPUInstancerTerrainSettings terrainSettings,
                                                                ComputeBuffer counterBuffer, int[] counterData)
        {
            Matrix4x4[] result = new Matrix4x4[instanceCount];

            if (instanceCount == 0)
                return result;

            ComputeBuffer visibilityBuffer;

            // set compute shader
            int grassInstantiationComputeKernelId = grassInstantiationComputeShader.FindKernel(GPUInstancerConstants.GRASS_INSTANTIATION_KERNEL);

            ComputeBuffer heightMapBuffer = new ComputeBuffer(heightMapData.Length, GPUInstancerConstants.STRIDE_SIZE_INT);
            heightMapBuffer.SetData(heightMapData);

            visibilityBuffer = new ComputeBuffer(instanceCount, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);

            counterBuffer.SetData(counterData);

            ComputeBuffer detailMapBuffer = new ComputeBuffer(Mathf.CeilToInt(detailMapSize * detailMapSize), GPUInstancerConstants.STRIDE_SIZE_INT);
            detailMapBuffer.SetData(detailMap);

            // dispatch compute shader
            DispatchDetailComputeShader(grassInstantiationComputeShader, grassInstantiationComputeKernelId,
                visibilityBuffer, detailMapBuffer, heightMapBuffer,
                new Vector4(detailMapSize, detailMapSize, heightMapSize, heightMapSize), startPosition, terrainSize, detailResolution, heightResolution, detailPrototype.detailScale,
                terrainSettings.GetHealthyDryNoiseTexture(detailPrototype), detailPrototype.noiseSpread, detailPrototype.GetInstanceID(), detailPrototype.detailDensity, counterBuffer);

            detailMapBuffer.Release();

            visibilityBuffer.GetData(result);
            visibilityBuffer.Release();

            heightMapBuffer.Release();

            return result;
        }

        private static ComputeBuffer GetComputeBufferForDetailPrototypeWithComputeShader(GPUInstancerDetailPrototype detailPrototype,
                                                                int detailMapSize, int heightMapSize,
                                                                int detailResolution, int heightResolution,
                                                                Vector3 startPosition, Vector3 terrainSize,
                                                                int instanceCount,
                                                                ComputeShader grassInstantiationComputeShader, GPUInstancerTerrainSettings terrainSettings,
                                                                ComputeBuffer heightMapBuffer, ComputeBuffer detailMapBuffer, ComputeBuffer counterBuffer, int[] counterData)
        {
            if (instanceCount == 0)
                return null;

            ComputeBuffer visibilityBuffer;

            // set compute shader
            int grassInstantiationComputeKernelId = grassInstantiationComputeShader.FindKernel(GPUInstancerConstants.GRASS_INSTANTIATION_KERNEL);

            visibilityBuffer = new ComputeBuffer(instanceCount, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);

            counterBuffer.SetData(counterData);

            // dispatch compute shader
            DispatchDetailComputeShader(grassInstantiationComputeShader, grassInstantiationComputeKernelId,
                visibilityBuffer, detailMapBuffer, heightMapBuffer,
                new Vector4(detailMapSize, detailMapSize, heightMapSize, heightMapSize), startPosition, terrainSize, detailResolution, heightResolution, detailPrototype.detailScale,
                terrainSettings.GetHealthyDryNoiseTexture(detailPrototype), detailPrototype.noiseSpread, detailPrototype.GetInstanceID(), detailPrototype.detailDensity, counterBuffer);

            return visibilityBuffer;
        }

        private static void DispatchDetailComputeShader(ComputeShader grassComputeShader, int grassInstantiationComputeKernelId,
            ComputeBuffer visibilityBuffer, ComputeBuffer detailMapBuffer, ComputeBuffer heightMapBuffer,
            Vector4 detailAndHeightMapSize, Vector3 startPosition, Vector3 terrainSize, int detailResolution, int heightResolution, Vector4 detailScale, Texture healthyDryNoiseTexture,
            float noiseSpread, int instanceID, float detailDensity, ComputeBuffer counterBuffer)
        {
            // setup compute shader
            grassComputeShader.SetBuffer(grassInstantiationComputeKernelId, GPUInstancerConstants.VisibilityKernelPoperties.INSTANCE_DATA_BUFFER, visibilityBuffer);
            grassComputeShader.SetBuffer(grassInstantiationComputeKernelId, GPUInstancerConstants.GrassKernelProperties.DETAIL_MAP_DATA_BUFFER, detailMapBuffer);
            grassComputeShader.SetBuffer(grassInstantiationComputeKernelId, GPUInstancerConstants.GrassKernelProperties.HEIGHT_MAP_DATA_BUFFER, heightMapBuffer);
            grassComputeShader.SetBuffer(grassInstantiationComputeKernelId, GPUInstancerConstants.GrassKernelProperties.COUNTER_BUFFER, counterBuffer);
            grassComputeShader.SetInt(GPUInstancerConstants.GrassKernelProperties.TERRAIN_DETAIL_RESOLUTION_DATA, detailResolution);
            grassComputeShader.SetInt(GPUInstancerConstants.GrassKernelProperties.TERRAIN_HEIGHT_RESOLUTION_DATA, heightResolution);
            grassComputeShader.SetVector(GPUInstancerConstants.GrassKernelProperties.GRASS_START_POSITION_DATA, startPosition);
            grassComputeShader.SetVector(GPUInstancerConstants.GrassKernelProperties.TERRAIN_SIZE_DATA, terrainSize);
            grassComputeShader.SetVector(GPUInstancerConstants.GrassKernelProperties.DETAIL_SCALE_DATA, detailScale);
            grassComputeShader.SetVector(GPUInstancerConstants.GrassKernelProperties.DETAIL_AND_HEIGHT_MAP_SIZE_DATA, detailAndHeightMapSize);
            if (healthyDryNoiseTexture != null)
            {
                grassComputeShader.SetTexture(grassInstantiationComputeKernelId, GPUInstancerConstants.GrassKernelProperties.HEALTHY_DRY_NOISE_TEXTURE, healthyDryNoiseTexture);
                grassComputeShader.SetFloat(GPUInstancerConstants.GrassKernelProperties.NOISE_SPREAD, noiseSpread);
            }
            grassComputeShader.SetFloat(GPUInstancerConstants.GrassKernelProperties.DETAIL_UNIQUE_VALUE, instanceID / 1000f);
            grassComputeShader.SetFloat(GPUInstancerConstants.GrassKernelProperties.DETAIL_DENSITY, detailDensity);

            // dispatch
            grassComputeShader.Dispatch(grassInstantiationComputeKernelId,
                Mathf.CeilToInt(detailAndHeightMapSize.x / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D),
                1,
                Mathf.CeilToInt(detailAndHeightMapSize.y / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT_2D));
        }

        private void StartCoroutineAlt(IEnumerator routine)
        {
            if (Application.isPlaying)
                StartCoroutine(routine);
            else
                while (routine.MoveNext()) ;
        }
        #endregion Helper Methods

        #region Spatial Partitioning Cell Management
        public override void InitializeSpatialPartitioning()
        {
            base.InitializeSpatialPartitioning();

            GPUInstancerUtility.ReleaseSPBuffers(spData);
            spData = new GPUInstancerSpatialPartitioningData<GPUInstancerCell>();
            GPUInstancerUtility.CalculateSpatialPartitioningValuesFromTerrain(spData, terrain, terrainSettings.maxDetailDistance, terrainSettings.autoSPCellSize ? 0 : terrainSettings.preferedSPCellSize);

            // initialize cells
            GenerateCellsInstanceDataFromTerrain();
        }

        private IEnumerator GenerateVisibilityBufferFromActiveCellsCoroutine()
        {
            if (!initalizingInstances)
            {
#if !UNITY_2017_1_OR_NEWER || UNITY_ANDROID
                if (_managedBuffer == null)
                    _managedBuffer = new ComputeBuffer(GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);
                if (_managedData == null)
                    _managedData = new Matrix4x4[GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER];
#endif
                List<GPUInstancerRuntimeData> runtimeDatas = new List<GPUInstancerRuntimeData>(runtimeDataList);

                int totalCount = 0;
                int lastbreak = 0;
                for (int r = 0; r < runtimeDatas.Count; r++)
                {
                    GPUInstancerRuntimeData rd = runtimeDatas[r];

                    if (spData.activeCellList != null && spData.activeCellList.Count > 0)
                    {
                        int totalInstanceCount = 0;

                        foreach (GPUInstancerDetailCell cell in spData.activeCellList)
                        {
                            if (cell != null && cell.detailInstanceList != null)
                            {
                                totalInstanceCount += cell.detailInstanceList[r].Length;
                            }
                        }

                        rd.bufferSize = totalInstanceCount;
                        rd.instanceCount = totalInstanceCount;

                        if (totalInstanceCount == 0)
                        {
                            if (rd.transformationMatrixVisibilityBuffer != null)
                                rd.transformationMatrixVisibilityBuffer.Release();
                            rd.transformationMatrixVisibilityBuffer = null;
                            continue;
                        }

                        _generatingVisibilityBuffer = new ComputeBuffer(totalInstanceCount, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);

                        int startIndex = 0;
                        int cellStartIndex = 0;
                        int count = 0;
                        for (int c = 0; c < spData.activeCellList.Count; c++)
                        {
                            GPUInstancerDetailCell detailCell = (GPUInstancerDetailCell)spData.activeCellList[c];
                            cellStartIndex = 0;
                            for (int i = 0; i < Mathf.Ceil((float)detailCell.detailInstanceList[r].Length / (float)GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER); i++)
                            {
                                cellStartIndex = i * GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER;
                                count = GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER;
                                if (cellStartIndex + count > detailCell.detailInstanceList[r].Length)
                                    count = detailCell.detailInstanceList[r].Length - cellStartIndex;
#if UNITY_2017_1_OR_NEWER && !UNITY_ANDROID
                                _generatingVisibilityBuffer.SetDataPartial(detailCell.detailInstanceList[r], cellStartIndex, startIndex, count);
#else
                                _generatingVisibilityBuffer.SetDataPartial(detailCell.detailInstanceList[r], cellStartIndex, startIndex, count, _managedBuffer, _managedData);
#endif
                                startIndex += count;
                                totalCount += count;

                                if (count + cellStartIndex < detailCell.detailInstanceList[r].Length - 1 && totalCount - lastbreak > GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER)
                                {
                                    lastbreak = totalCount;
                                    yield return null;
                                }
                            }

                            if (initalizingInstances)
                                break;
                        }
                        if (initalizingInstances)
                            break;

                        if (initalizingInstances)
                            break;

                        if (rd.transformationMatrixVisibilityBuffer != null)
                            rd.transformationMatrixVisibilityBuffer.Release();
                        rd.transformationMatrixVisibilityBuffer = _generatingVisibilityBuffer;
                    }
                    if (initalizingInstances)
                        break;

                    GPUInstancerUtility.InitializeGPUBuffer(rd);
                    lastbreak = totalCount;
                    yield return null;
                }
                if (initalizingInstances)
                {
                    if (_generatingVisibilityBuffer != null)
                        _generatingVisibilityBuffer.Release();
                    GPUInstancerUtility.ReleaseInstanceBuffers(runtimeDatas);
                    GPUInstancerUtility.ClearInstanceData(runtimeDatas);
                }

                _generatingVisibilityBuffer = null;
                replacingInstances = false;

                if (!initalizingInstances)
                {
                    if (_triggerEvent)
                        GPUInstancerUtility.TriggerEvent(GPUInstancerEventType.DetailInitializationFinished);
                    _triggerEvent = false;
                    isInitial = true;
                }
            }
        }

        private IEnumerator MergeVisibilityBufferFromActiveCellsCoroutine()
        {
            if (!initalizingInstances)
            {
                List<GPUInstancerRuntimeData> runtimeDatas = new List<GPUInstancerRuntimeData>(runtimeDataList);

                int detailMapSize = terrain.terrainData.detailResolution / spData.cellRowAndCollumnCountPerTerrain;
                int heightMapSize = (terrain.terrainData.heightmapResolution - 1) / spData.cellRowAndCollumnCountPerTerrain + 1;
                int detailResolution = terrain.terrainData.detailResolution;
                int heightmapResolution = terrain.terrainData.heightmapResolution;
                Vector3 terrainSize = terrain.terrainData.size;
                ComputeBuffer heightMapBuffer = null;
                ComputeBuffer detailMapBuffer = null;
                GPUInstancerDetailCell heightMapCell = null;
                int generatedBuffers = 0;

                if (spData.activeCellList != null && spData.activeCellList.Count > 0)
                {
                    foreach (GPUInstancerDetailCell cell in spData.activeCellList)
                    {
                        generatedBuffers = 0;
                        if (cell != null && cell.totalDetailCounts != null)
                        {
                            if (cell.detailInstanceBuffers == null)
                                cell.detailInstanceBuffers = new Dictionary<int, ComputeBuffer>();

                            for (int r = 0; r < runtimeDatas.Count; r++)
                            {
                                if (cell.totalDetailCounts[r] > 0)
                                {
                                    if (!cell.detailInstanceBuffers.ContainsKey(r) || cell.detailInstanceBuffers[r] == null)
                                    {
#if UNITY_EDITOR
                                        UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.MergeVisibilityBufferFromActiveCellsCoroutine");
#endif

                                        if (heightMapCell != cell)
                                        {
                                            if (heightMapBuffer == null)
                                                heightMapBuffer = new ComputeBuffer(cell.heightMapData.Length, GPUInstancerConstants.STRIDE_SIZE_INT);
                                            heightMapBuffer.SetData(cell.heightMapData);
                                            heightMapCell = cell;
                                        }

                                        if (detailMapBuffer == null)
                                            detailMapBuffer = new ComputeBuffer(detailMapSize * detailMapSize, GPUInstancerConstants.STRIDE_SIZE_INT);
                                        detailMapBuffer.SetData(cell.detailMapData[r]);

                                        cell.detailInstanceBuffers[r] =
                                            GetComputeBufferForDetailPrototypeWithComputeShader((GPUInstancerDetailPrototype)prototypeList[r],
                                                detailMapSize, heightMapSize,
                                                detailResolution, heightmapResolution,
                                                cell.instanceStartPosition,
                                                terrainSize, cell.totalDetailCounts[r], _grassInstantiationComputeShader, terrainSettings,
                                                heightMapBuffer, detailMapBuffer, counterBuffer, counterData);
                                        generatedBuffers += detailMapSize;

#if UNITY_EDITOR
                                        UnityEngine.Profiling.Profiler.EndSample();
#endif
                                        if (generatedBuffers >= GPUInstancerConstants.DETAIL_BUFFER_MERGE_FRAME_LIMIT)
                                        {
                                            generatedBuffers = 0;
                                            yield return null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int r = 0; r < runtimeDatas.Count; r++)
                {
                    GPUInstancerRuntimeData rd = runtimeDatas[r];

                    if (spData.activeCellList != null && spData.activeCellList.Count > 0)
                    {
                        int totalInstanceCount = 0;

                        foreach (GPUInstancerDetailCell cell in spData.activeCellList)
                        {
                            if (cell != null && cell.totalDetailCounts != null)
                            {
                                totalInstanceCount += cell.totalDetailCounts[r];
                            }
                        }

                        rd.bufferSize = totalInstanceCount;
                        rd.instanceCount = totalInstanceCount;

                        if (totalInstanceCount == 0)
                        {
                            if (rd.transformationMatrixVisibilityBuffer != null)
                                rd.transformationMatrixVisibilityBuffer.Release();
                            rd.transformationMatrixVisibilityBuffer = null;
                            continue;
                        }

                        _generatingVisibilityBuffer = new ComputeBuffer(totalInstanceCount, GPUInstancerConstants.STRIDE_SIZE_MATRIX4X4);

                        int startIndex = 0;
                        for (int c = 0; c < spData.activeCellList.Count; c++)
                        {
                            GPUInstancerDetailCell detailCell = (GPUInstancerDetailCell)spData.activeCellList[c];

                            if (!detailCell.detailInstanceBuffers.ContainsKey(r) || detailCell.detailInstanceBuffers[r] == null)
                                continue;

                            _generatingVisibilityBuffer.CopyComputeBuffer(startIndex, detailCell.detailInstanceBuffers[r].count, detailCell.detailInstanceBuffers[r]);

                            startIndex += detailCell.detailInstanceBuffers[r].count;

                            //yield return null;
                        }

                        if (rd.transformationMatrixVisibilityBuffer != null)
                            rd.transformationMatrixVisibilityBuffer.Release();
                        rd.transformationMatrixVisibilityBuffer = _generatingVisibilityBuffer;
                    }

                    GPUInstancerUtility.InitializeGPUBuffer(rd);
                }
                if (heightMapBuffer != null)
                    heightMapBuffer.Release();
                if (detailMapBuffer != null)
                    detailMapBuffer.Release();

                _generatingVisibilityBuffer = null;
                replacingInstances = false;

                if (_triggerEvent)
                    GPUInstancerUtility.TriggerEvent(GPUInstancerEventType.DetailInitializationFinished);
                _triggerEvent = false;
            }
        }

        private void GenerateCellsInstanceDataFromTerrain()
        {
            if (counterBuffer == null)
                counterBuffer = new ComputeBuffer(1, GPUInstancerConstants.STRIDE_SIZE_INT);

            StartCoroutineAlt(FillCellsDetailData(terrain));
        }

        public void FillCellsDetailDataCallBack()
        {
            ClearCompletedThreads();
            if (threadHeightMapData == null || (runInThreads && activeThreads.Count > 0))
                return;

            threadHeightMapData = null;
            threadDetailMapData = null;

            if (GPUInstancerConstants.DETAIL_STORE_INSTANCE_DATA)
            {
                int detailMapSize = terrain.terrainData.detailResolution / spData.cellRowAndCollumnCountPerTerrain;
                int heightMapSize = (terrain.terrainData.heightmapResolution - 1) / spData.cellRowAndCollumnCountPerTerrain + 1;
                int detailResolution = terrain.terrainData.detailResolution;
                int heightmapResolution = terrain.terrainData.heightmapResolution;
                Vector3 terrainSize = terrain.terrainData.size;

                StartCoroutineAlt(SetInstanceDataForDetailCells(spData, prototypeList, detailMapSize, heightMapSize, detailResolution, heightmapResolution, terrainSize,
                    counterBuffer, counterData, terrainSettings, SetInstanceDataForDetailCellsCallback));
            }
            else
                SetInstanceDataForDetailCellsCallback();
        }

        private void SetInstanceDataForDetailCellsCallback()
        {
            initalizingInstances = false;
            foreach (GPUInstancerCell cell in spData.activeCellList)
                cell.isActive = false;
            spData.activeCellList.Clear();
            _triggerEvent = true;
        }

        private static IEnumerator SetInstanceDataForDetailCells(GPUInstancerSpatialPartitioningData<GPUInstancerCell> spData, List<GPUInstancerPrototype> prototypeList,
            int detailMapSize, int heightMapSize, int detailResolution, int heightmapResolution, Vector3 terrainSize, ComputeBuffer counterBuffer, int[] counterData,
            GPUInstancerTerrainSettings terrainSettings, Action callback)
        {
            int totalCreated = 0;
            foreach (GPUInstancerDetailCell cell in spData.GetCellList())
            {
                if (cell.detailMapData == null)
                    continue;

                cell.detailInstanceList = new Dictionary<int, Matrix4x4[]>();
                for (int i = 0; i < prototypeList.Count; i++)
                {
                    totalCreated += cell.totalDetailCounts[i];
#if !UNITY_EDITOR && UNITY_ANDROID
                    if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Vulkan)
                    {
                        cell.detailInstanceList[i] = GetInstanceDataForDetailPrototype((GPUInstancerDetailPrototype)prototypeList[i], cell.detailMapData[i], cell.heightMapData,
                            detailMapSize, heightMapSize,
                            detailResolution, heightmapResolution,
                            cell.instanceStartPosition,
                            terrainSize, cell.totalDetailCounts[i]);
                    }
                    else
                    {
#endif
                    cell.detailInstanceList[i] = GetInstanceDataForDetailPrototypeWithComputeShader((GPUInstancerDetailPrototype)prototypeList[i], cell.detailMapData[i], cell.heightMapData,
                        detailMapSize, heightMapSize,
                        detailResolution, heightmapResolution,
                        cell.instanceStartPosition,
                        terrainSize, cell.totalDetailCounts[i], _grassInstantiationComputeShader, terrainSettings, counterBuffer, counterData);
#if !UNITY_EDITOR && UNITY_ANDROID
                    }
#endif
                    if (totalCreated >= GPUInstancerConstants.BUFFER_COROUTINE_STEP_NUMBER)
                    {
                        totalCreated = 0;
                        yield return null;
                    }
                }
            }
            callback();
        }

        public float[,] threadHeightMapData;
        public List<int[,]> threadDetailMapData;
        public int threadHeightResolution;
        public int threadDetailResolution;

        public IEnumerator FillCellsDetailData(Terrain terrain)
        {
            threadHeightResolution = terrain.terrainData.heightmapResolution;
            threadDetailResolution = terrain.terrainData.detailResolution;
            if (threadHeightMapData == null)
            {
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.FillCellsDetailData.GetHeights");
#endif
                threadHeightMapData = terrain.terrainData.GetHeights(0, 0, threadHeightResolution, threadHeightResolution);
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
            if (threadDetailMapData == null)
            {
                threadDetailMapData = new List<int[,]>();
                for (int i = 0; i < terrain.terrainData.detailPrototypes.Length; i++)
                {
#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.FillCellsDetailData.GetDetailLayer");
#endif
                    threadDetailMapData.Add(terrain.terrainData.GetDetailLayer(0, 0, threadDetailResolution, threadDetailResolution, i));
#if UNITY_EDITOR
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                    if (runInThreads && i % 3 == 0)
                        yield return null;
                }
            }

            if (runInThreads)
            {
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.FillCellsDetailData.Threading");
#endif
                ParameterizedThreadStart fillCellsDetailData = new ParameterizedThreadStart(FillCellsDetailDataThread);
                Thread fillCellsDetailDataThread = new Thread(fillCellsDetailData);
                fillCellsDetailDataThread.IsBackground = true;
                Vector4 coord = Vector4.zero;
                coord.z = Mathf.CeilToInt(spData.cellRowAndCollumnCountPerTerrain / 2.0f);
                coord.w = spData.cellRowAndCollumnCountPerTerrain;
                threadStartQueue.Enqueue(new GPUIThreadData() { thread = fillCellsDetailDataThread, parameter = coord });

                if (spData.cellRowAndCollumnCountPerTerrain > 1)
                {
                    fillCellsDetailDataThread = new Thread(fillCellsDetailData);
                    fillCellsDetailDataThread.IsBackground = true;
                    coord.x = coord.z;
                    coord.z = spData.cellRowAndCollumnCountPerTerrain;
                    threadStartQueue.Enqueue(new GPUIThreadData() { thread = fillCellsDetailDataThread, parameter = coord });
                }
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerDetailManager.FillCellsDetailData.Main");
#endif
                Vector4 coord = new Vector4(0, 0, spData.cellRowAndCollumnCountPerTerrain, spData.cellRowAndCollumnCountPerTerrain);
                StartCoroutineAlt(FillCellsDetailDataCoroutine(coord));
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
        }

        public void FillCellsDetailDataThread(object parameter)
        {
            try
            {
                Vector4 coord = (Vector4)parameter;
                int startX = (int)coord.x;
                int startZ = (int)coord.y;
                int endX = (int)coord.z;
                int endZ = (int)coord.w;

                int detailMapSize = threadDetailResolution / spData.cellRowAndCollumnCountPerTerrain;
                int heightMapSize = ((threadHeightResolution - 1) / spData.cellRowAndCollumnCountPerTerrain) + 1;
                //Debug.Log("Cell DetailMapSize: " + detailMapSize + " Cell HeightMapSize: " + heightMapSize);

                GPUInstancerCell cell = null;

                for (int z = startZ; z < endZ; z++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        int hash = GPUInstancerCell.CalculateHash(x, 0, z);
                        spData.GetCell(hash, out cell);

                        if (cell != null)
                        {
                            GPUInstancerDetailCell detailCell = (GPUInstancerDetailCell)cell;

                            detailCell.heightMapData = threadHeightMapData.MirrorAndFlatten(detailCell.coordX * (heightMapSize - 1), detailCell.coordZ * (heightMapSize - 1),
                            heightMapSize, heightMapSize);

                            detailCell.detailMapData = new List<int[]>();
                            detailCell.totalDetailCounts = new List<int>();

                            for (int i = 0; i < threadDetailMapData.Count; i++)
                            {
                                int[] detailMapData = threadDetailMapData[i].MirrorAndFlatten(detailCell.coordX * detailMapSize, detailCell.coordZ * detailMapSize,
                                    detailMapSize, detailMapSize);
                                detailCell.detailMapData.Add(detailMapData);
                                int total = 0;
                                foreach (int num in detailMapData)
                                    total += num;
                                detailCell.totalDetailCounts.Add(total);
                            }
                        }
                        else
                            throw new Exception("Can not find cell!");
                    }
                }

                threadQueue.Enqueue(FillCellsDetailDataCallBack);
            }
            catch (Exception e)
            {
                threadException = e;
                threadQueue.Enqueue(LogThreadException);
            }
        }

        public IEnumerator FillCellsDetailDataCoroutine(Vector4 coord)
        {
            int startX = (int)coord.x;
            int startZ = (int)coord.y;
            int endX = (int)coord.z;
            int endZ = (int)coord.w;

            int detailMapSize = threadDetailResolution / spData.cellRowAndCollumnCountPerTerrain;
            int heightMapSize = ((threadHeightResolution - 1) / spData.cellRowAndCollumnCountPerTerrain) + 1;
            //Debug.Log("Cell DetailMapSize: " + detailMapSize + " Cell HeightMapSize: " + heightMapSize);

            GPUInstancerCell cell = null;

            for (int z = startZ; z < endZ; z++)
            {
                for (int x = startX; x < endX; x++)
                {
                    int hash = GPUInstancerCell.CalculateHash(x, 0, z);
                    spData.GetCell(hash, out cell);

                    if (cell != null)
                    {
                        GPUInstancerDetailCell detailCell = (GPUInstancerDetailCell)cell;

                        detailCell.heightMapData = threadHeightMapData.MirrorAndFlatten(detailCell.coordX * (heightMapSize - 1), detailCell.coordZ * (heightMapSize - 1),
                        heightMapSize, heightMapSize);

                        detailCell.detailMapData = new List<int[]>();
                        detailCell.totalDetailCounts = new List<int>();

                        for (int i = 0; i < threadDetailMapData.Count; i++)
                        {
                            int[] detailMapData = threadDetailMapData[i].MirrorAndFlatten(detailCell.coordX * detailMapSize, detailCell.coordZ * detailMapSize,
                                detailMapSize, detailMapSize);
                            detailCell.detailMapData.Add(detailMapData);
                            int total = 0;
                            foreach (int num in detailMapData)
                                total += num;
                            detailCell.totalDetailCounts.Add(total);
                        }
                        yield return null;
                    }
                    else
                        Debug.LogError("Can not find cell!");
                }
            }

            FillCellsDetailDataCallBack();
        }
        #endregion Spatial Partitioning Cell Management

        #region Public Methods
        public void SetDetailMapData(List<int[,]> detailMapData)
        {
            threadDetailMapData = detailMapData;
        }

        public int[,] GetDetailLayer(int layer)
        {
            if (!isInitialized || terrain == null || spData == null)
                return null;
            int detailResolution = terrain.terrainData.detailResolution;
            int detailMapSize = detailResolution / spData.cellRowAndCollumnCountPerTerrain;
            int[,] result = new int[detailResolution, detailResolution];
            GPUInstancerCell cell;
            GPUInstancerDetailCell detailCell;
            for (int cx = 0; cx < spData.cellRowAndCollumnCountPerTerrain; cx++)
            {
                for (int cz = 0; cz < spData.cellRowAndCollumnCountPerTerrain; cz++)
                {
                    if (spData.GetCell(GPUInstancerCell.CalculateHash(cx, 0, cz), out cell))
                    {
                        detailCell = (GPUInstancerDetailCell)cell;
                        if (detailCell.detailMapData != null)
                        {
                            for (int x = 0; x < detailMapSize; x++)
                            {
                                for (int y = 0; y < detailMapSize; y++)
                                {
                                    result[y + cz * detailMapSize, x + cx * detailMapSize] = detailCell.detailMapData[layer][x + y * detailMapSize];
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<int[,]> GetDetailMapData()
        {
            if (!isInitialized || terrain == null || spData == null)
                return null;
            List<int[,]> result = new List<int[,]>();
            for (int i = 0; i < prototypeList.Count; i++)
            {
                result.Add(GetDetailLayer(i));
            }
            return result;
        }
        #endregion
    }

}