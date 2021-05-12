/* TileWorldCreator
 * ----------------------
 * Random noise mask behaviour
 * ----------------------
 * 
 * masks a map randomly
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
public class RandomNoiseMask : TileWorldCreator, IMaskBehaviour
{


    public bool[,] ApplyMask(bool[,] _map, TileWorldCreator _creator, TileWorldConfiguration _config)
    {
       
        bool[,] _tmpMap = new bool[_map.GetLength(0), _map.GetLength(1)];
        
        //get inner terrain cells
        for (int y = 0; y < _map.GetLength(1); y++)
        {
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                int _c = 0;
               _c = TileWorldNeighbourCounter.CountInnerTerrainBlocks(_map, x, y, 1, _config.global.invert);
                
                if (!_config.global.invert)
                {
                    if (_c == 8)
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

                    if (_c == 8)
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

        //get random cells from inner terrain map
        bool[,] _newMap = new bool[_config.global.width, _config.global.height];
        for (int y = 0; y < _map.GetLength(1); y++)
        {
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                float _rnd = Random.Range(0f, 1f);
                
                if (!_config.global.invert)
                {
                    //invert map if invert ist set to false 
                    _newMap[x, y] = true;
                }
              
                
                if (_rnd < 0.3f)
                {
                    if (_config.global.invert)
                    {     

                        if (_tmpMap[x, y])
                        {
                            _newMap[x, y] = true;
                        }
                    }
                    else
                    {
                        if (!_tmpMap[x, y])
                        {
                            _newMap[x, y] = false;
                        }                      
                    }

                }
            }
        }

        return _newMap;
    }
}
