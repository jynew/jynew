#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MTE
{
    public enum TerrainMaterialType
    {
        BuiltInStandard,
        BuiltInLegacyDiffuse,
        BuiltInLegacySpecular,
        Custom,
        LWRPTerrainLit_Removed,
        URPTerrainLit,
        Unknown,
    }

    public static class CompatibilityUtil
    {
        public static TerrainLayer[] GetSplatLayers(TerrainData terrainData)
        {
            return terrainData.terrainLayers;
        }
        public static void SetSplatLayers(TerrainData terrainData, TerrainLayer[] layers)
        {
            terrainData.terrainLayers = layers;
        }
        public static Texture2D GetSplatLayerNormalMap(TerrainLayer splatLayer)
        {
            if (!splatLayer)
            {
                return null;
            }
            return splatLayer.normalMapTexture;
        }
        public static Texture2D GetSplatLayerDiffuseTexture(TerrainLayer splatLayer)
        {
            if (!splatLayer)
            {
                return null;
            }
            return splatLayer.diffuseTexture;
        }
        public static float GetSplatLayerMetallic(TerrainLayer splatLayer)
        {
            if (!splatLayer)
            {
                return 0;
            }
            return splatLayer.metallic;
        }
        public static float GetSplatLayerSmoothness(TerrainLayer splatLayer)
        {
            if (!splatLayer)
            {
                return 0;
            }
            return splatLayer.smoothness;
        }

        public static Texture2D GetMaskTexture(TerrainLayer splatLayer)
        {
            if (!splatLayer)
            {
                return null;
            }
            return splatLayer.maskMapTexture;
        }

        public static TerrainLayer[] CreateSplatLayers(int number)
        {
            return new TerrainLayer[number];
        }

        public static TerrainLayer CreateSplatLayer()
        {
            return new TerrainLayer();
        }

        public static void SetSplatLayerDiffueTexture(TerrainLayer splatLayer, Texture2D diffuseTexture)
        {
            splatLayer.diffuseTexture = diffuseTexture;
        }
        public static void SetSplatLayerNormalMap(TerrainLayer splatLayer, Texture2D normalMap)
        {
            splatLayer.normalMapTexture = normalMap;
        }

        public static void SaveSplatLayer(TerrainLayer splat, string name)
        {
            AssetDatabase.CreateAsset(splat, name);
        }

        /// <summary>
        /// Create a prefab file for a GameObject.
        /// </summary>
        /// <param name="obj">the GameObject</param>
        /// <param name="relativePath">Unity file path of the prefab</param>
        public static void CreatePrefab(GameObject obj, string relativePath)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, relativePath);
        }

        /// <summary>
        /// Check if a gameObject is the root of a instantiated prefab.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool IsPrefab(GameObject gameObject)
        {
            return PrefabUtility.IsAnyPrefabInstanceRoot(gameObject);
        }

        public static bool IsInstanceOfPrefab(GameObject obj, GameObject prefab)
        {
            return PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) == prefab;
        }
        
        public static Object GetPrefabRoot(GameObject instance)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(instance);
        }

        //This field will invoke a compile error if this version isn't officially supported by MTE.
        public const string VersionCheckString =
#if UNITY_2018_4
                "2018.4"
#elif UNITY_2019_3
                    "2019.3"
#elif UNITY_2019_4
                    "2019.4"
#elif UNITY_2020_1
                    "2020.1"
#elif UNITY_2020_2
                    "2020.2"
#elif UNITY_2020_3
                    "2020.3"
#elif UNITY_2021_1
                    "2021.1"
#elif UNITY_2021_2
                    "2021.2"
#elif UNITY_2021_3_OR_NEWER
#warning Might be a unsupported Unity Version, not tested yet.
                    "2021.3+"
