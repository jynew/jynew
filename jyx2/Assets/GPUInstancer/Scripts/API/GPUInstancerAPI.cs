using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GPUInstancer
{
    public static class GPUInstancerAPI
    {
        #region Global

        /// <summary>
        ///     <para>Main GPU Instancer initialization Method. Generates the necessary GPUInstancer runtime data from predifined 
        ///     GPU Instancer prototypes that are registered in the manager, and generates all necessary GPU buffers for instancing.</para>
        ///     <para>Use this as the final step after you setup a GPU Instancer manager and all its prototypes.</para>
        ///     <para>Note that you can also use this to re-initialize the GPU Instancer prototypes that are registered in the manager at runtime.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="forceNew">If set to false the manager will not run initialization if it was already initialized before</param>
        public static void InitializeGPUInstancer(GPUInstancerManager manager, bool forceNew = true)
        {
            manager.InitializeRuntimeDataAndBuffers(forceNew);
        }

        /// <summary>
        ///     <para>Sets the active camera for a specific manager. This camera is used by GPU Instancer for various calculations (including culling operations). </para>
        ///     <para>Use this right after you add or change your camera at runtime. </para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="camera">The camera that GPU Instancer will use.</param>
        public static void SetCamera(GPUInstancerManager manager, Camera camera)
        {
            manager.SetCamera(camera);
        }

        /// <summary>
        ///     <para>Sets the active camera for all managers. This camera is used by GPU Instancer for various calculations (including culling operations). </para>
        ///     <para>Use this right after you add or change your camera at runtime. </para>
        /// </summary>
        /// <param name="camera">The camera that GPU Instancer will use.</param>
        public static void SetCamera(Camera camera)
        {
            if (GPUInstancerManager.activeManagerList != null)
                GPUInstancerManager.activeManagerList.ForEach(m => m.SetCamera(camera));
        }

        /// <summary>
        ///     <para>Returns a list of active managers. Use this if you want to access the managers at runtime.</para>
        /// </summary>
        /// <returns>The List of active managers. Null if no active managers present.</returns>
        public static List<GPUInstancerManager> GetActiveManagers()
        {
            return GPUInstancerManager.activeManagerList == null ? null : GPUInstancerManager.activeManagerList.ToList();
        }

        /// <summary>
        ///     <para>Starts listening the specified process and runs the given callback function when it finishes.</para>
        ///     <para>GPU Instancer does not lock Unity updates when initializing instances and instead, does this in a background process. 
        ///     Each prototype will show on the terrain upon its own initialization. Use this method to get notified when all prototypes are initialized.</para>
        ///     <para>The most common usage for this is to show a loading bar. For an example, see: <seealso cref="DetailDemoSceneController"/></para>
        /// </summary>
        /// <param name="eventType">The event type that will be listened for callback</param>
        /// <param name="callback">The callback function to run upon initialization completion. Can be any function that doesn't take any parameters.</param>
        public static void StartListeningGPUIEvent(GPUInstancerEventType eventType, UnityAction callback)
        {
            GPUInstancerUtility.StartListening(eventType, callback);
        }

        /// <summary>
        ///     <para>Stops listening the specified process and unregisters the given callback function that was registered with <see cref="StartListeningGPUIEvent"/>.</para>
        ///     <para>Use this in your callback function to unregister it (e.g. after hiding the loading bar).</para>
        ///     <para>For an example, see: <seealso cref="DetailDemoSceneController"/></para>
        /// </summary>
        /// <param name="eventType">The event type that was registered with <see cref="StartListeningGPUIEvent"/></param>
        /// <param name="callback">The callback function that was registered with <see cref="StartListeningGPUIEvent"/></param>
        public static void StopListeningGPUIEvent(GPUInstancerEventType eventType, UnityAction callback)
        {
            GPUInstancerUtility.StopListening(eventType, callback);
        }

        /// <summary>
        /// Updates all transform values in GPU memory with the given offset position.
        /// </summary>
        /// <param name="manager">GPUI Manager to apply the offset</param>
        /// <param name="offsetPosition">Offset Position</param>
        public static void SetGlobalPositionOffset(GPUInstancerManager manager, Vector3 offsetPosition)
        {
            GPUInstancerUtility.SetGlobalPositionOffset(manager, offsetPosition);
        }

        /// <summary>
        /// Removes the instances in GPU memory that are inside bounds. 
        /// </summary>
        /// <param name="manager">GPUI Manager to remove the instances from</param>
        /// <param name="bounds">Bounds that define the area that the instances will be removed</param>
        /// <param name="offset">Adds an offset around the area that the instances will be removed</param>
        /// <param name="prototypeFilter">If prototypeFilter parameter is given, only the instances of the given prototypes will be removed.</param>
        public static void RemoveInstancesInsideBounds(GPUInstancerManager manager, Bounds bounds, float offset = 0, List<GPUInstancerPrototype> prototypeFilter = null)
        {
            manager.RemoveInstancesInsideBounds(bounds, offset, prototypeFilter);
        }

        /// <summary>
        /// Removes the instances in GPU memory that are inside collider.
        /// </summary>
        /// <param name="manager">GPUI Manager to remove the instances from</param>
        /// <param name="collider">Collider that define the area that the instances will be removed</param>
        /// <param name="offset">Adds an offset around the area that the instances will be removed</param>
        /// <param name="prototypeFilter">If prototypeFilter parameter is given, only the instances of the given prototypes will be removed.</param>
        public static void RemoveInstancesInsideCollider(GPUInstancerManager manager, Collider collider, float offset = 0, List<GPUInstancerPrototype> prototypeFilter = null)
        {
            manager.RemoveInstancesInsideCollider(collider, offset, prototypeFilter);
        }

        /// <summary>
        /// [For Advanced Users Only] Returns the float4x4 ComputeBuffer that store the localToWorldMatrix for each instance in GPU memory. This buffer can be used to make
        /// modifications in GPU memory before the rendering process. 
        /// </summary>
        /// <param name="manager">GPUI Manager to get the buffer from</param>
        /// <param name="prototype">Prototype that the buffer belongs to</param>
        /// <returns></returns>
        public static ComputeBuffer GetTransformDataBuffer(GPUInstancerManager manager, GPUInstancerPrototype prototype)
        {
            return manager.GetTransformDataBuffer(prototype);
        }

        /// <summary>
        /// Changes the LODBias with the given value. Values lower than the LODBias in your Quality Settings will result in higher quality
        /// but less performance (e.g. more instances will use LOD0), values higher than the LODBias in your Quality Settings will 
        /// reduce the quality but increase performance (e.g. less instances will use LOD0)
        /// </summary>
        /// <param name="manager">GPUI Manager to adjust the LOD sizes</param>
        /// <param name="newLODBias">New LODBias value</param>
        /// <param name="prototypeFilter">If prototypeFilter parameter is given, only the LODBiases of the given prototypes will be changed.</param>
        public static void SetLODBias(GPUInstancerManager manager, float newLODBias, List<GPUInstancerPrototype> prototypeFilter = null)
        {
            manager.SetLODBias(newLODBias, prototypeFilter);
        }

        /// <summary>
        /// Can be used to change the material of a prototype at runtime
        /// </summary>
        /// <param name="manager">GPUI Manager</param>
        /// <param name="prototype">GPUI Prototype</param>
        /// <param name="material">New material to set on the renderer</param>
        /// <param name="lodLevel">LOD level</param>
        /// <param name="rendererIndex">Renderer index on the LOD level</param>
        /// <param name="subMeshIndex">Submesh index of the renderer</param>
        public static void ChangeMaterial(GPUInstancerManager manager, GPUInstancerPrototype prototype, Material material, int lodLevel = 0, int rendererIndex = 0, int subMeshIndex = 0)
        {
            GPUInstancerRuntimeData runtimeData = manager.GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return;
            GPUInstancerRenderer gpuiRenderer = runtimeData.instanceLODs[lodLevel].renderers[rendererIndex];

            // Generate proxy GO with a Mesh Renderer to get material property blocks
            GameObject proxyGameObject = new GameObject("ProxyGO");
            MeshFilter meshFilter = proxyGameObject.AddComponent<MeshFilter>();
            MeshRenderer proxyRenderer = proxyGameObject.AddComponent<MeshRenderer>();

            // Set mesh to proxy GO
            meshFilter.mesh = gpuiRenderer.mesh;
            // Set new material to runtime data
            gpuiRenderer.materials[subMeshIndex] = GPUInstancerConstants.gpuiSettings.shaderBindings.GetInstancedMaterial(material);
            // Set new material to proxy GO
            proxyRenderer.materials[subMeshIndex] = material;
            // Get material property blocks
            proxyRenderer.GetPropertyBlock(gpuiRenderer.mpb);
            if (gpuiRenderer.shadowMPB != null)
                proxyRenderer.GetPropertyBlock(gpuiRenderer.shadowMPB);

            // Destroy proxy GO
            GameObject.Destroy(proxyGameObject);

            // Setup new materials for instancing
            GPUInstancerUtility.SetAppendBuffers(runtimeData);
        }

        /// <summary>
        /// Can be used to change the mesh of a prototype at runtime
        /// </summary>
        /// <param name="manager">GPUI Manager</param>
        /// <param name="prototype">GPUI Prototype</param>
        /// <param name="mesh">New mesh to set on the renderer</param>
        /// <param name="lodLevel">LOD level</param>
        /// <param name="rendererIndex">Renderer index on the LOD level</param>
        /// <param name="subMeshIndex">Submesh index of the renderer</param>
        public static void ChangeMesh(GPUInstancerManager manager, GPUInstancerPrototype prototype, Mesh mesh, int lodLevel = 0, int rendererIndex = 0, int subMeshIndex = 0)
        {
            GPUInstancerRuntimeData runtimeData = manager.GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return;
            GPUInstancerRenderer gpuiRenderer = runtimeData.instanceLODs[lodLevel].renderers[rendererIndex];

            if (gpuiRenderer.mesh.subMeshCount != mesh.subMeshCount)
            {
                Debug.LogError("ChangeMesh method can not be used with a mesh that has different amount of submeshes than the original mesh.");
                return;
            }

            if (gpuiRenderer.mesh.vertexCount != mesh.vertexCount)
            {
                int argsLastIndex = gpuiRenderer.argsBufferOffset;
                // Setup the indirect renderer buffer:
                for (int j = 0; j < gpuiRenderer.mesh.subMeshCount; j++)
                {
                    runtimeData.args[argsLastIndex++] = gpuiRenderer.mesh.GetIndexCount(j); // index count per instance
                    runtimeData.args[argsLastIndex++] = 0;// (uint)runtimeData.bufferSize;
                    runtimeData.args[argsLastIndex++] = gpuiRenderer.mesh.GetIndexStart(j); // start index location
                    runtimeData.args[argsLastIndex++] = 0; // base vertex location
                    runtimeData.args[argsLastIndex++] = 0; // start instance location
                }
                runtimeData.argsBuffer.SetData(runtimeData.args);
            }
                
            gpuiRenderer.mesh = mesh;
        }

        /// <summary>
        /// SetInstanceCount can be used to discard instances that are indexed higher than the given index count
        /// </summary>
        /// <param name="manager">GPUI Manager</param>
        /// <param name="prototype">GPUI Prototype</param>
        /// <param name="instanceCount">New instance count to set on the runtime data</param>
        public static void SetInstanceCount(GPUInstancerManager manager, GPUInstancerPrototype prototype, int instanceCount)
        {
            GPUInstancerRuntimeData runtimeData = manager.GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return;
            if (instanceCount > runtimeData.bufferSize)
            {
                Debug.LogError("Instance count can not be higher than the buffer size.");
                return;
            }
            runtimeData.instanceCount = instanceCount;
        }

        /// <summary>
        /// Returns the array that stores the transform data of the instances
        /// </summary>
        /// <param name="manager">GPUI Manager</param>
        /// <param name="prototype">GPUI Prototype</param>
        /// <returns>Instance data array</returns>
        public static Matrix4x4[] GetInstanceDataArray(GPUInstancerManager manager, GPUInstancerPrototype prototype)
        {
            GPUInstancerRuntimeData runtimeData = manager.GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return null;
            return runtimeData.instanceDataArray;
        }

        /// <summary>
        /// Returns the prototype list of the given GPUI manager
        /// </summary>
        /// <param name="manager">GPUI Manager</param>
        /// <returns>Prototype List</returns>
        public static List<GPUInstancerPrototype> GetPrototypeList(GPUInstancerManager manager)
        {
            return manager.prototypeList.ToList();
        }

        /// <summary>
        /// Changes prototype's shadow LOD setting
        /// </summary>
        /// <param name="prototype">GPUI Prototype</param>
        /// <param name="lodLevel">LOD level to change the shadow setting</param>
        /// <param name="isShadowCasting">True if LOD level is shadow casting</param>
        /// <param name="shadowLOD">(Optional) Provide an LOD level that shadows will be rendered from</param>
        public static void ChangeLODShadow(GPUInstancerPrototype prototype, int lodLevel, bool isShadowCasting, int shadowLOD = -1)
        {
            int lodIndex = lodLevel * 4;
            if (lodLevel >= 4)
                lodIndex = (lodLevel - 4) * 4 + 1;
            if (isShadowCasting)
                prototype.shadowLODMap[lodIndex] = shadowLOD >= 0 ? shadowLOD : lodLevel;
            else
                prototype.shadowLODMap[lodIndex] = 9;
        }
        #endregion Global

        #region Prefab Instancing

        /// <summary>
        ///     <para>Registers a list of prefab instances with GPU Instancer. You must use <see cref="InitializeGPUInstancer"/> after registering these prefabs for final initialization.</para>
        ///     <para>The prefabs of the instances in this list must be previously defined in the given manager (either at runtime or editor time).</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstanceList">The list of prefabs instances to GPU instance.</param>
        public static void RegisterPrefabInstanceList(GPUInstancerPrefabManager manager, IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            manager.RegisterPrefabInstanceList(prefabInstanceList);
        }

        /// <summary>
        ///     <para>Unregisters a list of prefab instances from GPU Instancer. You must use <see cref="InitializeGPUInstancer"/> after unregistering these prefabs for final initialization.</para>
        ///     <para>The prefabs of the instances in this list must be previously defined in the given manager (either at runtime or editor time).</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstanceList">The list of prefabs instances to be removed from  GPU instancer.</param>
        public static void UnregisterPrefabInstanceList(GPUInstancerPrefabManager manager, IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            manager.UnregisterPrefabInstanceList(prefabInstanceList);
        }

        /// <summary>
        ///     <para>Clears the registered prefab instances from the prefab manager.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        public static void ClearRegisteredPrefabInstances(GPUInstancerPrefabManager manager)
        {
            manager.ClearRegisteredPrefabInstances();
        }

        /// <summary>
        ///     <para>Clears the registered prefab instances from the prefab manager for a specific prototype.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prototype">The prototype to clear registered instances for.</param>
        public static void ClearRegisteredPrefabInstances(GPUInstancerPrefabManager manager, GPUInstancerPrototype prototype)
        {
            manager.ClearRegisteredPrefabInstances(prototype);
        }

        /// <summary>
        ///     <para>Adds a new prefab instance for GPU instancing to an already initialized list of registered instances. </para>
        ///     <para>Use this if you want to add another instance of a prefab after you have initialized a list of prefabs with <see cref="InitializeGPUInstancer"/>.</para>
        ///     <para>The prefab of this instance must be previously defined in the given manager (either at runtime or editor time).</para>
        ///     <para>Note that the prefab must be enabled for adding and removal in the manager in order for this to work (for performance reasons).</para>
        ///     <para>Also note that the number of total instances is limited by the count of already initialized instances plus the extra amount you define in the manager.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstance">The prefab instance to add.</param>
        /// <param name="autoIncreaseBufferSize">(Optional) When true, buffer size designated for the prototype will automatically increase when there is not enough space for adding a new instance</param>
        public static void AddPrefabInstance(GPUInstancerPrefabManager manager, GPUInstancerPrefab prefabInstance, bool autoIncreaseBufferSize = false)
        {
            manager.AddPrefabInstance(prefabInstance, autoIncreaseBufferSize);
        }

        /// <summary>
        ///     <para>Removes a prefab instance from an already initialized list of registered instances. </para>
        ///     <para>Use this if you want to remove a prefab instance after you have initialized a list of prefabs with <see cref="InitializeGPUInstancer"/> 
        ///     (usually before destroying the GameObject).</para>
        ///     <para>The prefab of this instance must be previously defined in the given manager (either at runtime or editor time).</para>
        ///     <para>Note that the prefab must be enabled for adding and removal in the manager in order for this to work (for performance reasons).</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstance">The prefab instance to remove.</param>
        /// <param name="setRenderersEnabled">If set to false Mesh Renderer components will not be enabled after removing prefab instance which will make the
        /// instance invisible.</param>
        public static void RemovePrefabInstance(GPUInstancerPrefabManager manager, GPUInstancerPrefab prefabInstance, bool setRenderersEnabled = true)
        {
            manager.RemovePrefabInstance(prefabInstance, setRenderersEnabled);
        }

        /// <summary>
        ///     <para>Disables GPU instancing and enables Unity renderers for the given prefab instance without removing it from the list of registerd prefabs.</para>
        ///     <para>Use this if you want to pause GPU Instancing for a prefab (e.g. to enable physics).</para>
        ///     <para>Note that the prefab must be enabled for runtime modifications in the manager in order for this to work (for performance reasons).</para>
        ///     <para>Also note that you can also add <seealso cref="GPUInstancerModificationCollider"/> to a game object to use its collider to automatically 
        ///     enable/disable instancing when a prefab instance enters/exits its collider.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstance">The prefab instance to disable the GPU Instancing of.</param>
        /// <param name="setRenderersEnabled">If set to false Mesh Renderer components will not be enabled after disabling instancing which will make the
        /// instance invisible.</param>
        public static void DisableIntancingForInstance(GPUInstancerPrefabManager manager, GPUInstancerPrefab prefabInstance, bool setRenderersEnabled = true)
        {
            manager.DisableIntancingForInstance(prefabInstance, setRenderersEnabled);
        }

        /// <summary>
        ///     <para>Enables GPU instancing and disables Unity renderers for the given prefab instance without re-adding it to the list of registerd prefabs.</para>
        ///     <para>Use this if you want to unpause GPU Instancing for a prefab.</para>
        ///     <para>Note that the prefab must be enabled for runtime modifications in the manager in order for this to work (for performance reasons).</para>
        ///     <para>Also note that you can also add <seealso cref="GPUInstancerModificationCollider"/> to a game object to use its collider to automatically 
        ///     enable/disable instancing when a prefab instance enters/exits its collider.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstance">The prefab instance to enable the GPU Instancing of.</param>
        /// <param name="disableMeshRenderers">If set to false Mesh Renderer components will not be disabled after enabling instancing. Should be used only for instances
        /// that have already disabled mesh renderers to speed up the process.</param>
        public static void EnableInstancingForInstance(GPUInstancerPrefabManager manager, GPUInstancerPrefab prefabInstance, bool setRenderersDisabled = true)
        {
            manager.EnableInstancingForInstance(prefabInstance, setRenderersDisabled);
        }

        /// <summary>
        ///     <para>Updates and synchronizes the GPU Instancer transform data (position, rotation and scale) for the given prefab instance.</para>
        ///     <para>Use this if you want to update, rotate, and/or scale prefab instances after initialization.</para>
        ///     <para>The updated values are taken directly from the transformation operations made beforehand on the instance's Unity transform component. 
        ///     (These operations will not reflect on the GPU Instanced prefab automatically unless you use this method).</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstance">The prefab instance to update the transform values of. The instance's Unity transform component must be updated beforehand.</param>
        public static void UpdateTransformDataForInstance(GPUInstancerPrefabManager manager, GPUInstancerPrefab prefabInstance)
        {
            manager.UpdateTransformDataForInstance(prefabInstance);
        }

        /// <summary>
        ///     <para>Specifies a variation buffer for a GPU Instancer prototype that is defined in the prefab's shader. Required to use <see cref="AddVariation{T}"/></para>
        ///     <prara>Use this if you want any type of variation between this prototype's instances.</prara>
        ///     <para>To define the buffer necessary for this variation in your shader, you need to create a StructuredBuffer field of the relevant type in that shader. 
        ///     You can then access this buffer with "gpuiTransformationMatrix[unity_InstanceID]"</para>
        ///     <para>see <seealso cref="ColorVariations"/> and its demo scene for an example</para>
        /// </summary>
        /// 
        /// <example> 
        ///     This sample shows how to use the variation buffer in your shader:
        /// 
        ///     <code><![CDATA[
        ///     ...
        ///     fixed4 _Color;
        /// 
        ///     #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        ///         StructuredBuffer<float4> colorBuffer;
        ///     #endif
        ///     ...
        ///     void surf (Input IN, inout SurfaceOutputStandard o) {
        ///     ...
        ///         #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        ///             uint index = gpuiTransformationMatrix[unity_InstanceID];
        ///             col = colorBuffer[index];
        ///         #else
        ///             col = _Color;
        ///         #endif
        ///     ...
        ///     }
        ///     ]]></code>
        /// 
        ///     See "GPUInstancer/ColorVariationShader" for the full example.
        /// 
        /// </example>
        /// 
        /// <typeparam name="T">The type of variation buffer. Must be defined in the instance prototype's shader</typeparam>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prototype">The GPU Instancer prototype to define variations.</param>
        /// <param name="bufferName">The name of the variation buffer in the prototype's shader.</param>
        public static void DefinePrototypeVariationBuffer<T>(GPUInstancerPrefabManager manager, GPUInstancerPrefabPrototype prototype, string bufferName)
        {
            manager.DefinePrototypeVariationBuffer<T>(prototype, bufferName);
        }

        /// <summary>
        ///     <para>Sets the variation value for this prefab instance. The variation buffer for the prototype must be defined 
        ///     with <see cref="DefinePrototypeVariationBuffer{T}"/> before using this.</para>
        /// </summary>
        /// <typeparam name="T">The type of variation buffer. Must be defined in the instance prototype's shader.</typeparam>
        /// <param name="prefabInstance">The prefab instance to add the variation to.</param>
        /// <param name="bufferName">The name of the variation buffer in the prototype's shader.</param>
        /// <param name="value">The value of the variation.</param>
        public static void AddVariation<T>(GPUInstancerPrefab prefabInstance, string bufferName, T value)
        {
            prefabInstance.AddVariation(bufferName, value);
        }

        /// <summary>
        ///     <para>Updates the variation value for this prefab instance. The variation buffer for the prototype must be defined 
        ///     with <see cref="DefinePrototypeVariationBuffer{T}"/> before using this.</para>
        /// </summary>
        /// <typeparam name="T">The type of variation buffer. Must be defined in the instance prototype's shader.</typeparam>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prefabInstance">The prefab instance to update the variation at.</param>
        /// <param name="bufferName">The name of the variation buffer in the prototype's shader.</param>
        /// <param name="value">The value of the variation.</param>
        public static void UpdateVariation<T>(GPUInstancerPrefabManager manager, GPUInstancerPrefab prefabInstance, string bufferName, T value)
        {
            prefabInstance.AddVariation(bufferName, value);
            manager.UpdateVariationData(prefabInstance, bufferName, value);
        }

        /// <summary>
        /// Specifies a variation buffer for a GPU Instancer prototype that is defined in the prefab's shader. And sets the variation values for the given array.
        /// </summary>
        /// <typeparam name="T">The type of variation buffer. Must be defined in the instance prototype's shader</typeparam>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prototype">The GPU Instancer prototype to define variations.</param>
        /// <param name="bufferName">The name of the variation buffer in the prototype's shader.</param>
        /// <param name="variationArray">The array that stores the variation information.</param>
        public static PrefabVariationData<T> DefineAndAddVariationFromArray<T>(GPUInstancerPrefabManager manager, GPUInstancerPrefabPrototype prototype, string bufferName, T[] variationArray)
        {
            return manager.DefineAndAddVariationFromArray<T>(prototype, bufferName, variationArray);
        }

        /// <summary>
        /// Updates the variation values for the given array for the specified prototype and buffer.
        /// </summary>
        /// <typeparam name="T">The type of variation buffer. Must be defined in the instance prototype's shader</typeparam>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="prototype">The GPU Instancer prototype to define variations.</param>
        /// <param name="bufferName">The name of the variation buffer in the prototype's shader.</param>
        /// <param name="variationArray">The array that stores the variation information.</param>
        /// <param name="arrayStartIndex">Start index of the given array that the data will be uploaded to the buffer</param>
        /// <param name="bufferStartIndex">Start index of the buffer to set the data from the array</param>
        /// <param name="count">Total number of variation data to set to the buffer from the array</param>
        public static void UpdateVariationFromArray<T>(GPUInstancerPrefabManager manager, GPUInstancerPrefabPrototype prototype, string bufferName, T[] variationArray,
            int arrayStartIndex = 0, int bufferStartIndex = 0, int count = 0)
        {
            manager.UpdateVariationsFromArray<T>(prototype, bufferName, variationArray, arrayStartIndex, bufferStartIndex, count);
        }

        /// <summary>
        /// Use this method to create prefab instances with the given transform information without creating GameObjects.
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prototype">GPUI Prefab Prototype</param>
        /// <param name="matrix4x4Array">Array of Matrix4x4 that store the transform data of prefab instances</param>
        public static void InitializeWithMatrix4x4Array(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prototype, Matrix4x4[] matrix4x4Array)
        {
            GPUInstancerUtility.InitializeWithMatrix4x4Array(prefabManager, prototype, matrix4x4Array);
        }

        /// <summary>
        /// Use this method to initialize buffers for the given prototype and set the buffer data later with UpdateVisibilityBuffer API methods. Please note that you will 
        /// need to provide a positive integer buffer size to initialize the buffers successfully.
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prototype">GPUI Prefab Prototype</param>
        /// <param name="bufferSize">Size of the buffer to allocate in GPU memory</param>
        /// <param name="instanceCount">(Optional) Initial instance count to render. Can also be set later with SetInstanceCount API method</param>
        public static void InitializePrototype(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prototype, int bufferSize, int instanceCount = 0)
        {
            GPUInstancerUtility.InitializePrototype(prefabManager, prototype, bufferSize, instanceCount);
        }

        /// <summary>
        /// Use this method to update transform data of all prefab instances with a Matrix4x4 array. By default all the data from the array will be
        /// uploaded to the GPU. You can make partial uploads by setting the arrayStartIndex, bufferStartIndex, and count parameters.
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prototype">GPUI Prefab Prototype</param>
        /// <param name="matrix4x4Array">Array of Matrix4x4 that store the transform data of prefab instances</param>
        /// <param name="arrayStartIndex">Start index of the given array that the data will be uploaded to the buffer</param>
        /// <param name="bufferStartIndex">Start index of the buffer to set the data from the array</param>
        /// <param name="count">Total number of matrices to set to the buffer from the array</param>
        public static void UpdateVisibilityBufferWithMatrix4x4Array(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prototype, Matrix4x4[] matrix4x4Array,
            int arrayStartIndex = 0, int bufferStartIndex = 0, int count = 0)
        {
            GPUInstancerUtility.UpdateVisibilityBufferWithMatrix4x4Array(prefabManager, prototype, matrix4x4Array, arrayStartIndex, bufferStartIndex, count);
        }

#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Use this method to update transform data of all prefab instances with a float4x4 native array. By default all the data from the array will be
        /// uploaded to the GPU. You can make partial uploads by setting the arrayStartIndex, bufferStartIndex, and count parameters.
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prototype">GPUI Prefab Prototype</param>
        /// <param name="float4x4Array">Array of float4x4 that store the transform data of prefab instances. Struct reference is not forced so you can use any float4x4 struct (e.g. Matrix4x4 or float4x4 from Mathematics package)</param>
        /// <param name="arrayStartIndex">(Optional) Start index of the given array that the data will be uploaded to the buffer</param>
        /// <param name="bufferStartIndex">(Optional) Start index of the buffer to set the data from the array</param>
        /// <param name="count">(Optional) Total number of matrices to set to the buffer from the array</param>
        public static void UpdateVisibilityBufferWithNativeArray<T>(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prototype, NativeArray<T> float4x4Array,
            int arrayStartIndex = 0, int bufferStartIndex = 0, int count = 0) where T : struct
        {
            GPUInstancerUtility.UpdateVisibilityBufferWithNativeArray(prefabManager, prototype, float4x4Array, arrayStartIndex, bufferStartIndex, count);
        }
#endif

        /// <summary>
        /// Use this method to define Prefab Prototypes at runtime for procedurally generated GameObjects
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype will be defined on</param>
        /// <param name="prototypeGameObject">GameObject to use as reference for the prototype</param>
        /// <param name="attachScript">(Optional) If false, GPUI will not add the GPUInstancerPrefab component on the prototypeGameObject</param>
        /// <returns></returns>
        public static GPUInstancerPrefabPrototype DefineGameObjectAsPrefabPrototypeAtRuntime(GPUInstancerPrefabManager prefabManager, GameObject prototypeGameObject, bool attachScript = true)
        {
            return prefabManager.DefineGameObjectAsPrefabPrototypeAtRuntime(prototypeGameObject, attachScript);
        }

        /// <summary>
        /// Initialize single prefab prototype for preparing runtime data and buffers for instanced rendering
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prefabPrototype">GPUI Prefab Prototype</param>
        /// <returns></returns>
        public static GPUInstancerRuntimeData InitializeGPUInstancer(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prototype)
        {
            // initialize PrefabManager if it is not already initialized
            prefabManager.InitializeRuntimeDataAndBuffers(false);
            // generate and return prototype runtime data
            return prefabManager.InitializeRuntimeDataForPrefabPrototype(prototype);
        }

        /// <summary>
        /// Use this method to add new instances to prototype when you do not use prefabs (Ex: when you create a prototype with DefineGameObjectAsPrefabPrototypeAtRuntime API method)
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prefabPrototype">GPUI Prefab Prototype</param>
        /// <param name="instances">List of GameObjects to register on the manager</param>
        public static void AddInstancesToPrefabPrototypeAtRuntime(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prefabPrototype, IEnumerable<GameObject> instances)
        {
            prefabManager.AddInstancesToPrefabPrototypeAtRuntime(prefabPrototype, instances);
        }

        /// <summary>
        /// Use this method to remove a prototype definition at runtime
        /// </summary>
        /// <param name="prefabManager">The GPUI Prefab Manager that the prefab prototype is defined on</param>
        /// <param name="prefabPrototype">GPUI Prefab Prototype ro remove from the manager</param>
        public static void RemovePrototypeAtRuntime(GPUInstancerPrefabManager prefabManager, GPUInstancerPrefabPrototype prefabPrototype)
        {
            prefabManager.RemovePrototypeAtRuntime(prefabPrototype);
        }
        #endregion Prefab Instancing

        #region Detail & Tree Instancing

        /// <summary>
        ///     <para>Sets the Unity terrain to the GPU Instancer manager and generates the instance prototypes from Unity detail 
        ///     prototypes that are defined on the given Unity terrain component.</para>
        ///     <para>Use this to initialize the GPU Instancer detail manager if you want to generate your terrain at runtime. 
        ///     See <seealso cref="TerrainGenerator"/> and its demo scene for an example.</para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="terrain"></param>
        public static void SetupManagerWithTerrain(GPUInstancerTerrainManager manager, Terrain terrain)
        {
            manager.SetupManagerWithTerrain(terrain);
        }

        #endregion Detail & Tree Instancing

        #region Detail Instancing

        /// <summary>
        ///     <para>Updates and synchronizes the GPU Instancer detail prototypes with the modifications made in the manager at runtime.</para>
        ///     <para>Use this if you want to make changes to the detail prototypes at runtime. Prototypes in the manager must be modified before using this.</para>
        ///     <para>For example usages, see: <see cref="DetailDemoSceneController"/> and <seealso cref="TerrainGenerator"/></para>
        /// </summary>
        /// <param name="manager">The manager that defines the prototypes you want to GPU instance.</param>
        /// <param name="updateMeshes">Whether GPU Instancer should also update meshes. Send this value as "true" if you change properties 
        /// related to cross quadding, noise spread and/or detail scales</param>
        public static void UpdateDetailInstances(GPUInstancerDetailManager manager, bool updateMeshes = false)
        {
            GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(manager.runtimeDataList, manager.terrainSettings, updateMeshes, manager.detailLayer);
        }

        /// <summary>
        /// Returns a list of 2D array of detail object density for the all the prototypes of the manager.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static List<int[,]> GetDetailMapData(GPUInstancerDetailManager manager)
        {
            return manager.GetDetailMapData();
        }

        /// <summary>
        /// Returns a 2D array of detail object density for the given layer.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static int[,] GetDetailLayer(GPUInstancerDetailManager manager, int layerIndex)
        {
            return manager.GetDetailLayer(layerIndex);
        }

        /// <summary>
        /// Can be used to set the Detail Map Data to the Detail Manager before initialization.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="detailMapData"></param>
        public static void SetDetailMapData(GPUInstancerDetailManager manager, List<int[,]> detailMapData)
        {
            manager.SetDetailMapData(detailMapData);
        }
        #endregion

        #region Tree Instancing
        /// <summary>
        /// Use this method to add new terrains to Tree Manager at runtime
        /// </summary>
        /// <param name="manager">The GPUI Tree Manager to add the new terrain</param>
        /// <param name="terrain">New terrain to render the trees</param>
        public static void AddTerrainToManager(GPUInstancerTreeManager manager, Terrain terrain)
        {
            manager.AddTerrain(terrain);
        }

        /// <summary>
        /// Use this method to remove terrains from Tree Manager at runtime
        /// </summary>
        /// <param name="manager">The GPUI Tree Manager to remove the terrain</param>
        /// <param name="terrain">Terrain to remove</param>
        public static void RemoveTerrainFromManager(GPUInstancerTreeManager manager, Terrain terrain)
        {
            manager.RemoveTerrain(terrain);
        }
        #endregion Tree Instancing

        #region Editor Only
#if UNITY_EDITOR
        /// <summary>
        /// [EDITOR-ONLY] Shader auto-conversion can be run with this method without using a GPUI Manager
        /// </summary>
        /// <param name="shader">Shader to convert</param>
        /// <returns>True if successful</returns>
        public static bool SetupShaderForGPUI(Shader shader)
        {
            if (shader == null || shader.name == GPUInstancerConstants.SHADER_UNITY_INTERNAL_ERROR)
            {
                Debug.LogError("Can not find shader! Please make sure that the material has a shader assigned.");
                return false;
            }
            GPUInstancerConstants.gpuiSettings.shaderBindings.ClearEmptyShaderInstances();
            if (!GPUInstancerConstants.gpuiSettings.shaderBindings.IsShadersInstancedVersionExists(shader.name))
            {
                if (GPUInstancerUtility.IsShaderInstanced(shader))
                {
                    GPUInstancerConstants.gpuiSettings.shaderBindings.AddShaderInstance(shader.name, shader, true);
                    Debug.Log("Shader setup for GPUI has been successfully completed.");
                    return true;
                }
                else
                {
                    Shader instancedShader = GPUInstancerUtility.CreateInstancedShader(shader);
                    if (instancedShader != null)
                    {
                        GPUInstancerConstants.gpuiSettings.shaderBindings.AddShaderInstance(shader.name, instancedShader);
                        return true;
                    }
                    else
                    {
                        string originalAssetPath = UnityEditor.AssetDatabase.GetAssetPath(shader);
                        if (originalAssetPath.ToLower().EndsWith(".shadergraph"))
                            Debug.LogError(string.Format(GPUInstancerConstants.ERRORTEXT_shaderGraph, shader.name));
                        else
                            Debug.LogError("Can not create instanced version for shader: " + shader.name + ".");
                        return false;
                    }
                }
            }
            else
            {
                Debug.Log("Shader has already been setup for GPUI.");
                return true;
            }
        }

        /// <summary>
        /// [EDITOR-ONLY] Adds the shader variant used in the given material to the GPUIShaderVariantCollection. This collection is used to include the shader variants with GPUI support in your builds.
        /// Normally GPUI Managers makes this automatically, but if you generate your managers at runtime, this method can be usefull to add these shader variants manually.
        /// </summary>
        /// <param name="material"></param>
        public static void AddShaderVariantToCollection(Material material)
        {
            GPUInstancerConstants.gpuiSettings.AddShaderVariantToCollection(material);
        }

        /// <summary>
        /// [EDITOR-ONLY] Starts rendering GPUI instances for the given manager in editor mode. The simulation can be stopped using the StopEditorSimulation method.
        /// It will automatically stop when entering Play mode.
        /// It is usefull to render instances in Editor Mode while using no-GameObject workflow.
        /// </summary>
        /// <param name="manager">GPUI manager to start simulation for</param>
        public static void StartEditorSimulation(GPUInstancerManager manager)
        {
            if (!Application.isPlaying && manager.gpuiSimulator != null)
                manager.gpuiSimulator.StartSimulation();
        }

        /// <summary>
        /// [EDITOR-ONLY] Stops rendering GPUI instances for the given manager in editor mode. Can be used after StartEditorSimulation method to disable rendering.
        /// </summary>
        /// <param name="manager">GPUI manager to stop simulation for</param>
        public static void StopEditorSimulation(GPUInstancerManager manager)
        {
            if (!Application.isPlaying && manager.gpuiSimulator != null)
                manager.gpuiSimulator.StopSimulation();
        }
#endif
        #endregion Editor Only
    }
}
