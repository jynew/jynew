using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{

    public class GPUInstancerDetailCell : GPUInstancerCell
    {
        /// <summary>
        /// (Optional) Detail Prototype Instance Data 
        /// prototypeIndex, instanceList
        /// </summary>
        public Dictionary<int, Matrix4x4[]> detailInstanceList;
        public Dictionary<int, ComputeBuffer> detailInstanceBuffers;

        // Detail Data
        public float[] heightMapData;
        public List<int[]> detailMapData;
        public List<int> totalDetailCounts;
        public Vector3 instanceStartPosition;

        public GPUInstancerDetailCell(int coordX, int coordZ)
        {
            this.coordX = coordX;
            this.coordY = 0;
            this.coordZ = coordZ;
        }
    }

}
