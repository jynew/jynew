using UnityEngine;
using System.Collections.Generic;
using System;

namespace MeshCombineStudio
{
    public enum CombineMode { StaticObjects, DynamicObjects };//, OneDynamicObject };

    [ExecuteInEditMode]
    public class MeshCombiner : MonoBehaviour
    {
        public static EventMethod onInit;
        public static List<MeshCombiner> instances = new List<MeshCombiner>();
         
        public enum ObjectType { Normal, LodGroup, LodRenderer }
        public enum HandleComponent { Disable, Destroy };
        public enum ObjectCenter { BoundsCenter, TransformPosition };
        public enum BackFaceTriangleMode { Transform, Box, Direction, EulerAngles }
        public delegate void EventMethod(MeshCombiner meshCombiner);

        public event EventMethod onCombiningStart;
        public event EventMethod onCombiningAbort;
        public event EventMethod onCombiningReady; 

        public MeshCombineJobManager.JobSettings jobSettings = new MeshCombineJobManager.JobSettings(); 
        public LODGroupSettings[] lodGroupsSettings;

        public ComputeShader computeDepthToArray;
        public bool useCustomInstantiatePrefab;
        public GameObject instantiatePrefab;
        public bool instantiatePrefabValid;
        public const int maxLodCount = 8;
        public string saveMeshesFolder;

        public ObjectOctree.Cell octree;
        public List<ObjectOctree.MaxCell> changedCells;
        [NonSerialized] public bool octreeContainsObjects;

        public bool unitySettingsFoldout = true;

        // Search Conditions
        public SearchOptions searchOptions;

        // Original Objects
        public bool useOriginalObjectsHideFlags;
        public HideFlags orginalObjectsHideFlags;

        // Combine Conditions
        public CombineConditionSettings combineConditionSettings;

        // Output Settings
        public bool outputSettingsFoldout = true;
        public CombineMode combineMode;
        public int cellSize = 32;
        public Vector3 cellOffset;
        public int cellCount;

        public bool removeOriginalMeshReference = false;
        public bool usedRemoveOriginalMeshRederences;
        public bool useVertexOutputLimit;
        public int vertexOutputLimit = 64000;
        public enum RebakeLightingMode { CopyLightmapUvs, RegenarateLightmapUvs };
        public RebakeLightingMode rebakeLightingMode;
        public bool copyBakedLighting, validCopyBakedLighting;
        public bool rebakeLighting, validRebakeLighting;

        public float scaleInLightmap = 1;
        public bool addMeshColliders = false;
        public PhysicMaterial physicsMaterial;
        public bool addMeshCollidersInRange = false;
        public Bounds addMeshCollidersBounds;

        public bool makeMeshesUnreadable = true;
        public bool excludeSingleMeshes = false;

        public bool removeTrianglesBelowSurface;
        public bool noColliders;
        public LayerMask surfaceLayerMask;
        public float maxSurfaceHeight = 1000;

        public bool removeOverlappingTriangles;
        public bool removeSamePositionTriangles;
        public bool reportFoundObjectsNotOnOverlapLayerMask = true;
        public GameObject overlappingCollidersGO;
        public LayerMask overlapLayerMask;
        public int voxelizeLayer;
        public int lodGroupLayer;
        public GameObject overlappingNonCombineGO;
        public bool disableOverlappingNonCombineGO;

        public bool removeBackFaceTriangles;
        public BackFaceTriangleMode backFaceTriangleMode;
        public Transform backFaceT;
        public Vector3 backFaceDirection;
        public Vector3 backFaceRotation;
        public Bounds backFaceBounds;

#if MCSCaves
        public bool useExcludeOverlapRemovalTag;
        public string excludeOverlapRemovalTag;
#endif

        public bool useExcludeBackfaceRemovalTag;
        public string excludeBackfaceRemovalTag;

        public bool weldVertices;
        public bool weldSnapVertices;
        public float weldSnapSize = 0.025f;
        public bool weldIncludeNormals;

        // Job Settings
        public bool jobSettingsFoldout = true;

        // Runtime Settings
        public bool runtimeSettingsFoldout = true;
        public bool combineInRuntime;
        public bool combineOnStart = true;
        public bool useCombineSwapKey;
        public KeyCode combineSwapKey = KeyCode.Tab;
        public HandleComponent originalMeshRenderers = HandleComponent.Disable;
        public HandleComponent originalLODGroups = HandleComponent.Disable;

        // Mesh Save Settings
        public bool meshSaveSettingsFoldout = true;
        public bool deleteFilesFromSaveFolder;

        public Vector3 oldPosition, oldScale;
        public LodParentHolder[] lodParentHolders = new LodParentHolder[maxLodCount];

        // Is stored in data script to make Inspector faster
        [HideInInspector] public List<GameObject> combinedGameObjects = new List<GameObject>();
        [HideInInspector] public List<CachedGameObject> foundObjects = new List<CachedGameObject>();
        [HideInInspector] public List<CachedLodGameObject> foundLodObjects = new List<CachedLodGameObject>();
        [HideInInspector] public List<LODGroup> foundLodGroups = new List<LODGroup>();
        [HideInInspector] public List<Collider> foundColliders = new List<Collider>();
        
        public HashSet<LODGroup> uniqueFoundLodGroups = new HashSet<LODGroup>();
        
        public HashSet<Mesh> unreadableMeshes = new HashSet<Mesh>();
        public HashSet<Mesh> selectImportSettingsMeshes = new HashSet<Mesh>();
        public FoundCombineConditions foundCombineConditions = new FoundCombineConditions();

        public HashSet<MeshCombineJobManager.MeshCombineJob> meshCombineJobs = new HashSet<MeshCombineJobManager.MeshCombineJob>();
        public int totalMeshCombineJobs;

        public int mrDisabledCount = 0; 
        
        public bool combined = false;
        public bool isCombining = false;
        public bool activeOriginal = true;

        public bool combinedActive;
        public bool drawGizmos = true;
        public bool drawMeshBounds = true;
        
