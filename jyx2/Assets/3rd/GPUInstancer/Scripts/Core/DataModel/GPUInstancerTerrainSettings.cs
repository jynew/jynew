using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerTerrainSettings : ScriptableObject
    {
        public string terrainDataGUID;
        public float maxDetailDistance = 500F;
        [Range(0f, 2500f)]
        public float maxTreeDistance = 500F;
        public Vector2 windVector = new Vector2(0.4f, 0.8f);
        public Texture2D healthyDryNoiseTexture;
        public Texture2D windWaveNormalTexture;
        public bool autoSPCellSize = true;
        [Range(25, 500)]
        public int preferedSPCellSize = 125;
        [Range(0.0f, 1.0f)]
        public float detailDensity = 1.0f;
        public string warningText;

        public Texture2D GetHealthyDryNoiseTexture(GPUInstancerDetailPrototype detailPrototype)
        {
            if (detailPrototype.useCustomHealthyDryNoiseTexture && detailPrototype.healthyDryNoiseTexture != null)
                return detailPrototype.healthyDryNoiseTexture;
            return healthyDryNoiseTexture;
        }
    }
}