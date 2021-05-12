// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

namespace StylizedWater
{
    public class StylizedWaterResources : ScriptableObject
    {
        [Header("Intersection textures")]
        public Texture2D[] intersectionStyles;

        [Header("Heightmaps")]
        public Texture2D[] heightmapStyles;

        [Header("Normal maps")]
        public Texture2D[] waveStyles;

        private static StylizedWaterResources m_instance;
        public static StylizedWaterResources Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindResourcesAsset();
                }
                return m_instance;
            }
        }

        public static StylizedWaterResources FindResourcesAsset()
        {
            if (StylizedWaterUtilities.DEBUG) Debug.Log("Looking for Resources asset...");

            StylizedWaterResources resources;

            //Find resources asset
            string[] assets = AssetDatabase.FindAssets("SWS_Resources");
            if (assets.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

                resources = (StylizedWaterResources)AssetDatabase.LoadAssetAtPath(assetPath, typeof(StylizedWaterResources));
            }
            else
            {
                Debug.LogError("[Stylized Water] The \"SWS_Resources\" asset could not be found in the project. Please import the complete package from the Asset Store");
                resources = null;
            }

            return resources;

        }

         //DEV
         /*
        [MenuItem("Assets/Create/SWS Resources")]
        public static void CreateAsset()
        {
            CreateAsset<StylizedWaterResources>();
        }

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        */
    }
}
#endif