        public int originalTotalVertices, originalTotalTriangles;
        public int newTotalVertices, newTotalTriangles;
        public int originalDrawCalls, newDrawCalls;

        public int originalTotalNormalChannels, originalTotalTangentChannels;
        public int originalTotalUvChannels, originalTotalUv2Channels, originalTotalUv3Channels, originalTotalUv4Channels;
        public int originalTotalColorChannels;

        public int newTotalNormalChannels, newTotalTangentChannels;
        public int newTotalUvChannels, newTotalUv2Channels, newTotalUv3Channels, newTotalUv4Channels;
        public int newTotalColorChannels;

        public float combineTime;

        [NonSerialized] public MeshCombinerData data;
        public FastList<MeshColliderAdd> addMeshCollidersList = new FastList<MeshColliderAdd>();
        HashSet<Transform> uniqueLodObjects = new HashSet<Transform>();

        [NonSerialized] MeshCombiner thisInstance;
        
        bool hasFoundFirstObject;
        Bounds bounds;

        [Serializable]
        public class SearchOptions
        {
            public bool foldoutSearchParents = true;
            public bool foldoutSearchConditions = true;

            public enum ComponentCondition { And, Or, Not };
            public enum LODGroupSearchMode { LodGroup, LodRenderers };

            public GameObject parent;
            public GameObject[] parentGOs;
            public ObjectCenter objectCenter;
            public LODGroupSearchMode lodGroupSearchMode;
            public bool useSearchBox = false;
            public Bounds searchBoxBounds;
            public bool searchBoxSquare;
            public Vector3 searchBoxPivot;
            public Vector3 searchBoxSize = new Vector3(25, 25, 25); 
            public bool useMaxBoundsFactor = true;
            public float maxBoundsFactor = 1.5f;  
            public bool useVertexInputLimit = true;
            public int vertexInputLimit = 5000;

            public bool useLayerMask;
            public LayerMask layerMask = ~0;
            public bool useTag;
            public string tag;
            public bool useNameContains;
            public List<string> nameContainList = new List<string>();
            public bool onlyActive = true;
#if UNITY_EDITOR
            public UnityEditor.StaticEditorFlags editorStatic = UnityEditor.StaticEditorFlags.BatchingStatic;
#endif
            public bool onlyStatic = true;
            public bool onlyActiveMeshRenderers = true;
            public bool useComponentsFilter;
            public ComponentCondition componentCondition;
            public List<string> componentNameList = new List<string>();

            public void GetSearchBoxBounds()
            {
                searchBoxBounds = new Bounds(searchBoxPivot + new Vector3(0, searchBoxSize.y * 0.5f, 0), searchBoxSize);
            }
        }
       
