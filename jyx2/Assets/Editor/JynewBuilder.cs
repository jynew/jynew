using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSObjectWrapEditor;
using Jyx2.MOD.ModV2;
using UnityEditor;
using UnityEngine;
using Tools = Jyx2.Middleware.Tools;

namespace Editor
{
    /// <summary>
    /// 打包工具
    ///
    /// 知大虾 2022/12/03 整体重构
    ///
    /// - 所有的ab包临时生成在TemAbDir目录中，按不同平台区分
    /// - 不同平台的ab包如果没有修改，不会重复进行生成，这样可以提高打包效率
    /// - 所有勾选“原生MOD”的模组，将会随包发布（在Assert/Mods/{Your Mod Id}/ModSetting.asset中进行配置）
    /// 
    /// </summary>
    public class JynewBuilder
    {
        private string TempAbDir = "Temp/jynew";
        private const string StreamingAssetsDir = "Assets/StreamingAssets";
        
        public void Build(BuildTarget target, string defaultDirName, string defaultBuildFileName, BuildOptions options = BuildOptions.None)
        {
            TempAbDir = Path.Combine("Temp/jynew", target.ToString());
            if (!Directory.Exists(TempAbDir))
            {
                Directory.CreateDirectory(TempAbDir);
            }
            
            try
            {
                string path = EditorUtility.SaveFolderPanel("选择输出目录", "..", defaultDirName);
                if (string.IsNullOrEmpty(path))
                    return;

                //设置版本号
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                PlayerSettings.bundleVersion = currentDate;

                //清空StreamingAssets目录
                if (Directory.Exists(StreamingAssetsDir))
                {
                    Directory.Delete(StreamingAssetsDir, true);
                    Directory.CreateDirectory(StreamingAssetsDir);
                }
                
                //生成原生MOD的索引文件
                var nativeMods = BuildNativeModsIndexFiles(target);

                //生成xlua
                Generator.GenAll();
                EditorUtility.RequestScriptReload();

                //在临时目录生成ab包
                GenerateAssetBundlesInTempDirectory(target);

                //拷贝主文件
                DoCopyCoreAssetBundles();
                
                //拷贝MOD文件
                DoCopyNativeModFiles(nativeMods);
                
                //打包
                string buildPath = Path.Combine(path, defaultBuildFileName);
                BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, target, options);

                //输出
                Debug.Log($"<color=green>打包完成！目标平台={target} 生成位置={buildPath}</color>");
                Tools.openURL(path);
            }
            catch (Exception e)
            {
                Debug.LogError("打包失败：" + e.ToString());
            }
        }

        private List<string> BuildNativeModsIndexFiles(BuildTarget target)
        {
            List<string> nativeMods = new List<string>();
            var editorModLoader = new GameModEditorLoader();
            var modList = editorModLoader.LoadModsSync();
            foreach (var mod in modList)
            {
                var editorMod = (GameModEditor) mod;
                if (editorMod == null) continue;
                var modConfig = editorMod.RootConfig;

                if (modConfig == null) continue; 

                var modInfo = CreateModInfo(modConfig, target);

                var xmlContent = Tools.SerializeXML(modInfo);
                
                //给临时目录也写一份
                string tempDirPath = Path.Combine(TempAbDir, modInfo.Id + ".xml");
                File.WriteAllText(tempDirPath, xmlContent);
                
                if (modConfig.IsNativeMod)
                {
                    string targetXmlPath = Path.Combine(StreamingAssetsDir, modInfo.Id + ".xml");
                    File.WriteAllText(targetXmlPath, xmlContent);    
                    nativeMods.Add(modInfo.Id);
                }
            }

            if (nativeMods.Count > 0)
            {
                string nativeModListFilePath = Path.Combine(StreamingAssetsDir, "native_mods.txt");
                File.WriteAllText(nativeModListFilePath, string.Join(",", nativeMods.ToList()));
            }
            else
            {
                Debug.LogError("没有找到任何一个原生MOD，请检查MOD下是否至少有一个勾选了原生MOD标签");
            }

            return nativeMods;
        }

        public static GameModInfo CreateModInfo(MODRootConfig config, BuildTarget target)
        {
            var modInfo = config.CreateModInfo();
            switch (target)
            {
                case BuildTarget.Android:
                    modInfo.Platform = GameModBuildPlatform.Android;
                    break;
                case BuildTarget.StandaloneWindows64:
                    modInfo.Platform = GameModBuildPlatform.Windows;
                    break;
                case BuildTarget.StandaloneOSX:
                    modInfo.Platform = GameModBuildPlatform.MacOS;
                    break;
                case BuildTarget.iOS:
                    modInfo.Platform = GameModBuildPlatform.IOS;
                    break;
                default:
                    modInfo.Platform = GameModBuildPlatform.Unknown;
                    break;
            }

            return modInfo;
        }

        private void DoCopyCoreAssetBundles()
        {
            var abPath = Path.Combine(TempAbDir, $"base_assets");
            var abDestPath = Path.Combine(StreamingAssetsDir, $"base_assets");
            File.Copy(abPath, abDestPath);
        }
        
        private void DoCopyNativeModFiles(List<string> nativeMods)
        {
            foreach (var mod in nativeMods)
            {
                //拷贝相关文件到StreamingAssets下
                var abPath = Path.Combine(TempAbDir, $"{mod}_mod");
                var abDestPath = Path.Combine(StreamingAssetsDir, $"{mod}_mod");
                File.Copy(abPath, abDestPath);

                var mapPath = Path.Combine(TempAbDir, $"{mod}_maps");
                var mapDestPath = Path.Combine(StreamingAssetsDir, $"{mod}_maps");
                File.Copy(mapPath, mapDestPath);
            }
        }

        private void GenerateAssetBundlesInTempDirectory(BuildTarget target)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(TempAbDir);
            if (!dirInfo.Exists)
                dirInfo.Create();

            BuildPipeline.BuildAssetBundles(TempAbDir, BuildAssetBundleOptions.ChunkBasedCompression, target);
        }
    }
}