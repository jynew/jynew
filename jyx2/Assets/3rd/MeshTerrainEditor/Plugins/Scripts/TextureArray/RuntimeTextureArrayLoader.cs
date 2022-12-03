using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Create <see cref="Texture2DArray"/> according to a <see cref="TextureArraySettings"/>
    /// and assign to renderer's material.
    /// 
    /// The material must use a compatible TextureArray shader.
    /// </summary>
    public class RuntimeTextureArrayLoader : MonoBehaviour
    {
        public TextureArraySettings settings;
        private Texture2DArray array0, array1;
        [System.NonSerialized]
        private bool loaded;

        private void Start()
        {
            if (!settings)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }
            
#if UNITY_EDITOR
            Debug.Log($"Creating TextureArray according to TextureArraySettings {settings.name}...");
#endif

            //create in-memory texture arrays
            if (!CreateTextureArrays(settings, out array0, out array1))
            {
                Debug.LogError($"Failed to create texture array {settings.TextureArrayName}.");
                return;
            }
            array0.name += "(temporary in-memory)";
            array1.name += "(temporary in-memory)";

            //assign to corresponding material properties
            var meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                throw new MissingComponentException(nameof(MeshRenderer));
            }
            switch (settings.textureMode)
            {
                case TextureArrayMode.Color:
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.AlbedoArrayPropertyName, array0);
                    break;
                case TextureArrayMode.ColorAndNormal:
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.AlbedoArrayPropertyName, array0);
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.NormalArrayPropertyName, array1);
                    break;
                case TextureArrayMode.PBR:
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.AlbedoArrayPropertyName, array0);
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.RoughnessNormalAOArrayPropertyName, array1);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            
#if UNITY_EDITOR
            Debug.Log("Finished creating TextureArray.");
#endif

            loaded = true;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (loaded)
            {
                UnloadInEditor();
            }
#endif
        }

#if UNITY_EDITOR
        public void LoadInEditor()
        {
            if (loaded)
            {
                return;
            }
            Start();
        }

        public void UnloadInEditor()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                throw new MissingComponentException(nameof(MeshRenderer));
            }
            switch (settings.textureMode)
            {
                case TextureArrayMode.Color:
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.AlbedoArrayPropertyName, null);
                    break;
                case TextureArrayMode.ColorAndNormal:
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.AlbedoArrayPropertyName, null);
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.NormalArrayPropertyName, null);
                    break;
                case TextureArrayMode.PBR:
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.AlbedoArrayPropertyName, null);
                    meshRenderer.sharedMaterial.SetTexture(
                        TextureArrayShaderPropertyNames.RoughnessNormalAOArrayPropertyName, null);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            
            DestroyImmediate(array0);
            DestroyImmediate(array1);
            array0 = array1 = null;

            loaded = false;
        }
