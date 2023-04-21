using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace MeshCombineStudio
{
    public class ObjectOctree
    {
        public class LODParent
        {
            public GameObject cellGO;
            public Transform cellT;

            public LODGroup lodGroup;
            public LODLevel[] lodLevels;
            public bool hasChanged;
            public int jobsPending;

            public LODParent(int lodCount)
            {
                lodLevels = new LODLevel[lodCount];
                for (int i = 0; i < lodLevels.Length; i++) lodLevels[i] = new LODLevel();
            }

            public void AssignLODGroup(MeshCombiner meshCombiner)
            {
                LOD[] lods = new LOD[lodLevels.Length];
                int lodGroupParentIndex = lods.Length - 1;
                
                for (int i = 0; i < lodLevels.Length; i++)
                {
                    LODLevel lodLevel = lodLevels[i];
                    // Debug.Log(i + " " + lodLevel.newMeshRenderers.Count);
                    lods[i] = new LOD(meshCombiner.lodGroupsSettings[lodGroupParentIndex].lodSettings[i].screenRelativeTransitionHeight, lodLevel.newMeshRenderers.ToArray());
                }

                lodGroup.SetLODs(lods);
                lodGroup.size = meshCombiner.cellSize;
                meshCombiner.lodGroupsSettings[lodGroupParentIndex].CopyToLodGroup(lodGroup, lods);
            }

            public void ApplyChanges(MeshCombiner meshCombiner)
            {
                for (int i = 0; i < lodLevels.Length; i++) lodLevels[i].ApplyChanges(meshCombiner);
                hasChanged = false;
            }
        }

        public class LODLevel
        {
            public FastList<CachedGameObject> cachedGOs = new FastList<CachedGameObject>();
            public Dictionary<CombineCondition, MeshObjectsHolder> meshObjectsHoldersLookup;
            public FastList<MeshObjectsHolder> changedMeshObjectsHolders;
            public FastList<MeshRenderer> newMeshRenderers = new FastList<MeshRenderer>();
            public int vertCount, objectCount = 0;
           
            public void ApplyChanges(MeshCombiner meshCombiner)
            {
                for (int i = 0; i < changedMeshObjectsHolders.Count; i++)
                {
                    MeshObjectsHolder meshObjectHolder = changedMeshObjectsHolders.items[i];
                    meshObjectHolder.hasChanged = false;


                }
                changedMeshObjectsHolders.Clear();
            }
        }

        public class MaxCell : Cell
        {
            static public int maxCellCount;
            public LODParent[] lodParents;
            public List<LODParent> changedLodParents;
            public bool hasChanged;

            public void ApplyChanges(MeshCombiner meshCombiner)
            {
                for (int i = 0; i < changedLodParents.Count; i++) changedLodParents[i].ApplyChanges(meshCombiner);
                changedLodParents.Clear();
                hasChanged = false;
            }
        }

        public class Cell : BaseOctree.Cell
        {
            public Cell[] cells;

            public Cell() { }
            public Cell(Vector3 position, Vector3 size, int maxLevels) : base(position, size, maxLevels) { }

            public MaxCell GetCell(Vector3 position)
            {
                if (!InsideBounds(position)) return null;

                return GetCellInternal(position);
            }

            MaxCell GetCellInternal(Vector3 position)
            {
                if (level == maxLevels)
                {
                    return (MaxCell)this;
                }
                else
                {
                    Cell cell = GetCell(cells, position);
                    if (cell == null) return null;

                    return cell.GetCellInternal(position);
                }
            }

            public CachedGameObject AddObject(Vector3 position, MeshCombiner meshCombiner, CachedGameObject cachedGO, int lodParentIndex, int lodLevel, bool isChangeMode = false)
            {
                if (InsideBounds(position))
                {
                    AddObjectInternal(meshCombiner, cachedGO, position, lodParentIndex, lodLevel, isChangeMode);
                    return cachedGO;
                }
                return null;
            }

            void AddObjectInternal(MeshCombiner meshCombiner, CachedGameObject cachedGO, Vector3 position, int lodParentIndex, int lodLevel, bool isChangeMode)
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;

                    if (thisCell.lodParents == null) thisCell.lodParents = new LODParent[10];
                    if (thisCell.lodParents[lodParentIndex] == null) thisCell.lodParents[lodParentIndex] = new LODParent(lodParentIndex + 1);

                    LODParent lodParent = thisCell.lodParents[lodParentIndex];
                    LODLevel lod = lodParent.lodLevels[lodLevel];
                    
                    lod.cachedGOs.Add(cachedGO);
                    if (isChangeMode)
                    {
                        if (SortObject(meshCombiner, lod, cachedGO))
                        {
                            if (!thisCell.hasChanged)
                            {
                                thisCell.hasChanged = true;
                                if (meshCombiner.changedCells == null) meshCombiner.changedCells = new List<MaxCell>();
                                meshCombiner.changedCells.Add(thisCell);
                            }
                            if (!lodParent.hasChanged)
                            {
                                lodParent.hasChanged = true;
                                thisCell.changedLodParents.Add(lodParent);
                            }
                        }
                    }

                    lod.objectCount++;

                    lod.vertCount += cachedGO.mesh.vertexCount;
                    return;
                }
                else
                {
                    bool maxCellCreated;
                    int index = AddCell<Cell, MaxCell>(ref cells, position, out maxCellCreated);
                    if (maxCellCreated) MaxCell.maxCellCount++;
                    cells[index].AddObjectInternal(meshCombiner, cachedGO, position, lodParentIndex, lodLevel, isChangeMode);
                }
            }
            
            public void SortObjects(MeshCombiner meshCombiner)
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;

                    LODParent[] lodParents = thisCell.lodParents;

                    for (int i = 0; i < lodParents.Length; i++)
                    {
                        LODParent lodParent = lodParents[i];
                        if (lodParent == null) continue;

                        for (int j = 0; j < lodParent.lodLevels.Length; j++)
                        {
                            LODLevel lod = lodParent.lodLevels[j];
                            
                            if (lod == null || lod.cachedGOs.Count == 0) return;

                            for (int k = 0; k < lod.cachedGOs.Count; ++k)
                            {
                                CachedGameObject cachedGO = lod.cachedGOs.items[k];

                                if (!SortObject(meshCombiner, lod, cachedGO))
                                {
                                    lod.cachedGOs.RemoveAt(k--);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        if (cellsUsed[i]) cells[i].SortObjects(meshCombiner);
                    }
                }
            }

            public bool SortObject(MeshCombiner meshCombiner, LODLevel lod, CachedGameObject cachedGO, bool isChangeMode = false)
            {
                if (cachedGO.mr == null) return false;

                if (lod.meshObjectsHoldersLookup == null) lod.meshObjectsHoldersLookup = new Dictionary<CombineCondition, MeshObjectsHolder>();

                CombineConditionSettings combineConditions = meshCombiner.combineConditionSettings;
                Material[] mats = cachedGO.mr.sharedMaterials;

                // TODO check submeshes and material
                int length = Mathf.Min(cachedGO.mesh.subMeshCount, mats.Length);

                int rootInstanceId = -1;

                if (meshCombiner.combineMode == CombineMode.DynamicObjects)
                {
                    rootInstanceId = cachedGO.rootInstanceId;

                    if (rootInstanceId == -1)
                    {
                        cachedGO.GetRoot();
                        rootInstanceId = cachedGO.rootInstanceId;
                    }
                }

                for (int l = 0; l < length; l++)
                {
                    Material mat;

                    if (combineConditions.sameMaterial)
                    {
                        mat = mats[l];
                        if (mat == null) continue;
                    }
                    else mat = combineConditions.material;

                    CombineCondition combineCondition = new CombineCondition();
                    combineCondition.ReadFromGameObject(rootInstanceId, combineConditions, meshCombiner.copyBakedLighting && meshCombiner.validCopyBakedLighting, cachedGO.go, cachedGO.t, cachedGO.mr, mat);

                    MeshObjectsHolder meshObjectHolder;
                    if (!lod.meshObjectsHoldersLookup.TryGetValue(combineCondition, out meshObjectHolder))
                    {
                        meshCombiner.foundCombineConditions.combineConditions.Add(combineCondition);
                        meshObjectHolder = new MeshObjectsHolder(ref combineCondition, mat);
                        lod.meshObjectsHoldersLookup.Add(combineCondition, meshObjectHolder);
                    }

                    meshObjectHolder.meshObjects.Add(new MeshObject(cachedGO, l));

                    if (isChangeMode && !meshObjectHolder.hasChanged)
                    {
                        meshObjectHolder.hasChanged = true;
                        lod.changedMeshObjectsHolders.Add(meshObjectHolder);
                    }
                }
                
                return true;
            }

            public void CombineMeshes(MeshCombiner meshCombiner, int lodParentIndex)
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;
                    
                    LODParent lodParent = thisCell.lodParents[lodParentIndex];
                    if (lodParent == null) return;

                    CombineMode combineMode = meshCombiner.combineMode;

                    if (combineMode != CombineMode.DynamicObjects)
                    {
                        lodParent.cellGO = new GameObject(meshCombiner.combineMode == CombineMode.StaticObjects ? "Cell " + bounds.center : "Combined Objects");
                        lodParent.cellT = lodParent.cellGO.transform;
                        lodParent.cellT.position = bounds.center;
                        lodParent.cellT.parent = meshCombiner.lodParentHolders[lodParentIndex].t;
                    }

                    if (lodParentIndex > 0)
                    {
                        lodParent.lodGroup = lodParent.cellGO.AddComponent<LODGroup>();
                        lodParent.lodGroup.localReferencePoint = lodParent.cellT.position = bounds.center;
                    }

                    LODLevel[] lods = lodParent.lodLevels;
                    for (int i = 0; i < lods.Length; i++)
                    {
                        LODLevel lod = lodParent.lodLevels[i];
                        if (lod == null || lod.meshObjectsHoldersLookup == null) return;

                        GameObject lodGO;
                        Transform lodT = null;

                        if (lodParentIndex > 0)
                        {
                            lodGO = new GameObject("LOD" + i);
                            lodT = lodGO.transform;
                            lodT.parent = lodParent.cellT;
                        }

                        foreach (MeshObjectsHolder sortedMeshes in lod.meshObjectsHoldersLookup.Values)
                        {
                            sortedMeshes.lodParent = lodParent;
                            sortedMeshes.lodLevel = i;
                            Vector3 position = (combineMode == CombineMode.DynamicObjects ? sortedMeshes.meshObjects.items[0].cachedGO.rootT.position : bounds.center);
                            MeshCombineJobManager.instance.AddJob(meshCombiner, sortedMeshes, lodParentIndex > 0 ? lodT : lodParent.cellT, position);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        if (cellsUsed[i]) cells[i].CombineMeshes(meshCombiner, lodParentIndex);
                    }
                }
            }
            
            public void Draw(MeshCombiner meshCombiner, bool onlyMaxLevel, bool drawLevel0)
            {
                if (!onlyMaxLevel || level == maxLevels || (drawLevel0 && level == 0))
                {
                    Gizmos.DrawWireCube(bounds.center, bounds.size);

                    if (level == maxLevels)
                    {
                        if (meshCombiner.drawMeshBounds)
                        {
                            MaxCell thisCell = (MaxCell)this;

                            LODParent[] lodParents = thisCell.lodParents;

                            for (int i = 0; i < lodParents.Length; i++)
                            {
                                if (lodParents[i] == null) continue;

                                LODLevel[] lods = lodParents[i].lodLevels;

                                Gizmos.color = meshCombiner.activeOriginal ? Color.blue : Color.green;
                                for (int j = 0; j < lods.Length; j++)
                                {
                                    for (int k = 0; k < lods[j].cachedGOs.Count; k++)
                                    {
                                        if (lods[j].cachedGOs.items[k].mr == null) continue;
                                        Bounds meshBounds = lods[j].cachedGOs.items[k].mr.bounds;
                                        Gizmos.DrawWireCube(meshBounds.center, meshBounds.size);
                                    }
                                }
                                Gizmos.color = Color.white;
                            }
                            return;
                        }
                    }
                }

                if (cells == null || cellsUsed == null) { return; }
                
                for (int i = 0; i < 8; i++)
                {
                    if (cellsUsed[i]) cells[i].Draw(meshCombiner, onlyMaxLevel, drawLevel0);
                }
            }
        }
    }

    [Serializable]
    public class MeshObjectsHolder
    {
        public FastList<MeshObject> meshObjects = new FastList<MeshObject>();
        public ObjectOctree.LODParent lodParent;
        public FastList<CachedGameObject> newCachedGOs;
        public int lodLevel;
        public Material mat;
        public bool hasChanged;
        public CombineCondition combineCondition;
        
        
        public MeshObjectsHolder(ref CombineCondition combineCondition, Material mat)
        {
            // Debug.Log(useForLightmapping);
            this.mat = mat;
            this.combineCondition = combineCondition;
        }
    }

    [Serializable]
    public class FoundCombineConditions
    {
        public HashSet<CombineCondition> combineConditions = new HashSet<CombineCondition>();
        public int combineConditionsCount;
        public int matCount, lightmapIndexCount, shadowCastingCount, receiveShadowsCount, lightmapScale, receiveGICount, lightProbeUsageCount, reflectionProbeUsageCount, probeAnchorCount;
        public int motionVectorGenerationModeCount, layerCount, staticEditorFlagsCount;
    }

    [Serializable]
    public struct CombineCondition
    {
        public static HashSet<object> countSet = new HashSet<object>();

        public int matInstanceId;
        public int lightmapIndex;
        public ShadowCastingMode shadowCastingMode;
        public bool receiveShadows;
        public float lightmapScale;
        
        public LightProbeUsage lightProbeUsage;
        public ReflectionProbeUsage reflectionProbeUsage;
        public Transform probeAnchor;

        public MotionVectorGenerationMode motionVectorGenerationMode;
        public int layer;
#if UNITY_EDITOR
#if !UNITY_2017 && !UNITY_2018 && !UNITY_2019_1
        public ReceiveGI receiveGI;
#endif
        public UnityEditor.StaticEditorFlags staticEditorFlags;
#endif
        public int rootInstanceId;

        public static CombineCondition Default
        {
            get
            {
                return new CombineCondition()
                {
                    matInstanceId = -1,
                    lightmapIndex = -1,
                    shadowCastingMode = ShadowCastingMode.On,
                    receiveShadows = true,
                    lightmapScale = 1,

                    lightProbeUsage = LightProbeUsage.BlendProbes,
                    reflectionProbeUsage = ReflectionProbeUsage.BlendProbes,
                    probeAnchor = null,

                    motionVectorGenerationMode = MotionVectorGenerationMode.Camera,
                    layer = 0,
#if UNITY_EDITOR
#if !UNITY_2017 && !UNITY_2018 && !UNITY_2019_1
                    receiveGI = ReceiveGI.Lightmaps,
#endif
                    staticEditorFlags = 0,
#endif
                    rootInstanceId = -1
                };
            }
        }

        public static void MakeFoundReport(FoundCombineConditions fcc)
        {
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.matInstanceId); } fcc.matCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.lightmapIndex); } fcc.lightmapIndexCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.shadowCastingMode); } fcc.shadowCastingCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.receiveShadows); } fcc.receiveShadowsCount = countSet.Count;                
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.lightmapScale); } fcc.lightmapScale = countSet.Count; 
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.lightProbeUsage); } fcc.lightProbeUsageCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.reflectionProbeUsage); } fcc.reflectionProbeUsageCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.probeAnchor); } fcc.probeAnchorCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.motionVectorGenerationMode); } fcc.motionVectorGenerationModeCount = countSet.Count;
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.layer); } fcc.layerCount = countSet.Count;
#if UNITY_EDITOR
#if !UNITY_2017 && !UNITY_2018 && !UNITY_2019_1
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.receiveGI); } fcc.receiveGICount = countSet.Count;
#endif           
            countSet.Clear(); foreach (var combineCondition in fcc.combineConditions) { countSet.Add(combineCondition.staticEditorFlags); } fcc.staticEditorFlagsCount = countSet.Count;
