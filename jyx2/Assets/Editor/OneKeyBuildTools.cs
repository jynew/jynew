using System;
using System.Collections.Generic;
using CSObjectWrapEditor;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jyx2.MOD.ModV2;
using TMPro.EditorUtilities;
using UnityEditor.SceneManagement;
using XNode;
using Tools = Jyx2.Middleware.Tools;


namespace Editor
{
    /// <summary>
    /// 一键打包工具
    ///
    /// 知大虾 20221203 重构代码
    ///
    /// </summary>
    public static class OneKeyBuildTools
    {
        private const string TempAbDir = "Temp/jynew";
        private const string StreamingAssetsDir = "Assets/StreamingAssets";

        
        [MenuItem("一键打包/*Windows64")]
        private static void BuildWindows()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneWindows64, "windowsbuild", "jynew.exe");
        }
        [MenuItem("一键打包/*Windows64 Dev")]
        private static void BuildWindowsDev()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneWindows64, "windowsbuild", "jynew.exe", BuildOptions.Development);
        }
        [MenuItem("一键打包/*Android")]
        private static void BuildAndroid()
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string apkName = $"/jynew-Android-{currentDate}.apk";
            //动态设置keystore的密码
            PlayerSettings.Android.keystorePass = "123456";
            PlayerSettings.Android.keyaliasPass = "123456";
            new JynewBuilder().Build(BuildTarget.Android, "", apkName, BuildOptions.Development);
        }
        
        
        private static void GenerateAssetBundlesInTempDirectory(BuildTarget target)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(TempAbDir);
            if (!dirInfo.Exists)
                dirInfo.Create();

            BuildPipeline.BuildAssetBundles(TempAbDir, BuildAssetBundleOptions.ChunkBasedCompression, target);
        }
        
        
        
        
        /*
        [MenuItem("一键打包/Windows64")]
        private static void BuildWindows64()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "jyx2Win64Build");
            if (string.IsNullOrEmpty(path))
                return;

            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);


            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);

            string currentDate = DateTime.Now.ToString("yyyyMMdd");

            //设置版本号
            PlayerSettings.bundleVersion = currentDate;

            //exe路径
            string exePath = path + $"/jynew.exe";

            //打包
            BuildPipeline.BuildPlayer(GetScenePaths(), exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
            
#if UNITY_2019_3_OR_NEWER
            EditorUtility.RequestScriptReload();
#endif

            EditorUtility.DisplayDialog("打包完成", "输出目录:" + path, "确定");
        }

        [MenuItem("一键打包/Windows64_Develop")]
        private static void BuildWindows64_Dev()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "jyx2Win64Build");
            if (string.IsNullOrEmpty(path))
                return;
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);

            string currentDate = DateTime.Now.ToString("yyyyMMdd");

            //设置版本号
            PlayerSettings.bundleVersion = currentDate;

            //exe路径
            string exePath = path + $"/jynew.exe";

            //打包
            BuildPipeline.BuildPlayer(GetScenePaths(), exePath, BuildTarget.StandaloneWindows64,
                BuildOptions.Development);

            EditorUtility.DisplayDialog("打包完成", "输出目录:" + path, "确定");
        }
        */

        static string[] GetScenePaths()
        {
            return new string[] {"Assets/0_GameStart.unity", "Assets/0_MainMenu.unity"};
        }


        [MenuItem("一键打包/[调试用]Android仅AB包")]
        static void BuildAndroidAssetbundle()
        {
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        }
        
        [MenuItem("一键打包/Android_Develop")]
        private static void BuildAndroid_Dev()
        {
            //自动运行xLua的编译
            Generator.GenAll();
            

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            if (string.IsNullOrEmpty(path))
                return;        
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);


            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);



                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string apkPath = path + $"/jyx2AndroidBuild-{currentDate}.apk";

                //设置版本号
                PlayerSettings.bundleVersion = currentDate;

                //动态设置keystore的密码
                PlayerSettings.Android.keystorePass = "123456";
                PlayerSettings.Android.keyaliasPass = "123456";

                //打包
                BuildPipeline.BuildPlayer(GetScenePaths(), apkPath, BuildTarget.Android, BuildOptions.Development);

                EditorUtility.DisplayDialog("打包完成", "输出文件:" + apkPath, "确定");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("打包出错", e.ToString(), "确定");
                Debug.LogError(e.StackTrace);
            }
        }

        [MenuItem("一键打包/MacOS")]
        private static void BuildMacOS()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            if (string.IsNullOrEmpty(path))
                return;     
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);


            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSX);

#if UNITY_STANDALONE_OSX
                //支持m1芯片
                UnityEditor.OSXStandalone.UserBuildSettings.architecture = MacOSArchitecture.x64ARM64;
#endif



                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string outputPath = path + $"/jyxOSXBuild-{currentDate}.app";

                //设置版本号
                PlayerSettings.bundleVersion = currentDate;

                //打包
                BuildPipeline.BuildPlayer(GetScenePaths(), outputPath, BuildTarget.StandaloneOSX, BuildOptions.None);

                EditorUtility.DisplayDialog("打包完成", "输出文件:" + outputPath, "确定");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("打包出错", e.ToString(), "确定");
                Debug.LogError(e.StackTrace);
            }
        }


        [MenuItem("一键打包/MacOS_Develop")]
        private static void BuildMacOS_Dev()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            if (string.IsNullOrEmpty(path))
                return;
  
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);

            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSX);

#if UNITY_STANDALONE_OSX
                //支持m1芯片
                UnityEditor.OSXStandalone.UserBuildSettings.architecture = MacOSArchitecture.x64ARM64;
#endif


                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string outputPath = path + $"/jyxOSXBuild-{currentDate}.app";

                //设置版本号
                PlayerSettings.bundleVersion = currentDate;

                //打包
                BuildPipeline.BuildPlayer(GetScenePaths(), outputPath, BuildTarget.StandaloneOSX,
                    BuildOptions.Development);

                EditorUtility.DisplayDialog("打包完成", "输出文件:" + outputPath, "确定");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("打包出错", e.ToString(), "确定");
                Debug.LogError(e.StackTrace);
            }
        }


        [MenuItem("一键打包/iOS XCodeProject")]
        private static void BuildiOSXCodeProject()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            if (string.IsNullOrEmpty(path))
                return; 
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);

                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string outputPath = path + $"/jyxiOSBuild-{currentDate}";

                //设置版本号
                PlayerSettings.bundleVersion = currentDate;

                //打包
                BuildPipeline.BuildPlayer(GetScenePaths(), outputPath, BuildTarget.iOS, BuildOptions.None);

                EditorUtility.DisplayDialog("打包完成", "输出文件:" + outputPath, "确定");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("打包出错", e.ToString(), "确定");
                Debug.LogError(e.StackTrace);
            }
        }
        
        [MenuItem("一键打包/iOS XCodeProject Develop")]
        private static void BuildiOSXCodeProject_Dev()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            if (string.IsNullOrEmpty(path))
                return;
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);

                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string outputPath = path + $"/jyxiOSBuild-{currentDate}";

                //设置版本号
                PlayerSettings.bundleVersion = currentDate;

                //打包
                BuildPipeline.BuildPlayer(GetScenePaths(), outputPath, BuildTarget.iOS, BuildOptions.Development);

                EditorUtility.DisplayDialog("打包完成", "输出文件:" + outputPath, "确定");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("打包出错", e.ToString(), "确定");
                Debug.LogError(e.StackTrace);
            }
        }
    }
}