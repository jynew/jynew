using System;
using System.IO;
using System.Net;
using Jyx2.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityToolbarExtender.Examples;

namespace Jyx2Editor
{
    public class Jyx2MenuItems 
    {
        [MenuItem("配置管理器/技能编辑器")]
        private static void OpenSkillEditor()
        {
            SceneHelper.StartScene("Assets/Jyx2BattleScene/Jyx2SkillEditor.unity");
        }
        
        [MenuItem("配置管理器/全模型预览")]
        private static void OpenAllModels()
        {
            SceneHelper.StartScene("Assets/3D/AllModels.unity");
        }

        [MenuItem("一键打包/Windows64")]
        private static void BuildWindows64()
        {
            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "jyx2Win64Build");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
        

            if (string.IsNullOrEmpty(path))
                return;
            
            //重新生成Addressable相关文件
            AddressableAssetSettings.BuildPlayerContent();
            
            //强制GENDATA
            GenDataMenuCmd.GenerateDataForce();

            // 处理场景文件
            AddScenesToBuildTool.AddScenesToBuild();

            //打包
            BuildPipeline.BuildPlayer(GetScenePaths(), path + "/jyx2.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
            
            //强制移动目录
            //System.IO.Directory.Move("StandaloneWindows64", path + "/StandaloneWindows64");

            EditorUtility.DisplayDialog("打包完成", "输出目录:" + path, "确定");
        }
        
        static string[] GetScenePaths() {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for(int i = 0; i < scenes.Length; i++) {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            return scenes;
        }
        
        [MenuItem("一键打包/Android")]
        private static void BuildAndroid()
        {
            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        

            if (string.IsNullOrEmpty(path))
                return;
            
            //重新生成Addressable相关文件
            AddressableAssetSettings.BuildPlayerContent();
            
            
            //强制GENDATA
            GenDataMenuCmd.GenerateDataForce();

            // 处理场景文件
            AddScenesToBuildTool.AddScenesToBuild();

            string apkPath = path + $"/jyx2AndroidBuild-{DateTime.Now.ToString("yyyyMMdd")}.apk";
            
            //打包
            BuildPipeline.BuildPlayer(GetScenePaths(), apkPath, BuildTarget.Android, BuildOptions.None);
            
            //强制移动目录
            //System.IO.Directory.Move("StandaloneWindows64", path + "/StandaloneWindows64");

            EditorUtility.DisplayDialog("打包完成", "输出文件:" + apkPath, "确定");
        }
    }
}