#endif

        #region Implementation Details
        private static bool CreateTextureArrays(TextureArraySettings settings,
            out Texture2DArray array0, out Texture2DArray array1)
        {
            array0 = array1 = null;

            if (settings.textureMode == TextureArrayMode.Color)
            {
                Texture2DArray colorArray;
                CreateColorTextureArray(settings, out colorArray);
                if (!colorArray)
                {
                    Debug.LogError("Failed to create colorArray.");
                    return false;
                }
                array0 = colorArray;
            }
            else if (settings.textureMode == TextureArrayMode.ColorAndNormal)
            {
                Texture2DArray colorArray, normalArray;
                CreateColorTextureArray(settings, out colorArray);
                CreateNormalTextureArray(settings, out normalArray);
                if (!normalArray)
                {
                    Debug.LogError("Failed to create colorArray or normalArray");
                    return false;
                }
                array0 = colorArray;
                array1 = normalArray;
            }
            else//TextureArrayMode.PBR
            {
                Texture2DArray albedoArray, roughNormalAOArray;
                CreatePBRTextureArray(settings, out albedoArray, out roughNormalAOArray);
                if (!roughNormalAOArray)
                {
                    Debug.LogError("Failed to create albedoArray.");
                    return false;
                }
                if (!roughNormalAOArray)
                {
                    Debug.LogError("Failed to create roughNormalAOArray.");
                    return false;
                }
                array0 = albedoArray;
                array1 = roughNormalAOArray;
            }

            return true;
        }

        private static Texture2DArray Generate(IList<Texture2D> textures, TextureFormat format)
        {
            //We use first texture size as the texture array slices' size.

            // format needs to be ARGB32, RGBA32, RGB24, R8, Alpha8 or one of float formats.
            Texture2DArray texture2DArray = new Texture2DArray(textures[0].width,
                textures[0].height,
                textures.Count,
                format,
                true);
            for (int i = 0; i < textures.Count; i++)
            {
                // NOTE:
                // It is able to make a Texture2DArray with "Graphics.CopyTexture()".
                // However, it has a problem which is able to make Texture2DArray in Editor
                // without enabling read-write settings of texture.
                // And then, it causes some wrong result in build app.
                // So we should make a Texture2DArray with "SetPixels()".
                // 
                // Graphics.CopyTexture(textures[i], 0, 0, texture2DArray, i, 0);

                texture2DArray.SetPixels(textures[i].GetPixels(), i);
            }

            texture2DArray.Apply();
            return texture2DArray;
        }

        private static void CreateColorTextureArray(TextureArraySettings settings,
            out Texture2DArray array)
        {
            array = null;

            var layers = settings.Layers;
            if (layers.Count < 2)
            {
                throw new System.Exception("Less than 2 layers in array settings.");
            }

            if (!layers.TrueForAll(l => l.Albedo != null))
            {
                throw new System.Exception("Some albedo texture hasn't been assigned.");
            }

            var albedoTextureList = new List<Texture2D>(layers.Count);
            foreach (var layer in layers)
            {
                var albedoTexture = layer.Albedo;
                albedoTextureList.Add(albedoTexture);
            }
            var textureArray = Generate(albedoTextureList, TextureFormat.RGBA32);
            textureArray.name = settings.TextureArrayName + TextureArraySettings.ColorArrayPostfix;

            array = textureArray;
        }

        private static void CreateNormalTextureArray(TextureArraySettings settings,
            out Texture2DArray array)
        {
            array = null;

            var layers = settings.Layers;
            if (layers.Count < 2)
            {
                throw new System.Exception("Less than 2 layers in array settings.");
            }

            if (!layers.TrueForAll(l => l.Normal != null))
            {
                throw new System.Exception("Some normal texture hasn't been assigned.");
            }

            var textureList = new List<Texture2D>(layers.Count);
            foreach (var layer in layers)
            {
                var texture = layer.Normal;
                textureList.Add(texture);
            }
            var textureArray = Generate(textureList, TextureFormat.RGBA32);
            textureArray.name = settings.TextureArrayName + TextureArraySettings.NormalArrayPostfix;

            array = textureArray;
        }
        
        private static void CreatePBRTextureArray(TextureArraySettings settings,
            out Texture2DArray albedoArray, out Texture2DArray roughNormalAOArray)
        {
            albedoArray = roughNormalAOArray = null;
            var layers = settings.Layers;
            if (layers.Count < 2)
            {
                throw new System.Exception("Less than 2 layers in array settings.");
            }

            if (!layers.TrueForAll(l => l.IsReadyForPBRTextureArray()))
            {
                throw new System.Exception("Some layer hasn't been fully assigned.");
            }

            var albedoTextureList = new List<Texture2D>(layers.Count);
            for (var layerIndex = 0; layerIndex < layers.Count; layerIndex++)
            {
                var layer = layers[layerIndex];

                var albedoTexture = layer.Albedo;
                albedoTextureList.Add(albedoTexture);
            }

            //create Albedo texture array 
            {
                var textureArray = Generate(albedoTextureList, TextureFormat.RGBA32);
                textureArray.name =
                    settings.TextureArrayName + TextureArraySettings.AlbedoArrayPostfix;
                albedoArray = textureArray;
            }

            var roughNormalAOTextureList = new List<Texture2D>(layers.Count);
            for (var layerIndex = 0; layerIndex < layers.Count; layerIndex++)
            {
                var layer = layers[layerIndex];

                var roughnessTexture = layer.Roughness;
                var normalTexture = layer.Normal;
                var aoTexture = layer.AO;

                var packedTexture = MergeTextures(roughnessTexture, normalTexture, aoTexture, settings.TextureSize);
                roughNormalAOTextureList.Add(packedTexture);
            }

            //create RoughnessNormalAO texture array
            {
                var textureArray = Generate(roughNormalAOTextureList, TextureFormat.RGBA32);
                textureArray.name =
                    settings.TextureArrayName + TextureArraySettings.RoughnessNormalAOArrayPostfix;
                roughNormalAOArray = textureArray;
            }
        }

        private static Texture2D MergeTextures(Texture2D roughnessTexture, Texture2D normalTexture,
            Texture2D aoTexture, int textureSize)
        {
            Texture2D result = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false, true);
            var pixels = new Color[textureSize*textureSize];
            var roughnessPixels = roughnessTexture.GetPixels();
            var normalPixels = normalTexture.GetPixels();
            var aoPixels = aoTexture.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
            {
                var roughness = roughnessPixels[i].r;
                var normalX = normalPixels[i].r * 2 - 1;
                var normalY = normalPixels[i].g * 2 - 1;
                var normalZ = normalPixels[i].b * 2 - 1;
                var normalVectorPossiblyNotNormalized = new Vector3(normalX, normalY, normalZ);
                var normal = Vector3.Normalize(normalVectorPossiblyNotNormalized);
                var normalAsRGB = normal * 0.5f + new Vector3(0.5f, 0.5f, 0.5f);
                var ao = aoPixels[i].r;
                pixels[i].r = roughness;
                pixels[i].g = Mathf.Clamp01(normalAsRGB.x);
                pixels[i].b = Mathf.Clamp01(normalAsRGB.y);
                pixels[i].a = ao;
            }
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
        #endregion

    }

}