/* TILEWORLDCREATOR CellularAlgorithm
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 *
 * Generates a map based on the cellular algorithm
 *
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileWorld;


public class CellularAlgorithm : TileWorldAlgorithms, IAlgorithms {


    int width, height;
    float chanceToStartAlive;
    bool invert;
    Vector3 startPos;
    Vector3 endPos;

    // cellular return map
    bool[,] cellularMap;
    // temporary fill maps
    bool[,] fillmap = new bool[0, 0];
    bool[,] fillWaterMap = new bool[0, 0];

    [System.Serializable]
    public class Island
    {
        public List<int> x = new List<int>();
        public List<int> y = new List<int>();
        public int cellCount;

        //store coordinates
        public Island(int _x, int _y)
        {
            x.Add(_x);
            y.Add(_y);
            cellCount = 0;
        }
    }

    public List<Island> islands = new List<Island>();
    public List<Island> waterIslands = new List<Island>();
    public int bigIslandIndex = 0;

    /// <summary>
    /// Implemented interface IAlgorithms
    /// </summary>
    public bool[,] Generate(TileWorldConfiguration _config, out Vector3 _startPosition, out Vector3 _endPosition)
    {
        //we do not set any start or end position for cellular map
        _startPosition = new Vector2(0, 0);
        _endPosition = new Vector2(0, 0);

        //assign properties from config
        width = _config.global.width;
        height = _config.global.height;
        invert = _config.global.invert;
        chanceToStartAlive = _config.cellularAlgorithm.chanceToStartAlive;

        cellularMap = new bool[width, height];

        cellularMap = initialiseMap(cellularMap);


        //run simulation step for number of steps
        //more steps results in smoother worlds
        for (int i = 0; i < _config.cellularAlgorithm.numberOfSteps; i++)
        {
            cellularMap = DoSimulationStep(cellularMap, _config);
        }

        // flood all smaller islands
        if (_config.cellularAlgorithm.floodUnjoined && (_config.global.width <= 150 || _config.global.height <= 150))
        {
            //count how many caverns our world has
            CountCaverns(cellularMap);

            cellularMap = FloodFill(cellularMap);
        }

        //flood holes floods all smaller holes inside of a map
        if (_config.cellularAlgorithm.floodHoles && (_config.global.width <= 150 || _config.global.height <= 150))
        {
            CountWater(cellularMap);

            cellularMap = FloodFillWater(cellularMap);
        }


        //check if biggest island is big enough.
        //if not, regenerate map
        //only do this when the random seed is set to -1 so we don't get
        //a stackoverflow if a random seed generates a map who does not meet the min cell count.
        bool _mapSizeOK = false;
        //if (_config.global.randomSeed < 0)
        //{
            if (islands.Count > 0)
            {
                for (int i = 0; i < islands.Count; i++)
                {
                    if (islands[i].cellCount >= _config.cellularAlgorithm.minCellCount)
                    {
                        _mapSizeOK = true;
                    }
                }
            }
        //}

        if (!_mapSizeOK)
        {
            //Debug.Log("size to small");
            Generate(_config, out _startPosition, out _endPosition);
        }

        return cellularMap;
    }


    //Initialize Map
    //--------------
    bool[,] initialiseMap(bool[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float _rnd = Random.Range(0.0f, 1.0f);
                if (_rnd < chanceToStartAlive)
                {
                    map[x, y] = true;
                }
                else
                {
                    //in isle mode we have to make sure that the border is always a floor tile.
                    if (!invert)
                    {
                        if (((x < width) && (y == 0 || y == height - 1)) || ((x == 0 || x == width - 1) && (y < height)))
                        {
                            map[x, y] = true;
                        }
                    }
                }
            }
        }

        return map;
    }


    //Do Simulation steps
    //-------------------
    bool[,] DoSimulationStep(bool[,] oldMap, TileWorldConfiguration _config)
    {
        bool[,] newMap = new bool[_config.global.width, _config.global.height];

        //Loop over each row and column of the map
        for (int x = 0; x < oldMap.GetLength(0); x++)
        {
            for (int y = 0; y < oldMap.GetLength(1); y++)
            {
                int nbs = TileWorldNeighbourCounter.CountAllNeighbours(oldMap, x, y, 1, true);
                //The new value is based on our simulation rules
                //First, if a cell is alive but has too few neighbours, kill it.
                if (oldMap[x, y])
                {
                    if (nbs < _config.cellularAlgorithm.deathLimit)
                    {
                        newMap[x, y] = false;
                    }
                    else
                    {
                        newMap[x, y] = true;
                    }
                    //make sure the cavern is always closed on the border edges
                    if (_config.global.invert)
                    {
                        if (x < 1 || x >= _config.global.width - 1 || y >= _config.global.height - 1 || y < 1)
                        {
                            newMap[x, y] = true;
                        }
                    }


                }
                //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                else
                {
                    if (nbs > _config.cellularAlgorithm.birthLimit)
                    {
                        newMap[x, y] = true;
                    }
                    else
                    {
                        newMap[x, y] = false;
                    }
                    //make sure the cavern is always closed on the border edges
                    if (_config.global.invert)
                    {
                        if (x < 1 || x >= _config.global.width - 1 || y >= _config.global.height - 1 || y < 1)
                        {
                            newMap[x, y] = true;
                        }
                    }
                }
            }
        }

        return newMap;

    }



    //COUNT CAVERNS
    //-------------
    //for counting the caverns we have to fill each cavern
    //first we generate a new fillmap with the same content of the configuration.worldMap
    //for each x and y length of the configuration.worldMap we call the 
    //FillCavern method which fills each cell.
    void CountCaverns(bool[,] _map)
    {
        fillmap = new bool[_map.GetLength(0), _map.GetLength(1)];
        islands = new List<Island>();
        for (int cmy = 0; cmy < _map.GetLength(1); cmy++)
        {
            for (int cmx = 0; cmx < _map.GetLength(0); cmx++)
            {
                fillmap[cmx, cmy] = _map[cmx, cmy];
            }
        }



        for (int y = 0; y < _map.GetLength(1); y++)
        {
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                if (fillmap[x, y] == false)
                {
                    //ad new cavern
                    islands.Add(new Island(x, y));
                    //fill cell as long as neighbour
                    //is not filled
                    FillCavern(x, y);

                }
            }
        }
    }
    //-END Count Caverns


    //FillCavern
    //----------
    //this is where flood fill actually happens
    void FillCavern(int _x, int _y)
    {
        if (fillmap[_x, _y] != false)
        {
            return;
        }

        fillmap[_x, _y] = true;
        islands[islands.Count - 1].x.Add(_x);
        islands[islands.Count - 1].y.Add(_y);
        islands[islands.Count - 1].cellCount++;

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {

                int neighbour_x = _x + x;
                int neighbour_y = _y + y;

                if (y == 0 && x == 0)
                {
                    fillmap[_x, _y] = true;
                }
                //off the edge of the map
                else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= fillmap.GetLength(0) || neighbour_y >= fillmap.GetLength(1))
                {

                }
                //normal check of the neighbour
                else if (!fillmap[neighbour_x, neighbour_y])
                {

                    FillCavern(neighbour_x, neighbour_y);

                }

            }
        }
    }
    //-END Fill Cavern

    //FloodFill
    //---------
    //get biggest cavern from caverns list
    //flood all caverns in cellmap as long as it is not
    //the biggest cavern
    bool[,] FloodFill(bool[,] _map)
    {
        //get biggest cave
        int _bigCavernInx = 0;
        //int _count = 0;
        //int _oldCount = 0;
        int _cellCount = 0;

        //for (int c = 0; c < islands.Count; c++)
        //{
        //    _count = islands[c].x.Count;

        //    if (_count > _oldCount)
        //    {
        //        _bigCavernInx = c;
        //        _oldCount = _count;
        //    }
        //    else
        //    {
        //        _count = _oldCount;
        //    }
        //}

        for (int a = 0; a < islands.Count; a++)
        {
            if (islands[a].cellCount > _cellCount)
            {
                _cellCount = islands[a].cellCount;
                _bigCavernInx = a;
            }
        }

        for (int s = 0; s < islands.Count; s++)
        {
            if (s != _bigCavernInx)
            {
                for (int m = 0; m < islands[s].x.Count; m++)
                {
                    _map[islands[s].x[m], islands[s].y[m]] = true;
                }
            }
        }



        bigIslandIndex = _bigCavernInx;
        //yield return null; 

        return _map;
    }
    //-END Flood Fill


    //flood water holes
    //-----------------
    void CountWater(bool[,] _map)
    {
        fillWaterMap = new bool[_map.GetLength(0), _map.GetLength(1)];
        waterIslands = new List<Island>();
        for (int cmy = 0; cmy < _map.GetLength(1); cmy++)
        {
            for (int cmx = 0; cmx < _map.GetLength(0); cmx++)
            {
                fillWaterMap[cmx, cmy] = _map[cmx, cmy];
            }
        }



        for (int y = 0; y < _map.GetLength(1); y++)
        {
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                if (fillWaterMap[x, y] == true)
                {
                    //ad new cavern
                    waterIslands.Add(new Island(x, y));
                    //fill cell as long as neighbour
                    //is not filled
                    FillWater(x, y);

                }
            }
        }

    }

    //FillWater
    //----------
    //this is where flood fill actually happens
    void FillWater(int _x, int _y)
    {
        if (fillWaterMap[_x, _y] != true)
        {
            return;
        }

        fillWaterMap[_x, _y] = false;
        waterIslands[waterIslands.Count - 1].x.Add(_x);
        waterIslands[waterIslands.Count - 1].y.Add(_y);

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {

                int neighbour_x = _x + x;
                int neighbour_y = _y + y;

                if (y == 0 && x == 0)
                {
                    fillWaterMap[_x, _y] = false;
                }
                //off the edge of the map
                else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= fillWaterMap.GetLength(0) || neighbour_y >= fillWaterMap.GetLength(1))
                {

                }
                //normal check of the neighbour
                else if (fillWaterMap[neighbour_x, neighbour_y])
                {

                    FillWater(neighbour_x, neighbour_y);

                }

            }
        }
    }
    //-END FillWater


    bool[,] FloodFillWater(bool[,] _map)
    {
        //get biggest water cave
        int _bigCavernInx = 0;
        //int _count = 0;
        //int _oldCount = 0;
        int _cellCount = 0;

        //for (int c = 0; c < waterIslands.Count; c++)
        //{
        //    _count = waterIslands[c].x.Count;

        //    if (_count > _oldCount)
        //    {
        //        _bigCavernInx = c;
        //        _oldCount = _count;
        //    }
        //    else
        //    {
        //        _count = _oldCount;
        //    }
        //}

        for (int a = 0; a < waterIslands.Count; a ++)
        {
            if (waterIslands[a].cellCount > _cellCount)
            {
                _cellCount = waterIslands[a].cellCount;
                _bigCavernInx = a;
            }
        }

        for (int s = 0; s < waterIslands.Count; s++)
        {
            if (s != _bigCavernInx)
            {
                for (int m = 0; m < waterIslands[s].x.Count; m++)
                {
                    _map[waterIslands[s].x[m], waterIslands[s].y[m]] = false;
                }
            }
        }

        return _map;
    }
}
