using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    public abstract class GPUInstancerTerrainManager : GPUInstancerManager
    {
        [SerializeField]
        private Terrain _terrain;
        [SerializeField]
        public List<Terrain> additionalTerrains;
        [NonSerialized]
        protected List<Terrain> _terrains;
        public Terrain terrain
        {
            get
            {
                if (Application.isPlaying && _terrains != null && _terrains.Count > 0)
                    return _terrains[0];
                else
                    return _terrain;
            }
        }
        public GPUInstancerTerrainSettings terrainSettings;
        protected bool replacingInstances;
        protected bool initalizingInstances;
        protected bool _requiresTerrainUpdate;

        private Queue<Terrain> _terrainsToAdd;
        private Queue<Terrain> _terrainsToRemove;

        public override void Awake()
        {
            base.Awake();

            _terrains = new List<Terrain>();
            if (_terrain != null)
                _terrains.Add(_terrain);
            if (additionalTerrains != null)
            {
                foreach (Terrain additionalTerrain in additionalTerrains)
                {
                    if (additionalTerrain != null && !_terrains.Contains(additionalTerrain))
                        _terrains.Add(additionalTerrain);
                }
            }

            if (Application.isPlaying && useFloatingOriginHandler && terrain != null)
            {
                floatingOriginTransform = terrain.transform;
            }
        }

        public override void Update()
        {
            base.Update();

            if (_terrainsToAdd != null)
            {
                while(_terrainsToAdd.Count > 0)
                {
                    Terrain terrain = _terrainsToAdd.Dequeue();
                    if (!_terrains.Contains(terrain))
                    {
                        _terrains.Add(terrain);
                        UpdateTerrains();
                    }
                }
            }
            if (_terrainsToRemove != null)
            {
                while (_terrainsToRemove.Count > 0)
                {
                    Terrain terrain = _terrainsToRemove.Dequeue();
                    if (_terrains.Contains(terrain))
                    {
                        _terrains.Remove(terrain);
                        UpdateTerrains();
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
#if UNITY_EDITOR
            if (!Application.isPlaying && terrain != null && terrain.gameObject != null && terrain.GetComponent<GPUInstancerTerrainProxy>() != null && !terrain.GetComponent<GPUInstancerTerrainProxy>().beingDestroyed)
            {
                Undo.RecordObject(terrain.gameObject, "Remove GPUInstancerTerrainProxy");
                DestroyImmediate(terrain.GetComponent<GPUInstancerTerrainProxy>());
            }
#endif
        }

        public override void Reset()
        {
            base.Reset();

            if(terrain == null && gameObject.GetComponent<Terrain>() != null)
            {
                SetupManagerWithTerrain(gameObject.GetComponent<Terrain>());
            }

#if UNITY_EDITOR
            showRegisteredPrefabsBox = false;
#endif
        }

        // Remove comment-out status to see partitioning bound gizmos:
        //public void OnDrawGizmos()
        //{
        //    if (spData != null && spData.activeCellList != null)
        //    {
        //        Color oldColor = Gizmos.color;
        //        Gizmos.color = Color.blue;
        //        foreach (GPUInstancerCell cell in spData.activeCellList)
        //        {
        //            if (cell != null)
        //                Gizmos.DrawWireCube(cell.cellInnerBounds.center, cell.cellInnerBounds.size);
        //        }
        //        Gizmos.color = oldColor;
        //    }
        //}

#if UNITY_EDITOR
        public override void CheckPrototypeChanges()
        {
            if (terrain != null && terrain.terrainData != null && terrainSettings != null)
            {
                string guid = GPUInstancerUtility.GetAssetGUID(terrain.terrainData);
                if (!string.IsNullOrEmpty(guid) && guid != terrainSettings.terrainDataGUID)
                    terrainSettings.terrainDataGUID = guid;
            }

            base.CheckPrototypeChanges();
        }
#endif

        public virtual void SetupManagerWithTerrain(Terrain terrain)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.RecordObject(this, "Changed GPUInstancer Terrain Data for " + gameObject);
                if (_terrain != null && _terrain.GetComponent<GPUInstancerTerrainProxy>() != null)
                {
                    Undo.RecordObject(_terrain.gameObject, "Removed GPUInstancerTerrainProxy component");
                    DestroyImmediate(_terrain.GetComponent<GPUInstancerTerrainProxy>());
                }
            }
#endif

            _terrain = terrain;

            if (terrain != null)
            {
                if (terrainSettings != null)
                {
                    string guid = GPUInstancerUtility.GetAssetGUID(terrain.terrainData);
                    if (!string.IsNullOrEmpty(guid) && guid == terrainSettings.terrainDataGUID)
                        return;
                    else
                    {
                        prototypeList.Clear();
                        //RemoveTerrainSettings(terrainSettings);
                        terrainSettings = null;
                    }
                }
                terrainSettings = GenerateTerrainSettings(terrain, gameObject);
                GeneratePrototypes(false);
                if (!Application.isPlaying)
                    AddProxyToTerrain();
            }
            else
            {
                prototypeList.Clear();
                //RemoveTerrainSettings(terrainSettings);
                terrainSettings = null;
            }
        }

        public GPUInstancerTerrainProxy AddProxyToTerrain()
        {
#if UNITY_EDITOR
            if (terrain != null && gameObject != terrain.gameObject)
            {
                GPUInstancerTerrainProxy terrainProxy = terrain.GetComponent<GPUInstancerTerrainProxy>();
                if (terrainProxy == null)
                {
                    Undo.RecordObject(terrain.gameObject, "Added GPUInstancerTerrainProxy component");
                    terrainProxy = terrain.gameObject.AddComponent<GPUInstancerTerrainProxy>();
                }
                if (this is GPUInstancerDetailManager && terrainProxy.detailManager != this)
                    terrainProxy.detailManager = (GPUInstancerDetailManager)this;
                else if (this is GPUInstancerTreeManager && terrainProxy.treeManager != this)
                    terrainProxy.treeManager = (GPUInstancerTreeManager)this;

#if UNITY_2018_3_OR_NEWER
                PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(terrainProxy.gameObject);
                if (prefabType == PrefabAssetType.NotAPrefab)
                {
#endif
                    while (UnityEditorInternal.ComponentUtility.MoveComponentUp(terrainProxy)) ;
#if UNITY_2018_3_OR_NEWER
                }
#endif
                return terrainProxy;
            }
#endif
            return null;
        }

        private GPUInstancerTerrainSettings GenerateTerrainSettings(Terrain terrain, GameObject gameObject)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string[] guids = AssetDatabase.FindAssets("t:GPUInstancerTerrainSettings");
                string guid = GPUInstancerUtility.GetAssetGUID(terrain.terrainData);
                for (int i = 0; i < guids.Length; i++)
                {
                    GPUInstancerTerrainSettings ts = AssetDatabase.LoadAssetAtPath<GPUInstancerTerrainSettings>(AssetDatabase.GUIDToAssetPath(guids[i]));
                    if (ts != null && !string.IsNullOrEmpty(guid) && ts.terrainDataGUID == guid)
                    {
                        prototypeList.Clear();
                        if (this is GPUInstancerDetailManager)
                        {
                            List<GPUInstancerPrototype> detailPrototypeList = new List<GPUInstancerPrototype>();
                            GPUInstancerUtility.SetPrototypeListFromAssets(ts, detailPrototypeList, typeof(GPUInstancerDetailPrototype));
                            for (int p = 0; p < detailPrototypeList.Count; p++)
                            {
                                foreach (GPUInstancerDetailPrototype detailPrototype in detailPrototypeList)
                                {
                                    if (detailPrototype.prototypeIndex == p)
                                    {
                                        prototypeList.Add(detailPrototype);
                                        break;
                                    }
                                }
                            }
                        }
                        if (this is GPUInstancerTreeManager)
                        {
                            List<GPUInstancerPrototype> treePrototypeList = new List<GPUInstancerPrototype>();
                            GPUInstancerUtility.SetPrototypeListFromAssets(ts, treePrototypeList, typeof(GPUInstancerTreePrototype));
                            for (int p = 0; p < treePrototypeList.Count; p++)
                            {
                                foreach (GPUInstancerTreePrototype treePrototype in treePrototypeList)
                                {
                                    if (treePrototype.prototypeIndex == p)
                                    {
                                        prototypeList.Add(treePrototype);
                                        break;
                                    }
                                }
                            }
                        }
                        return ts;
                    }
                }
            }
#endif

            GPUInstancerTerrainSettings terrainSettings = ScriptableObject.CreateInstance<GPUInstancerTerrainSettings>();
            terrainSettings.name = (string.IsNullOrEmpty(terrain.terrainData.name) ? terrain.gameObject.name : terrain.terrainData.name) + "_" + terrain.terrainData.GetInstanceID();
            terrainSettings.terrainDataGUID = GPUInstancerUtility.GetAssetGUID(terrain.terrainData);
            terrainSettings.maxDetailDistance = terrain.detailObjectDistance;
            terrainSettings.maxTreeDistance = terrain.treeDistance;
            terrainSettings.detailDensity = terrain.detailObjectDensity;
            terrainSettings.healthyDryNoiseTexture = Resources.Load<Texture2D>(GPUInstancerConstants.NOISE_TEXTURES_PATH + GPUInstancerConstants.DEFAULT_HEALTHY_DRY_NOISE);
            terrainSettings.windWaveNormalTexture = Resources.Load<Texture2D>(GPUInstancerConstants.NOISE_TEXTURES_PATH + GPUInstancerConstants.DEFAULT_WIND_WAVE_NOISE);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string assetPath = GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH + terrainSettings.name + ".asset";

                // If there is already a file with the same name, change file name
                int counter = 2;
                while (System.IO.File.Exists(assetPath))
                {
                    assetPath = GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH + terrainSettings.name + "_" + counter + ".asset";
                    counter++;
                }

                if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH))
                {
                    System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH);
                }

                AssetDatabase.CreateAsset(terrainSettings, assetPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
            return terrainSettings;
        }

        private static void RemoveTerrainSettings(GPUInstancerTerrainSettings terrainSettings)
        {
#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(terrainSettings));
#endif
        }

        public virtual void UpdateTerrains() 
        {
            _requiresTerrainUpdate = true;
        }  

        public virtual void AddTerrain(Terrain newTerrain)
        {
            if (_terrainsToAdd == null)
                _terrainsToAdd = new Queue<Terrain>();
            _terrainsToAdd.Enqueue(newTerrain);
        }

        public virtual void RemoveTerrain(Terrain terrainToRemove)
        {
            if (_terrainsToRemove == null)
                _terrainsToRemove = new Queue<Terrain>();
            _terrainsToRemove.Enqueue(terrainToRemove);
        }

        public List<Terrain> GetTerrains()
        {
            return _terrains;
        }
    }
}