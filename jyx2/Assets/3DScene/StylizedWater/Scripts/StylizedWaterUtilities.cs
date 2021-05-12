// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StylizedWater
{
    public static class StylizedWaterUtilities
    {
        //DEV
        public static bool DEBUG = false;

        public static string[] ComposeDropdown(Texture2D[] resource, string replaceFilter)
        {
            //Compose dropdown menus
            string[] dropdownNames = new string[resource.Length + 1];
            for (int i = 0; i < resource.Length; i++)
            {
                if (resource[i] == null)
                {
                    dropdownNames[i] = "(Missing)";
                }
                else
                {
                    dropdownNames[i] = resource[i].name.Replace(replaceFilter, string.Empty);
                }
            }
            //Append to the end
            dropdownNames[resource.Length] = "Custom...";

            return dropdownNames;
        }

        //Check if values are equal, has error margin for floating point precision
        public static bool IsApproximatelyEqual(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.05f;
        }

        public static bool HasVertexColors(Mesh mesh)
        {
            Color[] colors = mesh.colors;

            bool hasColor = false;
            foreach (Color color in colors)
            {
                if (color != Color.clear)
                {
                    hasColor = true;
                    break;
                }
            }

            if (DEBUG) Debug.Log("Mesh: " + mesh.name + " has vertex colors: " + hasColor);
            return hasColor;
        }

        public static class CameraUtils
        {
            // Given position/normal of the plane, calculates plane in camera space.
            public static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float clipPlaneOffset)
            {
                Vector3 offsetPos = pos + normal * clipPlaneOffset;
                Matrix4x4 m = cam.worldToCameraMatrix;
                Vector3 cpos = m.MultiplyPoint(offsetPos);
                Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
                return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
            }

            // Calculates reflection matrix around the given plane
            public static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
            {
                reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
                reflectionMat.m01 = (-2F * plane[0] * plane[1]);
                reflectionMat.m02 = (-2F * plane[0] * plane[2]);
                reflectionMat.m03 = (-2F * plane[3] * plane[0]);

                reflectionMat.m10 = (-2F * plane[1] * plane[0]);
                reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
                reflectionMat.m12 = (-2F * plane[1] * plane[2]);
                reflectionMat.m13 = (-2F * plane[3] * plane[1]);

                reflectionMat.m20 = (-2F * plane[2] * plane[0]);
                reflectionMat.m21 = (-2F * plane[2] * plane[1]);
                reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
                reflectionMat.m23 = (-2F * plane[3] * plane[2]);

                reflectionMat.m30 = 0F;
                reflectionMat.m31 = 0F;
                reflectionMat.m32 = 0F;
                reflectionMat.m33 = 1F;
            }

            public static void CopyCameraSettings(Camera src, Camera dest)
            {
                if (dest == null) return;

                dest.clearFlags = src.clearFlags;
                dest.backgroundColor = src.backgroundColor;

                dest.farClipPlane = src.farClipPlane;
                dest.nearClipPlane = src.nearClipPlane;

                dest.fieldOfView = src.fieldOfView;
                dest.aspect = src.aspect;

                dest.orthographic = src.orthographic;
                dest.orthographicSize = src.orthographicSize;

                dest.renderingPath = src.renderingPath;
                dest.targetDisplay = src.targetDisplay;
            }
        }

    }
