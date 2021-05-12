/* TileWorldCreator
 * ----------------------
 * Border mask behaviour
 * ----------------------
 * 
 * returns only the border cells of a map
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
public class BorderMask : TileWorldCreator, IMaskBehaviour {


    public bool[,] ApplyMask(bool[,] _map, TileWorldCreator _creator, TileWorldConfiguration _config)
    {
       
        //create a temp map
        bool[,] _tmpMap = new bool[_map.GetLength(0), _map.GetLength(1)];
        
        //loop through the maps x and y length
        for (int y = 0; y < _map.GetLength(1); y++)
        {
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                //here we are using the TileWorldNeighbourCounter to count 
                //the neighbour cells of the current cell from the map.
                int _c = 0;
               _c = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_map, x, y, 1, _config.global.invert);
                
                //if invert is set to false in the settings of TileWorldCreator
                if (!_config.global.invert)
                {
                    //if the current cell has got 8 neighbours 
                    //then the cell is not located on the border of the island
                    if (_c == 8 || _map[x,y])
                    {
                         _tmpMap[x,y] = true;
                    }
                    //else we can be sure that the current cell is a border cell
                    else
                    {
                        _tmpMap[x, y] = false;
                    }
                }
                else
                {
                    //inverted tmpMap values when invert is set to true.
                    if (_c == 8)
                    {
                        _tmpMap[x, y] = false;
                    }
                    else
                    {
                        _tmpMap[x, y] = true;
                    }
                }

                
            }
        }

        //return the modified map
        return _tmpMap;
    }
}
