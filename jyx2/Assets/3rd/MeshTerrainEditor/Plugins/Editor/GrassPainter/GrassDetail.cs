using System;
using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Grass Prototype info
    /// </summary>
    [Serializable]
    internal class GrassDetail : Detail
    {
        public const float DefaultMinWidth = 0.1f;
        public const float DefaultMaxWidth = 0.2f;
        public const float DefaultMinHeight = 0.1f;
        public const float DefaultMaxHeight = 0.2f;
        public const GrassType DefaultGrassType = GrassType.OneQuad;

        [SerializeField]
        private Material material;
        [SerializeField]
        private float minWidth;
        [SerializeField]
        private float maxWidth;
        [SerializeField]
        private float minHeight;
        [SerializeField]
        private float maxHeight;
        [SerializeField]
        private GrassType grassType;


        public Material Material
        {
            get
            {
                return this.material;
            }

            set
            {
                this.material = value;
            }
        }

        public float MinWidth
        {
            get
            {
                return this.minWidth;
            }

            set
            {
                this.minWidth = value;
            }
        }

        public float MaxWidth
        {
            get
            {
                return this.maxWidth;
            }

            set
            {
                this.maxWidth = value;
            }
        }

        public float MinHeight
        {
            get
            {
                return this.minHeight;
            }

            set
            {
                this.minHeight = value;
            }
        }

        public float MaxHeight
        {
            get
            {
                return this.maxHeight;
            }

            set
            {
                this.maxHeight = value;
            }
        }

        public GrassType GrassType
        {
            get { return this.grassType; }
            set { this.grassType = value; }
        }

        public GrassDetail ShallowCopy()
        {
            var copy = new GrassDetail();
            copy.material  = material  ;
            copy.minWidth  = minWidth  ;
            copy.maxWidth  = maxWidth  ;
            copy.minHeight = minHeight ;
            copy.maxHeight = maxHeight ;
            copy.grassType = grassType ;
            return copy;
        }
    }
}