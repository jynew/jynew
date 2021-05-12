/* TILEWORLDCREATOR MazeAlgorithm
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 *
 * Generates a maze based on DFS
 *
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileWorld;


public class MazeAlgorithm : TileWorldAlgorithms, IAlgorithms
{
    int height, width;
    bool linear;
    bool useRandomStartPos;
    Vector3 startPosition;
    Vector3 endPosition;

    bool[,] mazeMap;

    float lastDistance = 0;
    float dist = 0;

    public bool[,] Generate(TileWorldConfiguration _config, out Vector3 _startPosition, out Vector3 _endPosition)
    {
        // Assign properties from config
        width = _config.global.width;
        height = _config.global.height;

        linear = _config.mazeAlgorithm.linear;
        useRandomStartPos = _config.mazeAlgorithm.useRandomStartPos;

        if (!_config.mazeAlgorithm.useRandomStartPos)
        {
            startPosition = new Vector3(_config.mazeAlgorithm.startPos.x, 0, _config.mazeAlgorithm.startPos.y);
        }

        mazeMap = new bool[width, height];
        mazeMap = GenerateMaze(height, width);

        _startPosition = startPosition;

        // Find end position after generation
        var _pos = FindEndPosition(mazeMap);
        _endPosition = new Vector3(_pos.x, 0, _pos.y);    

        return mazeMap;
    }


    bool[,] GenerateMaze(int _height, int _width)
    {
        // create temp maze
        bool[,] _tmpMaze = new bool[_width, _height];

        // Fill all cells
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _tmpMaze[x, y] = false;
            }
        }

      
        int _x = 1;
        int _y = 0;
 
        if (useRandomStartPos)
        {
            _x = Random.Range(1, width - 1);
            _y = Random.Range(1, height - 1);
        }
        else
        {
            _x = (int)startPosition.x;
            _y = (int)startPosition.z;
        }

        startPosition = new Vector3(_x, 0, _y);

        // Clear start cell
        _tmpMaze[_x, _y] = true;

        //carve
        // if start position is at border of the map
        // move two cells to the inside
        if (_x == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                _tmpMaze[_x + i, _y] = true;
            }

            _x += 2;
        }
        else if (_y == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                _tmpMaze[_x, _y + i] = true;
            }

            _y += 2;
        }
        else if (_x == width - 1)
        {
            for (int i = 0; i < 2; i++)
            {
                _tmpMaze[_x - i, _y] = true;
            }

            _x -= 2;
        }
        else if (_y == height - 1)
        {
            for (int i = 0; i < 2; i++)
            {
                _tmpMaze[_x, _y - i] = true;
            }

            _y -= 2;
        }

        //create maze using DFS (Depth First Search)
        DepthFirstSearch(_tmpMaze, _x, _y);


        //return maze
        return _tmpMaze;
    }

    void DepthFirstSearch(bool[,] _maze, int r, int c)
    {
       
        //Directions
        // 1 - West
        // 2 - North
        // 3 - East
        // 4 - South
        int[] _directions = new int[] { 1, 2, 3, 4 };

        if (!linear)
        {
            Shuffle(_directions);
        }

        // Look in a random direction 3 block ahead
        for (int i = 0; i < _directions.Length; i ++)
        {
            switch(_directions[i])
            {
                case 1: // West
                    // Check whether 3 cells to the left is out of maze
                    if (r - 3 <= 1)
                        continue;

                    if (_maze[r - 3, c ] != true)
                    {  
                        _maze[r - 3, c] = true;
                        _maze[r - 2, c] = true;
                        _maze[r - 1, c] = true;

                        DepthFirstSearch(_maze, r - 3, c);
                    }
                    break;
                case 2: // North
                    // Check whether 3 cells up is out of maze
                    if (c + 3 >= height - 1)
                        continue;

                    if (_maze[r, c + 3] != true)
                    {
                        _maze[r, c + 3] = true;
                        _maze[r, c + 2] = true;
                        _maze[r, c + 1] = true;

                        DepthFirstSearch(_maze, r, c + 3);
                    }

                    break;
                case 3: // East
                    // Check whether 3 cells to the right is out of maze
                    if (r + 3 >= width - 1)
                        continue;

                    if (_maze[r + 3, c] != true)
                    {
                        _maze[r + 3, c] = true;
                        _maze[r + 2, c] = true;
                        _maze[r + 1, c] = true;

                        DepthFirstSearch(_maze, r + 3, c);
                    }
                    break;
                case 4: // South
                    // Check whether 3 cells down is out of maze
                    if (c - 3 <= 1)
                        continue;

                    if (_maze[r, c - 3] != true)
                    {
                        _maze[r, c - 3] = true;
                        _maze[r, c - 2] = true;
                        _maze[r, c - 1] = true;


                        DepthFirstSearch(_maze, r, c - 3);
                    }
                    break;
            }
        }
    }

    void Shuffle<T>(T[] _array)
    {
        for (int i = _array.Length; i > 1; i --)
        {
            // Pick random element to swap
            int j = Random.Range(0, i);
            // Swap
            T _tmp = _array[j];
            _array[j] = _array[i - 1];
            _array[i - 1] = _tmp;
        }
    }


    Vector2 FindEndPosition(bool[,] _mazeMap)
    {
        var _pos = Vector2.zero;

        lastDistance = 0;

        for (int x = 0; x < _mazeMap.GetLength(0); x ++)
        {
            for (int y = 0; y < _mazeMap.GetLength(1); y ++)
            {
                if (_mazeMap[x, y])
                {
                    var _c = TileWorldNeighbourCounter.CountCrossNeighbours(_mazeMap, x, y, 1, true, false);

                    
                    if (_c == 3)
                    {
                        // we have found an end position
                        // check the distance from this position to start position
                        dist = Vector2.Distance(startPosition, new Vector2(x, y));
                        if (dist > lastDistance)
                        {
                            _pos = new Vector2(x, y);
                            lastDistance = dist;
                        }
                    }
                }
            }
        }

        return _pos;
    }
}