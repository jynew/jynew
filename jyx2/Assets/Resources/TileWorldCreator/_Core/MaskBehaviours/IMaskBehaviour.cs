/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * Mask Interface
 * 
*/

using UnityEngine;
using System.Collections;

namespace TileWorld
{
    public interface IMaskBehaviour
    {
        bool[,] ApplyMask(bool[,] _map, TileWorldCreator _creator, TileWorldConfiguration _config);
    }
}
