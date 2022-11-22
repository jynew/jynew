using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Editor;
using Jyx2.MOD.ModV2;
using UnityEditor;
using UnityEngine;
using Tools = Jyx2.Middleware.Tools;

namespace Jyx2Editor.BuildTool
{
    struct ModBuildItem
    {
        public string ModId;

        public string ModBundleName;

        public string MapBundleName;
    }

    
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
            m_BuildTargetPlatForm = (BuildTarget)EditorPrefs.GetInt("Jyx2_ModExportBuildTarget", (int)BuildTarget.StandaloneWindows);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EditorPrefs.SetString("Jyx2_ModExportPath", m_ExportPath);
            EditorPrefs.SetInt("Jyx2_ModExportBuildTarget", (int)m_BuildTargetPlatForm);
        }


        [AssetSelector]
        [OdinSerialize]
        //[VerticalGroup("ExportVertical")]
        [LabelText("Mod配置文件")]
        private List<MODRootConfig> m_AllModConfigs = new List<MODRootConfig>();

        [OdinSerialize]
        //[VerticalGroup("ExportVertical")]
        [LabelText("导出平台")]
        private BuildTarget m_BuildTargetPlatForm = BuildTarget.StandaloneWindows64;

        [OdinSerialize]
        [FolderPath(AbsolutePath = true)]
        [LabelText("Mod导出路径")]
        private string m_ExportPath;

        private List<ModBuildItem> m_BundleItemsToBuild = new List<ModBuildItem>();


        //[VerticalGroup("ExportVertical")]
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
            var allBundles = AssetDatabase.GetAllAssetBundleNames().ToList();
            if (!allBundles.Contains("base_assets"))
            {
                var msg = string.Format("未找到名为base_assets的基础资源包，导出失败");
                EditorUtility.DisplayDialog("提示", msg, "确定");
                return;
            }


            ClearModBuildItems();
            foreach (var modConfig in m_AllModConfigs)
            {
                RegisterModForExport(modConfig);
            }
            
            //Assetbundle 打包
            BuildAllAssetBundles();

            //将打好分到对应文件夹
            MoveModBundlesToModFolder();

            EditorUtility.DisplayDialog("提示", "导出结束", "确定");
        }

        private void RegisterModForExport(MODRootConfig config)
        {
            if (string.IsNullOrEmpty(config.ModId))
            {
                var msg = string.Format("[{0}]的ModId为空, 将不会导出", config.ModName);
                EditorUtility.DisplayDialog("提示", msg, "确定");
                return;
            }
            var allBundles = AssetDatabase.GetAllAssetBundleNames().ToHashSet();
            var modId = config.ModId.ToLower();
            var mapBundleName = GetModMapBundleName(modId);
            var modBundleName = GetModMainBundleName(modId);
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
            EnsureModExportDirectoryExist(modId);
            GenerateModXmlFile(config);
            AddModToBuildList(modId, mapBundleName, modBundleName);
        }

        private void ClearModBuildItems()
        {
            m_BundleItemsToBuild.Clear();
        }

        private void AddModToBuildList(string modId, string mapBundleName, string modBundleName)
        {
            if (m_BundleItemsToBuild.Exists(element => element.ModId == modId))
                return;
            var item = new ModBuildItem()
            {
                ModId = modId,
                MapBundleName = mapBundleName,
                ModBundleName = modBundleName,
            };
            m_BundleItemsToBuild.Add(item);
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
        
        private string[] GetValidAssetPathsInBundle(string bundle)
        {
            var results = AssetDatabase.GetAssetPathsFromAssetBundle(bundle).
                Where(assetPath => Path.GetExtension(assetPath) != ".cs").ToArray();
            return results;
        }
        
        private void BuildAllAssetBundles()
        {
            if (m_BundleItemsToBuild.Count == 0)
                return;


            var builds = new List<AssetBundleBuild>();

            //先把基础包放进去
            var baseBuild = new AssetBundleBuild();
            baseBuild.assetBundleName = "base_assets";
            baseBuild.assetNames = GetValidAssetPathsInBundle("base_assets");
            builds.Add(baseBuild);

            //再放mod
            foreach (var bundleItem in m_BundleItemsToBuild)
            {
                var modBuild = BundleName2BundleBuild(bundleItem.ModBundleName);
                var mapbuild = BundleName2BundleBuild(bundleItem.MapBundleName);

                builds.Add(modBuild);
                builds.Add(mapbuild);
                
                Debug.Log("assetBundle to build:" + modBuild.assetBundleName);
                Debug.Log("assetBundle to build:" + mapbuild.assetBundleName);
            }
            BuildPipeline.BuildAssetBundles(m_ExportPath, 
                                            builds.ToArray(), 
                                            BuildAssetBundleOptions.ChunkBasedCompression,
                                            m_BuildTargetPlatForm);
        }

        private AssetBundleBuild BundleName2BundleBuild(string bundleName)
        {
            var assetPaths = GetValidAssetPathsInBundle(bundleName);
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = assetPaths;
            return build;
        }    

        private void GenerateModXmlFile(MODRootConfig config)
        {
            var modId = config.ModId.ToLower().Trim();
            var modInfo = JynewBuilder.CreateModInfo(config, m_BuildTargetPlatForm);
            
            var xmlContent = Tools.SerializeXML(modInfo);
            var xmlPath = Path.Combine(GetModExportDirectory(modId), modId + ".xml");
            File.WriteAllText(xmlPath, xmlContent);
        }

        private void MoveModBundlesToModFolder()
        {
            foreach (var bundleItem in m_BundleItemsToBuild)
            {
                var modFolder = GetModExportDirectory(bundleItem.ModId);
                var modId = bundleItem.ModId.ToLower();
                var files = Directory.GetFiles(m_ExportPath, "*" + bundleItem.ModId + "*", SearchOption.TopDirectoryOnly);
                foreach(var file in files)
                {
                    var destination = Path.Combine(modFolder, Path.GetFileName(file));
                    if (File.Exists(destination))
                        File.Delete(destination);
                    File.Move(file, destination);
                }
            }
        }
    }
}