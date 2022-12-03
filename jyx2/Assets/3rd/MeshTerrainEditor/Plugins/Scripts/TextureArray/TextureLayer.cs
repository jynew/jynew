using UnityEngine;

namespace MTE
{
#if UNITY_EDITOR
    [UnityEngine.Scripting.APIUpdating.MovedFrom("PBRLayer")]
#endif
    [System.Serializable]
    public class TextureLayer
    {
        public Texture2D Albedo;
        public Texture2D Roughness;
        public Texture2D Normal;
        public Texture2D AO;
        public Texture2D Height;
        public Texture2D Metallic;

        public bool IsReadyForPBRTextureArray()
        {
            return Albedo != null && Roughness != null && Normal != null && AO != null;
        }
    }
}