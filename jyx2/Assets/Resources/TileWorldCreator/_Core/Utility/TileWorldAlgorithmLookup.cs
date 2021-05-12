/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * search for available TileWorldCreator Algorithm
 * by getting all classes which inherits from TileWorldAlgorithms
 * 
 * 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using TileWorld;

namespace TileWorld
{
    public class TileWorldAlgorithmLookup
    {

        public static void GetAlgorithms(out IAlgorithms[] _algorithms, out string[] _names)
        {
            IAlgorithms[] iAlgorithms;

            System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(TileWorldAlgorithms)) select type).ToArray();

            iAlgorithms = new IAlgorithms[_found.Length];

            for (int i = 0; i < iAlgorithms.Length; i++)
            {
                var _obj = ScriptableObject.CreateInstance(System.Type.GetType(_found[i].ToString()));
                iAlgorithms[i] = (IAlgorithms)_obj;
            }

            _algorithms = iAlgorithms;
            _names = new string[_found.Length];

            for (int i = 0; i < _found.Length; i++)
            {
                _names[i] = _found[i].ToString();
            }
        }


        public static void GetAlgorithmNames(out string[] _names)
        {
            System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(TileWorldAlgorithms)) select type).ToArray();

            _names = new string[_found.Length];

            for (int i = 0; i < _found.Length; i++)
            {
                _names[i] = _found[i].ToString();
            }
        }

    }
}
