using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace GPUInstancer
{
    public class GPUInstancerRuntimeData
    {
        public GPUInstancerPrototype prototype;

        // Mesh - Material - LOD info
        public List<GPUInstancerPrototypeLOD> instanceLODs;
        public Bounds instanceBounds;
        public float[] lodSizes = new float[] {
            1000, 1000, 1000, 1000,
            1000, 1000, 1000, 1000,
            1000, 1000, 1000, 1000,
            1000, 1000, 1000, 1000 };
        public float lodBiasApplied = -1;

        // Instance Data
        [HideInInspector]
        public Matrix4x4[] instanceDataArray;
        // Currently instanced count
        public int instanceCount;
        // Buffer size
        public int bufferSize;

        // Buffers Data
        public ComputeBuffer transformationMatrixVisibilityBuffer;
        public ComputeBuffer argsBuffer; // for multiple material (submesh) rendering
        public ComputeBuffer instanceLODDataBuffer; // for storing LOD data
        public uint[] args;

        public bool hasShadowCasterBuffer;
        public Material shadowCasterMaterial;
        public ComputeBuffer shadowArgsBuffer;
        public uint[] shadowArgs;

        public bool transformDataModified;

        public GPUInstancerRuntimeData(GPUInstancerPrototype prototype)
        {
            this.prototype = prototype;
        }

        public virtual void InitializeData()
        {
        }

        public virtual void ReleaseBuffers()
        {
        }

        #region AddLodAndRenderer

        /// <summary>
        /// Adds a new LOD and creates a single renderer for it.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="materials"></param>
        /// <param name="lodSize"></param>
        public virtual void AddLodAndRenderer(Mesh mesh, List<Material> materials, MaterialPropertyBlock mpb, bool castShadows, float lodSize = -1, MaterialPropertyBlock shadowMPB = null, 
            bool excludeBounds = false, int layer = 0, bool receiveShadows = true)
        {
            AddLod(lodSize, excludeBounds);
            AddRenderer(instanceLODs.Count - 1, mesh, materials, Matrix4x4.identity, mpb, castShadows, layer, shadowMPB, null, receiveShadows);
        }

        /// <summary>
        /// Registers an LOD to the prototype. LODs contain the renderers for instance prototypes,
        /// so even if no LOD is being used, the prototype must be registered as LOD0 using this method.
        /// </summary>
        /// <param name="screenRelativeTransitionHeight">if not defined, will default to 0</param>
        public virtual void AddLod(float screenRelativeTransitionHeight = -1, bool excludeBounds = false)
        {
            if (instanceLODs == null)
                instanceLODs = new List<GPUInstancerPrototypeLOD>();

            GPUInstancerPrototypeLOD instanceLOD = new GPUInstancerPrototypeLOD();
            instanceLOD.excludeBounds = excludeBounds;
            instanceLODs.Add(instanceLOD);

            // Ensure the LOD will render if this is the first LOD and lodDistance is not set.
            if (instanceLODs.Count == 1 && screenRelativeTransitionHeight < 0f)
                lodSizes[0] = 0;

            // Do not modify the lodDistances vector if LOD distance is not supplied.
            if (screenRelativeTransitionHeight < 0f)
                return;

            if (lodBiasApplied == -1)
                lodBiasApplied = QualitySettings.lodBias;

            int lodIndex = (instanceLODs.Count - 1) * 4;
            if (instanceLODs.Count > 4)
                lodIndex = (instanceLODs.Count - 5) * 4 + 1;

            float lodSize = (screenRelativeTransitionHeight / (prototype != null && prototype.lodBiasAdjustment > 0 ? prototype.lodBiasAdjustment : 1)) / lodBiasApplied;
            lodSizes[lodIndex] = lodSize;

            if (GPUInstancerUtility.matrixHandlingType != GPUIMatrixHandlingType.Default)
                prototype.isLODCrossFade = false;

            if (prototype.isLODCrossFade)
            {
                if (!prototype.isLODCrossFadeAnimate)
                {
                    float previousLodSize = 1;
                    if (instanceLODs.Count > 1)
                    {
                        if (instanceLODs.Count < 5)
                            previousLodSize = lodSizes[(instanceLODs.Count - 2) * 4];
                        else if (instanceLODs.Count == 5)
                            previousLodSize = lodSizes[12];
                        else
                            previousLodSize = lodSizes[(instanceLODs.Count - 6) * 4];
                    }
                    float dif = previousLodSize - lodSize;
                    float cfSize = lodSize + dif * prototype.lodFadeTransitionWidth;

                    lodSizes[lodIndex + 2] = cfSize;
                }
            }
        }

        /// <summary>
        /// Adds a renderer to an LOD. Renderers define the meshes and materials to render for a given instance prototype LOD.
        /// </summary>
        /// <param name="lod">The LOD to add this renderer to. LOD indices start from 0.</param>
        /// <param name="mesh">The mesh that this renderer will use.</param>
        /// <param name="materials">The list of materials that this renderer will use (must be GPU Instancer compatible materials)</param>
        /// <param name="transformOffset">The transformation matrix that represents a change in position, rotation and scale 
        /// for this renderer as an offset from the instance prototype. This matrix will be applied to the prototype instance 
        /// matrix for final rendering calculations in the shader. Use Matrix4x4.Identity if no offset is desired.</param>
        public virtual void AddRenderer(int lod, Mesh mesh, List<Material> materials, Matrix4x4 transformOffset, MaterialPropertyBlock mpb, bool castShadows,
            int layer = 0, MaterialPropertyBlock shadowMPB = null, Renderer rendererRef = null, bool receiveShadows = true)
        {

            if (instanceLODs == null || instanceLODs.Count <= lod || instanceLODs[lod] == null)
            {
                Debug.LogError("Can't add renderer: Invalid LOD");
                return;
            }

            if (mesh == null)
            {
                Debug.LogError("Can't add renderer: mesh is null. Make sure that all the MeshFilters on the objects has a mesh assigned.");
                return;
            }

            if (materials == null || materials.Count == 0)
            {
                Debug.LogError("Can't add renderer: no materials. Make sure that all the MeshRenderers have their materials assigned.");
                return;
            }

            if (instanceLODs[lod].renderers == null)
                instanceLODs[lod].renderers = new List<GPUInstancerRenderer>();
            
            GPUInstancerRenderer renderer = new GPUInstancerRenderer
            {
                mesh = mesh,
                materials = materials,
                transformOffset = transformOffset,
                mpb = mpb,
                shadowMPB = shadowMPB,
                layer = layer,
                castShadows = castShadows,
                receiveShadows = receiveShadows,
                rendererRef = rendererRef
            };

            instanceLODs[lod].renderers.Add(renderer);
            CalculateBounds();
        }

        public virtual void CalculateBounds()
        {
            if (instanceLODs == null || instanceLODs.Count == 0 || instanceLODs[0].renderers == null ||
                instanceLODs[0].renderers.Count == 0)
                return;

            Bounds rendererBounds;
            for (int lod = 0; lod < instanceLODs.Count; lod++)
            {
                if (instanceLODs[lod].excludeBounds)
                    continue;

                for (int r = 0; r < instanceLODs[lod].renderers.Count; r++)
                {
                    rendererBounds = new Bounds(instanceLODs[lod].renderers[r].mesh.bounds.center + (Vector3)instanceLODs[lod].renderers[r].transformOffset.GetColumn(3),
                        new Vector3(
                        instanceLODs[lod].renderers[r].mesh.bounds.size.x * instanceLODs[lod].renderers[r].transformOffset.GetRow(0).magnitude,
                        instanceLODs[lod].renderers[r].mesh.bounds.size.y * instanceLODs[lod].renderers[r].transformOffset.GetRow(1).magnitude,
                        instanceLODs[lod].renderers[r].mesh.bounds.size.z * instanceLODs[lod].renderers[r].transformOffset.GetRow(2).magnitude));
                    if (lod == 0 && r == 0)
                    {
                        instanceBounds = rendererBounds;
                        continue;
                    }
                    instanceBounds.Encapsulate(rendererBounds);

                    //Vector3[] verts = instanceLODs[lod].renderers[r].mesh.vertices;
                    //for (var v = 0; v < verts.Length; v++)
                    //    instanceBounds.Encapsulate(verts[v]);
                }
            }

            instanceBounds.size += prototype.boundsOffset;
        }

        #endregion AddLodAndRenderer

        #region CreateRenderersFromGameObject

        /// <summary>
        /// Generates instancing renderer data for a given GameObject, at the first LOD level.
        /// </summary>
        public virtual bool CreateRenderersFromGameObject(GPUInstancerPrototype prototype)
        {
            if (prototype.prefabObject == null)
                return false;

            if (prototype.isShadowCasting)
            {
                if (prototype.shadowLODMap == null || prototype.shadowLODMap.Length != 16)
                {
                    prototype.shadowLODMap = new float[] {
                                0, 4, 0, 0,
                                1, 5, 0, 0,
                                2, 6, 0, 0,
                                3, 7, 0, 0};
                }
            }

            if (prototype.prefabObject.GetComponent<LODGroup>() != null)
                return GenerateLODsFromLODGroup(prototype);
            else
            {
                if (instanceLODs == null || instanceLODs.Count == 0)
                    AddLod();
                return CreateRenderersFromMeshRenderers(0, prototype);
            }
        }

        /// <summary>
        /// Generates all LOD and render data from the supplied Unity LODGroup. Deletes all existing LOD data.
        /// </summary>
        /// <param name="prototype">The GPUI prototype</param>
        /// <param name="gpuiSettings">GPU Instancer settings to find appropriate shader for materials</param>
        /// <returns></returns>
        public virtual bool GenerateLODsFromLODGroup(GPUInstancerPrototype prototype)
        {
            LODGroup lodGroup = prototype.prefabObject.GetComponent<LODGroup>();

            if (instanceLODs == null)
                instanceLODs = new List<GPUInstancerPrototypeLOD>();
            else
                instanceLODs.Clear();

            for (int lod = 0; lod < lodGroup.GetLODs().Length; lod++)
            {
                bool hasBillboardRenderer = false;
                List<Renderer> lodRenderers = new List<Renderer>();
                if (lodGroup.GetLODs()[lod].renderers != null)
                {
                    foreach (Renderer renderer in lodGroup.GetLODs()[lod].renderers)
                    {
                        if (renderer != null && renderer is MeshRenderer && renderer.GetComponent<MeshFilter>() != null)
                        {
                            // Do not create runtime LOD renderer if this lod is a SpeedTree8 billboard and the GPUI generated billboard is used.
                            if (prototype.useGeneratedBillboard && prototype.treeType == GPUInstancerTreeType.SpeedTree8 && renderer.sharedMaterials[0].IsKeywordEnabled("EFFECT_BILLBOARD"))
                                hasBillboardRenderer = true;
                            else
                                lodRenderers.Add(renderer);
                        }
                        else if (renderer != null && renderer is BillboardRenderer)
                            hasBillboardRenderer = true;
                    }
                }

                if (!lodRenderers.Any())
                {
                    if (!hasBillboardRenderer)
                        Debug.LogWarning("LODGroup has no mesh renderers. Prefab: " + lodGroup.gameObject.name + " LODIndex: " + lod);
                    continue;
                }

                AddLod(lodGroup.GetLODs()[lod].screenRelativeTransitionHeight);

                for (int r = 0; r < lodRenderers.Count; r++)
                {
                    List<Material> instanceMaterials = new List<Material>();
                    for (int m = 0; m < lodRenderers[r].sharedMaterials.Length; m++)
                    {
                        instanceMaterials.Add(GPUInstancerConstants.gpuiSettings.shaderBindings.GetInstancedMaterial(lodRenderers[r].sharedMaterials[m]));
                        if (prototype.isLODCrossFade && GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                            instanceMaterials[m].EnableKeyword("LOD_FADE_CROSSFADE");
                    }

                    Matrix4x4 transformOffset = Matrix4x4.identity;
                    Transform currentTransform = lodRenderers[r].gameObject.transform;
                    while (currentTransform != lodGroup.gameObject.transform)
                    {
                        transformOffset = Matrix4x4.TRS(currentTransform.localPosition, currentTransform.localRotation, currentTransform.localScale) * transformOffset;
                        currentTransform = currentTransform.parent;
                    }

                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    lodRenderers[r].GetPropertyBlock(mpb);
                    MaterialPropertyBlock shadowMPB = null;
                    if (prototype.isShadowCasting)
                    {
                        shadowMPB = new MaterialPropertyBlock();
                        lodRenderers[r].GetPropertyBlock(shadowMPB);
                    }
                    AddRenderer(lod, lodRenderers[r].GetComponent<MeshFilter>().sharedMesh, instanceMaterials, transformOffset, mpb, lodRenderers[r].shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off, lodRenderers[r].gameObject.layer, shadowMPB, lodRenderers[r], lodRenderers[r].receiveShadows);
                }
            }
            return true;
        }

        /// <summary>
        /// Generates instancing renderer data for a given protoype from its Mesh renderers at the given LOD level.
        /// </summary>
        /// <param name="lod">Which LOD level to generate renderers in</param>
        /// <param name="prototype">GPU Instancer Prototype</param>
        /// <param name="gpuiSettings">GPU Instancer settings to find appropriate shader for materials</param>
        /// <returns></returns>
        public virtual bool CreateRenderersFromMeshRenderers(int lod, GPUInstancerPrototype prototype)
        {
            if (instanceLODs == null || instanceLODs.Count <= lod || instanceLODs[lod] == null)
            {
                Debug.LogError("Can't create renderer(s): Invalid LOD");
                return false;
            }

            if (!prototype.prefabObject)
            {
                Debug.LogError("Can't create renderer(s): reference GameObject is null");
                return false;
            }

            List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
            GetMeshRenderersOfTransform(prototype.prefabObject.transform, meshRenderers);

            if (meshRenderers == null || meshRenderers.Count == 0)
            {
                Debug.LogError("Can't create renderer(s): no MeshRenderers found in the reference GameObject <" + prototype.prefabObject.name +
                        "> or any of its children");
                return false;
            }

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer.GetComponent<MeshFilter>() == null)
                {
                    Debug.LogWarning("MeshRenderer with no MeshFilter found on GameObject <" + prototype.prefabObject.name +
                        "> (Child: <" + meshRenderer.gameObject + ">). Are you missing a component?");
                    continue;
                }
                
                List<Material> instanceMaterials = new List<Material>();

                for (int m = 0; m < meshRenderer.sharedMaterials.Length; m++)
                {
                    instanceMaterials.Add(GPUInstancerConstants.gpuiSettings.shaderBindings.GetInstancedMaterial(meshRenderer.sharedMaterials[m]));
                }

                Matrix4x4 transformOffset = Matrix4x4.identity;
                Transform currentTransform = meshRenderer.gameObject.transform;
                while (currentTransform != prototype.prefabObject.transform)
                {
                    transformOffset = Matrix4x4.TRS(currentTransform.localPosition, currentTransform.localRotation, currentTransform.localScale) * transformOffset;
                    currentTransform = currentTransform.parent;
                }

                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(mpb);
                MaterialPropertyBlock shadowMPB = null;
                if (prototype.isShadowCasting)
                {
                    shadowMPB = new MaterialPropertyBlock();
                    meshRenderer.GetPropertyBlock(shadowMPB);
                }
                AddRenderer(lod, meshRenderer.GetComponent<MeshFilter>().sharedMesh, instanceMaterials, transformOffset, mpb, 
                    meshRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off, meshRenderer.gameObject.layer, shadowMPB, meshRenderer, meshRenderer.receiveShadows);
            }

            return true;
        }

        public virtual void GetMeshRenderersOfTransform(Transform objectTransform, List<MeshRenderer> meshRenderers)
        {
            MeshRenderer meshRenderer = objectTransform.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderers.Add(meshRenderer);

            Transform childTransform;
            for (int i = 0; i < objectTransform.childCount; i++)
            {
                childTransform = objectTransform.GetChild(i);
                if (childTransform.GetComponent<GPUInstancerPrefab>() != null)
                    continue;
                GetMeshRenderersOfTransform(childTransform, meshRenderers);
            }
        }

        public bool IsLODShadowCasting(int lodLevel)
        {
            int lodIndex = lodLevel * 4;
            if (lodLevel >= 4)
                lodIndex = (lodLevel - 4) * 4 + 1;

            return prototype.isShadowCasting && prototype.shadowLODMap[lodIndex] < 8;
        }
        #endregion CreateRenderersFromGameObject
    }

    public class GPUInstancerPrototypeLOD
    {
        // Prototype Data
        public List<GPUInstancerRenderer> renderers; // support for multiple mesh renderers
        // Buffers Data
        public ComputeBuffer transformationMatrixAppendBuffer;
        public ComputeBuffer shadowAppendBuffer;
        public bool excludeBounds;

        public RenderTexture transformationMatrixAppendTexture;
        public RenderTexture shadowAppendTexture;

        public int argsBufferOffset { get { return renderers == null || renderers.Count == 0 ? -1 : renderers[0].argsBufferOffset; } }
    }

    public class GPUInstancerRenderer
    {
        public Mesh mesh;
        public List<Material> materials; // support for multiple submeshes.
        public Matrix4x4 transformOffset;
        public int argsBufferOffset;
        public MaterialPropertyBlock mpb;
        public MaterialPropertyBlock shadowMPB;
        public int layer;
        public bool castShadows;
        public bool receiveShadows;
        public Renderer rendererRef;
    }
}
