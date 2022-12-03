namespace MTE
{
    /// <summary>
    /// Shader property names used in MTE texture array shaders.
    /// </summary>
    public class TextureArrayShaderPropertyNames
    {
        public const string WeightMap0PropertyName = "_Control0";
        public const string WeightMap1PropertyName = "_Control1";
        public const string WeightMap2PropertyName = "_Control2";
        public const string AlbedoArrayPropertyName = "_TextureArray0";
        public const string NormalArrayPropertyName = "_TextureArray1";
        public const string RoughnessNormalAOArrayPropertyName = "_TextureArray1";
        public const string NormalIntensityPropertyName = "_NormalIntensity";
        public const string UVScaleOffsetPropertyName = "_UVScaleOffset";

        public static string[] ControlTexturePropertyNames =
        {
            WeightMap0PropertyName,
            WeightMap1PropertyName,
            WeightMap2PropertyName
        };
    }
}