        public void AddMeshColliders()
        {
            try
            {
                for (int i = 0; i < addMeshCollidersList.Count; i++)
                {
                    MeshColliderAdd mca = addMeshCollidersList.items[i];
                    var mc = mca.go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mca.mesh;
                    mc.material = physicsMaterial;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                addMeshCollidersList.Clear();
            }
        }

        public void ExecuteOnCombiningReady()
        {
            totalMeshCombineJobs = 0;

#if MCSCaves
            if (removeOverlappingTriangles)
            {
                CreateOverlapColliders.DestroyOverlapColliders(overlappingCollidersGO);
                if (overlappingNonCombineGO && disableOverlappingNonCombineGO) overlappingNonCombineGO.SetActive(false);
            }
#endif
            ExecuteHandleObjects(false, HandleComponent.Disable, HandleComponent.Disable);

            stopwatch.Stop();
            combineTime = (float)stopwatch.ElapsedMilliseconds / 1000;

            combinedActive = true;
            combined = true;
            isCombining = false;

            var lodGroupSetups = GetComponentsInChildren<LODGroupSetup>();
            if (lodGroupSetups != null)
            {
                foreach(var lodGroupSetup in lodGroupSetups)
                {
                    // ensure that lod groups are applied after combining
                    try
                    {
                        lodGroupSetup.ApplySetup();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("An error occurred applying lod setup");
                        Debug.LogException(e);
                    }
                }
            }

            if (onCombiningReady != null) onCombiningReady(this);
        }
        
        void Awake()
        {
            Init();
        }

        void OnEnable() 
        {
            Init();
        }
        void Init()
        {
            if (thisInstance == null)
            {
                instances.Add(this);
                thisInstance = this;

                if (onInit != null) onInit(this);
            }
        }

        void OnDisable()
        {
            thisInstance = null;
            instances.Remove(this);
        }
         
        public void InitData()
        {
            if ((searchOptions.parentGOs == null || searchOptions.parentGOs.Length == 0) && searchOptions.parent)
            {
                searchOptions.parentGOs = new GameObject[] { searchOptions.parent };
            }

            if (data == null)
            {
                data = GetComponent<MeshCombinerData>();
                if (data == null)
                {
                    data = gameObject.AddComponent<MeshCombinerData>();

                    data.combinedGameObjects = new List<GameObject>(combinedGameObjects);
                    data.foundObjects = new List<CachedGameObject>(foundObjects);
                    data.foundLodObjects = new List<CachedLodGameObject>(foundLodObjects);
                    data.foundLodGroups = new List<LODGroup>(foundLodGroups);
                    data.foundColliders = new List<Collider>(foundColliders);

                    combinedGameObjects.Clear();
                    foundObjects.Clear();
                    foundLodObjects.Clear();
                    foundLodGroups.Clear();
                    foundColliders.Clear();
                }
            }
        }

        void Start()
        {
            if (Application.isPlaying && !combineInRuntime) return;

            InitMeshCombineJobManager();
            if (instances[0] == this)
                MeshCombineJobManager.instance.SetJobMode(jobSettings);

            if (!Application.isPlaying && Application.isEditor) return;

            // Debug.Log("Start");
            StartRuntime();
        } 
        // ==========================================================================================================================

        void OnDestroy()
        {
            RestoreOriginalRenderersAndLODGroups(true);

            thisInstance = null;
            instances.Remove(this);

            // if (!Application.isPlaying && Application.isEditor) return;

            if (instances.Count == 0 && MeshCombineJobManager.instance != null)
            {
                Methods.Destroy(MeshCombineJobManager.instance.gameObject);
                MeshCombineJobManager.instance = null;
            }
        }

        static public MeshCombiner GetInstance(string name)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i].gameObject.name == name) return instances[i];
            }
            return null;
        }

        public void CopyJobSettingsToAllInstances()
        {
            for (int i = 0; i < instances.Count; i++) instances[i].jobSettings.CopySettings(jobSettings);
        }

        public void InitMeshCombineJobManager()
        {
            if (MeshCombineJobManager.instance == null)
            {
                MeshCombineJobManager.CreateInstance(this, instantiatePrefab);
            }
        }

        public void CreateLodGroupsSettings()
        {
            lodGroupsSettings = new LODGroupSettings[maxLodCount];
            for (int i = 0; i < lodGroupsSettings.Length; i++) lodGroupsSettings[i] = new LODGroupSettings(i);
        }

        private void StartRuntime() 
        {
            if (combineInRuntime)
            {
                if (combineOnStart) CombineAll();
                if (useCombineSwapKey && originalMeshRenderers == HandleComponent.Disable && originalLODGroups == HandleComponent.Disable)
                {
                    if (SwapCombineKey.instance == null) gameObject.AddComponent<SwapCombineKey>(); else SwapCombineKey.instance.meshCombinerList.Add(this);
                }
            }
        } 
        // ==========================================================================================================================

        public void DestroyCombinedObjects()
        {
            AbortAndClearMeshCombineJobs(false);
            RestoreOriginalRenderersAndLODGroups(false);
            Methods.DestroyChildren(transform);

            var combinedGameObjects = data.combinedGameObjects;

            for (int i = 0; i < combinedGameObjects.Count; i++)
            {
                Methods.Destroy(combinedGameObjects[i]);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif

            combinedGameObjects.Clear();
            data.ClearAll();

            combinedActive = false;
            combined = false;
        }

        public void Reset()
        {
            DestroyCombinedObjects();

            uniqueLodObjects.Clear();
            uniqueFoundLodGroups.Clear();
            
            unreadableMeshes.Clear();
            foundCombineConditions.combineConditions.Clear();

            ResetOctree();

            hasFoundFirstObject = false;

            bounds.center = bounds.size = Vector3.zero;

            if (searchOptions.useSearchBox) searchOptions.GetSearchBoxBounds();

            InitAndResetLodParentsCount();
        }

        public void AbortAndClearMeshCombineJobs(bool executeAbortEvent = true)
        {
            foreach (var meshCombineJob in meshCombineJobs)
            {
                meshCombineJob.abort = true;
                meshCombineJob.meshCombiner.isCombining = false;
            }

            ClearMeshCombineJobs(executeAbortEvent); 
        }

        public void ClearMeshCombineJobs(bool executeAbortEvent = true)
        {
#if MCSCaves
            if (removeOverlappingTriangles)
            {
                CreateOverlapColliders.DestroyOverlapColliders(overlappingCollidersGO);
                if (overlappingNonCombineGO && disableOverlappingNonCombineGO) overlappingNonCombineGO.SetActive(false);
            }
#endif

            meshCombineJobs.Clear();
            totalMeshCombineJobs = 0;

            if (executeAbortEvent && onCombiningAbort != null) onCombiningAbort(this);
        }

        public void AddObjects(Transform rootT, List<Transform> transforms, bool useSearchOptions, bool checkForLODGroups = true)
        {
            List<LODGroup> lodGroups = new List<LODGroup>();

            if (checkForLODGroups)
            {
                for (int i = 0; i < transforms.Count; i++)
                {
                    LODGroup lodGroup = transforms[i].GetComponent<LODGroup>();
                    if (lodGroup != null) lodGroups.Add(lodGroup);
                }

                if (lodGroups.Count > 0) AddLodGroups(rootT, lodGroups.ToArray(), useSearchOptions);
            }

            AddTransforms(rootT, transforms.ToArray(), useSearchOptions);
        }

        public void AddObjectsAutomatically(bool useSearchConditions = true)
        {
            InitData();

            Reset();
            AddObjectsFromSearchParent(useSearchConditions);

            if (combineMode == CombineMode.DynamicObjects && data.foundLodObjects.Count > 0)
            {
                Debug.Log("(MeshCombineStudio) => Lod Groups don't work yet for dynamic objects (they only work on static objects), this feature will be added in the next update.");
                data.foundLodObjects.Clear();
                return;
            }

            AddFoundObjectsToOctree();
            if (octreeContainsObjects)
            {
                octree.SortObjects(this);
                CombineCondition.MakeFoundReport(foundCombineConditions);
                cellCount = ObjectOctree.MaxCell.maxCellCount;
            }

            if (Console.instance != null) LogOctreeInfo();
        }

        public void AddFoundObjectsToOctree()
        {
            var foundObjects = data.foundObjects;
            var foundLodObjects = data.foundLodObjects;

            if (foundObjects.Count > 0 || foundLodObjects.Count > 0) octreeContainsObjects = true;
            else
            {
                Debug.Log("(MeshCombineStudio) => No matching GameObjects with chosen search options are found for combining.");
                return;
            }

            CalcOctreeSize(bounds);
            
            ObjectOctree.MaxCell.maxCellCount = 0;

            for (int i = 0; i < foundObjects.Count; i++)
            {
                CachedGameObject foundObject = foundObjects[i];

                Vector3 position = (searchOptions.objectCenter == ObjectCenter.TransformPosition ? foundObject.t.position : foundObject.mr.bounds.center);
                octree.AddObject(position, this, foundObject, 0, 0);
            }

            for (int i = 0; i < foundLodObjects.Count; i++)
            {
                CachedLodGameObject cachedLodGO = foundLodObjects[i];
                octree.AddObject(cachedLodGO.center, this, cachedLodGO, cachedLodGO.lodCount, cachedLodGO.lodLevel);
            }
        } 
        // ==========================================================================================================================
        
        public void ResetOctree()
        {
            // Debug.Log("ResetOctree");
            octreeContainsObjects = false;

            if (octree == null) { octree = new ObjectOctree.Cell(); return; }

            BaseOctree.Cell[] cells = octree.cells;
            octree.Reset(ref cells);
        } 
        // ==========================================================================================================================

        public void CalcOctreeSize(Bounds bounds)
        {
            float size;
            int levels;

            Methods.SnapBoundsAndPreserveArea(ref bounds, cellSize, combineMode == CombineMode.StaticObjects ? cellOffset : Vector3.zero);
            
            if (combineMode == CombineMode.StaticObjects)
            {
                float areaSize = Mathf.Max(Mathw.GetMax(bounds.size), cellSize);
                levels = Mathf.CeilToInt(Mathf.Log(areaSize / cellSize, 2));
                size = (int)Mathf.Pow(2, levels) * cellSize;
            }
            else
            {
                size = Mathw.GetMax(bounds.size);
                levels = 0;
            }
            
            if (levels == 0 && octree is ObjectOctree.Cell) octree = new ObjectOctree.MaxCell();
            else if (levels > 0 && octree is ObjectOctree.MaxCell) octree = new ObjectOctree.Cell();

            octree.maxLevels = levels;
            octree.bounds = new Bounds(bounds.center, new Vector3(size, size, size));

            // Debug.Log("size " + size + " levels " + levels);
        } 
        // ==========================================================================================================================
        
        public void ApplyChanges()
        {
            validRebakeLighting = rebakeLighting && !validCopyBakedLighting && !Application.isPlaying && Application.isEditor;

            for (int i = 0; i < changedCells.Count; i++)
            {
                ObjectOctree.MaxCell maxCell = changedCells[i];
                maxCell.hasChanged = false;

                maxCell.ApplyChanges(this);
            }

            changedCells.Clear();
        }

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public void CombineAll(bool useSearchConditions = true)
        {
            if (instantiatePrefab == null)
            {
                Debug.LogError("(MeshCombineStudio) => The `Custom Combined GameObject` is null. Make sure it's assigned in the 'Use Custom Combine GameObject` setting");
                return;
            }

            if (!combineConditionSettings.sameMaterial && combineConditionSettings.material == null)
            {
                Debug.LogError("(MeshCombineStudio) => You need to assign an output material in 'Combine Conditions' => 'Change Materials'. Keep in mind with this setting you ignore the source materials and combine all meshes into 1 output material.");
                return;
            }

            if (onCombiningStart != null) onCombiningStart(this);

            if (removeBackFaceTriangles && backFaceTriangleMode == BackFaceTriangleMode.Transform)
            {
                if (backFaceT == null)
                {
                    Debug.LogError("(MeshCombineStudio) => You need to assign the BackFace Transform in 'Output Settings'.");
                    return;
                }

                backFaceDirection = backFaceT.forward;
            }

            InitMeshCombineJobManager();

            isCombining = true;

            stopwatch.Reset();
            stopwatch.Start();

#if MCSCaves
            RemoveOverlappingTris.triangles.Clear();
#endif

            addMeshCollidersList.Clear();
            unreadableMeshes.Clear();
            selectImportSettingsMeshes.Clear();

            AddObjectsAutomatically(useSearchConditions);
            if (!octreeContainsObjects) return;

            // SetOriginalCollidersActive(false);

#if MCSCaves
            if (removeOverlappingTriangles)
            {
                if (overlappingNonCombineGO) overlappingNonCombineGO.SetActive(true);
                if (CreateOverlapColliders.IsAnythingOnFreeLayers(voxelizeLayer, lodGroupLayer)) return;
                if (!CreateOverlapColliders.Create(transform, overlapLayerMask, lodGroupLayer, ref overlappingCollidersGO, removeSamePositionTriangles))
                {
                    Debug.LogError("(MeshCombineStudio) => You need to select at least 1 layer in the `Overlap LayerMask` setting.");
                    return;
                }
            }
#endif

            validRebakeLighting = rebakeLighting && !validCopyBakedLighting && !Application.isPlaying && Application.isEditor;
           
            newTotalVertices = newTotalTriangles = originalTotalVertices = originalTotalTriangles = originalDrawCalls = newDrawCalls = 0;
            originalTotalNormalChannels = originalTotalTangentChannels = originalTotalUvChannels = originalTotalUv2Channels = originalTotalUv3Channels = originalTotalUv4Channels = originalTotalColorChannels = 0;
            newTotalNormalChannels = newTotalTangentChannels = newTotalUvChannels = newTotalUv2Channels = newTotalUv3Channels = newTotalUv4Channels = newTotalColorChannels = 0;

            for (int i = 0; i < lodParentHolders.Length; i++)
            {
                LodParentHolder lodParentHolder = lodParentHolders[i];
                if (!lodParentHolder.found) continue;

                if (lodParentHolder.go == null && combineMode != CombineMode.DynamicObjects) lodParentHolder.Create(this, i);
                
                octree.CombineMeshes(this, i);
            }

            if (MeshCombineJobManager.instance.jobSettings.combineJobMode == MeshCombineJobManager.CombineJobMode.CombineAtOnce) MeshCombineJobManager.instance.ExecuteJobs();

#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(this);
#endif
        } 
        // ==========================================================================================================================
        
        void InitAndResetLodParentsCount()
        {
            for (int i = 0; i < lodParentHolders.Length; i++)
            {
                if (lodParentHolders[i].lods == null || lodParentHolders[i].lods.Length != i + 1) lodParentHolders[i].Init(i + 1);
                else lodParentHolders[i].Reset();
            }
        }

        public void AddObjectsFromSearchParent(bool useSearchConditions)
        {
            if (searchOptions.parentGOs == null || searchOptions.parentGOs.Length == 0)
            {
                Debug.Log("(MeshCombineStudio) => You need to assign at least one Parent GameObject to 'Search Parents' in which meshes will be searched");
                return;
            }

            GameObject[] parentGOs = searchOptions.parentGOs;

            for (int i = 0; i < parentGOs.Length; i++)
            {
                GameObject parentGO = parentGOs[i];
                
                if (parentGO == null) continue;

                Transform parentT = parentGO.transform;
                LODGroup[] lodGroups = parentGO.GetComponentsInChildren<LODGroup>(true);
                AddLodGroups(parentT, lodGroups);

                Transform[] transforms = parentGO.GetComponentsInChildren<Transform>(true);
                AddTransforms(parentT, transforms, useSearchConditions);
            }

            var foundObjects = data.foundObjects;
            var foundLodGroups = data.foundLodGroups;
            var foundColliders = data.foundColliders;
            var colliderLookup = data.colliderLookup;
            var lodGroupLookup = data.lodGroupLookup;

            if (addMeshColliders)
            {
                for (int i = 0; i < foundObjects.Count; i++)
                {
                    Collider[] colliders = foundObjects[i].go.GetComponentsInChildren<Collider>(false);
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        Collider collider = colliders[j];
                        if (colliderLookup.ContainsKey(collider)) continue;
                        foundColliders.Add(collider);
                        colliderLookup.Add(collider, foundObjects[i]);
                    }
                }

                for (int i = 0; i < foundLodGroups.Count; i++)
                {
                    LODGroup lodGroup = foundLodGroups[i];
                    Collider[] colliders = foundLodGroups[i].gameObject.GetComponentsInChildren<Collider>(false);

                    for (int j = 0; j < colliders.Length; j++)
                    {
                        Collider collider = colliders[j];
                        if (colliderLookup.ContainsKey(collider)) continue;
                        foundColliders.Add(collider);
                        colliderLookup.Add(collider, lodGroupLookup[lodGroup]);
                    }
                }
            }
        } 
        // ==========================================================================================================================

        void CheckForFoundObjectNotOnOverlapLayerMask(GameObject go)
        {
            if (!Methods.IsLayerInLayerMask(overlapLayerMask, go.layer))
            {
                Debug.LogError("(MeshCombineStudio) => " + go.name + " on layer " + LayerMask.LayerToName(go.layer) + " is not part of the Overlap LayerMask", go);
            }
        }

        void AddLodGroups(Transform searchParentT, LODGroup[] lodGroups, bool useSearchOptions = true)
        {
            List<CachedLodGameObject> cachedLodRenderers = new List<CachedLodGameObject>();

            CachedGameObject cachedGODummy = null;

            for (int i = 0; i < lodGroups.Length; i++)
            {
                LODGroup lodGroup = lodGroups[i];

                bool validLodGroup;
                
                if (searchOptions.lodGroupSearchMode == SearchOptions.LODGroupSearchMode.LodGroup) validLodGroup = (ValidObject(searchParentT, lodGroup.transform, ObjectType.LodGroup, useSearchOptions, ref cachedGODummy) == 1);
                else
                {
                    if (searchOptions.onlyActive && !lodGroup.gameObject.activeInHierarchy) continue;
                    validLodGroup = true;
                }

                LOD[] lods = lodGroup.GetLODs();
                int lodParentIndex = lods.Length - 1;

                if (lodParentIndex <= 0) continue;
                if (i == 0) lodGroupsSettings[lodParentIndex].CopyFromLodGroup(lodGroup, lods);
                // Debug.Log(lods.Length);

                Vector3 center = Vector3.zero;

                int rendererCount = 0;

                for (int j = 0; j < lods.Length; j++)
                {
                    LOD lod = lods[j];

                    for (int k = 0; k < lod.renderers.Length; k++)
                    {
                        Renderer r = lod.renderers[k];

                        if (!r) continue;

                        if (validLodGroup)
                        {
                            CachedGameObject cachedGO = null;

                            int result = ValidObject(searchParentT, r.transform, ObjectType.LodRenderer, useSearchOptions, ref cachedGO);

                            if (result == -1) continue;
                            else if (result == -2)
                            {
                                cachedLodRenderers.Clear();
                                goto breakLoop;
                            }

                            if (removeOverlappingTriangles && reportFoundObjectsNotOnOverlapLayerMask)
                            {
                                CheckForFoundObjectNotOnOverlapLayerMask(cachedGO.go);
                            }

                            cachedLodRenderers.Add(new CachedLodGameObject(cachedGO, lodParentIndex, j));
                            if (searchOptions.objectCenter == ObjectCenter.BoundsCenter)
                            {
                                center += cachedGO.mr.bounds.center;
                                rendererCount++;
                            }
                        }
                        uniqueLodObjects.Add(r.transform);
                    }
                }

                breakLoop:

                if (cachedLodRenderers.Count > 0)
                {
                    if (searchOptions.objectCenter == ObjectCenter.BoundsCenter) center /= rendererCount;
                    else center = lodGroup.transform.position;

                    var foundLodObjects = data.foundLodObjects;

                    for (int j = 0; j < cachedLodRenderers.Count; j++)
                    {
                        CachedLodGameObject cachedLodGO = cachedLodRenderers[j];
                        if (j == 0)
                        {
                            data.lodGroupLookup[lodGroup] = cachedLodGO;
                        }
                        cachedLodGO.center = center;
                        if (!hasFoundFirstObject) { bounds.center = cachedLodGO.mr.bounds.center; hasFoundFirstObject = true; }
                        bounds.Encapsulate(cachedLodGO.mr.bounds);
                        foundLodObjects.Add(cachedLodGO);
                        lodParentHolders[lodParentIndex].found = true;
                        lodParentHolders[lodParentIndex].lods[cachedLodGO.lodLevel]++;
                    }

                    uniqueFoundLodGroups.Add(lodGroup);
                    cachedLodRenderers.Clear();
                }
            }

            data.foundLodGroups = new List<LODGroup>(uniqueFoundLodGroups);
        }
        
        void AddTransforms(Transform searchParentT, Transform[] transforms, bool useSearchConditions = true)
        {
            int uniqueLodObjectsCount = uniqueLodObjects.Count;

            var foundObjects = data.foundObjects;

            for (int i = 0; i < transforms.Length; i++)
            {
                Transform t = transforms[i];
                
                if (uniqueLodObjectsCount > 0 && uniqueLodObjects.Contains(t)) continue;

                CachedGameObject cachedGO = null;

                if (ValidObject(searchParentT, t, ObjectType.Normal, useSearchConditions, ref cachedGO) == 1)
                {
                    if (removeOverlappingTriangles && reportFoundObjectsNotOnOverlapLayerMask)
                    {
                        CheckForFoundObjectNotOnOverlapLayerMask(cachedGO.go);
                    }

                    if (!hasFoundFirstObject) { bounds.center = cachedGO.mr.bounds.center; hasFoundFirstObject = true; }
                    bounds.Encapsulate(cachedGO.mr.bounds);
                    foundObjects.Add(cachedGO);
                    lodParentHolders[0].lods[0]++;
                }
            }
            
            if (foundObjects.Count > 0) lodParentHolders[0].found = true;
            // Debug.Log("Count " + count);
            // Debug.Log(foundObjects.Count);
        } 
        // ==========================================================================================================================

        int ValidObject(Transform searchParentT, Transform t, ObjectType objectType, bool useSearchOptions, ref CachedGameObject cachedGameObject)
        {
            if (t == null) return -1;

            GameObject go = t.gameObject;

            MeshRenderer mr = null;
            MeshFilter mf = null;
            Mesh mesh = null;

            if (objectType != ObjectType.LodGroup || searchOptions.lodGroupSearchMode == SearchOptions.LODGroupSearchMode.LodRenderers)
            {
                mr = t.GetComponent<MeshRenderer>();
                if (mr == null || (!mr.enabled && searchOptions.onlyActiveMeshRenderers)) return -1;

                mf = t.GetComponent<MeshFilter>();
                if (mf == null) return -1;

                mesh = mf.sharedMesh;
                if (mesh == null) return -1;
                if (mesh.vertexCount > 65534) return -2;
            }

            if (useSearchOptions)
            {
                if (searchOptions.onlyActive && !go.activeInHierarchy) return -1;

                if (objectType != ObjectType.LodRenderer || searchOptions.lodGroupSearchMode == SearchOptions.LODGroupSearchMode.LodRenderers)
                {
                    if (searchOptions.useLayerMask)
                    {
                        int layer = 1 << t.gameObject.layer;
                        if ((searchOptions.layerMask.value & layer) != layer) return -1;
                    }

#if UNITY_EDITOR
                    // Debug.Log(go.name + " " + (int)UnityEditor.GameObjectUtility.GetStaticEditorFlags(go) + " " + searchOptions.editorStatic);
                    if (searchOptions.onlyStatic && !UnityEditor.GameObjectUtility.AreStaticEditorFlagsSet(go, searchOptions.editorStatic)) return -1;
#else
                    if (searchOptions.onlyStatic && !go.isStatic) return -1;
#endif

                    if (searchOptions.useTag)
                    {
                        if (!t.CompareTag(searchOptions.tag)) return -1;
                    }

                    if (searchOptions.useComponentsFilter)
                    {
                        if (searchOptions.componentCondition == SearchOptions.ComponentCondition.And)
                        {
                            bool pass = true;
                            for (int j = 0; j < searchOptions.componentNameList.Count; j++)
                            {
                                if (t.GetComponent(searchOptions.componentNameList[j]) == null) { pass = false; break; }
                            }
                            if (!pass) return -1;
                        }
                        else if (searchOptions.componentCondition == SearchOptions.ComponentCondition.Or)
                        {
                            bool pass = false;
                            for (int j = 0; j < searchOptions.componentNameList.Count; j++)
                            {
                                if (t.GetComponent(searchOptions.componentNameList[j]) != null) { pass = true; break; }
                            }
                            if (!pass) return -1;
                        }
                        else
                        {
                            bool pass = true;
                            for (int j = 0; j < searchOptions.componentNameList.Count; j++)
                            {
                                if (t.GetComponent(searchOptions.componentNameList[j]) != null) { pass = false; break; }
                            }
                            if (!pass) return -1;
                        }
                    }

                    if (searchOptions.useNameContains)
                    {
                        bool found = false;
                        for (int k = 0; k < searchOptions.nameContainList.Count; k++)
                        {
                            if (Methods.Contains(t.name, searchOptions.nameContainList[k])) { found = true; break; }
                        }
                        if (!found) return -1;
                    }

                    if (searchOptions.useSearchBox)
                    {
                        if (searchOptions.objectCenter == ObjectCenter.BoundsCenter)
                        {
                            if (!searchOptions.searchBoxBounds.Contains(mr.bounds.center)) return -2;
                        }
                        else if (!searchOptions.searchBoxBounds.Contains(t.position)) return -2;
                    }
                }

                if (objectType != ObjectType.LodGroup)
                {
                    if (searchOptions.useVertexInputLimit && mesh.vertexCount > searchOptions.vertexInputLimit) return -2;

                    if (useVertexOutputLimit && mesh.vertexCount > vertexOutputLimit) return -2;


                    if (searchOptions.useMaxBoundsFactor && combineMode == CombineMode.StaticObjects)
                    {
                        if (Mathw.GetMax(mr.bounds.size) > cellSize * searchOptions.maxBoundsFactor) return -2;
                    }
                }
            }

            if ((objectType != ObjectType.LodGroup || searchOptions.lodGroupSearchMode == SearchOptions.LODGroupSearchMode.LodRenderers) && !mesh.isReadable)
            {
                if (unreadableMeshes.Add(mesh))
                {
                    Debug.LogError("(MeshCombineStudio) => Read/Write is disabled on the mesh on GameObject " + go.name + " and can't be combined. Click the 'Make Meshes Readable' in the MCS Inspector to make it automatically readable in the mesh import settings.");
                }
                return -1;
            }

            if (objectType != ObjectType.LodGroup)
            {
                cachedGameObject = new CachedGameObject(searchParentT, go, t, mr, mf, mesh);
            }

            return 1;
        }

        public void RestoreOriginalRenderersAndLODGroups(bool onDestroy)
        {
            if (activeOriginal) return;

            ExecuteHandleObjects(true, HandleComponent.Disable, HandleComponent.Disable, true, onDestroy);
        }

        public void SwapCombine()
        {
            if (!combined) { CombineAll(); }
            else
            {
                combinedActive = !combinedActive;

                ExecuteHandleObjects(!combinedActive, originalMeshRenderers, originalLODGroups);
            }
        }

        void SetOriginalCollidersActive(bool active, bool onDestroy)
        {
            if (data == null && !onDestroy) InitData();
            if (data == null) return;

            var foundColliders = data.foundColliders;

            for (int i = 0; i < foundColliders.Count; i++)
            {
                Collider collider = foundColliders[i];
                if (collider)
                {
                    CachedGameObject cachedGO;
                    data.colliderLookup.TryGetValue(collider, out cachedGO);
                    if (cachedGO == null || !cachedGO.excludeCombine) collider.enabled = active;
                    else Methods.ListRemoveAt(foundColliders, i--); 
                }
                else Methods.ListRemoveAt(foundColliders, i--);
            }
        }

        void ExecuteMeshFilter(bool active, CachedGameObject cachedGO)
        {
            if (active)
            {
                if (cachedGO.mfr) cachedGO.mfr.RevertMeshFilter(cachedGO.mf);
            }
            else
            {
                var mfr = cachedGO.go.AddComponent<MeshFilterRevert>();
                if (mfr.DestroyAndReferenceMeshFilter(cachedGO.mf))
                {
                    cachedGO.mfr = mfr;
                }
                else Methods.Destroy(mfr);
            }
        }

        public void ExecuteHandleObjects(bool active, HandleComponent handleOriginalObjects, HandleComponent handleOriginalLodGroups, bool includeColliders = true, bool onDestroy = false)
        {
            activeOriginal = active;
            Methods.SetChildrenActive(transform, !active);

            List<CachedGameObject> foundObjects;
            List<CachedLodGameObject> foundLodObjects;
            List<LODGroup> foundLodGroups;
            List<Collider> foundColliders;

            bool useRemoveOriginalMeshReference = !Application.isPlaying && (removeOriginalMeshReference || usedRemoveOriginalMeshRederences);

            if (!active)
            {
                usedRemoveOriginalMeshRederences = useRemoveOriginalMeshReference;
            }
            else usedRemoveOriginalMeshRederences = false;

            if (onDestroy)
            {
                foundObjects = this.foundObjects;
                foundLodObjects = this.foundLodObjects;
                foundLodGroups = this.foundLodGroups;
                foundColliders = this.foundColliders;
            }
            else
            {
                InitData();
                if (data == null) return;

                foundObjects = data.foundObjects;
                foundLodObjects = data.foundLodObjects;
                foundLodGroups = data.foundLodGroups;
                foundColliders = data.foundColliders;
            }

            if (handleOriginalObjects == HandleComponent.Disable)
            {
                if (includeColliders) SetOriginalCollidersActive(active, onDestroy);

                for (int i = 0; i < foundObjects.Count; i++)
                {
                    CachedGameObject cachedGO = foundObjects[i];

                    if (cachedGO.mr && !cachedGO.excludeCombine)
                    {
                        cachedGO.mr.enabled = (cachedGO.mrEnabled & active);
                        if (active) cachedGO.go.hideFlags = HideFlags.None;
                        else if (useOriginalObjectsHideFlags) cachedGO.go.hideFlags = orginalObjectsHideFlags;

                        if (useRemoveOriginalMeshReference) ExecuteMeshFilter(active, cachedGO);
                    }
                    else Methods.ListRemoveAt(foundObjects, i--);
                }
                for (int i = 0; i < foundLodObjects.Count; i++)
                {
                    CachedLodGameObject cachedLodGO = foundLodObjects[i];

                    if (cachedLodGO.mr && !cachedLodGO.excludeCombine)
                    {
                        cachedLodGO.mr.enabled = (cachedLodGO.mrEnabled & active);
                        if (useRemoveOriginalMeshReference) ExecuteMeshFilter(active, cachedLodGO);
                    }
                    else Methods.ListRemoveAt(foundLodObjects, i--);
                }
            }
            if (handleOriginalObjects == HandleComponent.Destroy)
            {
                for (int i = 0; i < foundColliders.Count; i++)
                {
                    Collider collider = foundColliders[i];
                    if (collider)
                    {
                        CachedGameObject cachedGO;
                        data.colliderLookup.TryGetValue(collider, out cachedGO);
                        if (cachedGO == null || !cachedGO.excludeCombine) Destroy(collider);
                        else Methods.ListRemoveAt(foundColliders, i--);
                    }
                    else Methods.ListRemoveAt(foundColliders, i--);
                }

                for (int i = 0; i < foundObjects.Count; i++)
                {
                    bool remove = false;
                    CachedGameObject cachedGO = foundObjects[i];

                    if (!cachedGO.excludeCombine)
                    {
                        if (cachedGO.mf) Destroy(cachedGO.mf);
                        else remove = true;

                        if (cachedGO.mr) Destroy(cachedGO.mr);
                        else remove = true;
                    }
                    else remove = true;

                    if (remove) Methods.ListRemoveAt(foundObjects, i--);
                }

                for (int i = 0; i < foundLodObjects.Count; i++)
                {
                    bool remove = false;
                    CachedGameObject cachedGO = foundLodObjects[i];

                    if (!cachedGO.excludeCombine)
                    {
                        if (cachedGO.mf) Destroy(cachedGO.mf);
                        else remove = true;

                        if (cachedGO.mr) Destroy(cachedGO.mr);
                        else remove = true;
                    }
                    else remove = true;

                    if (remove) Methods.ListRemoveAt(foundLodObjects, i--);
                }
            }
            
            for (int i = 0; i < foundLodGroups.Count; i++)
            {
                LODGroup lodGroup = foundLodGroups[i];
                if (lodGroup)
                {
                    CachedGameObject cachedGO;
                    data.lodGroupLookup.TryGetValue(lodGroup, out cachedGO);
                    if (cachedGO == null || !cachedGO.excludeCombine)
                    {
                        if (active) lodGroup.gameObject.hideFlags = HideFlags.None;
                        else if (useOriginalObjectsHideFlags) lodGroup.gameObject.hideFlags = orginalObjectsHideFlags;

                        if (handleOriginalLodGroups == HandleComponent.Disable) lodGroup.enabled = active; else Destroy(lodGroup);
                    }
                    else Methods.ListRemoveAt(foundLodGroups, i--);
                }
                else Methods.ListRemoveAt(foundLodGroups, i--);
            }
        }

        void DrawGizmosCube(Bounds bounds, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color = new Color(color.r, color.g, color.b, 0.5f);
            Gizmos.DrawCube(bounds.center, bounds.size);
            Gizmos.color = Color.white;
        }

        void OnDrawGizmosSelected()
        {
            if (addMeshColliders && addMeshCollidersInRange)
            {
                DrawGizmosCube(addMeshCollidersBounds, Color.green);
            }

            if (removeBackFaceTriangles)
            {
                if (backFaceTriangleMode == BackFaceTriangleMode.Box)
                {
                    DrawGizmosCube(backFaceBounds, Color.blue);
                }
            }

            if (!drawGizmos) return;
            
            if (octree != null && octreeContainsObjects)
            {
                octree.Draw(this, true, !searchOptions.useSearchBox);
            }
            
            if (searchOptions.useSearchBox)
            {
                searchOptions.GetSearchBoxBounds();

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(searchOptions.searchBoxBounds.center, searchOptions.searchBoxBounds.size);
                Gizmos.color = Color.white;
            }
        }
        // ==========================================================================================================================

        void LogOctreeInfo()
        {
            Console.Log("Cells " + ObjectOctree.MaxCell.maxCellCount + " -> Found Objects: ");
            
            LodParentHolder[] lodParentsCount = lodParentHolders;

            if (lodParentsCount == null || lodParentsCount.Length == 0) return;

            for (int i = 0; i < lodParentsCount.Length; i++)
            {
                LodParentHolder lodParentCount = lodParentsCount[i];
                if (!lodParentCount.found) continue;

                string text = "";
                text = "LOD Group " + (i + 1) + " |";

                int[] lods = lodParentCount.lods;

                for (int j = 0; j < lods.Length; j++)
                {
                    text += " " + lods[j].ToString() + " |";
                }
                Console.Log(text);
            }
        }
        
        [Serializable]
        public class LODGroupSettings
        {
            public bool animateCrossFading;
            public LODFadeMode fadeMode;
            public LODSettings[] lodSettings;
            
            public LODGroupSettings(int lodParentIndex)
            {
                int lodCount = lodParentIndex + 1;
                lodSettings = new LODSettings[lodCount];
                float percentage = 1.0f / lodCount;

                for (int i = 0; i < lodSettings.Length; i++)
                {
                    lodSettings[i] = new LODSettings(1 - (percentage * (i + 1)));
                }
            }

            public void CopyFromLodGroup(LODGroup lodGroup, LOD[] lods)
            {
                animateCrossFading = lodGroup.animateCrossFading;
                fadeMode = lodGroup.fadeMode;

                for (int i = 0; i < lods.Length; i++)
                {
                    lodSettings[i].fadeTransitionWidth = lods[i].fadeTransitionWidth;
                }
            }

            public void CopyToLodGroup(LODGroup lodGroup, LOD[] lods)
            {
                lodGroup.animateCrossFading = animateCrossFading;
                lodGroup.fadeMode = fadeMode;

                for (int i = 0; i < lods.Length; i++)
                {
                    lods[i].fadeTransitionWidth = lodSettings[i].fadeTransitionWidth;
                }
            }
        }

        [Serializable]
        public class LODSettings
        {
            public float screenRelativeTransitionHeight;
            public float fadeTransitionWidth;

            public LODSettings(float screenRelativeTransitionHeight)
            {
                this.screenRelativeTransitionHeight = screenRelativeTransitionHeight;
            }
        }

        [Serializable]
        public class LodParentHolder
        {
            public GameObject go;
            public Transform t;

            public bool found;
            public int[] lods;

            public void Init(int lodCount)
            {
                lods = new int[lodCount];
            }
            
            public void Create(MeshCombiner meshCombiner, int lodParentIndex)
            {
                if (meshCombiner.data.foundLodGroups.Count == 0)
                {
                    go = new GameObject(meshCombiner.combineMode == CombineMode.StaticObjects ? "Cells" : "Combine Parent");
                }
                else
                {
                    go = new GameObject("LODGroup " + (lodParentIndex + 1));

                    var lodGroupSetup = go.AddComponent<LODGroupSetup>();
                    lodGroupSetup.Init(meshCombiner, lodParentIndex);
                }

                t = go.transform;

                Transform parentT = t.transform;
                parentT.parent = meshCombiner.transform;
            }

            public void Reset()
            {
                found = false;
                Array.Clear(lods, 0, lods.Length);
            }
        }
    }

    public struct MeshColliderAdd
    {
        public GameObject go;
        public Mesh mesh;

        public MeshColliderAdd(GameObject go, Mesh mesh)
        {
            this.go = go;
            this.mesh = mesh;
        }
    }
}