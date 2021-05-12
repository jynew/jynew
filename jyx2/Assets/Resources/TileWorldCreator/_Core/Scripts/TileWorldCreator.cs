/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 * 
 * Create awesome tile worlds in seconds.
 *
 * Using:
 * Drag this script on an empty gameobject and start creating.
 * OR
 * Drag the prefab called TileWorldCreator to an empty scene.
 *
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Reflection;
using System.Linq;
using TileWorld.Events;

//Disable Log warnings for assigned but not used variables 
#pragma warning disable 0219

namespace TileWorld
{

    [System.Serializable]
    public class TileWorldCreator : MonoBehaviour
    {
        //configuration file (scriptable object)
        public TileWorldConfiguration configuration;

        //mask interface
        public IMaskBehaviour[] iMasks;

        //algorithm interface
        public IAlgorithms[] iAlgorithms;

        public bool firstTimeBuild = true;

        //grid position
        public Vector3 origin = new Vector3(0, 0, 0);

        //build settings
        //bool merge = false;     

        float progress = 0;
        float mergeProgress = 0;

        int lastMapIndex = 0;
 

        public GameObject worldContainer;

        public List<GameObject> clusterContainers = new List<GameObject>();
        public List<GameObject> layerContainers = new List<GameObject>();

        //orientation of blocks
        public static string orientation = "";

#if UNITY_5_4_OR_NEWER
        Random.State currentSeed;
#else
        int currentSeed;
#endif

        //GUI / Editor Properties 
        bool[,] cloneMap;
        public bool mergeReady = false;
        public Vector3 mouseWorldPosition = new Vector3(0, 0, 0);
        public bool mouseOverGrid = false;
        public bool showGrid;

        public string[] maskNames;
        public string[] algorithmNames;

        //-------------------------------------------------------------------------------------------------------------

        void Awake()
        {
            progress = 0;
            mergeProgress = 0;

            //get all available masks
            TileWorldMaskLookup.GetMasks(out iMasks, out maskNames);

            //get all available algorithms
            TileWorldAlgorithmLookup.GetAlgorithms(out iAlgorithms, out algorithmNames);
        }

        void Start()
        {
            TileWorldEvents.CallBuildProgress(0);
            TileWorldEvents.CallMergeProgress(0);
        }

        //--------------------------
        //MAP GENERATION
        //--------------------------
        #region mapgeneration
        /// <summary>
        /// Generate and build a new map from scratch. 
        /// Set merge true if the map should be merged
        /// </summary>
        /// <param name="_merge"></param>
        public void GenerateAndBuild(bool _merge)
        {
            if (configuration.global.width == 0 || configuration.global.height == 0)
                return;

            // Reset progress
            TileWorldEvents.CallBuildProgress(0);
            TileWorldEvents.CallMergeProgress(0);

            //first generate Maps
            GenerateMaps();

            //optimization is set to false because
            //we do already an optimization in the GenerateMaps method
            BuildMapComplete(true, _merge, false);

        }

        
        /// <summary>
        /// Generate a new map from scratch without building it.
        /// use BuildMapComplete(); to build the generated map
        /// </summary>
        public void GenerateMaps()
        {
            if (configuration.global.randomSeed > -1)
            {
#if UNITY_5_4_OR_NEWER
                Random.InitState(configuration.global.randomSeed);
#else
                Random.seed = configuration.global.randomSeed;
#endif
            }
            else
            {
#if UNITY_5_4_OR_NEWER
                Random.InitState(System.Environment.TickCount);
#else
                Random.seed = System.Environment.TickCount;
#endif
            }

#if UNITY_5_4_OR_NEWER
            currentSeed = Random.state;
#else
            currentSeed = Random.seed;
#endif


            //if firstTimeBuild is true, TWC will build the whole map from ground up
            //after that we set it to false so only changes will be rebuilded (partial build)
            firstTimeBuild = true;     

            mergeReady = false;


            //set map index to zero
            lastMapIndex = configuration.ui.mapIndex;
            configuration.ui.mapIndex = 0;

            //get current mask settings and store them
            List<bool> _useMask = new List<bool>();
            List<int> _selectedMask = new List<int>();
            for (int ma = 0; ma < configuration.worldMap.Count; ma ++)
            {
                _useMask.Add(configuration.worldMap[ma].useMask);
                _selectedMask.Add(configuration.worldMap[ma].selectedMask);
            }

            //create new configuration.worldMap and initialize it
            configuration.worldMap = new List<TileWorldConfiguration.WorldMap>();

            if (_useMask.Count > 0)
            {
                configuration.worldMap.Add(new TileWorldConfiguration.WorldMap(configuration.global.width, configuration.global.height, false, _useMask[0], _selectedMask[0]));
            }
            else
            {
                configuration.worldMap.Add(new TileWorldConfiguration.WorldMap(configuration.global.width, configuration.global.height, false, false, 0));
            }


            // Generate map based on selected Algorithm
            configuration.worldMap[0].cellMap = new bool[configuration.global.width, configuration.global.height];
            configuration.worldMap[0].cellMap = iAlgorithms[configuration.global.selectedAlgorithm].Generate(configuration, out configuration.global.startPosition, out configuration.global.endPosition);



            //create new world map according
            //to the layer count
            for (int m = 1; m < configuration.global.layerCount; m++)
            {
                if (m > _useMask.Count - 1)
                {
                    AddNewLayer(false, _useMask[0], _selectedMask[0], false, 0);
                }
                else 
                {
                    AddNewLayer(false, _useMask[m], _selectedMask[m], false, 0);
                }
            }


            //create new containers
            NewContainer();


            //build cells for each layer
            //use layer inset if we have more than one layer
            for (int ms = 0; ms < configuration.global.layerCount; ms++)
            {
                //check if block is an inner terrain block
                for (int y = 0; y < configuration.worldMap[0].cellMap.GetLength(1); y++)
                {
                    for (int x = 0; x < configuration.worldMap[0].cellMap.GetLength(0); x++)
                    {
                        if (configuration.global.layerInset > 0)
                        {
                            int _itc = TileWorldNeighbourCounter.CountInnerTerrainBlocks(configuration.worldMap[ms].cellMap, x, y, configuration.global.layerInset, configuration.global.invert);
                          
                            if (_itc >= (((configuration.global.layerInset * 2) + 1) * ((configuration.global.layerInset * 2) + 1)) - 1)
                            {
                                if (!configuration.global.invert)
                                {
                                    if (ms + 1 < configuration.worldMap.Count)
                                    {
                                        configuration.worldMap[ms + 1].cellMap[x, y] = false;
                                    }
                                }
                                else
                                {
                                    if (ms + 1 < configuration.worldMap.Count)
                                    {
                                        //do not create cells on borders when invert is set to true
                                        if (x < 1 || x >= configuration.global.width - 1 || y >= configuration.global.height - 1 || y < 1)
                                        {
                                            configuration.worldMap[ms + 1].cellMap[x, y] = false;
                                        }
                                        else
                                        {
                                            configuration.worldMap[ms + 1].cellMap[x, y] = true;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            //if layer inset is set to 0 copy the layer cells from the lower layer to the higher layer
                            if (ms + 1 < configuration.worldMap.Count)
                            {
                                configuration.worldMap[ms + 1].cellMap[x, y] = configuration.worldMap[ms].cellMap[x, y];
                            }
                        }
                    }
                }

               

            }

          

            // Optimization pass
            for (int a = 0; a < configuration.worldMap.Count; a++)
            {
                for (int o = 0; o < configuration.global.optimizeSteps; o++)
                {
                    OptimizePass(a);
                }
            }


            //Update single dim array for serialization
            //this is called as soon as bool[,] map changes to update the single array map
            //for serialization
            UpdateMap();

            configuration.ui.mapIndex = lastMapIndex;
        }
        //-END MAIN LOOP

        // Calculates the cluster size automatically by
        // counting all vertecies from each tile and dividing it through the max Unity vertex count.
        // The square root of the result is the new cluster size.
        void CalculateClusterSize()
        {
            // Count vertecies
            var _presetVertexCount = 0;
            var _lastPresetVertexCount = 0;
            for (int i = 0; i < configuration.presets.Count; i++)
            {
                _presetVertexCount = 0;

                if (configuration.presets[i].tiles[0].tileI != null)
                {
                    _presetVertexCount += configuration.presets[i].tiles[0].tileI.GetComponent<MeshFilter>().sharedMesh.vertexCount;
                }

                if (configuration.presets[i].tiles[0].tileC != null)
                {
                    _presetVertexCount += configuration.presets[i].tiles[0].tileC.GetComponent<MeshFilter>().sharedMesh.vertexCount;
                }

                if (configuration.presets[i].tiles[0].tileCi != null)
                {
                    _presetVertexCount += configuration.presets[i].tiles[0].tileCi.GetComponent<MeshFilter>().sharedMesh.vertexCount;
                }

                if (configuration.presets[i].tiles[0].tileF != null)
                { 
                    _presetVertexCount += configuration.presets[i].tiles[0].tileF.GetComponent<MeshFilter>().sharedMesh.vertexCount;
                }

                if (configuration.presets[i].tiles[0].tileB != null)
                {
                    _presetVertexCount += configuration.presets[i].tiles[0].tileB.GetComponent<MeshFilter>().sharedMesh.vertexCount;
                }

                if (_presetVertexCount > _lastPresetVertexCount)
                {
                    _lastPresetVertexCount = _presetVertexCount;
                }
            }

            if (_lastPresetVertexCount != 0)
            {
                var _sqrRoot = Mathf.Sqrt((64000 / _lastPresetVertexCount));
                // set the new calculated cluster size
                configuration.global.clusterSize = Mathf.CeilToInt(_sqrRoot);
            }
        }


        //BUILD CURRENT MAP
        //-------------------
        /// <summary>
        /// Build the generated map.
        /// </summary>
        /// <param name="_coroutine"></param>
        /// <param name="_merge"></param>
        /// <param name="_optimize"></param>
        public void BuildMapComplete(bool _coroutine, bool _merge, bool _optimize)
        {

            //if (configuration.worldMap[0].tileObjects[0, 0] == null)
            //{
            //    LoadBack();
            //}

            // Set the cluster size if automatic is selected
            if (configuration.global.automaticClustersize)
            {
                CalculateClusterSize();
            }

            //first step optimize map before building it
            if (_optimize)
            {
                for (int o = 0; o < configuration.global.optimizeSteps; o++)
                {
                    OptimizePass(configuration.ui.mapIndex);
                }

            }

            if (_coroutine)
            {
                StartCoroutine(BuildMapIE(_merge, true));
            }
            else
            {
                //IEnumerator _e = BuildMapIE(_merge, true);
                //while (_e.MoveNext()) ;
                BuildMap(true);
            }

          
        }


        //build map partial
        /// <summary>
        /// Only rebuild a part of the map from the x and y position
        /// </summary>
        /// <param name="_coroutine"></param>
        /// <param name="_optimize"></param>
        /// <param name="_xPos"></param>
        /// <param name="_yPos"></param>
        public void BuildMapPartial(bool _coroutine, bool _optimize) //, int _xPos, int _yPos)
        {
            ////first step optimize map before building it
            if (_optimize)
            {
                for (int y = 0; y < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(1); y++)
                {
                    for (int x = 0; x < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(0); x++)
                    {
                        if (configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] != configuration.worldMap[configuration.ui.mapIndex].oldCellMap[x, y])
                        {
                            for (int i = 0; i < configuration.global.optimizeSteps; i++)
                            {
                                //optimize partial needs a x and y pos to know where on the map 
                                //it should optimize
                                OptimizePassPartial(configuration.ui.mapIndex, x, y);
                            }
                        }
                    }
                }
            }

            PartialMapChecker(_coroutine);
        }



        //BuildMap Coroutine Method
        IEnumerator BuildMapIE(bool _merge, bool _allLayers)
        {
            //merge = _merge;
            int _currentMapIndex = configuration.ui.mapIndex;
            //mapIndex = 0;

            progress = 0;

            worldContainer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            if (_allLayers)
            {
                //build world for each layer
                for (int i = 0; i < configuration.global.layerCount; i++)
                {
                    yield return StartCoroutine(BuildWorldIE(configuration.worldMap[i].cellMap, i, i, true));

                    //assign the oldmap
                    configuration.worldMap[i].oldCellMap = new bool[configuration.global.width, configuration.global.height];
                    for (int y = 0; y < configuration.global.height; y++)
                    {
                        for (int x = 0; x < configuration.global.width; x++)
                        {
                            configuration.worldMap[i].oldCellMap[x, y] = configuration.worldMap[i].cellMap[x, y];
                        }
                    }
                }

                // Call event build complete
                if (Application.isPlaying)
                {
                    TileWorldEvents.CallOnBuildComplete();
                }

                if (_merge)
                {
                    yield return StartCoroutine(MergeWorldIE());
                }
            }
                //only build current selected layer
            else
            {
                yield return StartCoroutine(BuildWorldIE(configuration.worldMap[configuration.ui.mapIndex].cellMap, configuration.ui.mapIndex, configuration.ui.mapIndex, true));
                
                //assign the oldmap
                configuration.worldMap[configuration.ui.mapIndex].oldCellMap = new bool[configuration.global.width, configuration.global.height];
                for (int y = 0; y < configuration.global.height; y++)
                {
                    for (int x = 0; x < configuration.global.width; x++)
                    {
                        configuration.worldMap[configuration.ui.mapIndex].oldCellMap[x, y] = configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y];
                    }
                }

                // Call event build complete
                if (Application.isPlaying)
                {
                    TileWorldEvents.CallOnBuildComplete();
                }

                if (_merge)
                {
                    yield return StartCoroutine(MergeWorldIE());
                }
            }

            //assign all clusters to worldContainer
            for (int c = 0; c < layerContainers.Count; c++)
            {
                layerContainers[c].transform.parent = worldContainer.transform;
            }

            // Rotate container according to map orientation
            if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xy)
            {
                worldContainer.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            }

            configuration.ui.mapIndex = _currentMapIndex;

            firstTimeBuild = false;

        }


        //default BuildMap method
        void BuildMap(bool _allLayers)//public void BuildMapEditor()
        {
          
            mergeReady = true;

            int _currentMapIndex = configuration.ui.mapIndex;
            //mapIndex = 0;

            if (firstTimeBuild)
            {
                NewContainer();
            }

            worldContainer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            if (_allLayers)
            {
                //build world for each layer
                for (int i = 0; i < configuration.global.layerCount; i++)
                {
                    BuildWorld(configuration.worldMap[i].cellMap, i, i, true);

                    //assign the oldmap
                    configuration.worldMap[i].oldCellMap = new bool[configuration.global.width, configuration.global.height];

                    if (configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(0) <= 0)
                    {
                        LoadBack();
                    }

                    for (int y = 0; y < configuration.worldMap[i].cellMap.GetLength(1); y++)
                    {
                        for (int x = 0; x < configuration.worldMap[i].cellMap.GetLength(0); x++)
                        {
                            configuration.worldMap[i].oldCellMap[x, y] = configuration.worldMap[i].cellMap[x, y];
                        }
                    }

                }
            }
            //only build current selected layer
            else
            {

                BuildWorld(configuration.worldMap[configuration.ui.mapIndex].cellMap, configuration.ui.mapIndex, configuration.ui.mapIndex, true);

                //assign the oldmap
                configuration.worldMap[configuration.ui.mapIndex].oldCellMap = new bool[configuration.global.width, configuration.global.height];


                if (configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(0) <= 0)
                {
                    LoadBack();
                }

                for (int y = 0; y < configuration.global.height; y++)
                {
                    for (int x = 0; x < configuration.global.width; x++)
                    {                    
                        configuration.worldMap[configuration.ui.mapIndex].oldCellMap[x, y] = configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y];
                    }
                }
                
            }



            //assign all clusters to worldContainer
            for (int c = 0; c < layerContainers.Count; c++)
            {
                layerContainers[c].transform.parent = worldContainer.transform;
            }

            // Rotate container according to map orientation
            if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xy)
            {
                worldContainer.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            }

