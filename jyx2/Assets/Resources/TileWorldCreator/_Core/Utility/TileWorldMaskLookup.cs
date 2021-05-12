/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * search for available TileWorldCreator Masks
 * by getting all classes which inherits from TileWorldCreator
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
    public class TileWorldMaskLookup
    {

        public static void GetMasks(out IMaskBehaviour[] _masks, out string[] _names)
        {
            IMaskBehaviour[] iMasks;

            System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(TileWorldCreator)) select type).ToArray();

            iMasks = new IMaskBehaviour[_found.Length];

            for (int i = 0; i < iMasks.Length; i++)
            {
                var _obj = ScriptableObject.CreateInstance(System.Type.GetType(_found[i].ToString()));
                iMasks[i] = (IMaskBehaviour)_obj;
            }

            _masks = iMasks;
            _names = new string[_found.Length];

            for (int i = 0; i < _found.Length; i++)
            {
                _names[i] = _found[i].ToString();
            }
        }

    }
}
