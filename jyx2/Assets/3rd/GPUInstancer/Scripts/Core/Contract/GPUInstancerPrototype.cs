using System;
using UnityEngine;

namespace GPUInstancer
{
    [Serializable]
    public abstract class GPUInstancerPrototype : ScriptableObject
    {
        public GameObject prefabObject;

        // Shadows
        public bool isShadowCasting = true;
        public bool useCustomShadowDistance = false;
        public float shadowDistance = 0;
        public float[] shadowLODMap = new float[] {
            0, 4, 0, 0,
            1, 5, 0, 0,
            2, 6, 0, 0,
            3, 7, 0, 0};
        public bool useOriginalShaderForShadow = false;
        public bool cullShadows = false;

        // Culling
        public float minDistance = 0;
        public float maxDistance = 500;
        public bool isFrustumCulling = true;
        public bool isOcclusionCulling = true;
        public float frustumOffset = 0.2f;
        public float minCullingDistance = 0;
        public float occlusionOffset = 0;
        public int occlusionAccuracy = 1;

        // Bounds
        public Vector3 boundsOffset;

        // LOD
        public bool isLODCrossFade = false;
        public bool isLODCrossFadeAnimate = true;
        [Range(0.01f, 1.0f)]
        public float lodFadeTransitionWidth = 0.1f;
        public float lodBiasAdjustment = 1;

        // Billboard
        public GPUInstancerBillboard billboard;
        public bool isBillboardDisabled;
        // Set to true if the object should not have a billboard option
        public bool useGeneratedBillboard = false;
        public bool checkedForBillboardExtensions;

        // Other
        public bool autoUpdateTransformData;
        public GPUInstancerTreeType treeType;
        public string warningText;

        public override string ToString()
        {
            if (prefabObject != null)
                return prefabObject.name;
            return base.ToString();
        }
    }

    [Serializable]
    public class GPUInstancerBillboard
    {
        public BillboardQuality billboardQuality = BillboardQuality.Mid;
        public int atlasResolution = 2048;
        public int frameCount = 8;
        public bool replaceLODCullWithBillboard = true;
        public bool isOverridingOriginalCutoff = false;
        public float cutoffOverride = -1f;
        [Range(0.0f, 1.0f)]
        public float billboardBrightness = 0.5f;
        [Range(0.01f, 1.0f)]
        public float billboardDistance = 0.8f;

        public float quadSize;
        public float yPivotOffset;

        public Texture2D albedoAtlasTexture;
        public Texture2D normalAtlasTexture;

        // true if LOD group already has a billboard
        public bool customBillboardInLODGroup;
        // Custom billboard mesh-material options
        public bool useCustomBillboard;
        public Mesh customBillboardMesh;
        public Material customBillboardMaterial;

        public bool isBillboardShadowCasting = false;

        public bool billboardFaceCamPos = false;
    }

    public enum BillboardQuality
    {
        Low = 0,
        Mid = 1,
        High = 2,
        VeryHigh = 3
    }
}