            configuration.ui.mapIndex = _currentMapIndex;

            firstTimeBuild = false;
        }


        //-------------------------


        //Update map single array for serialization
        //-----------------------------------------
        public void UpdateMap()
        {
            if (configuration == null)
                return;

            if (configuration.worldMap.Count < 1)
                return;

            for (int l = 0; l < configuration.global.layerCount; l++)
            {
                int _index = 0;

                for (int y = 0; y < configuration.global.height; y++)
                {
                    for (int x = 0; x < configuration.global.width; x++)
                    {
                        if (_index < configuration.worldMap[l].cellMapSingle.Length)
                        {
                            if (x < configuration.worldMap[l].cellMap.GetLength(0) && y < configuration.worldMap[l].cellMap.GetLength(1))
                            {
                                configuration.worldMap[l].cellMapSingle[_index] = configuration.worldMap[l].cellMap[x, y];

                                if (configuration.worldMap[l].maskMap.GetLength(0) > 0)
                                {
                                    configuration.worldMap[l].maskMapSingle[_index] = configuration.worldMap[l].maskMap[x, y];
                                }

                           
                                configuration.worldMap[l].tileObjectsSingle[_index] = configuration.worldMap[l].tileObjects[x, y];
                                configuration.worldMap[l].tileTypesSingle[_index] = configuration.worldMap[l].tileTypes[x, y];

                                _index++;
                            }
                        }
                    }
                }
            }
        }


        //Load back multidimensional array from single dim array
        public void LoadBack()
        {

            for (int l = 0; l < configuration.global.layerCount; l++)
            {
                configuration.worldMap[l].cellMap = new bool[configuration.global.width, configuration.global.height];
                configuration.worldMap[l].tileObjects = new GameObject[configuration.global.width, configuration.global.height];
                configuration.worldMap[l].tileTypes = new TileWorldConfiguration.TileInformation.TileTypes[configuration.global.width, configuration.global.height];
                configuration.worldMap[l].maskMap = new bool[configuration.global.width, configuration.global.height];


                int _index = 0;

                for (int y = 0; y < configuration.global.height; y++)
                {
                    for (int x = 0; x < configuration.global.width; x++)
                    {
                        if (x < configuration.worldMap[l].cellMap.GetLength(0) && y < configuration.worldMap[l].cellMap.GetLength(1))
                        {
                            if (_index < configuration.worldMap[l].cellMapSingle.Length)
                            {
                                configuration.worldMap[l].cellMap[x, y] = configuration.worldMap[l].cellMapSingle[_index];
                            }
                        }

                        configuration.worldMap[l].maskMap[x, y] = configuration.worldMap[l].maskMapSingle[_index];

                        if (configuration.worldMap[l].tileObjects[x, y] != null)
                        {
                            configuration.worldMap[l].tileObjects[x, y] = configuration.worldMap[l].tileObjectsSingle[_index];
                            configuration.worldMap[l].tileTypes[x, y] = configuration.worldMap[l].tileTypesSingle[_index];
                        }

                        _index++;
                    }
                }
            }
        }

        #endregion

        //---------------------------------
        //BUILD WORLD // MAP INSTANTIATION
        //---------------------------------
        #region mapinstantiation
        //build block gameobjects 
        //this is the runtime IEnumerator method
        IEnumerator BuildWorldIE(bool[,] _map, int _layerIndex, int _layerCount, bool _rebuild)
        {
            if (configuration.presets.Count < 1)
                yield break; 
            if (configuration.worldMap.Count < 1)
                yield break; 

            //Build world in clusters
            //Divide width and height by clusterSize
            //Build each cluster and parent objects in a new gameobject
            float _clusterSize = (float)configuration.global.clusterSize;
            if (_clusterSize == 0)
            {
                _clusterSize = 1;
            }
            float xSizeF = Mathf.Floor(configuration.global.width / ((float)configuration.global.width / _clusterSize));
            float ySizeF = Mathf.Floor(configuration.global.height / ((float)configuration.global.height / _clusterSize));

            int xSize = (int)xSizeF;
            int ySize = (int)ySizeF;

            int xStep = 0;
            int yStep = 0;

            int index = 0;

            
            GameObject _layerContainer = null;
            GameObject _clusterContainer = null;

            if (_layerIndex > layerContainers.Count - 1)
            {
                _layerContainer = new GameObject();
                _layerContainer.transform.localScale = new Vector3(1, 1, 1);
                _layerContainer.name = "layer_" + _layerIndex.ToString();
                layerContainers.Add(_layerContainer);
            }
            else
            {
                _layerContainer = layerContainers[_layerIndex];
            }

            float _progressStep = (100f / (Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize))) / configuration.global.layerCount;/// layerCount

