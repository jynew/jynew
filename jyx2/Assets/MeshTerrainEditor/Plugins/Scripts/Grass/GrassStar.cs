using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    [Serializable]
    public class GrassStar
    {
        [Header("Basic")]
        [SerializeField]
        private Material material;
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private float rotationY;
        [SerializeField]
        private float width;
        [SerializeField]
        private float height;

        [Space(10)]

        [Header("Lightmap")]
        [SerializeField]
        private int lightmapIndex = -1;
        [SerializeField]
        private Vector4 lightmapScaleOffset = new Vector4(1, 1, 0, 0);

        public Material Material
        {
            get { return this.material; }
        }

        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public float RotationY
        {
            get { return this.rotationY; }
            set { this.rotationY = value; }
        }

        public float Width
        {
            get { return this.width; }
        }

        public float Height
        {
            get { return this.height; }
        }

        public int LightmapIndex
        {
            get { return this.lightmapIndex; }
        }

        public Vector4 LightmapScaleOffset
        {
            get { return this.lightmapScaleOffset; }
        }

        /// <summary>Save lightmapping data to this GrassStar.</summary>
        /// <param name="lightmapIndex"></param>
        /// <param name="lightmapScaleOffset"></param>
        public void SaveLightmapData(int lightmapIndex, Vector4 lightmapScaleOffset)
        {
            this.lightmapIndex = lightmapIndex;
            this.lightmapScaleOffset = lightmapScaleOffset;
        }

        /// <summary>Initialize this GrassStar.</summary>
        /// <param name="material"></param>
        /// <param name="position">position in world space</param>
        /// <param name="rotationY">rotation Y (Euler angles Y)</param>
        /// <param name="width">width of a quad</param>
        /// <param name="height">height of a quad</param>
        public void Init(Material material,
            Vector3 position, float rotationY, float width, float height)
        {
            this.material = material;
            this.position = position;
            this.rotationY = rotationY;
            this.width = width;
            this.height = height;
        }
    }
}
