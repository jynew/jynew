/* TILEWORLDCREATOR SimpleDungeonAlgorithm
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 *
 * Generates a simple Dungeon based on random room placement
 *
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileWorld;

public class SimpleDungeonAlgorithm : TileWorldAlgorithms, IAlgorithms {
 
    int width, height;
    int roomCount;
    int minRoomSizeWidth, minRoomSizeHeight;
    int maxRoomSizeWidth, maxRoomSizeHeight;
    bool allowRoomIntersections;
    int corridorWidth;
    bool createCircleRooms;
    float minCircleRoomRadius;
    float maxCircleRoomRadius;
    float rndRadius;

    Vector3 startPosition;
    Vector3 endPosition;

    public class Room
    {
        public int x1, x2, y1, y2;
        public int w, h;
        public bool isConnected;

        public Vector3 center;

        public Room(int x, int y, int w, int h)
        {
            x1 = x;
            x2 = x + w;
            y1 = y;
            y2 = y + h;
            this.w = w;
            this.h = h;
            isConnected = false;
            center = new Vector3(Mathf.Floor((x1 + x2) / 2), 0, Mathf.Floor((y1 + y2) / 2));
        }

        // Returns true or false if this room is intersecting with other room
        public bool Intersects(Room room)
        {
            return (x1 <= room.x2 && x2 >= room.x1 && y1 <= room.y2 && room.y2 >= room.y1);
        }
    }

    // Store all rooms inside of this list array
    List<Room> rooms = new List<Room>();

    // Map to return
    bool[,] dungeonMap;

    // Implemented interface
    public bool[,] Generate(TileWorldConfiguration _config, out Vector3 _startPosition, out Vector3 _endPosition)
    {
        // Assign properties from config
        width = _config.global.width;
        height = _config.global.height;

        createCircleRooms = _config.simpleDungeonAlgorithm.createCircleRooms;
        minCircleRoomRadius = _config.simpleDungeonAlgorithm.minCircleRoomRadius;
        maxCircleRoomRadius = _config.simpleDungeonAlgorithm.maxCircleRoomRadius;

        roomCount = _config.simpleDungeonAlgorithm.roomCount;
        minRoomSizeHeight = _config.simpleDungeonAlgorithm.minRoomSizeHeight;
        minRoomSizeWidth = _config.simpleDungeonAlgorithm.minRoomSizeWidth;
        maxRoomSizeHeight = _config.simpleDungeonAlgorithm.maxRoomSizeHeight;
        maxRoomSizeWidth = _config.simpleDungeonAlgorithm.maxRoomSizeWidth;
        allowRoomIntersections = _config.simpleDungeonAlgorithm.allowRoomIntersections;
        corridorWidth = _config.simpleDungeonAlgorithm.minCorridorWidth;

        
        GenerateDungeonMap();

        _startPosition = startPosition;
        _endPosition = endPosition;

        // return map
        return dungeonMap;
    }

    void GenerateDungeonMap()
    {

        dungeonMap = new bool[width, height];


        rooms = new List<Room>();

        //PoissonDiscSampler sampler = new PoissonDiscSampler(width - maxRoomSizeWidth, height - maxRoomSizeHeight, 1);
        //List<Vector2> points = new List<Vector2>();
        //foreach (Vector2 sample in sampler.Samples())
        //{
        //    points.Add(sample);
        //}

        for (int c = 0; c < roomCount; c ++)
        {
            

            var _maxWidth = Random.Range(minRoomSizeWidth, maxRoomSizeWidth);
            var _maxHeight = Random.Range(minRoomSizeHeight, maxRoomSizeHeight);

            var _startX = Random.Range(0, width - maxRoomSizeWidth);
            var _startY = Random.Range(0, height - maxRoomSizeHeight);

            //var _startX = (int)points[Random.Range(0, points.Count)].x;
            //var _startY = (int)points[Random.Range(0, points.Count)].y;

            rndRadius = Random.Range(minCircleRoomRadius, maxCircleRoomRadius);


            GenerateRoomAndCorridor(_startX, _startY, _maxWidth, _maxHeight, c);
        }


        // start position
        var _firstRoom = new Room(rooms[0].x1, rooms[0].y1, rooms[0].w, rooms[0].h);
        startPosition = _firstRoom.center;
        
        // end position
        // instead of assigning the last room we will 
        // get the room which is furthest away from the start position
        var _lastDistance = 0f;
        for (int r = 0; r < rooms.Count; r ++)
        {
            float _dist = Vector3.Distance(startPosition, rooms[r].center);
            if (_dist > _lastDistance)
            {
                endPosition = rooms[r].center;
                _lastDistance = _dist;
            }
        }

        // last room
        //var _lastRoom = new Room(rooms[rooms.Count - 1].x1, rooms[rooms.Count - 1].y1, rooms[rooms.Count - 1].w, rooms[rooms.Count - 1].h);
        //endPosition = _lastRoom.center;
            
        
    }

    // generate one single room and corridors
    void GenerateRoomAndCorridor(int _startX, int _startY, int _maxWidth, int _maxHeight, int _c)
    {
        var _newRoom = new Room(_startX, _startY, _maxWidth, _maxHeight);
        bool _intersecting = false;

        if (!allowRoomIntersections)
        {
            foreach (Room _r in rooms)
            {
                if (_newRoom.Intersects(_r))
                {
                    _intersecting = true;
                    break;
                }
            }
        }
        else
        {
            _intersecting = false;
        }

        if (!_intersecting)
        {
           

            BuildRoom(_newRoom);

            if (rooms.Count > 0)
            {
                
                //build corridor
                var _prevCenter = rooms[rooms.Count - 1].center;
                var _newCenter = _newRoom.center;

                if (!rooms[rooms.Count - 1].isConnected)
                {
                    if (Random.Range(0, 10) <= 5)
                    {
                        HCorridor(_prevCenter.x, _newCenter.x, _prevCenter.z);
                        VCorridor(_prevCenter.z, _newCenter.z, _newCenter.x);
                    }
                    else
                    {
                        VCorridor(_prevCenter.z, _newCenter.z, _prevCenter.x);
                        HCorridor(_prevCenter.x, _newCenter.x, _newCenter.z);
                    }

                    //set room connected
                    rooms[rooms.Count - 1].isConnected = true;
                }
            }

            rooms.Add(_newRoom);
        }
    }

    void BuildRoom(Room _room)
    {
        if (createCircleRooms)
        {
            for (int w = 1; w < width - 1; w++)
            {
                for (int h = 1; h < height - 1; h++)
                {
                    Vector2 _currentPos = new Vector2(w, h);
                    Vector2 _center = new Vector2(Mathf.Floor((_room.x1 + _room.x2) / 2), Mathf.Floor((_room.y1 + _room.y2) / 2));

                    float _dist = Vector3.Distance(_currentPos, _center);
                    if (_dist <= rndRadius)
                    {

                        dungeonMap[w, h] = true;
                       
                    }
                }
            }
        }
        else
        {
            for (int x = 1; x < _room.w; x++)
            {
                for (int y = 1; y < _room.h; y++)
                {
                    if (_room.x1 + x < width - 1 && _room.y1 + y < height - 1)
                    {
                        dungeonMap[_room.x1 + x, _room.y1 + y] = true;
                    }
                }
            }
        }
    }


    void HCorridor(float x1, float x2, float y)
    {
        var _min = Mathf.Min(x1, x2);
        var _max = Mathf.Max(x1, x2);

        for (int w = 0; w < corridorWidth; w++)
        {
            for (int x = (int)_min; x < (int)_max + 1; x++)
            {
                //remove tiles
                if ((int)y - w > 0 && x < width && y < height)
                {
                    dungeonMap[x, (int)y - w] = true;
                }
            }
        }
    }


    void VCorridor(float y1, float y2, float x)
    {
        var _min = Mathf.Min(y1, y2);
        var _max = Mathf.Max(y1, y2);

        for (int w = 0; w < corridorWidth; w++)
        {
            for (int y = (int)_min; y < (int)_max; y++)
            {
                //remove tiles
                if ((int)x - w > 0 && x < width && y < height)
                {
                    dungeonMap[(int)x - w, y] = true;
                }         
            }
        }
    }

}
