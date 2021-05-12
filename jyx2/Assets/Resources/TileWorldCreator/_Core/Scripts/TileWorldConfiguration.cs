/* TILE WORLD CREATOR TileWorldConfiguration
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 * 
 * All TileWorldCreator properties
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TileWorld
{
    [System.Serializable]
    public class TileWorldConfiguration : ScriptableObject
    {
        /// <summary>
        /// Create a new configuration. 
        /// _currentConfiguration = assign the current configuration to copy the presets from if _preservePresets is true
        /// _preservePresets = copy presets from current configuration to the new configuration.
        /// </summary>
        /// <param name="_currentConfig"></param>
        /// <param name="_preservePresets"></param>
        /// <returns></returns>
        public static TileWorldConfiguration NewConfiguration(TileWorldConfiguration _currentConfig, bool _preservePresets)
        {
            var _so = ScriptableObject.CreateInstance<TileWorldConfiguration>();

            if (_preservePresets)
            {
                _so.presets = _currentConfig.presets;
            }

            _so.Init();

            return _so;
        }

        // Initiate new configuration
        void Init()
        {
            ui = new UIConfiguration();
            global = new GlobalConfiguration();
            cellularAlgorithm = new CellularAlgorithmConfig();
            mazeAlgorithm = new MazeAlgorithmConfig();
            simpleDungeonAlgorithm = new SimpleDungeonAlgorithmConfig();
            bspDungeonAlgorithm = new BSPDungeonAlgorithmConfig();
        }

        [System.Serializable]
        public class TileInformation
        {
            public enum TileTypes
            {
                edge,
                corner,
                icorner,
                block,
                ground
            }

            public TileTypes tileType;
            public GameObject go;

            public TileInformation()
            {
                go = null;
                tileType = TileTypes.ground;
            }

            public TileInformation(GameObject _go, TileTypes _type)
            {
                go = _go;
                tileType = _type;
            }
        }

        [System.Serializable]
        public class WorldMap
        {
            //public TileInformation[,] tileInformation;

            public GameObject[,] tileObjects;
            public TileInformation.TileTypes[,] tileTypes;

            public bool[,] cellMap;
            public bool[,] maskMap;
            //public GameObject[,] objectsMap;

            //single map arrays for serialization
            public bool[] cellMapSingle;
            public bool[] maskMapSingle;

            public GameObject[] tileObjectsSingle;
            public TileInformation.TileTypes[] tileTypesSingle;

            public bool[,] oldCellMap;

            public bool useMask;
            public bool paintMask;
            public int selectedMask;

            public WorldMap()
            {

                tileObjects = new GameObject[0, 0];

                cellMap = new bool[0, 0];
                maskMap = new bool[0, 0];

                cellMapSingle = new bool[0];
                maskMapSingle = new bool[0];
                tileObjectsSingle = new GameObject[0];
                tileTypesSingle = new TileInformation.TileTypes[0];

                oldCellMap = new bool[0, 0];

                useMask = false;
                paintMask = false;
                selectedMask = 0;
            }

            public WorldMap(int _x, int _y, bool _invert, bool _useMask, int _selectedMask)
            {
               
                tileObjects = new GameObject[_x, _y];
                tileTypes = new TileInformation.TileTypes[_x, _y];

                cellMap = new bool[_x, _y];
                maskMap = new bool[_x, _y];

                cellMapSingle = new bool[_x * _y];
                maskMapSingle = new bool[_x * _y];
                tileObjectsSingle = new GameObject[_x * _y];
                tileTypesSingle = new TileInformation.TileTypes[_x * _y];

                oldCellMap = new bool[_x, _y];

                useMask = _useMask;
                selectedMask = _selectedMask;

                int _index = 0;
                for (int i = 0; i < _y; i++)
                {
                    for (int j = 0; j < _x; j++)
                    {
                        cellMap[j, i] = _invert;

                        tileObjects[j, i] = null;
                        tileTypes[j, i] = TileInformation.TileTypes.ground;

                        oldCellMap[j, i] = _invert;

                        cellMapSingle[_index] = _invert;

                        _index++;
                    }
                }
            }
        }

        [System.Serializable]
        public class UIConfiguration
        {
            public bool showGeneralSettings;
            public bool showPresetSettings;
            public bool showEdit;

            public bool autoBuild ;
            public int brushSize;


            public Color cellColor;
            public Color floorCellColor;
            public Color gridColor;

            public Color brushColor;
            public Color maskColor;
            

            public bool showGridAlways = false;
            public bool showGrid = true;
            public bool showAllMaps = false;


            //selected layer
            public int mapIndex;

            //availablePresets stores the available presets as string array
            public string[] availablePresets;

            public UIConfiguration ()
            {
                showGeneralSettings = false;
                showPresetSettings = false;
                showEdit = false;

                autoBuild = true;
                brushSize = 2;
               
                cellColor = new Color(120f / 255f, 255f / 255f, 120f / 255f, 150f / 255f);
                floorCellColor = new Color(0f / 255f, 180f / 255f, 255f / 255f, 150f / 255f);
                gridColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 180f / 255f);
                brushColor = new Color(226f / 255f, 41f / 255f, 32f / 255f, 150f / 255f);
                maskColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 150f / 255f);

                showGridAlways = false;
                showGrid = true;
                showAllMaps = false;

                mapIndex = 0;

                availablePresets = new string[0];
            }
        }

        [System.Serializable]
        public class GlobalConfiguration
        {
            // Global config
            public string worldName;

            public enum MapOrientations
            {
                xz,
                xy
            }

            public MapOrientations mapOrientation;

            public bool invert;
            public bool buildGroundTiles;

            // Layer config
            public int layerCount;
            public int layerInset;
            public bool buildOverlappingTiles;

            //layerPresetIndex stores the assigned preset for each layer
            public int[] layerPresetIndex;

            // Grid config
            public int width;
            public int height;
            public float globalScale;
            public bool scaleTiles;
            // Merge config
            public bool automaticClustersize;
            public int clusterSize;
            public bool createMultiMaterialMesh;
            public bool addMeshCollider;
            // Map generation config            
            public int optimizeSteps;
            public int randomSeed;
            public int selectedAlgorithm;

            public Vector3 startPosition;
            public Vector3 endPosition;

            // Constructor default values
            public GlobalConfiguration()
            {
                //default values
                worldName = "TWC_World";
                invert = false;

                buildGroundTiles = true;
                mapOrientation = MapOrientations.xz;

                layerCount = 1;
                layerInset = 1;
                buildOverlappingTiles = true;
                layerPresetIndex = new int[1];


                width = 20;
                height = 20;
                globalScale = 1;
                scaleTiles = true;

                automaticClustersize = true;
                clusterSize = 5;
                createMultiMaterialMesh = true;
                addMeshCollider = true;

                optimizeSteps = 4;
                
                randomSeed = -1;
                selectedAlgorithm = 0;

                startPosition = Vector3.zero;
                endPosition = Vector3.zero;
            }

        }


        [System.Serializable]
        public class TileSets
        {
            //pattern
            //
            //	I = edge tile
            //	C = corner tile
            //	Ci = corner inverted tile
            //	B = block tile
            //	F = ground tile

            public Vector3 blockOffset = new Vector3(0.5f, 0.5f, 0.5f);
            public Vector3 floorOffset = new Vector3(0.5f, 0f, 0.5f);

            //Blocks Prefabs
            public GameObject tileI;
            public GameObject tileC;
            public GameObject tileCi;
            public GameObject tileB;
            public GameObject tileF;

            public float yRotationOffsetI;
            public float yRotationOffsetC;
            public float[] yRotationOffset = new float[5] { 0f, 0f, 0f, 0f, 0f };


            public TileSets() { }

            public TileSets(TileSets _tileSet)
            {
                blockOffset = _tileSet.blockOffset;
                floorOffset = _tileSet.floorOffset;

                tileI = _tileSet.tileI;
                tileC = _tileSet.tileC;
                tileCi = _tileSet.tileCi;
                tileB = _tileSet.tileB;
                tileF = _tileSet.tileF;

                yRotationOffset = new float[_tileSet.yRotationOffset.Length];
                for (int f = 0; f < _tileSet.yRotationOffset.Length; f ++)
                {
                    yRotationOffset[f] = _tileSet.yRotationOffset[f];
                }
            }
        }



        [System.Serializable]
        public class PresetsConfiguration
        {
            public List<TileSets> tiles = new List<TileSets>();

            public bool showPresetEditor = false;

            public PresetsConfiguration()
            {
                tiles.Add(new TileSets());
            }
        }


        //===================================
        // Alogorithm configuration classes
        //===================================

        [System.Serializable]
        public class CellularAlgorithmConfig
        {
            public int deathLimit;
            public int birthLimit;
            public int numberOfSteps;
            public float chanceToStartAlive;
            public bool floodUnjoined;
            public bool floodHoles;
            public int minCellCount; // the minimum cell count of the biggest generated island

            public CellularAlgorithmConfig()
            {
                chanceToStartAlive = 0.45f;
                deathLimit = 4;
                birthLimit = 4;
                numberOfSteps = 5;
                floodUnjoined = true;
                floodHoles = true;
                minCellCount = 5;
            }
        }

        [System.Serializable]
        public class MazeAlgorithmConfig
        {
            public bool linear;
            public bool useRandomStartPos;
            public Vector2 startPos;

            public MazeAlgorithmConfig()
            {
                linear = false;
                useRandomStartPos = false;
                startPos = new Vector2(2, 2);
            }
        }

        [System.Serializable]
        public class SimpleDungeonAlgorithmConfig
        {
            public bool createCircleRooms;
            public float minCircleRoomRadius;
            public float maxCircleRoomRadius;

            public int roomCount;
            public bool allowRoomIntersections;
            public int minRoomSizeWidth;
            public int minRoomSizeHeight;
            public int maxRoomSizeWidth;
            public int maxRoomSizeHeight;
            public int minCorridorWidth;

            // Default value constructor
            public SimpleDungeonAlgorithmConfig()
            {
                createCircleRooms = false;
                minCircleRoomRadius = 2;
                maxCircleRoomRadius = 4;
                roomCount = 4;
                allowRoomIntersections = false;
                minRoomSizeWidth = 2;
                minRoomSizeHeight = 2;
                maxRoomSizeWidth = 4;
                maxRoomSizeHeight = 4;
                minCorridorWidth = 2;
            }
        }

        [System.Serializable]
        public class BSPDungeonAlgorithmConfig
        {
            public int minBSPLeafWidth;
            public int minBSPLeafHeight;
            public int corridorWidth;

            public BSPDungeonAlgorithmConfig()
            {
                minBSPLeafWidth = 10;
                minBSPLeafHeight = 10;
                corridorWidth = 2;
            }
        }

        //===================================================
        // Add your own algorithm configuration classes here
        //===================================================
        //
        // ->

        [HideInInspector]
        public UIConfiguration ui;

        [HideInInspector]
        public GlobalConfiguration global;

        [HideInInspector]
        public List<PresetsConfiguration> presets = new List<PresetsConfiguration>();

        [HideInInspector]
        public List<WorldMap> worldMap = new List<WorldMap>();

        public BSPDungeonAlgorithmConfig bspDungeonAlgorithm;
        public CellularAlgorithmConfig cellularAlgorithm;
        public MazeAlgorithmConfig mazeAlgorithm;
        public SimpleDungeonAlgorithmConfig simpleDungeonAlgorithm;
        
    }
}