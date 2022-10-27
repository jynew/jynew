using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Jyx2Editor.BuildTool
{
    class Jyx2ModExportWindow:OdinEditorWindow
    {
        [MenuItem("Game Tools/Mod导出")]
        public static void OpenWindow()
        {
            var window = GetWindow<Jyx2ModExportWindow>();
            window.titleContent = new GUIContent("Mod 导出");
            window.Show();
        }

        protected void Awake()
        {
            m_ExportPath = EditorPrefs.GetString("Jyx2_ModExportPath", Application.dataPath);
            m_BuildTargetPlatForm = (BuildTarget)EditorPrefs.GetInt("Jyx2_ModExportBuildTarget", (int)BuildTarget.StandaloneWindows64);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EditorPrefs.SetString("Jyx2_ModExportPath", m_ExportPath);
            EditorPrefs.SetInt("Jyx2_ModExportBuildTarget", (int)m_BuildTargetPlatForm);
        }

        [OdinSerialize]
        [VerticalGroup("ExportVertical")]
        [FolderPath(AbsolutePath = true)]
        [LabelText("Mod导出路径")]
        private string m_ExportPath;

        [AssetSelector]
        [OdinSerialize]
        [VerticalGroup("ExportVertical")]
        [LabelText("Mod配置文件")]
        private List<MODRootConfig> m_AllModConfigs = new List<MODRootConfig>();

        [OdinSerialize]
        [VerticalGroup("ExportVertical")]
        [LabelText("导出平台")]
        private BuildTarget m_BuildTargetPlatForm = BuildTarget.StandaloneWindows64;

        [VerticalGroup("ExportVertical")]
        [Button(ButtonHeight = 30, Name = "导出Mod")]
        private void ExportAll()
        {
            if(m_AllModConfigs.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "请先选择Mod配置文件", "确定");
                return;
            }
            if(!Directory.Exists(m_ExportPath))
            {
                EditorUtility.DisplayDialog("提示", "无效的导出路径，请修改后重试", "确定");
                return;
            }
            foreach(var modConfig in m_AllModConfigs)
            {
                ExportSingleMod(modConfig);
            }
            EditorUtility.DisplayDialog("提示", "导出完毕", "确定");
        }

        private void ExportSingleMod(MODRootConfig config)
        {
            if (string.IsNullOrEmpty(config.ModId))
            {
                var msg = string.Format("[{0}]的ModId为空, 将不会导出", config.ModName);
                EditorUtility.DisplayDialog("提示", msg, "确定");
                return;
            }
            var allBundles = AssetDatabase.GetAllAssetBundleNames().ToHashSet();
            var mapBundleName = GetModMapBundleName(config.ModId);
            var modBundleName = GetModMainBundleName(config.ModId);
            if(!allBundles.Contains(mapBundleName))
            {
                var msg = string.Format("未找到[{0}]的地图资源包，请将相关Scene移动到名字为[{1}]的AssetBundle中", config.ModName, mapBundleName);
                EditorUtility.DisplayDialog("提示", msg, "确定");
                return;
            }
            if (!allBundles.Contains(modBundleName))
            {
                var msg = string.Format("未找到[{0}]的Mod资源包，请将相关资源移动到名字为[{1}]的AssetBundle中", config.ModName, modBundleName);
                EditorUtility.DisplayDialog("提示", msg, "确定");
                return;
            }
            EnsureModExportDirectoryExist(config.ModId);
            BuildAssetBundlesByNames(config, mapBundleName, modBundleName);
            GenerateModXmlFile(config);
        }


        private string GetModMapBundleName(string modId)
        {
            modId = modId.ToLower();
            return $"{modId}_maps";
        }

        private string GetModMainBundleName(string modId)
        {
            modId = modId.ToLower();
            return $"{modId}_mod";
        }

        private string GetModExportDirectory(string modId)
        {
            return Path.Combine(m_ExportPath, modId);
        }

        private void EnsureModExportDirectoryExist(string modId)
        {
            var dir = GetModExportDirectory(modId.ToLower());
            if (Directory.Exists(dir))
                return;
            Directory.CreateDirectory(dir);
        }

        private void BuildAssetBundlesByNames(MODRootConfig modConfig, params string[] assetBundleNames)
        {
            if (assetBundleNames == null || assetBundleNames.Length == 0)
            {
                return;
            }

            var builds = new List<AssetBundleBuild>();
            
            foreach (var assetBundle in assetBundleNames)
            {
                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundle);

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = assetBundle;
                build.assetNames = assetPaths;

                builds.Add(build);
                Debug.Log("assetBundle to build:" + build.assetBundleName);
            }
            var bunbleExportFolder = GetModExportDirectory(modConfig.ModId.ToString());
            BuildPipeline.BuildAssetBundles(bunbleExportFolder, 
                                            builds.ToArray(), 
                                            BuildAssetBundleOptions.ChunkBasedCompression, 
                                            m_BuildTargetPlatForm);
        }

        private void GenerateModXmlFile(MODRootConfig config)
        {
            var modId = config.ModId.ToLower();
            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlBuilder.AppendFormat("<ModItem ModId=\"{0}\" Name=\"{1}\"/>", modId, config.ModName);

            var xmlPath = Path.Combine(GetModExportDirectory(modId), "mod.xml");
            File.WriteAllText(xmlPath, xmlBuilder.ToString());
        }
    }
}