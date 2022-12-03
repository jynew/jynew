using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Specifies textures and other required parameters when creating <see cref="Texture2DArray"/>.
    /// </summary>
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewTextureArraySettings.asset",
        menuName = "Mesh Terrain Editor/TextureArraySettings")]
    [UnityEngine.Scripting.APIUpdating.MovedFrom("PBRTextureArraySettings")]
#endif
    public class TextureArraySettings : ScriptableObject
    {
        [HideInInspector]
        public TextureArrayMode textureMode = TextureArrayMode.PBR;

        [HideInInspector]
        public string TextureArrayName = "NewTexture2DArray";
        
        [HideInInspector]
        public TextureFormat TextureFormat = TextureFormat.RGBA32;

        [HideInInspector]
        public int TextureSize = 512;

        [HideInInspector]
        public List<TextureLayer> Layers = new List<TextureLayer>(12);
        
        public const string ColorArrayPostfix = "_Color";
        public const string NormalArrayPostfix = "_Normal";
        public const string AlbedoArrayPostfix = "_Albedo";
        public const string RoughnessNormalAOArrayPostfix = "_RoughnessNormalAO";
        public const int MinLayerNumber = 2;
        public const int MaxLayerNumber = 12;
    }

    public enum TextureArrayMode
    {
        Color,
        ColorAndNormal,
        PBR,
    }
}