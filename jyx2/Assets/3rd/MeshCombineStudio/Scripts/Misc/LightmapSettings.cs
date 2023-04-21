using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    [ExecuteInEditMode]
    public class LightmapSettings : MonoBehaviour
    {
        public MeshRenderer mr;
        public int lightmapIndex;
        public bool setLightmapScaleOffset;
        public Vector4 lightmapScaleOffset;

        void OnEnable()
        {
            if (mr)
            {
                mr.lightmapIndex = lightmapIndex;

                if (setLightmapScaleOffset) mr.lightmapScaleOffset = lightmapScaleOffset;
            }
        }
    }
}
