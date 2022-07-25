using System;
using System.IO;
using System.Net;
using System.Threading;
using CSObjectWrapEditor;
using DG.DemiLib;
using Jyx2.MOD;
using UnityEngine;
using UnityEditor;

#if UNITY_STANDALONE_OSX
using UnityEditor.OSXStandalone;
#endif
using UnityToolbarExtender.Examples;

namespace Jyx2Editor
{
    public class Jyx2MenuItems
    {
        
        [MenuItem("项目快速导航/技能编辑器")]
        private static void OpenSkillEditor()
        {
            SceneHelper.StartScene("Assets/Jyx2Tools/Jyx2SkillEditor.unity");
        }

        [MenuItem("项目快速导航/全模型预览")]
        private static void OpenAllModels()
        {
            SceneHelper.StartScene("Assets/3D/AllModels.unity");
        }

        [MenuItem("项目快速导航/游戏事件脚本/蓝图脚本")]
        private static void OpenEventsGraphMenu()
        {
            NavigateToPath("Assets/BuildSource/EventsGraph/README.txt");
        }

        [MenuItem("项目快速导航/游戏事件脚本/lua脚本")]
        private static void OpenLuaMenu()
        {
            EditorUtility.RevealInFinder("data/lua/jygame");
        }

        [MenuItem("项目快速导航/资源/角色头像")]
        private static void OpenRoleHeadsMenu()
        {
            NavigateToPath("Assets/BuildSource/head/0.png");
        }

        [MenuItem("项目快速导航/资源/角色模型(FBX)")]
        private static void OpenRoleModelsMenu()
        {
            NavigateToPath("Assets/3D/Jyx2RoleModels/Models/README.txt");
        }

        [MenuItem("项目快速导航/资源/角色预设(Prefabs)")]
        private static void OpenRolePrefabsMenu()
        {
            NavigateToPath("Assets/BuildSource/ModelCharacters/角色预设说明.txt");
        }

        [MenuItem("项目快速导航/资源/角色动作(Animation)")]
        private static void OpenRoleAnimations()
        {
            NavigateToPath("Assets/BuildSource/Animations");
        }

        [MenuItem("项目快速导航/资源/角色动作控制器(AnimationController)")]
        private static void OpenRoleAnimationControllers()
        {
            NavigateToPath("Assets/BuildSource/AnimationControllers");
        }

        [MenuItem("项目快速导航/资源/道具图标")]
        private static void OpenItemsMenu()
        {
            NavigateToPath("Assets/BuildSource/Items/0.png");
        }

        [MenuItem("项目快速导航/资源/音乐")]
        private static void OpenMusicMenu()
        {
            NavigateToPath("Assets/BuildSource/Musics/0.mp3");
        }

        [MenuItem("项目快速导航/资源/音效")]
        private static void OpenWaveMenu()
        {
            NavigateToPath("Assets/BuildSource/sound/atk00.wav");
        }

        public static void NavigateToPath(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        [MenuItem("一键打包/Windows64")]
        private static void BuildWindows64()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "jyx2Win64Build");
            
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

            EditorUtility.DisplayDialog("打包完成", "输出目录:" + path, "确定");
        }

        [MenuItem("一键打包/Windows64_Develop")]
        private static void BuildWindows64_Dev()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "jyx2Win64Build");
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);

            if (string.IsNullOrEmpty(path))
                return;
            
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

        static string[] GetScenePaths()
        {
            return new string[] {"Assets/0_GameStart.unity", "Assets/0_MainMenu.unity"};
        }

        [MenuItem("一键打包/Android")]
        private static void BuildAndroid()
        {
            //自动运行xLua的编译
            Generator.GenAll();

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);


            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

                if (string.IsNullOrEmpty(path))
                    return;
                

                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string apkPath = path + $"/jyx2AndroidBuild-{currentDate}.apk";

                //设置版本号
                PlayerSettings.bundleVersion = currentDate;

                //动态设置keystore的密码
                PlayerSettings.Android.keystorePass = "123456";
                PlayerSettings.Android.keyaliasPass = "123456";

                //打包
                BuildPipeline.BuildPlayer(GetScenePaths(), apkPath, BuildTarget.Android, BuildOptions.None);

                EditorUtility.DisplayDialog("打包完成", "输出文件:" + apkPath, "确定");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("打包出错", e.ToString(), "确定");
                Debug.LogError(e.StackTrace);
            }
        }


        [MenuItem("一键打包/Android_Develop")]
        private static void BuildAndroid_Dev()
        {
            //自动运行xLua的编译
            Generator.GenAll();
            

            //BUILD
            string path = EditorUtility.SaveFolderPanel("选择打包输出目录", "", "");
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);


            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

                if (string.IsNullOrEmpty(path))
                    return;

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
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);


            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSX);

#if UNITY_STANDALONE_OSX
                //支持m1芯片
                UnityEditor.OSXStandalone.UserBuildSettings.architecture = MacOSArchitecture.x64ARM64;
#endif

                if (string.IsNullOrEmpty(path))
                    return;

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
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);

            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSX);

#if UNITY_STANDALONE_OSX
                //支持m1芯片
                UnityEditor.OSXStandalone.UserBuildSettings.architecture = MacOSArchitecture.x64ARM64;
#endif

                if (string.IsNullOrEmpty(path))
                    return;

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
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);

                if (string.IsNullOrEmpty(path))
                    return;
                

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
            
            //生成ab包
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

            try
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);

                if (string.IsNullOrEmpty(path))
                    return;
                

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