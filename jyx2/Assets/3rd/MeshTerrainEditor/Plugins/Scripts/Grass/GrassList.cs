using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    [Serializable]
    public class GrassList : ScriptableObject
    {
        public List<GrassStar> grasses;
        public List<GrassQuad> quads;
    }
}