#else
#error Unsupported Unity Version. You can try to fix this file or report the issue.
#endif
            ;
        

        public static bool IsWebRequestError(UnityEngine.Networking.UnityWebRequest request)
        {
#if UNITY_2018_4 || UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1
            return request.isNetworkError || request.isHttpError;
#elif UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            return request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError
                || request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError;
#else
#error not supported on any Unity build targets
#endif
        }

        public static object AttachOnSceneGUI(System.Action<SceneView> action)
        {
#if UNITY_2018_4
            SceneView.OnSceneFunc func = view => action(view);
            SceneView.onSceneGUIDelegate += func;
            return func;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            SceneView.duringSceneGui += action;
            return action;
#else
#error not supported on any Unity build targets
#endif
        }

        public static void DetachOnSceneGUI(object cachedOnSceneGUI)
        {
#if UNITY_2018_4
            SceneView.OnSceneFunc func = (SceneView.OnSceneFunc)cachedOnSceneGUI;
            SceneView.onSceneGUIDelegate -= func;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            var action = (System.Action<SceneView>)cachedOnSceneGUI;
            SceneView.duringSceneGui -= action;
#else
#error not supported on any Unity build targets
#endif
        }

        public static void DontOptimizeMesh(ModelImporter importer)
        {
#if UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            importer.optimizeMeshVertices = false;
            importer.optimizeMeshPolygons = false;
            importer.weldVertices = false;
#elif UNITY_2018_4
            importer.optimizeMesh = false;
#else
#error not supported on any Unity build targets
#endif
        }

        public static TerrainMaterialType GetTerrainMaterialType(Terrain terrain)
        {
#if UNITY_2018_4
            switch (terrain.materialType)
            {
                case Terrain.MaterialType.BuiltInStandard:
                    return TerrainMaterialType.BuiltInStandard;
                case Terrain.MaterialType.BuiltInLegacyDiffuse:
                    return TerrainMaterialType.BuiltInLegacyDiffuse;
                case Terrain.MaterialType.BuiltInLegacySpecular:
                    return TerrainMaterialType.BuiltInLegacySpecular;
                case Terrain.MaterialType.Custom:
                    return TerrainMaterialType.Custom;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            var material = terrain.materialTemplate;
            if (material == null)
            {
                Debug.LogErrorFormat(
                    "Terrain<{0}> is using an empty material." +
                    "The conversion result is probably not right." +
                    " Please check the material of the terrain",
                    terrain.name);
                return TerrainMaterialType.Custom;
            }

            if (material.name == "Default-Terrain-Standard")
            {
                return TerrainMaterialType.BuiltInStandard;
            }
            if (material.name == "Default-Terrain-Diffuse")
            {
                return TerrainMaterialType.BuiltInLegacyDiffuse;
            }
            if (material.name == "Default-Terrain-Specular")
            {
                return TerrainMaterialType.BuiltInLegacySpecular;
            }
            if (material.shader != null
                && material.shader.name == "Lightweight Render Pipeline/Terrain/Lit")
            {
                return TerrainMaterialType.LWRPTerrainLit_Removed;
            }
            if (material.shader != null
                && material.shader.name == "Universal Render Pipeline/Terrain/Lit")
            {
                return TerrainMaterialType.URPTerrainLit;
            }

            var materialFileRelativePath = AssetDatabase.GetAssetPath(material);
            Debug.LogErrorFormat(
                "Terrain<{0}> is using a material<{1}> at {2} that is unknown to MTE." +
                "The conversion result is probably not right.",
                terrain.name, material.name, materialFileRelativePath);

            return TerrainMaterialType.Unknown;
#else
#error not supported on any Unity build targets
#endif
        }

        public static Color GetTerrainMaterialSpecularColor(Terrain terrain)
        {
#if UNITY_2018_4
            return terrain.legacySpecular;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            var material = terrain.materialTemplate;
            return material.GetColor("_SpecColor");
#else
#error not supported on any Unity build targets
#endif
        }

        public static float GetTerrainMaterialShininess(Terrain terrain)
        {
#if UNITY_2018_4
            return terrain.legacyShininess;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            var material = terrain.materialTemplate;
            return material.GetFloat("_Shininess");
#else
#error not supported on any Unity build targets
#endif
        }

        public static int GetHeightmapWidth(TerrainData terrainData)
        {
#if UNITY_2018_4
            return terrainData.heightmapWidth;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            return terrainData.heightmapResolution;
#endif
        }

        public static int GetHeightmapHeight(TerrainData terrainData)
        {
#if UNITY_2018_4
            return terrainData.heightmapHeight;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            return terrainData.heightmapResolution;
#endif
        }

        public static void SetTerrainMaterialType(Terrain terrain, TerrainMaterialType materialType)
        {
            switch (materialType)
            {
                case TerrainMaterialType.BuiltInStandard:
                {
#if UNITY_2018_4
                    terrain.materialType = Terrain.MaterialType.BuiltInStandard;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
                    var material = Resources.Load<Material>("unity_builtin_extra/Default-Terrain-Standard");
                    terrain.materialTemplate = material;
#else
#error not supported on any Unity build targets
#endif
                }
                    break;
                case TerrainMaterialType.BuiltInLegacyDiffuse:
                {
#if UNITY_2018_4
                    terrain.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
                    var material = Resources.Load<Material>("unity_builtin_extra/Default-Terrain-Diffuse");
                    terrain.materialTemplate = material;
#else
#error not supported on any Unity build targets
#endif
                }
                    break;
                case TerrainMaterialType.BuiltInLegacySpecular:
                {
#if UNITY_2018_4
                    terrain.materialType = Terrain.MaterialType.BuiltInLegacySpecular;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
                    var material = Resources.Load<Material>("unity_builtin_extra/Default-Terrain-Specular");
                    terrain.materialTemplate = material;
#else
#error not supported on any Unity build targets
#endif
                }
                    break;
                default:
                    throw new System.NotSupportedException("Cannot set terrain material.");
            }
        }

        //Unity 2021.2.0 changed
        //https://unity3d.com/unity/whats-new/2021.2.0
        //Graphics: Changed: Renamed Texture2D.Resize to Reinitialize.
        public static void ReinitializeTexture2D(Texture2D texture, int newWidth, int newHeight)
        {
#if UNITY_2018_4 || UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1
            texture.Resize(newWidth, newHeight);
#elif UNITY_2021_2_OR_NEWER
            texture.Reinitialize(newWidth, newHeight);
#endif
        }

        public static byte[] EncodeTextureToPNG(Texture2D texture)
        {
            return ImageConversion.EncodeToPNG(texture);
        }

        public static bool GetTextureReadable(Texture texture)
        {
            return texture.isReadable;
        }

        public static bool BeginFoldoutHeaderGroup(bool foldout, string content)
        {
#if UNITY_2018_4
            return GUILayout.Toggle(foldout, content, "button");
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            return EditorGUILayout.BeginFoldoutHeaderGroup(foldout, content);
#else
#error not supported on any Unity build targets
#endif
        }

        public static void EndFoldoutHeaderGroup()
        {
#if UNITY_2018_4
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            EditorGUILayout.EndFoldoutHeaderGroup();
#else
#error not supported on any Unity build targets
#endif
        }

        //FBXExporter support
        public static System.Func<string, UnityEngine.Object, string> exportFbxMethod = null;
        public static bool InitFbxExport()
        {
            var ModelExporterType = System.Type.GetType("UnityEditor.Formats.Fbx.Exporter.ModelExporter, Unity.Formats.Fbx.Editor");
            if (ModelExporterType == null)
            {
                return false;
            }
            var method = ModelExporterType.GetMethod("ExportObject");
            if (method == null)
            {
                Debug.LogError("Failed to fetch the method: UnityEditor.Formats.Fbx.Exporter.ModelExporter.");
                return false;
            }

            exportFbxMethod = (System.Func<string, UnityEngine.Object, string>)System.Delegate.CreateDelegate(typeof(System.Func<string, UnityEngine.Object, string>), method);
            //Debug.Log("Successfully fetched the method: UnityEditor.Formats.Fbx.Exporter.ModelExporter().");
            return true;
        }
        
        private static Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();

            return result;
        }

        public static GUIStyle GetGridListStyle()
        {
            var editorLabelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label;
            var gridListStyle = new GUIStyle(editorLabelStyle)
            {
                fixedWidth = 0,
                fixedHeight = 0,
                stretchWidth = true,
                stretchHeight = true,
                alignment = TextAnchor.MiddleCenter
            };

            Color32 hoverColor = Color.grey;
            Color32 selectedColor = new Color32(62, 125, 231, 255);
            gridListStyle.onHover.background = MakeTexture(1, 1, selectedColor);
            gridListStyle.hover.background = MakeTexture(1, 1, hoverColor);
            gridListStyle.normal.background = MakeTexture(1, 1, Color.clear);
            gridListStyle.onNormal.background = MakeTexture(1, 1, selectedColor);

            //gridListStyle.hover.background = MakeTexture(1, 1, new Color32(255, 255, 255, 62));
            //gridListStyle.onNormal.background = MakeTexture(1, 1, new Color32(62, 125, 231, 255));
            return gridListStyle;
        }

        public static object GetRenderPipelineAsset()
        {
            return UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        }

        public static void SetTextureFormatToA8(TextureImporterPlatformSettings textureSettings)
        {
            textureSettings.format = TextureImporterFormat.R8;
        }

        public static int FindPropertyIndex(Shader shader, string propertyName)
        {
#if UNITY_2018_4
            var propertyCount = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < propertyCount; i++)
            {
                var name = ShaderUtil.GetPropertyName(shader, i);
                if (name == propertyName)
                {
                    return i;
                }
            }
            return -1;
#elif UNITY_2019_3 || UNITY_2019_4 || UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3 || UNITY_2020_3 || UNITY_2021_1 || UNITY_2021_2_OR_NEWER
            return shader.FindPropertyIndex(propertyName);
#else
#error not supported on any Unity build targets
#endif
        }

        public class GUI
        {
            public static float Slider(
                Rect position,
                float value,
                float size,
                float start,
                float end,
                GUIStyle slider,
                GUIStyle thumb,
                bool horiz,
                int id)
            {
                return UnityEngine.GUI.Slider(
                    position,
                    value,
                    size,
                    start,
                    end,
                    slider,
                    thumb,
                    horiz,
                    id);
            }
        }
    }
}

#endif