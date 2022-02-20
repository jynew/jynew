using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Make sure the lightmap data are correct at runtime.
    /// Unity somehow starts clear programmatically assigned lightMapIndex and lightmapScaleOffset when enter play mode.
    /// This class fixed the issue https://github.com/zwcloud/MeshTerrainEditor-issues/issues/209
    /// </summary>
    [ExecuteInEditMode]
    public class BakedLightmapKeeper : MonoBehaviour
    {
        public int lightMapIndex = -1;
        public Vector4 lightmapScaleOffset = new Vector4(1, 1, 0, 0);
        private void Awake()
        {
            var theRenderer = GetComponent<Renderer>();
            if (!theRenderer)
            {
                return;
            }
            theRenderer.lightmapIndex = lightMapIndex;
            theRenderer.lightmapScaleOffset = lightmapScaleOffset;
        }
    }
}
