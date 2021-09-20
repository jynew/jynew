using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{

    public class GPUInstancerPrefabCell : GPUInstancerCell
    {
        /// <summary>
        /// (Optional) Prefab Prototype Instance Data 
        /// prototypeIndex, instanceList
        /// </summary>
        public Dictionary<int, List<Matrix4x4>> prefabInstanceList;

        public GPUInstancerPrefabCell(int coordX, int coordY, int coordZ)
        {
            this.coordX = coordX;
            this.coordY = coordY;
            this.coordZ = coordZ;
        }
    }

}