#endif
            fcc.combineConditionsCount = fcc.combineConditions.Count;
        }

        public void ReadFromGameObject(int rootInstanceId, CombineConditionSettings combineConditions, bool copyBakedLighting, GameObject go, Transform t, MeshRenderer mr, Material mat)
        {
            matInstanceId = (combineConditions.sameMaterial ? mat.GetInstanceID() : combineConditions.combineCondition.matInstanceId);
            lightmapIndex = (copyBakedLighting ? mr.lightmapIndex : lightmapIndex = -1);
            shadowCastingMode = (combineConditions.sameShadowCastingMode ? mr.shadowCastingMode : combineConditions.combineCondition.shadowCastingMode);
            receiveShadows = (combineConditions.sameReceiveShadows ? mr.receiveShadows : combineConditions.combineCondition.receiveShadows);
            lightmapScale = (combineConditions.sameLightmapScale ? GetLightmapScale(mr) : combineConditions.combineCondition.lightmapScale);
            
            lightProbeUsage = (combineConditions.sameLightProbeUsage ? mr.lightProbeUsage : combineConditions.combineCondition.lightProbeUsage);
            reflectionProbeUsage = (combineConditions.sameReflectionProbeUsage ? mr.reflectionProbeUsage : combineConditions.combineCondition.reflectionProbeUsage);
            probeAnchor = (combineConditions.sameProbeAnchor ? mr.probeAnchor : combineConditions.combineCondition.probeAnchor);

            motionVectorGenerationMode = (combineConditions.sameMotionVectorGenerationMode ? mr.motionVectorGenerationMode : combineConditions.combineCondition.motionVectorGenerationMode);

            layer = (combineConditions.sameLayer ? go.layer : combineConditions.combineCondition.layer);
#if UNITY_EDITOR
#if !UNITY_2017 && !UNITY_2018 && !UNITY_2019_1
            receiveGI = (combineConditions.sameReceiveGI ? mr.receiveGI : combineConditions.combineCondition.receiveGI);
#endif
            staticEditorFlags = (combineConditions.sameStaticEditorFlags ? UnityEditor.GameObjectUtility.GetStaticEditorFlags(go) : combineConditions.combineCondition.staticEditorFlags);
#endif
            this.rootInstanceId = rootInstanceId;
            // Debug.Log(go.name + " " + shadowCastingMode);
        }

        float GetLightmapScale(MeshRenderer mr)
        {
#if UNITY_EDITOR
            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(mr);
            return so.FindProperty("m_ScaleInLightmap").floatValue;
#else
            return 1;
#endif
        }

        void SetLightmapScale(MeshRenderer mr, float lightmapScale)
        {
#if UNITY_EDITOR
            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(mr);
            so.FindProperty("m_ScaleInLightmap").floatValue = lightmapScale;
            so.ApplyModifiedProperties();
#else
            return;
#endif
        }

        public void WriteToGameObject(GameObject go, MeshRenderer mr)
        {
            mr.lightmapIndex = lightmapIndex;
            mr.shadowCastingMode = shadowCastingMode;
            mr.receiveShadows = receiveShadows;
            if (lightmapScale != 1) SetLightmapScale(mr, lightmapScale);

            mr.lightProbeUsage = lightProbeUsage;
            mr.reflectionProbeUsage = reflectionProbeUsage;
            mr.motionVectorGenerationMode = motionVectorGenerationMode;
            mr.probeAnchor = probeAnchor;
            
            go.layer = layer;
#if UNITY_EDITOR
#if !UNITY_2017 && !UNITY_2018 && !UNITY_2019_1
            mr.receiveGI = receiveGI;
#endif
            staticEditorFlags &= ~UnityEditor.StaticEditorFlags.BatchingStatic;
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(go, staticEditorFlags);
#endif
        }
    }

    [Serializable]
    public class CombineConditionSettings
    {
        public bool foldout = true;

        public bool sameMaterial = true;

        public bool sameShadowCastingMode;
        public bool sameReceiveShadows;
#if !UNITY_2017 && !UNITY_2018
        public bool sameReceiveGI;
#endif
        public bool sameLightmapScale;

        public bool sameLightProbeUsage;
        public bool sameReflectionProbeUsage;
        public bool sameProbeAnchor;

        public bool sameMotionVectorGenerationMode;
        public bool sameStaticEditorFlags;
        public bool sameLayer;

        public Material material;

        public CombineCondition combineCondition = CombineCondition.Default;
    }


    [Serializable]
    public class MeshObject
    {
        public CachedGameObject cachedGO;
        public MeshCache meshCache;
        public int subMeshIndex;
        public Vector3 position, scale;
        public Quaternion rotation;
        public Vector4 lightmapScaleOffset;
        public bool intersectsSurface;
        public int startNewTriangleIndex, newTriangleCount;
        public bool skip;

        public MeshObject(CachedGameObject cachedGO, int subMeshIndex)
        {
            this.cachedGO = cachedGO;
            this.subMeshIndex = subMeshIndex;
            Transform t = cachedGO.t;
            position = t.position;
            rotation = t.rotation;
            scale = t.lossyScale;
            lightmapScaleOffset = cachedGO.mr.lightmapScaleOffset;
        }
    }

    [Serializable]
    public class CachedGameObject
    {
        public Transform searchParentT;
        public GameObject go;
        public Transform t;
        public MeshRenderer mr;
        public MeshFilterRevert mfr;
        public MeshFilter mf;
        public Mesh mesh;
        public Matrix4x4 mt;
        public Matrix4x4 mtNormals;
        public Transform rootT;
        public Vector3 rootTLossyScale;
        public int rootInstanceId = -1;
        public bool excludeCombine;
        public bool mrEnabled;

        public CachedGameObject(Transform searchParentT, GameObject go, Transform t, MeshRenderer mr, MeshFilter mf, Mesh mesh)
        {
            this.searchParentT = searchParentT;
            this.go = go;
            this.t = t;
            this.mr = mr;
            this.mf = mf;
            this.mesh = mesh;
            mt = t.localToWorldMatrix;
            mrEnabled = mr.enabled;
            mtNormals = mt.inverse.transpose;
        }

        public CachedGameObject(CachedComponents cachedComponent)
        {
            go = cachedComponent.go;
            t = cachedComponent.t;
            mr = cachedComponent.mr;
            mf = cachedComponent.mf;
            mesh = cachedComponent.mf.sharedMesh;
            mt = t.localToWorldMatrix;
            mtNormals = mt.inverse.transpose;
        }

        public void GetRoot()
        {
            rootT = Methods.GetChildRootTransform(t, searchParentT);
            rootInstanceId = rootT.GetInstanceID();
            rootTLossyScale = rootT.lossyScale;
        }
    }

    [Serializable]
    public class CachedLodGameObject : CachedGameObject
    {
        public Vector3 center;
        public int lodCount, lodLevel;

        public CachedLodGameObject(CachedGameObject cachedGO, int lodCount, int lodLevel) : base(cachedGO.searchParentT, cachedGO.go, cachedGO.t, cachedGO.mr, cachedGO.mf, cachedGO.mesh)
        {
            this.lodCount = lodCount;
            this.lodLevel = lodLevel;
        }
    }
}