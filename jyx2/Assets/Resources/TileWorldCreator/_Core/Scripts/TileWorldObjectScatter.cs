/* TILE WORLD CREATOR OBJECT SCATTER
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 * 
 * Create awesome tile worlds in seconds.
 * Use this script only in combination with 
 * TileWorldCreator
 *
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileWorld.Events;

namespace TileWorld
{
    [RequireComponent(typeof(TileWorldCreator))]
    public class TileWorldObjectScatter : MonoBehaviour {


        public TileWorldObjectScatterConfiguration configuration;

        public GameObject objectScatterContainer;
        public GameObject paintObjectsContainer;
        public GameObject positionObjectsContainer;
        public GameObject proceduralObjectsContainer;
        public List<GameObject> clusterContainers = new List<GameObject>();

        enum SpawnTypes
        {
            paint,
            position,
            procedural
        }
        
        SpawnTypes spawnTypes;

        enum RuleBasedOptions
        {
            random,
            pattern
        }

        RuleBasedOptions ruleBasedOptions;

        public bool mouseOverGrid;
        public bool showGrid;

        // saves the occupied tiles
        // to make sure objects are not placed on the same tile twice.
        // can be deactivated
        bool[,] occupyMap = new bool[0, 0] { };

        public TileWorldCreator creator;

        void OnEnable()
        {
            creator = this.GetComponent<TileWorldCreator>();
        }
       
        #region scatterpositionobjects     

        /// <summary>
        /// Scatter all position based objects
        /// </summary>
        public void ScatterPositionBasedObjects()
        {
            if (creator == null)
            {
                creator = this.GetComponent<TileWorldCreator>();
            }

            if (objectScatterContainer == null)
            {
                objectScatterContainer = new GameObject(creator.configuration.global.worldName + "_Objects");
            }

            objectScatterContainer.transform.rotation = Quaternion.identity;

            if (positionObjectsContainer != null)
            {
                DestroyImmediate(positionObjectsContainer);
            }

            positionObjectsContainer = new GameObject("positionObjects");
            positionObjectsContainer.transform.parent = objectScatterContainer.transform;

            occupyMap = new bool[creator.configuration.global.width, creator.configuration.global.height];

            for (int i = 0; i < configuration.positionObjects.Count; i ++)
            {
                if (configuration.positionObjects[i].placementType == TileWorldObjectScatterConfiguration.PositionObjectConfiguration.PlacementType.bestGuess)
                {
                    if (!configuration.positionObjects[i].isChild && configuration.positionObjects[i].active)
                    {
                        InstantiatePositionBestGuess(configuration.positionObjects[i].blockType, i);
                    }
                }
                else
                {
                    if (!configuration.positionObjects[i].isChild && configuration.positionObjects[i].active)
                    {
                        InstantiatePositionMapBased(i);
                    }
                }
            }

            //set object world container rotation according to map orientation
            if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xy)
            {
                objectScatterContainer.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            }

            if (Application.isPlaying)
            {
                TileWorldEvents.CallOnScatterPositionBasedComplete();
            }
        }

        // Scatter start end objects based on best guess
        void InstantiatePositionBestGuess(TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BlockType _blockType, int _index)
        {
            // divide map in 3x3 clusters
            var _clusterHeight = creator.configuration.worldMap[0].cellMap.GetLength(1) / 3;
            var _clusterWidth = creator.configuration.worldMap[0].cellMap.GetLength(0) / 3;
            Vector2 _center = Vector2.zero;
            Vector3 _pos = Vector3.zero;

            int _selectedLayer = configuration.positionObjects[_index].selectedLayer;

            var _terrain = false;
            if (_blockType == TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BlockType.block)
            {
                _terrain = true;
            }

            switch (configuration.positionObjects[_index].bestGuessSpawnPosition)
            {
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topLeft:

                    // Get center of top left cluster
                    _center = new Vector2(_clusterWidth / 2, (_clusterHeight * 2) + _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);
                   

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topMiddle:

                    // Get center of top middle
                    _center = new Vector2(_clusterWidth + (_clusterWidth / 2), (_clusterHeight * 2) + _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topRight:

                    // Get center of top right
                    _center = new Vector2((_clusterWidth * 2) + (_clusterWidth / 2), (_clusterHeight * 2) + _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerLeft:

                    // Get center of center left
                    _center = new Vector2((_clusterWidth / 2), (_clusterHeight) + _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerMiddle:

                    // Get center of center middle
                    _center = new Vector2(_clusterWidth + (_clusterWidth / 2), (_clusterHeight) + _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerRight:

                    // Get center of center right
                    _center = new Vector2((_clusterWidth * 2) + (_clusterWidth / 2), (_clusterHeight) + _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomLeft:

                    // Get center of bottom left
                    _center = new Vector2((_clusterWidth / 2), _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomMiddle:

                    // Get center of bottom middle
                    _center = new Vector2(_clusterWidth + (_clusterWidth / 2), _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomRight:

                    // Get center of bottom right
                    _center = new Vector2((_clusterWidth * 2) + (_clusterWidth / 2), _clusterHeight / 2);
                    _pos = new Vector3((int)_center.x, 0, (int)_center.y);

                    // check if we can place the object on this position
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[(int)_center.x, (int)_center.y] == !_terrain)
                    {
                        InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                    }
                    else // else we have to check the other tiles of the cluster 
                    {
                        DoSpiralCheck(_center, _clusterWidth, _clusterHeight, _index, _terrain, _blockType);
                    }

                    break;
            }
        }

        void DoSpiralCheck(Vector2 _center, int _clusterWidth, int _clusterHeight, int _index, bool _terrain, TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BlockType _blockType)
        {
            // check tiles from the center of the cluster in a spiral way
            int _x = 0;
            int _y = 0;
            int _dx = 0;
            int _dy = -1;

            int _t = Mathf.Max(_clusterWidth, _clusterHeight);
            int _maxI = _t * _t;

            bool _instantiationOK = false; // fallback

            for (int i = 0; i < _maxI; i++)
            {
                if ((-_clusterWidth / 2 <= _x) && (_x <= _clusterWidth / 2) && (-_clusterHeight / 2 <= _y) && (_y <= _clusterHeight / 2))
                {                  
                    if (creator.configuration.worldMap[configuration.positionObjects[_index].selectedLayer].cellMap[_x + (int)_center.x, _y + (int)_center.y] == !_terrain)
                    {
                        
                        Vector3 _pos2 = new Vector3(_x + _center.x, 0, _y + _center.y);
                        if (_x > 0 && _y > 0)
                        {
                            _instantiationOK = true;
                            InstantiateObjects(configuration.positionObjects[_index], _pos2, _index, _x, _y, SpawnTypes.position, positionObjectsContainer);
                        }
                        else
                        {
                            _instantiationOK = true;
                            InstantiateObjects(configuration.positionObjects[_index], _pos2, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
                        }

                        break;
                    }
                }

                if ((_x == _y) || ((_x < 0) && (_x == -_y)) || ((_x > 0) && (_x == 1 - _y)))
                {
                    _t = _dx;
                    _dx = -_dy;
                    _dy = _t;
                }

                _x += _dx;
                _y += _dy;
            }

            // fallback if instantiation did not worked
            if (!_instantiationOK)
            {
                // retry with different map position
                switch (configuration.positionObjects[_index].bestGuessSpawnPosition)
                {
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topLeft:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topMiddle;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topMiddle:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topRight;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.topRight:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerLeft;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerLeft:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerMiddle;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerMiddle:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerRight;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerRight:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.centerLeft;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomLeft:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomMiddle;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomMiddle:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomRight;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                    case TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomRight:
                        configuration.positionObjects[_index].bestGuessSpawnPosition = TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition.bottomLeft;
                        InstantiatePositionBestGuess(_blockType, _index);
                        break;
                }
            }
        }

        // scatter start end objects on map based
        void InstantiatePositionMapBased(int _index)
        {
            Vector3 _pos = new Vector3(0.5f, 0, 0.5f);

            if (configuration.positionObjects[_index].mapBasedSpawnPosition == TileWorldObjectScatterConfiguration.PositionObjectConfiguration.MapBasedSpawnPosition.startPosition)
            {        
                _pos += creator.configuration.global.startPosition;
            }
            else
            {
                _pos += creator.configuration.global.endPosition;
            }


            InstantiateObjects(configuration.positionObjects[_index], _pos, _index, 0, 0, SpawnTypes.position, positionObjectsContainer);
        }

        #endregion scatterpositionobjects

        #region scatterproceduralobjects

        /// <summary>
        /// Scatter all procedural objects
        /// </summary>
        public void ScatterProceduralObjects()
        {
            if (objectScatterContainer == null)
            {
                objectScatterContainer = new GameObject(creator.configuration.global.worldName + "_Objects"); 
            }

            objectScatterContainer.transform.rotation = Quaternion.identity;

            if (proceduralObjectsContainer != null)
            {
                DestroyImmediate(proceduralObjectsContainer);
            }

            proceduralObjectsContainer = new GameObject("proceduralObjects");
            proceduralObjectsContainer.transform.parent = objectScatterContainer.transform;

            clusterContainers = new List<GameObject>();

            occupyMap = new bool[creator.configuration.global.width, creator.configuration.global.height];

            for (int i = 0; i < configuration.proceduralObjects.Count; i ++)
            {
                if (!configuration.proceduralObjects[i].isChild && configuration.proceduralObjects[i].active)
                {
                    if (configuration.proceduralObjects[i].ruleType == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.RuleTypes.pattern)
                    {
                        switch (configuration.proceduralObjects[i].tileTypes)
                        {
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.edge:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.edge, i);
                                // Coroutine check for cluster instantiation // Deactivated because of experimental state
                                //InstantiateClusterCoroutineCheck(RuleBasedOptions.pattern, TileWorldConfiguration.Maps.TileTypes.edge, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.corner:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.corner, i);
                                // Coroutine check for cluster instantiation // Deactivated because of experimental state
                                //InstantiateClusterCoroutineCheck(RuleBasedOptions.pattern, TileWorldConfiguration.Maps.TileTypes.corner, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.invertedCorner:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.icorner, i);
                                // Coroutine check for cluster instantiation // Deactivated because of experimental state
                                //InstantiateClusterCoroutineCheck(RuleBasedOptions.pattern, TileWorldConfiguration.Maps.TileTypes.icorner, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.block:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.block, i);
                                // Coroutine check for cluster instantiation // Deactivated because of experimental state
                                //InstantiateClusterCoroutineCheck(RuleBasedOptions.pattern, TileWorldConfiguration.Maps.TileTypes.terrain, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.ground:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.ground, i);
                                // Coroutine check for cluster instantiation // Deactivated because of experimental state
                                //InstantiateClusterCoroutineCheck(RuleBasedOptions.pattern, TileWorldConfiguration.Maps.TileTypes.water, i);
                                break;
                        }
                    }
                    else
                    {
                        switch (configuration.proceduralObjects[i].tileTypes)
                        {
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.edge:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.edge, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.corner:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.corner, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.invertedCorner:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.icorner, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.block:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.block, i);
                                break;
                            case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.ground:
                                InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes.ground, i);
                                break;
                                // Coroutine check for cluster instantiation // Deactivated because of experimental state
                                //InstantiateClusterCoroutineCheck(RuleBasedOptions.random, TileWorldConfiguration.Maps.TileTypes.edge, i);
                        }
                    }
                }
            }


            //assign all clusters to worldContainer
            for (int c = 0; c < clusterContainers.Count; c++)
            {
                clusterContainers[c].transform.parent = proceduralObjectsContainer.transform;
            }

            //set object world container rotation according to map orientation
            if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xy)
            {
                objectScatterContainer.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            }

            if (Application.isPlaying)
            {
                TileWorldEvents.CallOnScatterProceduralComplete();
            }
        }


        // Check if we are running in editor or not
        //void InstantiateClusterCoroutineCheck(RuleBasedOptions _rbOptions, TileWorldConfiguration.Maps.TileTypes _type, int _objectIndex)
        //{
        //    if (Application.isPlaying)
        //    {
        //        StartCoroutine(InstantiateClustersIE(_rbOptions, _type, _objectIndex));
        //    }
        //    else // Editor Method
        //    {
        //        InstantiateClusters(_rbOptions, _type, _objectIndex);
        //    }
        //}

        // Cluster build coroutine
        //IEnumerator InstantiateClustersIE(RuleBasedOptions _rbOptions, TileWorldConfiguration.Maps.TileTypes _type, int _objectIndex)
        //{
        //    float _clusterSize = (float)creator.configuration.global.clusterSize;
        //    if (_clusterSize == 0)
        //    {
        //        _clusterSize = 1;
        //    }

        //    float xSizeF = Mathf.Floor(creator.configuration.global.width / ((float)creator.configuration.global.width / _clusterSize));
        //    float ySizeF = Mathf.Floor(creator.configuration.global.height / ((float)creator.configuration.global.height / _clusterSize));

        //    int xSize = (int)xSizeF;
        //    int ySize = (int)ySizeF;

        //    int xStep = 0;
        //    int yStep = 0;

        //    int index = 0;

        //    GameObject _container = null;

        //    for (int s = 0; s < Mathf.Ceil((float)creator.configuration.global.width / _clusterSize) * Mathf.Ceil((float)creator.configuration.global.height / _clusterSize); s++)
        //    {
        //        if (clusterContainers.Count != Mathf.Ceil((float)creator.configuration.global.width / _clusterSize) * Mathf.Ceil((float)creator.configuration.global.height / _clusterSize))
        //        {
        //            _container = new GameObject();
        //            _container.transform.localScale = new Vector3(1, 1, 1);
        //            _container.name = "cluster_" + s.ToString();
        //            clusterContainers.Add(_container);
        //        }

        //        for (int y = 0 + (ySize * yStep); y < ySize + (ySize * yStep); y++)
        //        {
        //            for (int x = 0 + (xSize * xStep); x < xSize + (xSize * xStep); x++)
        //            {
        //                if (_rbOptions == RuleBasedOptions.pattern)
        //                {
        //                    InstantiateRuleBased(x, y, s, _type, _objectIndex);
        //                }
        //                else
        //                {
        //                    InstantiateRandomBased(x, y, s, _objectIndex);
        //                }
        //            }
        //        }

        //        if (index >= (creator.configuration.global.width / _clusterSize) - 1)
        //        {
        //            yStep++;
        //            xStep = 0;
        //            index = 0;
        //        }
        //        else
        //        {
        //            xStep++;
        //            index++;
        //        }

        //        yield return null;
        //    }

        //}

        //void InstantiateClusters(RuleBasedOptions _rbOptions, TileWorldConfiguration.Maps.TileTypes _type, int _objectIndex)
        //{
        //    float _clusterSize = (float)creator.configuration.global.clusterSize;
        //    if (_clusterSize == 0)
        //    {
        //        _clusterSize = 1;
        //    }

        //    float xSizeF = Mathf.Floor(creator.configuration.global.width / ((float)creator.configuration.global.width / _clusterSize));
        //    float ySizeF = Mathf.Floor(creator.configuration.global.height / ((float)creator.configuration.global.height / _clusterSize));

        //    int xSize = (int)xSizeF;
        //    int ySize = (int)ySizeF;

        //    int xStep = 0;
        //    int yStep = 0;

        //    int index = 0;

        //    GameObject _container = null;

        //    for (int s = 0; s < Mathf.Ceil((float)creator.configuration.global.width / _clusterSize) * Mathf.Ceil((float)creator.configuration.global.height / _clusterSize); s++)
        //    {
        //        if (clusterContainers.Count != Mathf.Ceil((float)creator.configuration.global.width / _clusterSize) * Mathf.Ceil((float)creator.configuration.global.height / _clusterSize))
        //        {
        //            _container = new GameObject();
        //            _container.transform.localScale = new Vector3(1, 1, 1);
        //            _container.name = "cluster_" + s.ToString();
        //            clusterContainers.Add(_container);
        //        }

        //        for (int y = 0 + (ySize * yStep); y < ySize + (ySize * yStep); y++)
        //        {
        //            for (int x = 0 + (xSize * xStep); x < xSize + (xSize * xStep); x++)
        //            {
        //                if (_rbOptions == RuleBasedOptions.pattern)
        //                {
        //                    InstantiateRuleBased(x, y, s, _type, _objectIndex);
        //                }
        //                else
        //                {
        //                    InstantiateRandomBased(x, y, s, _objectIndex);
        //                }
        //            }
        //        }

        //        if (index >= (creator.configuration.global.width / _clusterSize) - 1)
        //        {
        //            yStep++;
        //            xStep = 0;
        //            index = 0;
        //        }
        //        else
        //        {
        //            xStep++;
        //            index++;
        //        }
        //    }
        //}

        void InstantiateProcedural(TileWorldConfiguration.TileInformation.TileTypes _type, int _index) //(int _x, int _y, int _s, TileWorldConfiguration.Maps.TileTypes _type, int _index)
        {

            int _selectedLayer = configuration.proceduralObjects[_index].selectedLayer;

            // if block or ground tile is selected a temp map has to be created
            // to modify the map with an inset.
            if (_type == TileWorldConfiguration.TileInformation.TileTypes.block || _type == TileWorldConfiguration.TileInformation.TileTypes.ground)
            {
                
                bool[,] _tmpMap = new bool[creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0), creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1)];
                bool[,] _newMap = new bool[creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0), creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1)];

                // Assign current world map to newmap
                for (int x = 0; x < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0); x++)
                {
                    for (int y = 0; y < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1); y++)
                    {
                        _newMap[x, y] = creator.configuration.worldMap[_selectedLayer].cellMap[x, y];
                    }
                }

                for (int i = 0; i < configuration.proceduralObjects[_index].inset + 1; i++) // add one inset more to skip the border edges
                {
                    for (int x = 0; x < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0); x++)
                    {
                        for (int y = 0; y < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1); y++)
                        {
                            if (creator.configuration.global.invert)
                            { 
                                if (_type == TileWorldConfiguration.TileInformation.TileTypes.block)
                                {
                                    int _itc = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_newMap, x, y, 1, true);// creator.configuration.global.invert);

                                    if (_itc == 8)
                                    {
                                        _tmpMap[x, y] = true;// creator.configuration.global.invert; //invert true = true
                                    }
                                    else
                                    {
                                        _tmpMap[x, y] = false;// !creator.configuration.global.invert; //invert true = false
                                    }
                                }
                                else if (_type == TileWorldConfiguration.TileInformation.TileTypes.ground)
                                {
                                    int _itc = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_newMap, x, y, 1, false);

                                    if (_itc == 8)
                                    {
                                        _tmpMap[x, y] = false;// !creator.configuration.global.invert; // invert true = false
                                    }
                                    else
                                    {
                                        _tmpMap[x, y] = true;// creator.configuration.global.invert; //invert true = true
                                    }
                                }
                            }
                            else
                            {
                                if (_type == TileWorldConfiguration.TileInformation.TileTypes.block)
                                {
                                    int _itc = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_newMap, x, y, 1, false);// creator.configuration.global.invert);

                                    if (_itc == 8)
                                    {
                                        _tmpMap[x, y] = true;// creator.configuration.global.invert; //invert true = true
                                    }
                                    else
                                    {
                                        _tmpMap[x, y] = false;// !creator.configuration.global.invert; //invert true = false
                                    }
                                }
                                else if (_type == TileWorldConfiguration.TileInformation.TileTypes.ground)
                                {
                                    int _itc = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_newMap, x, y, 1, true);

                                    if (_itc == 8)
                                    {
                                        _tmpMap[x, y] = false;// !creator.configuration.global.invert; // invert true = false
                                    }
                                    else
                                    {
                                        _tmpMap[x, y] = true;// creator.configuration.global.invert; //invert true = true
                                    }
                                }
                            }
                        }
                    }

                    // Add modified map back to newmap so we cann add another inset based on the modified map
                    for (int x = 0; x < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0); x++)
                    {
                        for (int y = 0; y < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1); y++)
                        {
                            _newMap[x, y] = _tmpMap[x, y];
                        }
                    }
                }

                for (int x = 0; x < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0); x++)
                {
                    for (int y = 0; y < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1); y++)
                    {
                        //if (creator.configuration.worldMap[_selectedLayer].tiletype[x, y] == _type)
                        //{
                        if (_type == TileWorldConfiguration.TileInformation.TileTypes.block)
                        {
                            // pattern based
                            if (configuration.proceduralObjects[_index].ruleType == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.RuleTypes.pattern)
                            {
                                if (_tmpMap[x, y] && (x + y) % configuration.proceduralObjects[_index].everyNTile == 0)
                                {
                                    var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);

                                    InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, proceduralObjectsContainer);
                                }
                            }
                            else // random based
                            {
                                float _rnd = Random.Range(0f, 1f);
                                if (_tmpMap[x, y] && configuration.proceduralObjects[_index].weight > _rnd)
                                {
                                    var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);
                                    InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, proceduralObjectsContainer);
                                }
                            }
                        }
                        else
                        {
                            // pattern based
                            if (configuration.proceduralObjects[_index].ruleType == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.RuleTypes.pattern)
                            {
                                if (!_tmpMap[x, y] && (x + y) % configuration.proceduralObjects[_index].everyNTile == 0)
                                {
                                    var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);

                                    InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, proceduralObjectsContainer);
                                }
                            }
                            else // random based
                            {
                                float _rnd = Random.Range(0f, 1f);
                                if (!_tmpMap[x, y] && configuration.proceduralObjects[_index].weight > _rnd)
                                {
                                    var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);
                                    InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, proceduralObjectsContainer);
                                }
                            }
                        }
                    }
                }

            }
            else // instantiate objects based on their tile rotation
            {
                for (int x = 0; x < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0); x++)
                {
                    for (int y = 0; y < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1); y++)
                    {
                        if (creator.configuration.worldMap[_selectedLayer].tileTypes[x, y] == _type)
                        {
                            if (configuration.proceduralObjects[_index].ruleType == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.RuleTypes.pattern)
                            {
                                if ((x + y) % configuration.proceduralObjects[_index].everyNTile == 0) // && y % proceduralObjects[_index].everyNTile == 0)
                                {
                                    if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y] != null)
                                    {
                                        InsetCalculations(x, y, _index, _selectedLayer);
                                    }
                                }
                            }
                            else // Random based
                            {
                                var _check = creator.configuration.global.invert;
                                if (configuration.proceduralObjects[_index].tileTypes == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.ground)
                                {
                                    _check = !creator.configuration.global.invert;
                                }
                                if (creator.configuration.worldMap[_selectedLayer].cellMap[x, y] == _check && creator.configuration.worldMap[_selectedLayer].tileTypes[x, y] == _type)
                                {
                                    float _rnd = Random.Range(0f, 1f);
                                    if (configuration.proceduralObjects[_index].weight > _rnd)
                                    {
                                        //var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);
                                        //InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, rulebasedObjectsContainer);
                                        InsetCalculations(x, y, _index, _selectedLayer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Do some inset calculations if tile type is not water or terrain
        void InsetCalculations(int x, int y, int _index, int _selectedLayer)
        {
            var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);

            // Add inset position based on selected tiletype and tile rotation
            switch (configuration.proceduralObjects[_index].tileTypes)
            {
                case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.corner:

                    // remove the added offset rotation from the selected preset
                    var _offsetRotationCorner = new Vector3(0, creator.configuration.presets[creator.configuration.global.layerPresetIndex[_selectedLayer]].tiles[0].yRotationOffset[1], 0);

                    if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, 0, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, 360, 0))
                    {
                        _pos += new Vector3(configuration.proceduralObjects[_index].inset, 0, configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, 270, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, -90, 0))
                    {
                        _pos += new Vector3(-configuration.proceduralObjects[_index].inset, 0, configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, 180, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, -180, 0))
                    {
                        _pos += new Vector3(-configuration.proceduralObjects[_index].inset, 0, -configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationCorner == new Vector3(0, 90, 0))
                    {
                        _pos += new Vector3(configuration.proceduralObjects[_index].inset, 0, -configuration.proceduralObjects[_index].inset);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.edge:

                    // remove the added offset rotation from the selected preset
                    var _offsetRotationEdge = new Vector3(0, creator.configuration.presets[creator.configuration.global.layerPresetIndex[_selectedLayer]].tiles[0].yRotationOffset[0], 0);

                    if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationEdge == new Vector3(0, 0, 0))
                    {
                        _pos += new Vector3(0, 0, configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationEdge == new Vector3(0, -90, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationEdge == new Vector3(0, 270, 0))
                    {
                        _pos += new Vector3(-configuration.proceduralObjects[_index].inset, 0, 0);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationEdge == new Vector3(0, 90, 0))
                    {
                        _pos += new Vector3(configuration.proceduralObjects[_index].inset, 0, 0);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationEdge == new Vector3(0, -180, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationEdge == new Vector3(0, 180, 0))
                    {
                        _pos += new Vector3(0, 0, -configuration.proceduralObjects[_index].inset);
                    }

                    break;
                case TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.invertedCorner:

                    // remove the added offset rotation from the selected preset
                    var _offsetRotationInvertedCorner = new Vector3(0, creator.configuration.presets[creator.configuration.global.layerPresetIndex[_selectedLayer]].tiles[0].yRotationOffset[2], 0);

                    if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, 0, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, 360, 0))
                    {
                        _pos += new Vector3(-configuration.proceduralObjects[_index].inset, 0, -configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, 270, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, -90, 0))
                    {
                        _pos += new Vector3(configuration.proceduralObjects[_index].inset, 0, -configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, 90, 0))
                    {
                        _pos += new Vector3(-configuration.proceduralObjects[_index].inset, 0, configuration.proceduralObjects[_index].inset);
                    }
                    else if (creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, 180, 0)
                        || creator.configuration.worldMap[_selectedLayer].tileObjects[x, y].transform.localEulerAngles - _offsetRotationInvertedCorner == new Vector3(0, -180, 0))
                    {
                        _pos += new Vector3(configuration.proceduralObjects[_index].inset, 0, configuration.proceduralObjects[_index].inset);
                    }

                    break;
            }

            InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, proceduralObjectsContainer);
        }


        void InstantiateRandomBased(TileWorldConfiguration.TileInformation.TileTypes _type, int _index)
        {
            int _selectedLayer = configuration.proceduralObjects[_index].selectedLayer;

            for (int x = 0; x < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(0); x++)
            {
                for (int y = 0; y < creator.configuration.worldMap[_selectedLayer].cellMap.GetLength(1); y++)
                {
                    var _check = creator.configuration.global.invert;
                    if (configuration.proceduralObjects[_index].tileTypes == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes.ground)
                    {
                        _check = !creator.configuration.global.invert;
                    }
                    if (creator.configuration.worldMap[_selectedLayer].cellMap[x, y] == _check && creator.configuration.worldMap[_selectedLayer].tileTypes[x, y] == _type)
                    {
                        float _rnd = Random.Range(0f, 1f);
                        if (configuration.proceduralObjects[_index].weight > _rnd)
                        {
                            var _pos = new Vector3(x + 0.5f, 0, y + 0.5f);
                            InstantiateObjects(configuration.proceduralObjects[_index], _pos, _index, x, y, SpawnTypes.procedural, proceduralObjectsContainer);
                        }
                    }
                }
            }
        }

        #endregion scatterrulebasedobjects

        #region paintobjects
        /// <summary>
        /// Paint objects. Instantiate all objects based on user paint position
        /// </summary>
        /// <param name="_pos"></param>
        public void PaintObjects(Vector3 _pos)
        {
            if (objectScatterContainer == null)
            {
                objectScatterContainer = new GameObject(creator.configuration.global.worldName + "_Objects");
            }

            objectScatterContainer.transform.rotation = Quaternion.identity;

            if (paintObjectsContainer == null)
            {
                paintObjectsContainer = new GameObject("paintObjects");
                paintObjectsContainer.transform.parent = objectScatterContainer.transform;
            }


            var _x = (int)_pos.x;
            var _y = (int)_pos.z;

            _pos += new Vector3(0.5f, 0, 0.5f);

            if (_pos.x < 0 || _pos.z < 0)
                return;

            for (int i = 0; i < configuration.paintObjects.Count; i ++)
            {
                if (configuration.paintObjects[i].paintThisObject)
                {
                    InstantiateObjects(configuration.paintObjects[i], _pos, i, _x, _y, SpawnTypes.paint, paintObjectsContainer);
                }
            }

            //set object world container rotation according to map orientation
            if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xy)
            {
                objectScatterContainer.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            }
        }

        #endregion paintobjects



        // instantiate parent objects
        void InstantiateObjects(TileWorldObjectScatterConfiguration.DefaultObjectConfiguration _class, Vector3 _pos, int _index, int _arrayX, int _arrayY, SpawnTypes _spawnTypes, GameObject _container)
        {
            // check if theres another layer above
            if (_spawnTypes != SpawnTypes.paint && _class.selectedLayer < creator.configuration.global.layerCount - 1)
            {
                if (creator.configuration.worldMap[_class.selectedLayer + 1].cellMap[_arrayX, _arrayY] == creator.configuration.global.invert)
                {
                    return;
                }
            }

            // check if tile is already occupied by another object
            if (_spawnTypes != SpawnTypes.paint && _spawnTypes != SpawnTypes.position)
            {
                if (occupyMap[_arrayX, _arrayY] && !_class.placeOnOccupiedCell)
                {
                    return;
                }

                occupyMap[_arrayX, _arrayY] = true;
            }

            // add prefab transform position and offset position
            _pos = new Vector3(((_pos.x  + _class.offsetPosition.x) * creator.configuration.global.globalScale) + this.transform.position.x, ((_pos.y + _class.offsetPosition.y) * creator.configuration.global.globalScale) + this.transform.position.y, ((_pos.z + _class.offsetPosition.z) * creator.configuration.global.globalScale) + this.transform.position.z);

    
            // Rotations
            Quaternion _qt = Quaternion.identity;
            
            if (_class.useTileRotation)
            {
                // use rotation from tile game object
                if (creator.configuration.worldMap[_class.selectedLayer].tileObjects[_arrayX, _arrayY] != null)
                {
                    _qt = creator.configuration.worldMap[_class.selectedLayer].tileObjects[_arrayX, _arrayY].transform.localRotation;
                }
                else
                {
                    _qt = _class.go.transform.rotation;
                }
            }
            else if (_class.useRandomRotation)
            {
                
                float _rndRotX = Random.Range(_class.randomRotationMin.x, _class.randomRotationMax.x);
                float _rndRotY = Random.Range(_class.randomRotationMin.y, _class.randomRotationMax.y);
                float _rndRotZ = Random.Range(_class.randomRotationMin.z, _class.randomRotationMax.z);

                // random rotation
                _qt = Quaternion.Euler(new Vector3(_rndRotX, _rndRotY, _rndRotZ));
            }

            // Add offset rotation
            Quaternion _offsetRotation = Quaternion.Euler(_class.offsetRotation);
            Quaternion _newRotation = _qt * _offsetRotation;

            GameObject _go = Instantiate(_class.go, _pos, _class.go.transform.rotation * _newRotation) as GameObject;

            _go.transform.position = _pos;

            // Add random scaling
            if (_class.useRandomScaling)
            {
                // Random scaling
                float _rndScaleX = 1;
                float _rndScaleY = 1;
                float _rndScaleZ = 1;

                if (_class.uniformScaling)
                {
                    _rndScaleX = Random.Range(_class.randomScalingMin.x, _class.randomScalingMax.x);
                    _rndScaleY = _rndScaleX;
                    _rndScaleZ = _rndScaleX;
                }
                else
                {
                    _rndScaleX = Random.Range(_class.randomScalingMin.x, _class.randomScalingMax.x);
                    _rndScaleY = Random.Range(_class.randomScalingMin.y, _class.randomScalingMax.y);
                    _rndScaleZ = Random.Range(_class.randomScalingMin.z, _class.randomScalingMax.z);
                }

                // add random scaling
                _go.transform.localScale = new Vector3(_rndScaleX, _rndScaleY, _rndScaleZ);
            }

            //parent object
            //if (_s > -1)
            //{
            //    _go.transform.parent = clusterContainers[_s].transform; // _container.transform;
            //}
            //else
            //{
            _go.transform.parent = _container.transform;
            //}

            // Instantiate child objects of parent object
            switch (_spawnTypes)
            {
                case SpawnTypes.paint:

                    for (int i = _index + 1; i < configuration.paintObjects.Count; i++)
                    {
                        if (configuration.paintObjects[i].isChild && configuration.paintObjects[i].active)
                        {
                            InstantiateChildObjects(configuration.paintObjects[i], _pos, SpawnTypes.paint);
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                case SpawnTypes.procedural:

                    for (int i = _index + 1; i < configuration.proceduralObjects.Count; i++)
                    {
                        if (configuration.proceduralObjects[i].isChild && configuration.proceduralObjects[i].active)
                        {
                            InstantiateChildObjects(configuration.proceduralObjects[i], _pos, SpawnTypes.procedural);
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                case SpawnTypes.position:

                    for (int i = _index + 1; i < configuration.positionObjects.Count; i++)
                    {
                        if (configuration.positionObjects[i].isChild && configuration.positionObjects[i].active)
                        {

                            InstantiateChildObjects(configuration.positionObjects[i], _pos, SpawnTypes.position);
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
            }

         
        }

        // instantiate child objects
        void InstantiateChildObjects(TileWorldObjectScatterConfiguration.DefaultObjectConfiguration _class, Vector3 _pos, SpawnTypes _spawnType)
        {
            
            for (int i = 0; i < _class.spawnCount; i++)
            {

                Quaternion _qt = Quaternion.identity;

                if (_class.useRandomRotation)
                {
                    float _rndRotX = Random.Range(_class.randomRotationMin.x, _class.randomRotationMax.x);
                    float _rndRotY = Random.Range(_class.randomRotationMin.y, _class.randomRotationMax.y);
                    float _rndRotZ = Random.Range(_class.randomRotationMin.z, _class.randomRotationMax.z);

                    _qt = Quaternion.Euler(new Vector3(_rndRotX, _rndRotY, _rndRotZ));
                }

                // Instantiate children based on parent position
                GameObject _inst = Instantiate(_class.go, Random.insideUnitSphere * _class.radius + _pos, _class.go.transform.rotation * _qt) as GameObject;
                //add offset position
                _inst.transform.position = new Vector3((_inst.transform.position.x + _class.offsetPosition.x), (_pos.y + _class.offsetPosition.y), (_inst.transform.position.z + _class.offsetPosition.z));

                // Random scaling
                if (_class.useRandomScaling)
                {
                    float _rndScaleX = 1f;
                    float _rndScaleY = 1f;
                    float _rndScaleZ = 1f;

                    if (_class.uniformScaling)
                    {
                        _rndScaleX = Random.Range(_class.randomScalingMin.x, _class.randomScalingMax.x);
                        _rndScaleY = _rndScaleX;
                        _rndScaleZ = _rndScaleX;
                    }
                    else
                    {
                        _rndScaleX = Random.Range(_class.randomScalingMin.x, _class.randomScalingMax.x);
                        _rndScaleY = Random.Range(_class.randomScalingMin.y, _class.randomScalingMax.y);
                        _rndScaleZ = Random.Range(_class.randomScalingMin.z, _class.randomScalingMax.z);
                    }

                    // add random scaling
                    _inst.transform.localScale = new Vector3(_rndScaleX, _rndScaleY, _rndScaleZ);

                }

                // Parent
               switch (_spawnType)
                {
                    case SpawnTypes.paint:
                        _inst.transform.parent = paintObjectsContainer.transform;
                        break;
                    case SpawnTypes.procedural:
                        _inst.transform.parent = proceduralObjectsContainer.transform;
                        break;
                    case SpawnTypes.position:
                        _inst.transform.parent = positionObjectsContainer.transform;
                        break;
                }
                
            }
        }


        public string[] GetLayers()
        {
            if (creator == null)
            {
                creator = this.GetComponent<TileWorldCreator>();
            }
           if (creator.configuration == null)
               return null;

            string[] _placeOnLayer = new string[creator.configuration.global.layerCount];
            for (int i = 0; i < creator.configuration.global.layerCount; i++)
            {
                _placeOnLayer[i] = "layer: " + (i + 1);
            }

            return _placeOnLayer;
        }


        // Draw brush and paint grid
        //--------------------------
        void OnDrawGizmosSelected()
        {
            if (showGrid)
            {
                DrawGrid();
                DrawBrush();
            }
        }

        void DrawGrid()
        {
            if (creator.configuration == null)
                return;

            Vector3 pos = Vector3.zero;
            if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
            {
                pos = new Vector3(transform.position.x, (transform.position.y + creator.configuration.global.globalScale + (creator.configuration.ui.mapIndex * creator.configuration.global.globalScale) + creator.configuration.presets[creator.configuration.global.layerPresetIndex[creator.configuration.ui.mapIndex]].tiles[0].blockOffset.y), transform.position.z);
            }
            else
            {
                pos = new Vector3(transform.position.x, (transform.position.y - creator.configuration.global.globalScale - (creator.configuration.ui.mapIndex * creator.configuration.global.globalScale) - creator.configuration.presets[creator.configuration.global.layerPresetIndex[creator.configuration.ui.mapIndex]].tiles[0].blockOffset.y), transform.position.z);
            }

            //draw grid
            for (float z = 0; z <= (float)creator.configuration.global.height * creator.configuration.global.globalScale; z += 1 * creator.configuration.global.globalScale)
            {

                Gizmos.color = creator.configuration.ui.gridColor;

             

                if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                {
                    Gizmos.DrawLine(new Vector3(pos.x, pos.y, pos.z + z),
                        new Vector3(pos.x + creator.configuration.global.width * creator.configuration.global.globalScale, pos.y, pos.z + z));
                }
                else
                {

                    Gizmos.DrawLine(new Vector3(pos.x, pos.z + z, pos.y),
                        new Vector3(pos.x + creator.configuration.global.width * creator.configuration.global.globalScale, pos.z + z, pos.y));
                }
            }

            for (float x = 0; x <= (float)creator.configuration.global.width * creator.configuration.global.globalScale; x += 1 * creator.configuration.global.globalScale)
            {

                Gizmos.color = creator.configuration.ui.gridColor;


                if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                {
                    Gizmos.DrawLine(new Vector3(pos.x + x, pos.y, pos.z),
                    new Vector3(pos.x + x, pos.y, pos.z + creator.configuration.global.height * creator.configuration.global.globalScale));
                }
                else
                {
                    Gizmos.DrawLine(new Vector3(pos.x + x, pos.z, pos.y),
                        new Vector3(pos.x + x, pos.z + creator.configuration.global.height * creator.configuration.global.globalScale, pos.y));
                }
            }
        }

        void DrawBrush()
        {
            if (mouseOverGrid)
            {
                Gizmos.color = creator.configuration.ui.brushColor;

                for (int y = 0; y < 1; y++)
                {
                    for (int x = 0; x < 1; x++)
                    {

                        Vector3 _pos = new Vector3(0, 0, 0);

                        if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
                        {
                            _pos = new Vector3((Mathf.Floor(creator.mouseWorldPosition.x / creator.configuration.global.globalScale) + 0.5f + x) * creator.configuration.global.globalScale, (transform.position.y + (1.05f * creator.configuration.global.globalScale) + creator.configuration.ui.mapIndex) + creator.configuration.presets[creator.configuration.global.layerPresetIndex[creator.configuration.ui.mapIndex]].tiles[0].blockOffset.y, (Mathf.Floor(creator.mouseWorldPosition.z / creator.configuration.global.globalScale) + 0.5f + y) * creator.configuration.global.globalScale);

                            if (_pos.x > 0 + transform.position.x && _pos.z > 0 + transform.position.z && _pos.x < transform.position.x + (creator.configuration.global.width * creator.configuration.global.globalScale) && _pos.z < transform.position.z + (creator.configuration.global.height * creator.configuration.global.globalScale))
                            {
                                Gizmos.DrawCube(_pos, new Vector3(1 * creator.configuration.global.globalScale, 0.05f, 1 * creator.configuration.global.globalScale));
                            }
                        }
                        else
                        {
                            _pos = new Vector3((Mathf.Floor(creator.mouseWorldPosition.x / creator.configuration.global.globalScale) + 0.5f + x) * creator.configuration.global.globalScale, (Mathf.Floor(creator.mouseWorldPosition.y / creator.configuration.global.globalScale) + 0.5f + y) * creator.configuration.global.globalScale, (transform.position.z - (1.05f * creator.configuration.global.globalScale) - creator.configuration.ui.mapIndex) - creator.configuration.presets[creator.configuration.global.layerPresetIndex[creator.configuration.ui.mapIndex]].tiles[0].blockOffset.y);

                            if (_pos.x > 0 + transform.position.x && _pos.y > 0 + transform.position.y && _pos.x < transform.position.x + (creator.configuration.global.width * creator.configuration.global.globalScale) && _pos.y < transform.position.y + (creator.configuration.global.height * creator.configuration.global.globalScale))
                            {
                                Gizmos.DrawCube(_pos, new Vector3(1 * creator.configuration.global.globalScale, 1 * creator.configuration.global.globalScale, 0.05f));
                            }
                        }
                    }
                }
            }
        }
    }
}
