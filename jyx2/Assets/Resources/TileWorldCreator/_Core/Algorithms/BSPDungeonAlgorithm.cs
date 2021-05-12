/* TILEWORLDCREATOR BSPDungeonAlgorithm
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 *
 * Generates a BSP type dungeon
 *
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileWorld;
//Disable Log warnings for assigned but not used variables 
#pragma warning disable 0219
#pragma warning disable 0168
public class BSPDungeonAlgorithm : TileWorldAlgorithms, IAlgorithms {

    public class Leaf
    {
        public int x, y, width, height;

        public Leaf parentLeaf;
        public Leaf firstChild;
        public Leaf secondChild;

        public Leaf() { }

        public Leaf (List<Leaf> _tree, List<Leaf> _parentTree, Leaf _parent, int _x, int _y, int _width, int _height)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            parentLeaf = _parent;

            var _r = Split(_tree, _parentTree, this);

            if(!_r)
            {
                _parentTree.Add(_parent);
            }
        }

        // Split current leaf into two smaller leafs
        public bool Split(List<Leaf> _tree, List<Leaf> _parentTree, Leaf _leaf)
        {
            if (_splitH)
            {
                // Split horizontally
                int _newX = (int)Random.Range((_leaf.width * 0.25f), _leaf.width - (_leaf.width * 0.25f));

                if (_leaf.width >= minWidth && _leaf.height >= minHeight)
                {
                    _splitH = false;
                    _leaf.firstChild = null;
                    _leaf.secondChild = null;
                    firstChild = new Leaf(_tree, _parentTree, _leaf, _newX + _leaf.x, _leaf.y, _leaf.width - _newX, _leaf.height);
                    secondChild = new Leaf(_tree, _parentTree, _leaf, _leaf.x, _leaf.y, _leaf.width - firstChild.width, _leaf.height);
                    return true;
                }
                else
                {
                    _tree.Add(_leaf);
                    return false;
                }
            }
            else
            {
                // Split vertically
                int _newY = (int)Random.Range((_leaf.height * 0.25f), _leaf.height - (_leaf.height * 0.25f));

                if (_leaf.width >= minWidth && _leaf.height >= minHeight)
                {
                    _splitH = true;
                    _leaf.firstChild = null;
                    _leaf.secondChild = null;
                    firstChild = new Leaf(_tree, _parentTree, _leaf, _leaf.x, _newY + _leaf.y, _leaf.width, _leaf.height - _newY);
                    secondChild = new Leaf(_tree, _parentTree, _leaf, _leaf.x, _leaf.y, _leaf.width, _leaf.height - firstChild.height);
                    return true;
                }
                else
                {

                    _tree.Add(_leaf);
                    return false;
                }
            }
        }
    }

  

    [System.Serializable]
    public class Room
    {
        public int x, y, width, height;

        public Room(int _x, int _y, int _width, int _height)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
        }
    }

    // store tree with all smallest leafs
    public List<Leaf> tree = new List<Leaf>();
    // store all parent leafs
    public List<Leaf> parentTree = new List<Leaf>();

    static bool _splitH = false;

    int mapWidth;
    int mapHeight;
    static int minWidth;
    static int minHeight;
    int columnWidth; 

    // store all new generated rooms
    public List<Room> rooms = new List<Room>();
    public List<Room> hallways = new List<Room>();


    // return values
    bool[,] dungeonMap;
    Vector3 endPosition;
    Vector3 startPosition;

    public bool[,] Generate(TileWorldConfiguration _config, out Vector3 _startPosition, out Vector3 _endPosition)
    {
        _splitH = false;

        // assign values from config
        mapWidth = _config.global.width;
        mapHeight = _config.global.height;
        minHeight = _config.bspDungeonAlgorithm.minBSPLeafHeight;
        minWidth = _config.bspDungeonAlgorithm.minBSPLeafWidth;
        columnWidth = _config.bspDungeonAlgorithm.corridorWidth;

        dungeonMap = new bool[_config.global.width, _config.global.height];

        // Generate the BSP tree
        GenerateBSPTree();

        // assign rooms to map
        for (int r = 0; r < rooms.Count; r++)
        {
            for (int x = rooms[r].x; x < rooms[r].x + rooms[r].width; x++)
            {
                for (int y = rooms[r].y; y < rooms[r].y + rooms[r].height; y++)
                {
                    dungeonMap[x, y] = true;
                }
            }
        }

        // assign hallways to map
        for (int h = 0; h < hallways.Count; h++)
        {
            for (int x = hallways[h].x; x < hallways[h].x + hallways[h].width; x++)
            {
                for (int y = hallways[h].y; y < hallways[h].y + hallways[h].height; y++)
                {
                    dungeonMap[x, y] = true;
                }
            }
        }

        // clean up borders
        for (int x = 0; x < mapWidth; x ++)
        {
            for (int y = 0; y < mapHeight; y ++)
            {
                if (x == 0 || x == 1 || x == mapWidth - 1 || x == mapWidth - 2 || y == 0 || y == 1 || y == mapHeight - 1 || y == mapHeight -2)
                {
                    dungeonMap[x, y] = false;
                }
            }
        }

        // assign start and end position
        // get random room as start position

        // use random start position
        //--------------------------------
        //var _rndStart = Random.Range(0, rooms.Count - 1);
        //_startPosition = new Vector3((rooms[_rndStart].width / 2) + rooms[_rndStart].x, 0, (rooms[_rndStart].height / 2) + rooms[_rndStart].y);
        //--------------------------------
        
        //use smallest room as start position
        //--------------------------------
        var _lastSquareSize = rooms[0].width * rooms[0].height;
        for (int r = 0; r < rooms.Count; r ++)
        {
            var _squareSize = rooms[r].width * rooms[r].height;
            if (_squareSize < _lastSquareSize)
            {
                startPosition = new Vector3((rooms[r].width / 2) + rooms[r].x, 0, (rooms[r].height / 2) + rooms[r].y);
                _lastSquareSize = _squareSize;
            }
        }
        //---------------------------------

        _startPosition = startPosition;

        // end position
        // instead of assigning the last room we will 
        // get the room which is furthest away from the start position
        var _lastDistance = 0f;
        for (int r = 0; r < rooms.Count; r++)
        {
            var _roomCenter = new Vector3((rooms[r].width / 2) + rooms[r].x, 0, (rooms[r].height / 2) + rooms[r].y);
            float _dist = Vector3.Distance(_startPosition, _roomCenter);
            if (_dist > _lastDistance)
            {
                endPosition = _roomCenter;
                _lastDistance = _dist;
            }
        }

        _endPosition = endPosition;

        return dungeonMap;

    }


    // First generate BSP tree
    void GenerateBSPTree()
    {
        tree = new List<Leaf>();
        parentTree = new List<Leaf>();

        // start generating by creating a new root leaf
        var _rootLeaf = new Leaf(tree, parentTree, null, 0, 0, mapWidth, mapHeight);

        BuildRooms();
    }

    // Build Rooms and hallways from BSP tree
    void BuildRooms()
    {
        rooms = new List<Room>();

        for (int i = 0; i < tree.Count; i++)
        {

            var _roomWidth = (int)Random.Range(tree[i].width * 0.5f, tree[i].width - 1);
            var _roomHeight = (int)Random.Range(tree[i].height * 0.5f, tree[i].height - 1);
            var _roomX = (int)Random.Range(tree[i].x + 1, (tree[i].width - _roomWidth - 1) + tree[i].x);
            var _roomY = (int)Random.Range(tree[i].y + 1, (tree[i].height - _roomHeight - 1) + tree[i].y);

            if (_roomWidth >= 2 && _roomHeight >= 2)
            {
                rooms.Add(new Room(_roomX, _roomY, _roomWidth, _roomHeight));
            }
        }

        BuildHallways();
    }

    // Build hallways
    void BuildHallways()
    {
        hallways = new List<Room>();

        for (int i = 0; i < parentTree.Count; i++)
        {
            // first connect the first child with the second child of the parent leafs
            var _centerPointFirstChild = new Vector2((parentTree[i].firstChild.width / 2) + parentTree[i].firstChild.x, (parentTree[i].firstChild.height / 2) + parentTree[i].firstChild.y);
            var _centerPointSecondChild = new Vector2((parentTree[i].secondChild.width / 2) + parentTree[i].secondChild.x, (parentTree[i].secondChild.height / 2) + parentTree[i].secondChild.y);

            // if first child has the same x pos as second get distance and build hallway along the z axis
            if (_centerPointFirstChild.x == _centerPointSecondChild.x)
            {
                var _distZ = Vector3.Distance(_centerPointFirstChild, _centerPointSecondChild);

                if (_centerPointSecondChild.y > _centerPointFirstChild.y)
                {
                    hallways.Add(new Room((int)_centerPointFirstChild.x, (int)_centerPointFirstChild.y, columnWidth, (int)_distZ));
                }
                else
                {
                    hallways.Add(new Room((int)_centerPointFirstChild.x, (int)_centerPointFirstChild.y - (int)_distZ, columnWidth, (int)_distZ));
                }
            }
            // build hallway along the x axis
            else
            {
                var _distX = Vector3.Distance(_centerPointFirstChild, _centerPointSecondChild);

                if (_centerPointSecondChild.x > _centerPointFirstChild.x)
                {
                    hallways.Add(new Room((int)_centerPointFirstChild.x, (int)_centerPointFirstChild.y, (int)_distX, columnWidth));
                }
                else
                {
                    hallways.Add(new Room((int)_centerPointFirstChild.x - (int)_distX, (int)_centerPointFirstChild.y, (int)_distX, columnWidth));
                }
            }
        }

        // now we connect all parent leafs and build hallways between them
        // to make sure all rooms are connected
        for (int i = 0; i < parentTree.Count; i ++)
        {

            if (i + 1 < parentTree.Count)
            {
                var _nextParentCenter = new Vector2((parentTree[i + 1].width / 2) + parentTree[i + 1].x, (parentTree[i + 1].height / 2) + parentTree[i + 1].y);
                var _parentCenter = new Vector2((parentTree[i].width / 2) + parentTree[i].x, (parentTree[i].height / 2) + parentTree[i].y);

                if (Random.Range(0f, 1f) <= 0.5f)
                {
                    var _minX = (int)Mathf.Min(_parentCenter.x, _nextParentCenter.x);
                    var _maxX = (int)Mathf.Max(_parentCenter.x, _nextParentCenter.x);
                    var _minY = (int)Mathf.Min(_parentCenter.y, _nextParentCenter.y);
                    var _maxY = (int)Mathf.Max(_parentCenter.y, _nextParentCenter.y);
                    // Horizontal corridor
                    hallways.Add(new Room(_minX, _minY, _maxX - _minX, columnWidth));
                    // Vertical corridor
                    hallways.Add(new Room(_minX, _minY, columnWidth, _maxY - _minY));

                }
                else
                {
                    var _minX = (int)Mathf.Min(_parentCenter.x, _nextParentCenter.x);
                    var _maxX = (int)Mathf.Max(_parentCenter.x, _nextParentCenter.x);
                    var _minY = (int)Mathf.Min(_parentCenter.y, _nextParentCenter.y);
                    var _maxY = (int)Mathf.Max(_parentCenter.y, _nextParentCenter.y);
                    // Vertical corridor
                    hallways.Add(new Room(_minX, _minY, columnWidth, _maxY - _minY));
                    // Horizontal corridor
                    hallways.Add(new Room(_minX, _minY, _maxX - _minX, columnWidth));
                }
            }
        }
    }

}
