/* TileWorldCreator
 * ----------------------
 * Circle mask behaviour
 * ----------------------
 * 
 * returns a circle shaped map
 * 
 */
using UnityEngine;
using System.Collections;
using TileWorld;

/* To create a mask behaviour we have to inherit from TileWorldCreator 
 * and use the IMaskBehaviour interface.
 * 
 * Required method of the IMaskBehaviour interface:
 * public bool[,] ApplyMask(bool[,] _map, TileWorldCreator _creator)
 * 
 * the ApplyMask method receives the current map which then can be modified
 * inside of the class. After modifications are done, you will have to return
 * the modified map.
 */
public class CircleMask : TileWorldCreator, IMaskBehaviour
{

    public bool[,] ApplyMask(bool[,] _map, TileWorldCreator _creator, TileWorldConfiguration _config)
    {
        //create a new temp map
        bool[,] _tmpMap = new bool[_map.GetLength(0), _map.GetLength(1)];
        //set the circle radius
        float _radius = 5;
        
        //loop through the maps x and y length
        for (int x = 0; x < _config.global.width; x ++)
        {
            for (int y = 0; y < _config.global.height; y ++)
            {
                //get center point of the map
                Vector2 _center = new Vector2(_config.global.width / 2, _config.global.height / 2);
                //current tile position
                Vector2 _currPos = new Vector2(x,y);
                //get distance from its current cell position to the center of the map
                float _dist = Vector2.Distance(_center, _currPos);
                
                //check if the current distance is not to far away from its radius
                //and set the appropriate values depending of if invert is set to false or true
                if (_dist <= _radius)
                {
                    if (!_config.global.invert)
                    {
                        _tmpMap[x, y] = false;
                    }
                    else
                    {
                        _tmpMap[x, y] = true;
                    }
                }
                else
                {
                    if (!_config.global.invert)
                    {
                        _tmpMap[x, y] = true;
                    }
                    else
                    {
                        _tmpMap[x, y] = false;
                    }
                }
            }
        }
        
        //return the map
        return _tmpMap;
    }

}