            var _maxClusterCount = Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize);

            for (int s = 0; s < Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize); s++)
            {
                if (clusterContainers.Count != (_layerIndex + 1) * ((Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize))))
                {
                    _clusterContainer = new GameObject();
                    _clusterContainer.transform.localScale = new Vector3(1, 1, 1);
                    _clusterContainer.name = "cluster_" + s.ToString();
                    _clusterContainer.transform.parent = _layerContainer.transform;
                    clusterContainers.Add(_clusterContainer);
                }

                for (int y = 0 + (ySize * yStep); y < ySize + (ySize * yStep); y++)
                {
                    for (int x = 0 + (xSize * xStep); x < xSize + (xSize * xStep); x++)
                    {
                        CallInstantiation(_map, y, x, s + ((int)_maxClusterCount * _layerIndex), _layerCount, _layerIndex, _rebuild);
                        ApplyMask(_map, x, y, _layerIndex);  
                    }
                }

                if (index >= (configuration.global.width / _clusterSize) - 1)
                {
                    yStep++;
                    xStep = 0;
                    index = 0;
                }
                else
                {
                    xStep++;
                    index++;
                }

                //if (merge && _container != null)
                //{
                //    TileWorldCombiner.CombineMesh(_container, createMultiMaterialMesh, addCollider);
                //}

                progress += _progressStep;

                TileWorldEvents.CallBuildProgress(progress);

                yield return null;
            }
        }

        //Called from BuildMapIE if merge is true
        IEnumerator MergeWorldIE()
        {
            mergeProgress = 0;
            var _containerCount = 0;

            for (int c = 0; c < clusterContainers.Count; c++)
            {
                TileWorldCombiner.CombineMesh(clusterContainers[c], configuration.global.addMeshCollider);

                if (configuration.global.createMultiMaterialMesh)
                {
                    //wait till next frame to make sure all objects are destroyed correctly before
                    //doing a second merge operation with multi material
                    yield return new WaitForEndOfFrame();
                    
                    TileWorldCombiner.CombineMulitMaterialMesh(clusterContainers[c], configuration.global.addMeshCollider);
                }

                _containerCount++;

                mergeProgress = (100 * _containerCount) / clusterContainers.Count;

                TileWorldEvents.CallMergeProgress(mergeProgress);
            }

            TileWorldEvents.CallOnMergeComplete();

            yield return null;
        }
    

        void BuildWorld(bool[,] _map, int _layerIndex, int _layerCount, bool _rebuild)
        {
            if (configuration.presets.Count < 1)
                return;


            float _clusterSize = (float)configuration.global.clusterSize;
            if (_clusterSize == 0)
            {
                _clusterSize = 1;
            }

            //Build world in clusters
            //Divide width and height by clusterSize
            //Build each cluster and parent objects in a new gameobject
            float xSizeF = Mathf.Floor(configuration.global.width / ((float)configuration.global.width / _clusterSize));
            float ySizeF = Mathf.Floor(configuration.global.height / ((float)configuration.global.height / _clusterSize));

            int xSize = (int)xSizeF;
            int ySize = (int)ySizeF;

            int xStep = 0;
            int yStep = 0;

            int index = 0;

          
            GameObject _layerContainer = null;
            GameObject _clusterContainer = null;


            if (_layerIndex > layerContainers.Count - 1)
            {
                _layerContainer = new GameObject();
                _layerContainer.transform.localScale = new Vector3(1, 1, 1);
                _layerContainer.name = "layer_" + _layerIndex.ToString();
                layerContainers.Add(_layerContainer);
            }
            else
            {
                _layerContainer = layerContainers[_layerIndex];
            }

            var _maxClusterCount = Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize);

            for (int s = 0; s < Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize); s++)
            {
                if (clusterContainers.Count != (_layerIndex + 1) * ((Mathf.Ceil((float)configuration.global.width / _clusterSize) * Mathf.Ceil((float)configuration.global.height / _clusterSize))) )
                {
                    _clusterContainer = new GameObject();
                    _clusterContainer.transform.localScale = new Vector3(1, 1, 1);
                    _clusterContainer.name = "cluster_" + s.ToString();
                    _clusterContainer.transform.parent = _layerContainer.transform;
                    clusterContainers.Add(_clusterContainer);
                }

             
                for (int y = 0 + (ySize * yStep); y < ySize + (ySize * yStep); y++)
                {
                    for (int x = 0 + (xSize * xStep); x < xSize + (xSize * xStep); x++)
                    {
                        CallInstantiation(_map, y, x, s + ((int)_maxClusterCount * _layerIndex), _layerCount, _layerIndex, _rebuild);
                        ApplyMask(_map, x, y, _layerIndex);   
                    }
                }

                if (index >= (configuration.global.width / _clusterSize) - 1)
                {
                    yStep++;
                    xStep = 0;
                    index = 0;
                }
                else
                {
                    xStep++;
                    index++;
                }
            }

        }


        void ApplyMask(bool[,] _map, int _x, int _y, int _layerIndex)
        {

            if (configuration.worldMap[_layerIndex].useMask)
            {   
                if (_x < _map.GetLength(0) && _y < _map.GetLength(1))
                {
                    if (_map[_x, _y] != configuration.worldMap[_layerIndex].maskMap[_x, _y])
                    {
                        if (_x < configuration.worldMap[_layerIndex].tileObjects.GetLength(0) && _y < configuration.worldMap[_layerIndex].tileObjects.GetLength(1))
                        {
                            if (configuration.worldMap[_layerIndex].tileObjects[_x, _y] != null) 
                            {
                                DestroyImmediate(configuration.worldMap[_layerIndex].tileObjects[_x, _y].gameObject);
                            }
                        }
                    }
                }
            }
        }

        void PartialMapChecker(bool _coroutine)
        {
          
            var i = configuration.ui.mapIndex;
            //for (int i = 0; i < layerCount; i++)
            //{
                if (configuration.worldMap[i].oldCellMap.GetLength(0) > 0 && configuration.worldMap[i].oldCellMap.GetLength(1) > 0)
                {
                    for (int y = 0; y < configuration.worldMap[i].cellMap.GetLength(1); y++)
                    {
                        for (int x = 0; x < configuration.worldMap[i].cellMap.GetLength(0); x++)
                        {
                            if (configuration.worldMap[i].cellMap[x, y] != configuration.worldMap[i].oldCellMap[x, y])
                            {
                                //t = how many border tiles should be replaced
                                for (int t = 1; t < 2; t++)
                                {
                                    
                                    if (configuration.worldMap[i].tileObjects[x, y] != null)
                                    {
                                        DestroyImmediate(configuration.worldMap[i].tileObjects[x, y].gameObject);
                                    }
                           

                                    if (x + t < configuration.global.width)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x + t, y] != null) 
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x + t, y].gameObject);
                                        }
                                    }
                                    if (x - t >= 0)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x - t, y] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x - t, y].gameObject);
                                            
                                        }
                                    }
                                    if (y + t < configuration.global.height)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x, y + t] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x, y + t].gameObject);
                                            
                                        }
                                    }
                                    if (y - t >= 0)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x, y - t] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x, y - t].gameObject);
                                            
                                        }
                                    }
                                    if (x - t >= 0 && y - t >= 0)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x - t, y - t] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x - t, y - t].gameObject);
                                            
                                        }
                                    }
                                    if (x - t >= 0 && y + t < configuration.global.height)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x - t, y + t] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x - t, y + t].gameObject);
                                            
                                        }
                                    }
                                    if (x + t < configuration.global.width && y - t >= 0)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x + t, y - t] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x + t, y - t].gameObject);
                                            
                                        }
                                    }
                                    if (x + t < configuration.global.width && y + t < configuration.global.height)
                                    {
                                        if (configuration.worldMap[i].tileObjects[x + t, y + t] != null)
                                        {
                                            DestroyImmediate(configuration.worldMap[i].tileObjects[x + t, y + t].gameObject);
                                           
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            //}

            if (_coroutine)
            {
                StartCoroutine(BuildMapIE(false, false));
            }
            else
            {
                BuildMap(false);
            } 
        }

        void CallInstantiation(bool[,] _map, int y, int x, int s, int _layerCount, int _layerIndex, bool _rebuild)
        {
            int _rndTilesetIndex = Random.Range(0, configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles.Count);

            float _rX = configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTilesetIndex].blockOffset.x;
            float _rY = configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTilesetIndex].blockOffset.y;
            float _rZ = configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTilesetIndex].blockOffset.z;

            float _rfX = configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTilesetIndex].floorOffset.x;
            float _rfY = configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTilesetIndex].floorOffset.y;
            float _rfZ = configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTilesetIndex].floorOffset.z;

            Vector3 _selT = new Vector3(_rX, _rY, _rZ);
            Vector3 _selTFloor = new Vector3(_rfX, _rfY, _rfZ);

            //BUILD INVERT FALSE
            //-------------------
            if (!configuration.global.invert && y < configuration.global.height && y >= 0 && x < configuration.global.width && x >= 0)
            {
                if (x >= _map.GetLength(0) && y >= _map.GetLength(1))
                    return;
                 
                //build blocks
                if (!_map[x, y])
                {

                    origin = new Vector3(transform.position.x + (_selT.x + x) * configuration.global.globalScale, transform.position.y + (_selT.y + _layerIndex) * configuration.global.globalScale, transform.position.z + (_selT.z + y) * configuration.global.globalScale);


                    if (configuration.global.layerCount < 2)
                    {                       
                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);    
                    }
                    else
                    {
                        //we have to check if the tile in the layer above is overlapping
                        if (!configuration.global.buildOverlappingTiles)
                        {
                            bool _isOverlapping = true;
                            
                            //check if tiles from layer above is overlapping
                            int lc = _layerCount + 1;

                            if (lc < configuration.global.layerCount)
                            {
                                if (!configuration.worldMap[lc].useMask)
                                {
                                    //only instantiate if tiles are not overlapping
                                    if (!_map[x, y] && configuration.worldMap[lc].cellMap[x, y])
                                    {
                                        _isOverlapping = false;
                                    }
                                    else
                                    {
                                        _isOverlapping = true;
                                    }
                                }
                                else
                                {
                                    if (!_map[x, y] && configuration.worldMap[lc].maskMap[x, y])
                                    {
                                        _isOverlapping = false;
                                    }
                                    else
                                    {
                                        _isOverlapping = true;
                                    }
                                }
                            }

                            //tiles from the top layer will always get builded
                            if (_layerCount == configuration.global.layerCount - 1 )
                            {
                                _isOverlapping = false;
                            }

                            if (!_isOverlapping)
                            {
                                InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);
                            }

                            /*
                            if (_layerCount + 1 < layerCount)
                            {
                                if (!configuration.worldMap[_layerCount + 1].useMask)
                                {
                                    //only instantiate if tiles are not overlapping
                                    if (!_map[x, y] && configuration.worldMap[_layerCount + 1].map[x, y])
                                    {
                                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, containers[s], _rndTilesetIndex);
                                    }
                                }
                                else
                                {
                                    //only instantiate if tiles are not overlapping with mask map
                                    if (!_map[x, y] && configuration.worldMap[_layerCount + 1].maskMap[x, y])
                                    {
                                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, containers[s], _rndTilesetIndex);
                                    }
                                }

                            }
                            else //no layers above current
                            {

                                InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, containers[s], _rndTilesetIndex);
                            }*/
                            
                        }
                        else
                        {         
                            InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);        
                        }
                    }
                }

                //build ground tiles
                if (_map[x, y] && configuration.global.buildGroundTiles && _layerCount < 1)
                {

                    origin = new Vector3(transform.position.x + (_selTFloor.x + x) * configuration.global.globalScale, transform.position.y + (_selTFloor.y + _layerIndex) * configuration.global.globalScale, transform.position.z + (_selTFloor.z + y) * configuration.global.globalScale);


                    if (configuration.global.layerCount < 2)
                    {
                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                    }
                    else
                    {
                        if (!configuration.global.buildOverlappingTiles)
                        {
                            if (_layerCount == 0 && _layerCount + 1 < configuration.global.layerCount)
                            {

                                //only instantiate if tiles are not overlapping
                                if (_map[x, y] && configuration.worldMap[_layerCount + 1].cellMap[x, y])
                                {

                                    InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                                }
                            }
                            else
                            {
                                InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                            }
                        }
                        else
                        {
                            InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                        }
                    }
                }
            }


            //BUILD INVERT TRUE
            //-----------------
            else if (y < configuration.global.height && y >= 0 && x < configuration.global.width && x >= 0)
            {
                if (_map[x, y])
                {

                    origin = new Vector3(transform.position.x + (_selT.x + x) * configuration.global.globalScale, transform.position.y + (_selT.y + _layerIndex) * configuration.global.globalScale, transform.position.z + (_selT.z + y) * configuration.global.globalScale);


                    if (configuration.global.layerCount < 2)
                    {
                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);
                    }
                    else
                    {
                        if (!configuration.global.buildOverlappingTiles)
                        {
                            //we have to check if the tile in the layer above is overlapping
                            if (_layerCount + 1 < configuration.global.layerCount)
                            {
                                if (!configuration.worldMap[_layerCount + 1].useMask)
                                {
                                    //only instantiate if tiles are not overlapping
                                    if (_map[x, y] && !configuration.worldMap[_layerCount + 1].cellMap[x, y])
                                    {
                                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);
                                    }
                                }
                                else
                                {
                                    //only instantiate if tiles are not overlapping
                                    if (_map[x, y] && !configuration.worldMap[_layerCount + 1].maskMap[x, y])
                                    {
                                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);
                                    }
                                }
                            }
                            else //no layers above current
                            {
                                InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);
                            }

                        }
                        else
                        {
                            InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, false, clusterContainers[s], _rndTilesetIndex);
                        }
                    }

                }

                //build ground tiles
                if (!_map[x, y] && configuration.global.buildGroundTiles && _layerCount < 1)
                {

                    origin = new Vector3(transform.position.x + (_selTFloor.x + x) * configuration.global.globalScale, transform.position.y + (_selTFloor.y + _layerIndex) * configuration.global.globalScale, transform.position.z + (_selTFloor.z + y) * configuration.global.globalScale);
                   
                    if (configuration.global.layerCount < 2)
                    {
                        InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                    }
                    else
                    {
                        if (!configuration.global.buildOverlappingTiles)
                        {
                            if (_layerCount == 0 && _layerCount + 1 < configuration.global.layerCount)
                            {

                                //only instantiate if tiles are not overlapping
                                if (!_map[x, y] && !configuration.worldMap[_layerCount + 1].cellMap[x, y])
                                {
                                    InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                                }

                            }
                            else
                            {
                                InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                            }
                        }
                        else
                        {
                            InstantiateBlocks(_map, x, y, _layerIndex, _layerCount, _rebuild, true, clusterContainers[s], _rndTilesetIndex);
                        }
                    }

                }
            }
        }


        void InstantiateBlocks(bool[,] _map, int x, int y, int _layerIndex, int _layerCount, bool _rebuild, bool _createFloor, GameObject _container, int _rndTileSetIndex)
        {
          
            if  (configuration.worldMap[_layerIndex].tileObjects[x, y] != null && !firstTimeBuild)
            {
                return;
            }
           

            // I = Tile with one edge
            // C = Corner tile
            // Ci = Corner tile inverted
            // B = Full tile
            // F = floor tile
            GameObject _instance = null;


            //var _currentTileType = TileWorldConfiguration.Maps.TileTypes.ground;
            var _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.ground;

            if (_map[x, y] == configuration.global.invert)
            {

                int cbs = TileWorldNeighbourCounter.CountCrossNeighbours(_map, x, y, 1, _createFloor, configuration.global.invert);

                //different block types according to different neighbours count
                //B = full tile
                if (cbs == 0)
                {

                    if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB != null)
                    {
                        _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB, origin, Quaternion.identity) as GameObject;
                        _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.block;

                        AddYOffsetRotation(3, _instance, _rndTileSetIndex, _layerIndex);

                    }


                    int _itc = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_map, x, y, 1, configuration.global.invert);

                    //only assign the very first time
                    var _li = 1;

                    if (_itc >= (((_li * 2) + 1) * ((_li * 2) + 1)) - 1)
                    {

                        if (!_rebuild)
                        {
                            //this will only be executed on first layer layerCount = 0
                            if (_layerCount < configuration.worldMap.Count - 1) //.mountainMaps.Count)
                            {
                                configuration.worldMap[_layerCount + 1].cellMap[x, y] = false;
                            }
                        }

                    }
                    else
                    {
                        //This is important for assigning new orientations
                        //even though we do not use the integer values
                        int _itv = TileWorldNeighbourCounter.CountDiagonalNeighbours(_map, x, y, 1, true, configuration.global.invert);

                        if (_instance != null)
                        {
                            DestroyImmediate(_instance.gameObject);
                        }

                        if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi != null)
                        {
                            _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                            _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                            if (orientation == "swsenw")
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                            }
                            else if (orientation == "swsene")
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                            }
                            else if (orientation == "senwne")
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            }
                            else if (orientation == "swnwne")
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                            }



                            AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                        }
                    }

                }


                //I = one edge tile
                if (cbs == 1)
                {

                    if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI != null)
                    {
                        _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                        _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                        if (orientation == "w")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                        }
                        else if (orientation == "n")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                        }
                        else if (orientation == "e")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                        }
                        else if (orientation == "s")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        }




                        AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                    }

                    //if we are at the edge of the map
                    //replace this tile with a block tileB
                    if (x == configuration.global.width - 1 || x == 0 || y == configuration.global.height - 1 || y == 0)
                    {

                        if (_instance != null)
                        {
                            DestroyImmediate(_instance.gameObject);
                        }

                        if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB != null)
                        {
                            _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB, origin, Quaternion.identity) as GameObject;
                            _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.block;


                            AddYOffsetRotation(3, _instance, _rndTileSetIndex, _layerIndex);
                        }
                    }

                    //theres one special case here
                    //check how many neighbours we have diagonal
                    int _idd = TileWorldNeighbourCounter.CountDiagonalNeighbours(_map, x, y, 1, false, configuration.global.invert);

                    //if theres only one neighbour
                    //change this tile with a Ci tile
                    if (_idd == 1)
                    {
                        //check if tile is on a border edge
                        if (x == 0 || y == 0 || x == configuration.global.width - 1 || y == configuration.global.height - 1)
                        {

                            if (_instance != null)
                            {
                                DestroyImmediate(_instance.gameObject);
                            }

                            if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi != null)
                            {
                                _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                                _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                                if (orientation == "ne")
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                }
                                else if (orientation == "nw")
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                }
                                else if (orientation == "sw")
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                }
                                else if (orientation == "se")
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                }
                              


                                AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                            }
                        }
                    }

                }

                //C = corner
                if (cbs == 2)
                {
                    //for all four corners we have to check if theres a diagonal neighbour
                    //if theres one, instantiate a inverted corner instead of a block tile.
                    int _gc = 0;

                    if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileC != null)
                    {
                        //Instantiate corner
                        _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileC, origin, Quaternion.identity) as GameObject;
                        _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.corner;


                        AddYOffsetRotation(1, _instance, _rndTileSetIndex, _layerIndex);
                    }

                    //special case
                    //those are no corner blocks but blocks with a west and east side
                    //or north and south side.
                    //mostly on the border of a map
                    //also we have to check if it is in a single row
                    //next to a double row, then we have to replace it by an inverted corner
                    if (orientation == "we")
                    {

                        //TileWorldCache.Instance.DespawnTile(_instance, _type);
                        if (_instance != null)
                        {
                            DestroyImmediate(_instance);
                        }

                        //_type = TileWorldCache.TileType.I;
                        //_instance = TileWorldCache.Instance.SpawnTile(presets[layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, _type, origin, Quaternion.identity);

                        if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI != null)
                        {
                            _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                            _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                            //if its on the right border edge
                            if (x > 0)
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                            }
                            //else its on the left border edge
                            else
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                            }



                            AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                        }

                    }
                    else if (orientation == "ns")
                    {

                        if (_instance != null)
                        {
                            DestroyImmediate(_instance);
                        }

                        if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI != null)
                        {
                            _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                            _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                            //if its on the upper edge
                            if (y > 0)
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                            }
                            //else its on the lower border edge
                            else
                            {
                                _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            }


                            AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                        }
                    }

                    //Normal Corner blocks
                    //--------------------
                    else if (orientation == "nw")
                    {

                        if (_instance != null)
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));


                            AddYOffsetRotation(1, _instance, _rndTileSetIndex, _layerIndex);
                        }

                        //Special cases!
                        //--------------
                        //bottomleft corner replace with a block tile or if theres one diagonal neighbour replace with an inverted corner
                        if (y == 0 && x == 0)
                        {
                            _gc = TileWorldNeighbourCounter.CountDiagonalNeighbours(_map, x, y, 1, false, configuration.global.invert);

                            if (_gc == 1)
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi != null)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                                    //if (x == 0 && y == 0)
                                    //{
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                    //}

                                    AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                            //bottom left corner
                            else
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB != null)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.block;

                                    AddYOffsetRotation(3, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                        }

                        //check if tile is at the left or bottom border edge
                        //if it is change corner tile to edge tile
                        else if ((y != configuration.global.height - 1 && y != 0 && x == 0) || (x != configuration.global.width - 1 && x != 0 && y == 0))
                        {

                            if (_instance != null)
                            {
                                DestroyImmediate(_instance);
                            }

                            if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI)
                            {
                                _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                                _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                                //left side
                                if (y != configuration.global.height - 1 && y != 0 && x == 0)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                }
                                //bottom side
                                else if (x != configuration.global.width - 1 && x != 0 && y == 0)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                }



                                AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                            }
                        }
                        //-----------


                    }
                    else if (orientation == "ne")
                    {

                        if (_instance != null)
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

                            AddYOffsetRotation(1, _instance, _rndTileSetIndex, _layerIndex);
                        }

                        //Special cases!
                        //--------------
                        //bottomRight corner replace with a block tile or if theres one diagonal neighbour replace with an inverted corner
                        if (y == 0 && x == configuration.global.width - 1)
                        {
                            _gc = TileWorldNeighbourCounter.CountDiagonalNeighbours(_map, x, y, 1, false, configuration.global.invert);

                            if (_gc == 1)
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi != null)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

                                    AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                            //bottom right corner
                            else
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB != null)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.block;

                                    AddYOffsetRotation(3, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                        }

                        //check if tile is at the right or bottom border edge
                        //if it is change corner tile to edge tile
                        else if ((y != configuration.global.height - 1 && y != 0 && x == configuration.global.width - 1) || (x != configuration.global.width && x != 0 && y == 0))
                        {
                            if (_instance != null)
                            {
                                DestroyImmediate(_instance);
                            }

                            if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI)
                            {
                                _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                                _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                                //right side
                                if (y != configuration.global.height - 1 && y != 0 && x == configuration.global.width - 1)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                }
                                //bottom side
                                else if ((x != configuration.global.width && x != 0 && y == 0))
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                }


                                AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                            }
                        }
                        //--------------

                    }
                    else if (orientation == "es")
                    {
                        if (_instance != null)
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                            AddYOffsetRotation(1, _instance, _rndTileSetIndex, _layerIndex);
                        }

                        //Special cases!
                        //--------------
                        //topRight corner replace with a block tile or if theres one diagonal neighbour replace with an inverted corner
                        if (y == configuration.global.height - 1 && x == configuration.global.width - 1)
                        {
                            _gc = TileWorldNeighbourCounter.CountDiagonalNeighbours(_map, x, y, 1, false, configuration.global.invert);

                            if (_gc == 1)
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                                    AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                            else
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.block;

                                    AddYOffsetRotation(3, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                        }

                        //check if tile is at the right or top border edge
                        //if it is change corner tile to edge tile
                        if ((y != configuration.global.height - 1 && y != 0 && x == configuration.global.width - 1) || (x != 0 && x != configuration.global.width - 1 && y == configuration.global.height - 1))
                        {
                            if (_instance != null)
                            {
                                DestroyImmediate(_instance);
                            }

                            if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI)
                            {
                                _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                                _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                                //right side
                                if (y != configuration.global.height - 1 && y != 0 && x == configuration.global.width - 1)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                }
                                //top side
                                else if (x != 0 && x != configuration.global.width - 1 && y == configuration.global.height - 1)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                }

                                AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                            }
                        }
                        //--------------

                    }
                    else if (orientation == "ws")
                    {
                        if (_instance != null)
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

                            AddYOffsetRotation(1, _instance, _rndTileSetIndex, _layerIndex);
                        }

                        //Special cases!
                        //--------------
                        //topLeft corner replace with a block tile or if theres one diagonal neighbour replace with an inverted corner	
                        if (y == configuration.global.height - 1 && x == 0)
                        {
                            _gc = TileWorldNeighbourCounter.CountDiagonalNeighbours(_map, x, y, 1, false, configuration.global.invert);
                            //_gc = CountDiagonalNeighbours(_map, x, y, 1, false);

                            if (_gc == 1)
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi != null)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));

                                    AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                            else
                            {
                                if (_instance != null)
                                {
                                    DestroyImmediate(_instance);
                                }

                                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB)
                                {
                                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileB, origin, Quaternion.identity) as GameObject;
                                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.block;

                                    AddYOffsetRotation(3, _instance, _rndTileSetIndex, _layerIndex);
                                }
                            }
                        }

                        //check if tile is at the left or top border edge
                        //if it is change corner tile to edge tile
                        if ((y != configuration.global.height - 1 && y != 0 && x == 0) || (x != 0 && x != configuration.global.width - 1 && y == configuration.global.height - 1))
                        {
                            if (_instance != null)
                            {
                                DestroyImmediate(_instance);
                            }

                            if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI)
                            {
                                _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileI, origin, Quaternion.identity) as GameObject;
                                _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.edge;

                                //left side
                                if (y != configuration.global.height - 1 && y != 0 && x == 0)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                }
                                //top side
                                else if (x != 0 && x != configuration.global.width - 1 && y == configuration.global.height - 1)
                                {
                                    _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                }



                                AddYOffsetRotation(0, _instance, _rndTileSetIndex, _layerIndex);
                            }
                        }
                        //--------------
                    }
                }


                //Ci = corner inverted
                if (cbs == 3)
                {
                    if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi != null)
                    {
                        _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileCi, origin, Quaternion.identity) as GameObject;
                        _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.icorner;

                        if (orientation == "nwe")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                        }
                        else if (orientation == "wes")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        }
                        else if (orientation == "nws")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                        }
                        else if (orientation == "nes")
                        {
                            _instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                        }



                        AddYOffsetRotation(2, _instance, _rndTileSetIndex, _layerIndex);
                    }
                }

            }
            //create floor tile
            else if (_createFloor)
            {

                if (configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileF != null)
                {
                    _instance = Instantiate(configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].tileF, origin, Quaternion.identity) as GameObject;
                    //_currentTileType = TileWorldConfiguration.Maps.TileTypes.ground;
                    _currentTileType = TileWorldConfiguration.TileInformation.TileTypes.ground;

                    AddYOffsetRotation(4, _instance, _rndTileSetIndex, _layerIndex);
                }


            }

            //parent instance to container
            if (_instance != null)
            {
                _instance.transform.parent = _container.transform;
            }


            if (configuration.global.scaleTiles && _instance != null)
            {
                _instance.transform.localScale = new Vector3(_instance.transform.localScale.x * configuration.global.globalScale, _instance.transform.localScale.y * configuration.global.globalScale, _instance.transform.localScale.z * configuration.global.globalScale);
            }

           
            //if (configuration.worldMap[_layerIndex].tileInformation[x,y] != null)
            //{ 
                // Assign instance gameobject
                configuration.worldMap[_layerIndex].tileObjects[x, y] = _instance as GameObject;
                // Assign tiletype
                configuration.worldMap[_layerIndex].tileTypes[x, y] = _currentTileType;
            //}        

        }

        void AddYOffsetRotation(int _type, GameObject _instance, int _rndTileSetIndex, int _layerIndex)
        {
            _instance.transform.rotation *= Quaternion.Euler(new Vector3(0, configuration.presets[configuration.global.layerPresetIndex[_layerIndex]].tiles[_rndTileSetIndex].yRotationOffset[_type], 0));
        }
        #endregion

        //--------------------
        //MAP OPTIMIZATION
        //--------------------
        #region optimization
        //this should get rid of all cells with only one or two neighbouring cells
        //occured when generating maps with multiple layers.
        void OptimizePass(int _index)
        {
     
                bool[,] _tmpMap = new bool[configuration.global.width, configuration.global.height];

                for (int h = 0; h < configuration.global.height; h++)
                {
                    for (int g = 0; g < configuration.global.width; g++)
                    {
                        _tmpMap[g, h] = configuration.worldMap[_index].cellMap[g, h];
                    }
                }

            
                for (int y = 0; y < configuration.global.height; y++)
                {
                    for (int x = 0; x < configuration.global.width; x++)
                    {

                        if (_tmpMap[x, y] == configuration.global.invert)
                        {

                            int _countN = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x, y, 1, false, !configuration.global.invert);
                            if (_countN <= 2)
                            {
                                //check if each neighbour is on the opposite side
                                switch (orientation)
                                {
                                    case "ns":
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                    case "we":
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                    case "n":
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                    case "s":
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                    case "w":
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                    case "e":   
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                    case "":                  
                                        configuration.worldMap[_index].cellMap[x, y] = !configuration.global.invert;
                                        break;
                                }
                            }


                            //fill tiles between narrow spaces
                            //example:
                            /*
                                x = tile
				
                                ooxxo
                                oFxxo
                                oxxFo
                                oxxoo
			
                                F = fails must be filled with a block tile
                            */

                            int _countD = TileWorldNeighbourCounter.CountDiagonalNeighbours(_tmpMap, x, y, 1, false, !configuration.global.invert);
                            if (_countD <= 2)
                            {
                               
                                switch(orientation)
                                {
                                    case "swne":
                                        int _countCA = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x, y, 1, false, !configuration.global.invert);
                                        if (_countCA == 3)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y + 1] = configuration.global.invert;
                                        }
                                        else if (_countCA == 4)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y + 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x - 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y + 1] = configuration.global.invert;
                                        }        
                                        break;
                                    case "nesw":
                                        int _countCB = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x, y, 1, false, !configuration.global.invert);
                                        if (_countCB == 3)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y + 1] = configuration.global.invert;
                                        }
                                        else if (_countCB == 4)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y + 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x - 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y + 1] = configuration.global.invert;
                                        }
                                        break;
                                    case "senw":
                                        int _countCC = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x, y, 1, false, !configuration.global.invert);
                                        if (_countCC == 3)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y + 1] = configuration.global.invert;
                                        }
                                        else if (_countCC == 4)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y + 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x - 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y + 1] = configuration.global.invert;
                                        }
                                        break;
                                    case "nwse":
                                        int _countCD = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x, y, 1, false, !configuration.global.invert);
                                        if (_countCD == 3)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x, y + 1] = configuration.global.invert;
                                        }
                                        else if (_countCD == 4)
                                        {
                                            configuration.worldMap[_index].cellMap[x - 1, y + 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x - 1, y - 1] = configuration.global.invert;
                                            configuration.worldMap[_index].cellMap[x + 1, y + 1] = configuration.global.invert;
                                        }
                                        break;
                                }
                            }
                        }                     
                    }
                }
        }

        //same as OptimizePass but does it only in a certain radius from x and y position
        /// <summary>
        /// Does an optimization pass only in a certain area from x and y position
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public void OptimizePassPartial(int _index, int _x, int _y)
        {
            //create a new partial map 9x9
            bool[,] _tmpMap = new bool[9, 9];

            for (int h = -4; h < 5; h++)
            {
                for (int g = -4; g < 5; g++)
                {
                    
                    if (_x + g < configuration.worldMap[_index].cellMap.GetLength(0) && _x + g >= 0 && _y + h < configuration.worldMap[_index].cellMap.GetLength(1) && _y + h >= 0)
                    {
                        _tmpMap[g + 4, h + 4] = configuration.worldMap[_index].cellMap[_x + g, _y + h];
                    }
                    else
                    {
                        _tmpMap[g + 4, h + 4] = !configuration.global.invert;
                    }
                }
            }

            
            for (int y = -3; y < 4; y++)
            {
                for (int x = -3; x < 4; x++)
                {

                    if (_tmpMap[x + 4, y + 4] == configuration.global.invert)
                    {

                        int _countN = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x + 4, y + 4, 1, false, !configuration.global.invert);
                        if (_countN <= 2)
                        {
                            //check if each neighbour is on the opposite side
                            switch (orientation)
                            {
                                case "ns":
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                                case "we":
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                                case "n":
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                                case "s":                                
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                                case "w":
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                                case "e":
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                                case "":
                                    configuration.worldMap[_index].cellMap[_x + x, _y + y] = !configuration.global.invert;
                                    break;
                            }
                        }

                    //fill tiles between narrow spaces
                    //example:
                    /*
                        x = tile

                        ooxxo
                        oFxxo
                        oxxFo
                        oxxoo

                        F = fails must be filled with a block tile
                    */

                    int _countD = TileWorldNeighbourCounter.CountDiagonalNeighbours(_tmpMap, x + 4, y + 4, 1, false, !configuration.global.invert);
                        if (_countD <= 2)
                        {
                               
                            switch(orientation)
                            {
                                case "swne":
                                    int _countCA = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x + 4, y + 4, 1, false, !configuration.global.invert);
                                    if (_countCA == 3)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    else if (_countCA == 4)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) + 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) + 1] = configuration.global.invert;
                                    }        
                                    break;
                                case "nesw":
                                    int _countCB = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x + 4, y + 4, 1, false, !configuration.global.invert);
                                    if (_countCB == 3)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x), (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    else if (_countCB == 4)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) + 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    break;
                                case "senw":
                                    int _countCC = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x + 4, y + 4, 1, false, !configuration.global.invert);
                                    if (_countCC == 3)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    else if (_countCC == 4)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) + 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    break;
                                case "nwse":
                                    int _countCD = TileWorldNeighbourCounter.CountCrossNeighbours(_tmpMap, x + 4, y + 4, 1, false, !configuration.global.invert);
                                    if (_countCD == 3)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, _y + y] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[_x + x, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    else if (_countCD == 4)
                                    {
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) + 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) - 1, (_y + y) - 1] = configuration.global.invert;
                                        configuration.worldMap[_index].cellMap[(_x + x) + 1, (_y + y) + 1] = configuration.global.invert;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //--------------------
        //EDITOR METHODS
        //--------------------
        #region editormethods
      

        void NewContainer()
        { 
            
            if (configuration.global.worldName == "")
            {
                configuration.global.worldName = "TWC_World";
            }

            worldContainer = GameObject.Find(configuration.global.worldName);

            if (worldContainer != null)
            {
                DestroyImmediate(worldContainer);
            }

            //delete old container list and create a new one
            if (layerContainers.Count > 0)
            {
                for (int c = 0; c < layerContainers.Count; c++)
                {
                    if (layerContainers[c] != null)
                    {
                        DestroyImmediate(layerContainers[c].gameObject);
                    }
                }
            }

            worldContainer = new GameObject();
            

            worldContainer.name = configuration.global.worldName;

            clusterContainers = new List<GameObject>();
            layerContainers = new List<GameObject>();

        }


        /// <summary>
        /// Editor method. Fill the current layer
        /// </summary>
        public void FillLayerBlock()
        {
            for (int y = 0; y < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(1); y++)
            {
                for (int x = 0; x < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(0); x++)
                {
                    if (configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] != configuration.global.invert)
                    {
                        configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] = configuration.global.invert;
                    }
                }
            }

            UpdateMap();
        }


        /// <summary>
        /// Editor method. Clear the current layer.
        /// </summary>
        public void FillLayerGround()
        {
            for (int y = 0; y < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(1); y++)
            {
                for (int x = 0; x < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(0); x++)
                {
                    if (configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] == configuration.global.invert)
                    {

                        configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] = !configuration.global.invert;

                    }
                }
            }

            UpdateMap();
        }


        /// <summary>
        /// add new layer
        /// </summary>
        /// <param name="_addNewLayerFromEdit"></param>
        /// <param name="_useMask"></param>
        /// <param name="_selectedMask"></param>
        /// <param name="_duplicate"></param>
        /// <param name="_duplicateFromIndex"></param>
        public void AddNewLayer(bool _addNewLayerFromEdit, bool _useMask, int _selectedMask, bool _duplicate, int _duplicateFromIndex)
        {
            //simple fix for blocking layer creation the first time
            //if there's no map generated
            if (configuration.worldMap.Count < 1)
                return;

            if (configuration.global.invert)
            {
                //configuration.worldMap.Add(new TileWorldConfiguration.Maps(configuration.global.width, configuration.global.height, false, _useMask, _selectedMask));
                configuration.worldMap.Add(new TileWorldConfiguration.WorldMap(configuration.global.width, configuration.global.height, false, _useMask, _selectedMask));
            }
            else
            {
                configuration.worldMap.Add(new TileWorldConfiguration.WorldMap(configuration.global.width, configuration.global.height, true, _useMask, _selectedMask));
            }

            //ResizeStringArray(false, 0);
            ResizeIntArray(false, 0);

            if (_duplicate)
            {
                configuration.worldMap[configuration.worldMap.Count - 1].cellMap = (bool[,])configuration.worldMap[_duplicateFromIndex].cellMap.Clone();
            }


            if (_addNewLayerFromEdit)
            {
                configuration.global.layerCount++;
                UpdateMap();
            }


        }

        /// <summary>
        /// Editor method. Remove layer.
        /// </summary>
        /// <param name="_inx"></param>
        public void RemoveLayer(int _inx)
        {

            for (int y = 0; y < configuration.worldMap[_inx].tileObjects.GetLength(1); y++)
            {
                for (int x = 0; x < configuration.worldMap[_inx].tileObjects.GetLength(0); x++)
                {
                    if (configuration.worldMap[_inx].tileObjects[x, y] != null)
                    {
                        DestroyImmediate(configuration.worldMap[_inx].tileObjects[x, y].gameObject);
                    }
                }
            }

            ResizeIntArray(true, _inx);

            configuration.worldMap.RemoveAt(_inx);
            configuration.ui.mapIndex = 0;
            configuration.global.layerCount--;

        }

        /// <summary>
        /// Editor method. Add a new mask to layer
        /// </summary>
        /// <param name="_inx"></param>
        public void AddNewMask(int _inx)
        {
            configuration.worldMap[_inx].useMask = true;

            configuration.worldMap[_inx].maskMap = new bool[configuration.global.width, configuration.global.height];
            configuration.worldMap[_inx].maskMap = iMasks[configuration.worldMap[_inx].selectedMask].ApplyMask(configuration.worldMap[_inx].cellMap, GetComponent<TileWorldCreator>(), configuration);
        }

        /// <summary>
        /// Editor method. Clear all settings.
        /// </summary>
        /// <param name="_clearPresets"></param>
        /// <param name="_layerCount"></param>
        public void ClearSettings(bool _clearPresets, int _layerCount)
        {
            configuration.worldMap = new List<TileWorldConfiguration.WorldMap>();


            configuration.global.layerPresetIndex = new int[_layerCount];


            if (_clearPresets)
            {
                configuration.presets = new List<TileWorldConfiguration.PresetsConfiguration>();
                configuration.presets.Add(new TileWorldConfiguration.PresetsConfiguration());

                //blockLayerPresets = new string[0];

                configuration.global.layerPresetIndex = new int[0];
                configuration.ui.availablePresets = new string[0];

                ResizeStringArray(false, 0);
                ResizeIntArray(false, 0);

                configuration.worldMap = new List<TileWorldConfiguration.WorldMap>();
                configuration.worldMap.Add(new TileWorldConfiguration.WorldMap(configuration.global.width, configuration.global.height, true, false, 0));
                //settings = new Settings();
                //AssignSettings();
            }


            configuration.ui.mapIndex = 0;

        }

        /// <summary>
        /// Editor method. Copy map from a layer.
        /// </summary>
        public void CopyMapFromLayer()
        {
            cloneMap = new bool[configuration.global.width, configuration.global.height];

            cloneMap = (bool[,])configuration.worldMap[configuration.ui.mapIndex].cellMap.Clone();
        }

        /// <summary>
        /// Editor method. Paste copied map to a layer.
        /// </summary>
        public void PasteMapToLayer()
        {
            configuration.worldMap[configuration.ui.mapIndex].cellMap = (bool[,])cloneMap.Clone();

            UpdateMap();
        }

        // this is only for the layer presets editor
        //EditorGUILayout.popup
        //we have to resize the int[] and string[] array by preserving the correct values
        public void ResizeIntArray(bool _remove, int _index)
        {

            List<int> _tmpInt = new List<int>();


            for (int s = 0; s < configuration.global.layerPresetIndex.Length; s++)
            {
                _tmpInt.Add(configuration.global.layerPresetIndex[s]);
            }


            if (_remove)
            {
                _tmpInt.RemoveAt(_index);


                configuration.global.layerPresetIndex = new int[_tmpInt.Count];

                for (int n = 0; n < configuration.global.layerPresetIndex.Length; n++)
                {
                    configuration.global.layerPresetIndex[n] = _tmpInt[n];
                }

            }
            else
            {
                _tmpInt.Add(0);


                configuration.global.layerPresetIndex = new int[_tmpInt.Count];

                for (int m = 0; m < _tmpInt.Count; m++)
                {
                    configuration.global.layerPresetIndex[m] = _tmpInt[m];
                }
            }
        }

        public void ResizeStringArray(bool _remove, int _index)
        {
            List<string> _tmpString = new List<string>();

            for (int i = 0; i < configuration.ui.availablePresets.Length; i++)
            {
                _tmpString.Add(configuration.ui.availablePresets[i]);
            }

            if (_remove)
            {
                _tmpString.RemoveAt(_index);

                configuration.ui.availablePresets = new string[_tmpString.Count];

                for (int m = 0; m < configuration.ui.availablePresets.Length; m++)
                {
                    configuration.ui.availablePresets[m] = _tmpString[m];
                }
            }
            else
            {
                _tmpString.Add("Preset: " + configuration.ui.availablePresets.Length.ToString());


                configuration.ui.availablePresets = new string[_tmpString.Count];
                for (int a = 0; a < _tmpString.Count; a++)
                {
                    configuration.ui.availablePresets[a] = "Preset: " + a;
                }
            }
        }
        #endregion

        //--------------------
        // Aditional API
        //--------------------
        #region api

        /// <summary>
        /// returns the map multidim. index array position as a Vector2
        /// </summary>
        /// <param name="_worldPosition"></param>
        public Vector2 WorldToMapPosition(Vector3 _worldPosition)
        {
            if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
            {
                int x = Mathf.FloorToInt(_worldPosition.x / configuration.global.globalScale);
                int y = Mathf.FloorToInt(_worldPosition.z / configuration.global.globalScale);
                return new Vector2(x, y);
            }
            else
            {
                int x = Mathf.FloorToInt(_worldPosition.x / configuration.global.globalScale);
                int y = Mathf.FloorToInt(_worldPosition.y / configuration.global.globalScale);
                return new Vector2(x, y);
            }
        }

        #endregion

        //--------------------
        //MAP MODIFICATION
        //--------------------
        #region mapmodifications

        /// <summary>
        /// Add new tiles to the given position in a certain range
        /// set heighDependent = true if you want to make sure to add tiles only on the layer with the same height.
        /// set rebuild = true if you want to rebuild the map after modifying it.
        /// set optimize = true if you wish to optimize the map before building it.(makes sure there are no strange build behaviours)
        /// </summary>
        /// <param name="_position"></param>
        /// <param name="_range"></param>
        /// <param name="_heightDependent"></param>
        /// <param name="_rebuild"></param>
        /// <param name="_optimize"></param>
        public void AddTiles(Vector3 _position, int _range, bool _heightDependent, bool _rebuild, bool _optimize)
        {
            for (int l = 0; l < configuration.global.layerCount; l++)
            {
                Vector3 _gPAbs = new Vector3(Mathf.Round(_position.x) - 1, Mathf.Round(_position.y), Mathf.Round(_position.z) - 1);

                int _layerHeightIndex = l;

                //if height dependant is true, set layer index according to the transform height of the object
                if (_heightDependent)
                {
                    _layerHeightIndex = (int)_gPAbs.y;
                }

                if (_layerHeightIndex < configuration.global.layerCount && _layerHeightIndex >= 0)
                {
                    for (int rY = 0; rY < _range; rY++)
                    {
                        for (int rX = 0; rX < _range; rX++)
                        {
                            //if (!brushx2)
                            //{
                            //    configuration.worldMap[_layerHeightIndex].map[(int)_gPAbs.x, (int)_gPAbs.z] = configuration.global.invert;
                            //}
                            //else
                            //{
                                if (_gPAbs.x + rX < 0)
                                {
                                    _gPAbs.x = -1;
                                }
                                if (_gPAbs.z + rY < 0)
                                {
                                    _gPAbs.z = -1;
                                }

                                if (_gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0 && _gPAbs.x + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1))
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + rY] = configuration.global.invert;
                                }

                                if (_gPAbs.x + 1 + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1) && _gPAbs.z + rY >= 0 && _gPAbs.x + rX >= 0)
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + rY] = configuration.global.invert;
                                }

                                if (_gPAbs.x + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + 1 + rY] = configuration.global.invert;
                                }

                                if (_gPAbs.x + 1 + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + 1 + rY] = configuration.global.invert;
                                }
                            //}
                        }
                    }
                }

            }

            if (_rebuild)
            {
                //this is a custom optimization loop because we are using a radius 
                //instead of just a single point position             
                if (_optimize)
                {
                    for (int l = 0; l < configuration.global.layerCount; l++)
                    {
                        for (int rY = -_range; rY < _range; rY++)
                        {
                            for (int rX = -_range; rX < _range; rX++)
                            {
                                OptimizePassPartial(l, (int)_position.x + rX, (int)_position.z + rY);
                            }
                        }
                    }
                }

                //BuildMapPartial(false, false, 0, 0);
                BuildMapPartial(false, false);
            }

        }

        /// <summary>
        /// Remove tiles from the given position in a certain range
        /// set heighDependent = true if you want to make sure to remove tiles only on the layer with the same height.
        /// set rebuild = true if you want to rebuild the map after modifying it.
        /// set optimize = true if you wish to optimize the map before building it.(makes sure there are no strange build behaviours)
        /// </summary>
        /// <param name="_position"></param>
        /// <param name="_range"></param>
        /// <param name="_heightDependent"></param>
        /// <param name="_rebuild"></param>
        /// <param name="_optimize"></param>
        public void RemoveTiles(Vector3 _position, int _range, bool _heightDependent, bool _rebuild, bool _optimize)
        {
            for (int l = 0; l < configuration.global.layerCount; l++)
            {

                Vector3 _gPAbs = new Vector3(Mathf.Round(_position.x) - 1, Mathf.Round(_position.y), Mathf.Round(_position.z) - 1);

                int _layerHeightIndex = l;

                //if height dependent is true, set layer index according to the transform height of the object
                if (_heightDependent)
                {
                    _layerHeightIndex = (int)_gPAbs.y;
                }

                if (_layerHeightIndex < configuration.global.layerCount && _layerHeightIndex >= 0)
                {

                    for (int rY = 0; rY < _range; rY++)
                    {
                        for (int rX = 0; rX < _range; rX++)
                        {

                            //if (!brushx2)
                            //{
                            //    configuration.worldMap[_layerHeightIndex].map[(int)_gPAbs.x, (int)_gPAbs.z] = !configuration.global.invert;
                            //}
                            //else
                            //{
                                //if x or z position of position is out of grid
                                //set a correct int value
                                if (_gPAbs.x + rX < 0)
                                {
                                    _gPAbs.x = -1;
                                }
                                if (_gPAbs.z + rY < 0)
                                {
                                    _gPAbs.z = -1;
                                }

                                if (_gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0 && _gPAbs.x + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1))
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + rY] = !configuration.global.invert;
                                }

                                if (_gPAbs.x + 1 + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1) && _gPAbs.z + rY >= 0 && _gPAbs.x + rX >= 0)
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + rY] = !configuration.global.invert;
                                }

                                if (_gPAbs.x + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + 1 + rY] = !configuration.global.invert;
                                }

                                if (_gPAbs.x + 1 + rX < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < configuration.worldMap[_layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                                {
                                    configuration.worldMap[_layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + 1 + rY] = !configuration.global.invert;
                                }
                            //}
                        }
                    }
                }
            }

            if (_rebuild)
            {
                //this is a custom optimization loop because we are using a radius 
                //instead of just a single point position             
                if (_optimize)
                {
                    for (int l = 0; l < configuration.global.layerCount; l++)
                    {
                        for (int rY = -_range; rY < _range; rY++)
                        {
                            for (int rX = -_range; rX < _range; rX++)
                            {
                                OptimizePassPartial(l, (int)_position.x + rX, (int)_position.z + rY);
                            }
                        }
                    }
                }

                //BuildMapPartial(false, false, 0, 0);
                BuildMapPartial(false, true);
            }
        }

