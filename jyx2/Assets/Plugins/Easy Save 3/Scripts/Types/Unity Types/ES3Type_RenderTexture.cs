using System;
using UnityEngine;

namespace ES3Types
{
    [UnityEngine.Scripting.Preserve]
    [ES3PropertiesAttribute("width", "height", "dimension", "graphicsFormat", "useMipMap", "vrUsage", "memorylessMode", "format", "stencilFormat", "autoGenerateMips", "volumeDepth", "antiAliasing", "bindTextureMS", "enableRandomWrite", "useDynamicScale", "isPowerOfTwo", "depth", "descriptor", "masterTextureLimit", "anisotropicFiltering", "wrapMode", "wrapModeU", "wrapModeV", "wrapModeW", "filterMode", "anisoLevel", "mipMapBias", "imageContentsHash", "streamingTextureForceLoadAll", "streamingTextureDiscardUnusedMips", "allowThreadedTextureCreation", "name")]
    public class ES3Type_RenderTexture : ES3ObjectType
    {
        public static ES3Type Instance = null;

        public ES3Type_RenderTexture() : base(typeof(UnityEngine.RenderTexture)) { Instance = this; }


        protected override void WriteObject(object obj, ES3Writer writer)
        {
            var instance = (UnityEngine.RenderTexture)obj;

            writer.WriteProperty("descriptor", instance.descriptor);
            writer.WriteProperty("antiAliasing", instance.antiAliasing, ES3Type_int.Instance);
            writer.WriteProperty("isPowerOfTwo", instance.isPowerOfTwo, ES3Type_bool.Instance);
            writer.WriteProperty("masterTextureLimit", UnityEngine.RenderTexture.globalMipmapLimit, ES3Type_int.Instance);
            writer.WriteProperty("anisotropicFiltering", UnityEngine.RenderTexture.anisotropicFiltering);
            writer.WriteProperty("wrapMode", instance.wrapMode);
            writer.WriteProperty("wrapModeU", instance.wrapModeU);
            writer.WriteProperty("wrapModeV", instance.wrapModeV);
            writer.WriteProperty("wrapModeW", instance.wrapModeW);
            writer.WriteProperty("filterMode", instance.filterMode);
            writer.WriteProperty("anisoLevel", instance.anisoLevel, ES3Type_int.Instance);
            writer.WriteProperty("mipMapBias", instance.mipMapBias, ES3Type_float.Instance);

#if UNITY_2020_1_OR_NEWER
            writer.WriteProperty("streamingTextureForceLoadAll", UnityEngine.RenderTexture.streamingTextureForceLoadAll, ES3Type_bool.Instance);
			writer.WriteProperty("streamingTextureDiscardUnusedMips", UnityEngine.RenderTexture.streamingTextureDiscardUnusedMips, ES3Type_bool.Instance);
			writer.WriteProperty("allowThreadedTextureCreation", UnityEngine.RenderTexture.allowThreadedTextureCreation, ES3Type_bool.Instance);
#endif
        }

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var instance = (UnityEngine.RenderTexture)obj;
            foreach (string propertyName in reader.Properties)
            {
                switch (propertyName)
                {
                    case "width":
                        instance.width = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "height":
                        instance.height = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "dimension":
                        instance.dimension = reader.Read<UnityEngine.Rendering.TextureDimension>();
                        break;
                    case "useMipMap":
                        instance.useMipMap = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "memorylessMode":
                        instance.memorylessMode = reader.Read<UnityEngine.RenderTextureMemoryless>();
                        break;
                    case "format":
                        instance.format = reader.Read<UnityEngine.RenderTextureFormat>();
                        break;
                    case "autoGenerateMips":
                        instance.autoGenerateMips = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "volumeDepth":
                        instance.volumeDepth = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "antiAliasing":
                        instance.antiAliasing = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "enableRandomWrite":
                        instance.enableRandomWrite = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "isPowerOfTwo":
                        instance.isPowerOfTwo = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "depth":
                        instance.depth = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "descriptor":
                        instance.descriptor = reader.Read<UnityEngine.RenderTextureDescriptor>();
                        break;
                    case "masterTextureLimit":
                        UnityEngine.RenderTexture.globalMipmapLimit = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "anisotropicFiltering":
                        UnityEngine.RenderTexture.anisotropicFiltering = reader.Read<UnityEngine.AnisotropicFiltering>();
                        break;
                    case "wrapMode":
                        instance.wrapMode = reader.Read<UnityEngine.TextureWrapMode>();
                        break;
                    case "wrapModeU":
                        instance.wrapModeU = reader.Read<UnityEngine.TextureWrapMode>();
                        break;
                    case "wrapModeV":
                        instance.wrapModeV = reader.Read<UnityEngine.TextureWrapMode>();
                        break;
                    case "wrapModeW":
                        instance.wrapModeW = reader.Read<UnityEngine.TextureWrapMode>();
                        break;
                    case "filterMode":
                        instance.filterMode = reader.Read<UnityEngine.FilterMode>();
                        break;
                    case "anisoLevel":
                        instance.anisoLevel = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "mipMapBias":
                        instance.mipMapBias = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;
                    case "name":
                        instance.name = reader.Read<System.String>(ES3Type_string.Instance);
                        break;

#if UNITY_2020_1_OR_NEWER
                    case "vrUsage":
                        instance.vrUsage = reader.Read<UnityEngine.VRTextureUsage>();
                        break;
                    case "graphicsFormat":
                        instance.graphicsFormat = reader.Read<UnityEngine.Experimental.Rendering.GraphicsFormat>();
                        break;
                    case "stencilFormat":
                        instance.stencilFormat = reader.Read<UnityEngine.Experimental.Rendering.GraphicsFormat>();
                        break;
                    case "bindTextureMS":
                        instance.bindTextureMS = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "useDynamicScale":
                        instance.useDynamicScale = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "streamingTextureForceLoadAll":
						UnityEngine.RenderTexture.streamingTextureForceLoadAll = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "streamingTextureDiscardUnusedMips":
						UnityEngine.RenderTexture.streamingTextureDiscardUnusedMips = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "allowThreadedTextureCreation":
						UnityEngine.RenderTexture.allowThreadedTextureCreation = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
#endif

                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        protected override object ReadObject<T>(ES3Reader reader)
        {
            var descriptor = reader.ReadProperty<RenderTextureDescriptor>();
            var instance = new UnityEngine.RenderTexture(descriptor);
            ReadObject<T>(reader, instance);
            return instance;
        }
    }


    public class ES3Type_RenderTextureArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_RenderTextureArray() : base(typeof(UnityEngine.RenderTexture[]), ES3Type_RenderTexture.Instance)
        {
            Instance = this;
        }
    }
}