using UnityEngine;
using System;

namespace GPUInstancer
{

    [Serializable]
    public class GPUInstancerDetailPrototype : GPUInstancerPrototype
    {
        public int prototypeIndex;
        public DetailRenderMode detailRenderMode;
        public bool usePrototypeMesh;
        public Texture2D prototypeTexture;

        public bool useCustomMaterialForTextureDetail = false;
        public Material textureDetailCustomMaterial;

        [Range(0.0f, 1.0f)]
        public float detailDensity = 1.0f;

        public bool useCrossQuads = true;
        [Range(1, 4)]
        public int quadCount = 2;

        public bool isBillboard = false;
        [Range(0.5f, 1.0f)]
        public float billboardDistance = 0.9f;
        public bool billboardDistanceDebug = false;
        public Color billboardDistanceDebugColor = Color.red;
        public bool billboardFaceCamPos = false;

        // Color Parameters
        public Color detailHealthyColor;
        public Color detailDryColor;
        public float noiseSpread = 0.2f;
        [Range(0f, 1f)]
        public float ambientOcclusion = 0.5f;
        [Range(0f, 1f)]
        public float gradientPower = 0.3f;

        // Wind Parameters
        public Color windWaveTintColor;
        [Range(0f, 1f)]
        public float windIdleSway = 0.6f;
        public bool windWavesOn = true;
        [Range(0f, 1f)]
        public float windWaveSize = 0.5f;
        [Range(0f, 1f)]
        public float windWaveTint = 0.5f;
        [Range(0f, 1f)]
        public float windWaveSway = 0.3f;

        // minWidth, maxWidth, minHeight, maxHeight
        public Vector4 detailScale;

        public bool useCustomHealthyDryNoiseTexture;
        public Texture2D healthyDryNoiseTexture;

        public override string ToString()
        {
            if (prototypeTexture != null)
                return prototypeTexture.name;
            return base.ToString();
        }
    }

}