#if UNITY_5_4_OR_NEWER
        /// <summary>
        /// returns the current random seed.
        /// </summary>
        /// <returns></returns>
        public Random.State GetMapSeed()
        {
            return currentSeed;
        }
#else
        /// <summary>
        /// returns the current random seed.
        /// </summary>
        /// <returns></returns>
        public int GetMapSeed()
        {
            return currentSeed;
        }
#endif

        #endregion

        //--------------------
        // DRAW GRID AND BRUSH
        //--------------------
        #region scenegui
        void OnDrawGizmosSelected()
        {
            if (configuration == null || !showGrid)
                return;

            if (!configuration.ui.showGridAlways)
            {
                DrawGrid();
                DrawBrush();
            }

#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabType(this.gameObject) != PrefabType.DisconnectedPrefabInstance)
            {
                PrefabUtility.DisconnectPrefabInstance(this.gameObject);

                if (configuration.presets.Count == 0)
                {
                    configuration.presets.Add(new TileWorldConfiguration.PresetsConfiguration());
                    configuration.global.layerPresetIndex = new int[0];
                    configuration.ui.availablePresets = new string[0];

                    ResizeStringArray(false, 0);
                    ResizeIntArray(false, 0);

                }
            }
#endif
        }

        void OnDrawGizmos()
        {
            if (configuration == null || !showGrid)
                return;

            if (configuration.ui.showGridAlways)
            {
                DrawGrid();
                DrawBrush();
            }
        }

        void DrawGrid()
        {
            if (!configuration.ui.showGrid || configuration == null)
                return;
            if (configuration.worldMap.Count < 1 || configuration.presets.Count < 1)
                return;
            //Load back multidimensional array from single dim array
            if (configuration.worldMap[0].cellMap == null)
            {
                //Debug.Log("load one dim back");
                for (int l = 0; l < configuration.global.layerCount; l++)
                {
                    configuration.worldMap[l].cellMap = new bool[configuration.global.width, configuration.global.height];
                    int _index = 0;

                    for (int y = 0; y < configuration.global.height; y++)
                    {
                        for (int x = 0; x < configuration.global.width; x++)
                        {
                            configuration.worldMap[l].cellMap[x, y] = configuration.worldMap[l].cellMapSingle[_index];
                            _index++;
                        }
                    }
                }
            }

            //if map length is zero load mapSingle array to multidim array
            if (configuration.worldMap[0].cellMap.Length == 0)
            {
                LoadBack();
            }

            Vector3 pos = Vector3.zero;
            if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
            {
                pos = new Vector3(transform.position.x, (transform.position.y + configuration.global.globalScale + (configuration.ui.mapIndex * configuration.global.globalScale) + configuration.presets[configuration.global.layerPresetIndex[configuration.ui.mapIndex]].tiles[0].blockOffset.y), transform.position.z);
            }
            else
            {
                pos = new Vector3(transform.position.x, (transform.position.y - configuration.global.globalScale - (configuration.ui.mapIndex * configuration.global.globalScale) - configuration.presets[configuration.global.layerPresetIndex[configuration.ui.mapIndex]].tiles[0].blockOffset.y), transform.position.z);
            }

            //draw grid
            int _step1 = 0;

            for (float z = 0; z <= (float)configuration.global.height * configuration.global.globalScale; z += 1 * configuration.global.globalScale)
            {
                if (_step1 == 0)
                {
                    Gizmos.color = configuration.ui.gridColor;
                    _step1 = 1;
                }
                else
                {
                    Gizmos.color = new Color(configuration.ui.gridColor.r, configuration.ui.gridColor.g, configuration.ui.gridColor.b, configuration.ui.gridColor.a / 2);
                    _step1 = 0;
                }

                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                {
                    Gizmos.DrawLine(new Vector3(pos.x, pos.y, pos.z + z),
                        new Vector3(pos.x + configuration.global.width * configuration.global.globalScale, pos.y, pos.z + z));
                }
                else
                {

                    Gizmos.DrawLine(new Vector3(pos.x, pos.z + z, pos.y),
                        new Vector3(pos.x + configuration.global.width * configuration.global.globalScale, pos.z + z, pos.y));
                }
            }

            int _step2 = 0;
            for (float x = 0; x <= (float)configuration.global.width * configuration.global.globalScale; x += 1 * configuration.global.globalScale)
            {
                if (_step2 == 0)
                {
                    Gizmos.color = configuration.ui.gridColor;
                    _step2 = 1;
                }
                else
                {
                    Gizmos.color = new Color(configuration.ui.gridColor.r, configuration.ui.gridColor.g, configuration.ui.gridColor.b, configuration.ui.gridColor.a / 2);
                    _step2 = 0;
                }

                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                {
                    Gizmos.DrawLine(new Vector3(pos.x + x, pos.y, pos.z),
                    new Vector3(pos.x + x, pos.y, pos.z + configuration.global.height * configuration.global.globalScale));
                }
                else
                {
                    Gizmos.DrawLine(new Vector3(pos.x + x, pos.z, pos.y),
                        new Vector3(pos.x + x, pos.z + configuration.global.height * configuration.global.globalScale, pos.y));
                }
            }


            //draw cells
            if (configuration.worldMap.Count > 0)
            {
                for (int y = 0; y < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(1); y++)
                {
                    for (int x = 0; x < configuration.worldMap[configuration.ui.mapIndex].cellMap.GetLength(0); x++)
                    {
                        if (!configuration.global.invert)
                        {
                            //draw block cells
                            if (!configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y])
                            {
                                Gizmos.color = configuration.ui.cellColor;

                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), pos.y, (pos.z + (y + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), (pos.z + (y + 0.5f) * configuration.global.globalScale), pos.y), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }

                            //draw ground tiles
                            if (configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] && configuration.global.buildGroundTiles)
                            {
                                Gizmos.color = configuration.ui.floorCellColor;
                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), pos.y, (pos.z + (y + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), (pos.z + (y + 0.5f) * configuration.global.globalScale), pos.y), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                        }
                        else
                        {
                            if (configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y])
                            {
                                Gizmos.color = configuration.ui.cellColor;
                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), pos.y, (pos.z + (y + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), (pos.z + (y + 0.5f) * configuration.global.globalScale), pos.y), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                            //draw ground tiles
                            if (!configuration.worldMap[configuration.ui.mapIndex].cellMap[x, y] && configuration.global.buildGroundTiles)
                            {
                                Gizmos.color = configuration.ui.floorCellColor;
                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), pos.y, (pos.z + (y + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * configuration.global.globalScale), (pos.z + (y + 0.5f) * configuration.global.globalScale), pos.y), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                        }
                    }
                }
            }

            //draw mask cells
            if (configuration.worldMap[configuration.ui.mapIndex].paintMask)
            {
                //bool[,] _maskMap = iMasks[configuration.worldMap[mapIndex].selectedMask].ApplyMask(configuration.worldMap[mapIndex].map, this);
                for (int ym = 0; ym < configuration.worldMap[configuration.ui.mapIndex].maskMap.GetLength(1); ym++)
                {
                    for (int xm = 0; xm < configuration.worldMap[configuration.ui.mapIndex].maskMap.GetLength(0); xm++)
                    {
                        if (!configuration.global.invert)
                        {
                            //draw block cells
                            if (!configuration.worldMap[configuration.ui.mapIndex].maskMap[xm, ym]) // && configuration.worldMap[mapIndex].maskMap[xm, ym] != !configuration.worldMap[mapIndex].map[xm, ym])
                            {
                                Gizmos.color = configuration.ui.maskColor;

                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f), (pos.z + (ym + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.z + (ym + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f)), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                            else
                            {
                                Gizmos.color = new Color(0f / 255f, 0f / 255f, 0f / 255f, 150f / 255f);
                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f), (pos.z + (ym + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.z + (ym + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f)), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                        }
                        else
                        {
                            //draw block cells
                            if (configuration.worldMap[configuration.ui.mapIndex].maskMap[xm, ym] && configuration.worldMap[configuration.ui.mapIndex].maskMap[xm, ym] != !configuration.worldMap[configuration.ui.mapIndex].cellMap[xm, ym])
                            {
                                Gizmos.color = configuration.ui.maskColor;
                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f), (pos.z + (ym + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.z + (ym + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f)), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                            else
                            {
                                Gizmos.color = new Color(0f / 255f, 0f / 255f, 0f / 255f, 150f / 255f);
                                if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f), (pos.z + (ym + 0.5f) * configuration.global.globalScale)), new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));
                                }
                                else
                                {
                                    Gizmos.DrawCube(new Vector3((pos.x + (xm + 0.5f) * configuration.global.globalScale), (pos.z + (ym + 0.5f) * configuration.global.globalScale), (pos.y + 0.1f)), new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                                }
                            }
                        }
                    }
                }
            }

        }


        void DrawBrush()
        {

            if (configuration.worldMap.Count < 1)
                return;

            if (mouseOverGrid)
            {
                Gizmos.color = configuration.ui.brushColor;


                for (int y = 0; y < configuration.ui.brushSize; y ++)
                {
                    for (int x = 0; x < configuration.ui.brushSize; x++)
                    {

                        Vector3 _pos = new Vector3(0, 0, 0);

                        if (configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                        {
                            _pos = new Vector3((Mathf.Floor(mouseWorldPosition.x / configuration.global.globalScale) + 0.5f + x) * configuration.global.globalScale, (transform.position.y + (1.05f * configuration.global.globalScale) + configuration.ui.mapIndex) + configuration.presets[configuration.global.layerPresetIndex[configuration.ui.mapIndex]].tiles[0].blockOffset.y, (Mathf.Floor(mouseWorldPosition.z / configuration.global.globalScale) + 0.5f + y) * configuration.global.globalScale);

                            if (_pos.x > 0 + transform.position.x && _pos.z > 0 + transform.position.z && _pos.x < transform.position.x + (configuration.global.width * configuration.global.globalScale) && _pos.z < transform.position.z + (configuration.global.height * configuration.global.globalScale))
                            {
                                Gizmos.DrawCube(_pos, new Vector3(1 * configuration.global.globalScale, 0.05f, 1 * configuration.global.globalScale));             
                            }
                        }
                        else
                        {
                            _pos = new Vector3((Mathf.Floor(mouseWorldPosition.x / configuration.global.globalScale) + 0.5f + x) * configuration.global.globalScale, (Mathf.Floor(mouseWorldPosition.y / configuration.global.globalScale) + 0.5f + y) * configuration.global.globalScale, (transform.position.z - (1.05f * configuration.global.globalScale) - configuration.ui.mapIndex) - configuration.presets[configuration.global.layerPresetIndex[configuration.ui.mapIndex]].tiles[0].blockOffset.y);

                            if (_pos.x > 0 + transform.position.x && _pos.y > 0 + transform.position.y && _pos.x < transform.position.x + (configuration.global.width * configuration.global.globalScale) && _pos.y < transform.position.y + (configuration.global.height * configuration.global.globalScale))
                            {                              
                                Gizmos.DrawCube(_pos, new Vector3(1 * configuration.global.globalScale, 1 * configuration.global.globalScale, 0.05f));
                            }  
                        }
                        //Vector3 _pos = new Vector3((Mathf.Floor(mouseWorldPosition.x / configuration.global.globalScale) + 0.5f + x) * configuration.global.globalScale, (transform.position.y + (1.05f * configuration.global.globalScale) + configuration.ui.mapIndex) + configuration.presets[configuration.global.layerPresetIndex[configuration.ui.mapIndex]].tiles[0].blockOffset.y, (Mathf.Floor(mouseWorldPosition.z / configuration.global.globalScale) + 0.5f + y) * configuration.global.globalScale);

                        
                    }
                }
            }
        }
        
        #endregion
    }  
}