#if UNITY_EDITOR
    public static class TexturePacker
    {
        #region Variables
        //Normal map
        private static Shader m_NormalRenderShader;
        private static Shader NormalRenderShader
        {
            get
            {
                if (m_NormalRenderShader == null)
                {
                    m_NormalRenderShader = Shader.Find("Hidden/SWS/NormalMapRenderer");
                    return m_NormalRenderShader;
                }
                else
                {
                    return m_NormalRenderShader;
                }
            }
        }

        private static Material m_NormalRenderMat;
        private static Material NormalRenderMat
        {
            get
            {
                if (m_NormalRenderMat == null)
                {
                    m_NormalRenderMat = new Material(NormalRenderShader);
                    return m_NormalRenderMat;
                }
                else
                {
                    return m_NormalRenderMat;
                }
            }
        }

        private const float NORMAL_STRENGTH = 6f;
        private const float NORMAL_OFFSET = 0.005f;

        //Shader map
        private static Shader m_ShaderMapShader;
        private static Shader ShaderMapShader
        {
            get
            {
                if (m_ShaderMapShader == null)
                {
                    m_ShaderMapShader = Shader.Find("Hidden/SWS/ShaderMapRenderer");
                    return m_ShaderMapShader;
                }
                else
                {
                    return m_ShaderMapShader;
                }
            }
        }

        private static Material m_ShaderMapRenderMat;
        private static Material ShaderMapRenderMat
        {
            get
            {
                if (m_ShaderMapRenderMat == null)
                {
                    m_ShaderMapRenderMat = new Material(ShaderMapShader);
                    return m_ShaderMapRenderMat;
                }
                else
                {
                    return m_ShaderMapRenderMat;
                }
            }
        }
        #endregion

        //Fixed for now, max texture size can be overridden in the inspector by user
        private static int SHADERMAP_RESOLUTION = 1024;
        private static int NORMAL_RESOLUTION = 1024;
        private static int GRADIENT_RESOLUTION = 32;

        public static bool useCompression = false;

        #region Texture generation
        //TODO (Polish): Use a struct
        public static Texture2D RenderShaderMap(Material targetMaterial, int intersectionStyle, int waveStyle, int heightmapStyle, bool useCompression = false, Texture2D customIntersectionTex = null, Texture2D customHeightmapTex = null)
        {
            StylizedWaterResources r = StylizedWaterResources.Instance;

            //Set compression setting
            TexturePacker.useCompression = useCompression;

            //Intersection
            Texture2D intersectionTex;
            if (customIntersectionTex)
            {
                intersectionTex = customIntersectionTex;
            }
            else
            {
                intersectionTex = r.intersectionStyles[intersectionStyle];
            }

            //Foam
            Texture2D foamTex;
            //When a custom normal map is used, force the usage of the intersection texture for foam
            if (waveStyle == r.waveStyles.Length)
            {
                foamTex = Texture2D.blackTexture;
            }
            else
            {
                foamTex = r.waveStyles[waveStyle];
            }

            //Heightmap
            Texture2D heightmapTex;
            if (customHeightmapTex)
            {
                heightmapTex = customHeightmapTex;
            }
            else
            {
                heightmapTex = r.heightmapStyles[heightmapStyle];
            }

            Texture2D shadermap = new Texture2D(SHADERMAP_RESOLUTION, SHADERMAP_RESOLUTION, TextureFormat.RGB24, true)
            {
                name = "_shadermap", //Prefix and suffix to be appended upon saving
                anisoLevel = 2,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat
            };

            RenderTexture rt = new RenderTexture(shadermap.width, shadermap.width, 0);
            RenderTexture.active = rt;

            //Constants
            ShaderMapRenderMat.SetTexture("_RedInput", foamTex);
            ShaderMapRenderMat.SetTexture("_GreenInput", heightmapTex);
            ShaderMapRenderMat.SetTexture("_BlueInput", intersectionTex);
            //ShaderMapRenderMat.SetTexture("_AlphaInput", null);

            //Pack textures on GPU
            Graphics.Blit(null, rt, ShaderMapRenderMat);

            //Copy result into texture
            shadermap.ReadPixels(new Rect(0, 0, shadermap.width, shadermap.height), 0, 0);
            shadermap.Apply();

            //Cleanup
            RenderTexture.active = null;

            shadermap = SaveAndGetTexture(targetMaterial, shadermap);

            return shadermap;
        }

        public static Texture2D RenderNormalMap(Material targetMaterial, int waveStyle, bool useCompression = false, Texture2D customNormalTex = null)
        {
            //If a custom texture is assigned, return that
            if (customNormalTex) return customNormalTex;

            StylizedWaterResources r = StylizedWaterResources.Instance;

            //Set compression setting
            TexturePacker.useCompression = useCompression;

            Texture2D heightmap = r.waveStyles[waveStyle];

            Texture2D normalTexture = new Texture2D(NORMAL_RESOLUTION, NORMAL_RESOLUTION, TextureFormat.ARGB32, true)
            {
                name = "_normal",
                filterMode = FilterMode.Trilinear,
                wrapMode = TextureWrapMode.Repeat
            };

            RenderTexture rt = new RenderTexture(normalTexture.width, normalTexture.height, 0);
            RenderTexture.active = rt;

            //Constants
            NormalRenderMat.SetFloat("_Offset", NORMAL_OFFSET);
            NormalRenderMat.SetFloat("_Strength", NORMAL_STRENGTH);

            //Convert heightmap to normal on GPU
            Graphics.Blit(heightmap, rt, NormalRenderMat);

            //Copy result into texture
            normalTexture.ReadPixels(new Rect(0, 0, normalTexture.width, normalTexture.height), 0, 0);
            normalTexture.Apply();

            //Cleanup
            RenderTexture.active = null;

            normalTexture = SaveAndGetTexture(targetMaterial, normalTexture);

            return normalTexture;
        }

        public static Texture2D GenerateGradientMap(Material targetMaterial, Gradient gradient)
        {
            Texture2D m_gradientTex = null;

            m_gradientTex = new Texture2D(GRADIENT_RESOLUTION, 1, TextureFormat.ARGB32, false)
            {
                name = "_gradient",
                filterMode = FilterMode.Bilinear, //Smooth interpolation
                wrapMode = TextureWrapMode.Clamp //Repeat far right pixel
            };

            for (int x = 0; x < GRADIENT_RESOLUTION; x++)
            {
                Color gradientPixel = gradient.Evaluate(x / (float)GRADIENT_RESOLUTION);
                m_gradientTex.SetPixel(x, 1, gradientPixel);
            }

            m_gradientTex.Apply();

            m_gradientTex = SaveAndGetTexture(targetMaterial, m_gradientTex);

            return m_gradientTex;
        }

        /// <summary>
        /// Save the sourceTexture in a "Textures" folder next to the targetMaterial
        /// And returns the file reference
        /// </summary>
        /// <param name="targetMaterial">File is stored next to this material</param>
        /// <param name="sourceTexture">Texture to be saved</param>
        /// <returns>Reference to saved texture file</returns>
        private static Texture2D SaveAndGetTexture(Material targetMaterial, Texture2D sourceTexture)
        {
            //Material root folder
            string targetFolder = AssetDatabase.GetAssetPath(targetMaterial);
            targetFolder = targetFolder.Replace(targetMaterial.name + ".mat", string.Empty);

            //Append textures folder
            targetFolder += "Textures/";

            //Create Textures folder if it doesn't exist
            if (!Directory.Exists(targetFolder))
            {
                Debug.Log("[Stylized Water] Directory: " + targetFolder + " didn't exist and was created.");
                Directory.CreateDirectory(targetFolder);

                AssetDatabase.Refresh();
            }

            //Compose file path
            string path = targetFolder + targetMaterial.name + sourceTexture.name + "_baked.png";

            File.WriteAllBytes(path, sourceTexture.EncodeToPNG());

            if (StylizedWaterUtilities.DEBUG) Debug.Log("Written file to: " + path);

            AssetDatabase.Refresh();

            //Trigger SWSImporter
            AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);

            //Grab the file
            sourceTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

            //Return file reference
            return sourceTexture;
        }
        #endregion
    }

    //Catch the normal map when it is being imported and flag it accordingly
    internal sealed class SWSImporter : AssetPostprocessor
    {
        TextureImporter textureImporter;

        private void OnPreprocessTexture()
        {
            textureImporter = assetImporter as TextureImporter;

            //Only run for SWS created textures, which have the _baked suffix
            if (!assetPath.Contains("_baked")) return;

            if (TexturePacker.useCompression)
            {
#if UNITY_5_5_OR_NEWER
                textureImporter.textureType = TextureImporterType.Default;
#else
                textureImporter.textureType = TextureImporterType.Advanced;
#endif
#if UNITY_5_6_OR_NEWER
            textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
#else
                textureImporter.textureFormat = TextureImporterFormat.PVRTC_RGB2;
#endif
            }
            else
            {
#if UNITY_5_6_OR_NEWER
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
#else
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif
            }

            //Look for the given name, this will also apply to textures outside of the SWS package, but it's unlikely this naming convention will occur
            if (assetPath.Contains("_normal_baked"))
            {
#if !UNITY_5_5_OR_NEWER
                textureImporter.normalmap = true;
#endif
#if UNITY_5_5_OR_NEWER
                textureImporter.textureType = TextureImporterType.NormalMap;
#else
                textureImporter.textureType = TextureImporterType.Bump;
#endif
                textureImporter.wrapMode = TextureWrapMode.Repeat;

            }
            else if (assetPath.Contains("_shadermap_baked"))
            {
#if !UNITY_5_5_OR_NEWER
                textureImporter.normalmap = false;
#endif
                textureImporter.alphaIsTransparency = false;
                textureImporter.wrapMode = TextureWrapMode.Repeat;
#if UNITY_5_5_OR_NEWER
                textureImporter.textureType = TextureImporterType.Default;
#else
                textureImporter.textureType = TextureImporterType.Image;
#endif
            }
            else if (assetPath.Contains("_gradient_baked"))
            {
                textureImporter.wrapMode = TextureWrapMode.Clamp;
            }

        }
    }
#endif

}//Namespace
