/* TILEWORLDCREATOR IAlgorithm
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 *
 * Implement IAlgorithm to your own algorithm
 * For more info  about how to implement your own algorithm refer to 
 * the documentation.
*/
using UnityEngine;
using System.Collections;

namespace TileWorld
{
    public interface IAlgorithms
    {
        /// <summary>
        /// Return the generated map as a bool[,] array to TileWorldCreator.
        /// Additional _startPosition and _endPosition can be provided for the TileWorldObjectScatter
        /// </summary>
        /// <param name="_config"></param>
        /// <param name="_startPosition"></param>
        /// <param name="_endPosition"></param>
        /// <returns></returns>
        bool[,] Generate(TileWorldConfiguration _config, out Vector3 _startPosition, out Vector3 _endPosition);
    }
}
