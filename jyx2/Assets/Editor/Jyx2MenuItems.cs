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
        [MenuItem("项目快速导航/技能编辑器")]
        private static void OpenSkillEditor()
        {
            SceneHelper.StartScene("Assets/Jyx2BattleScene/Jyx2SkillEditor.unity");
        }
        
        [MenuItem("项目快速导航/全模型预览")]
        private static void OpenAllModels()
        {
            SceneHelper.StartScene("Assets/3D/AllModels.unity");
        }

        [MenuItem("项目快速导航/脚本编辑/事件图形化编辑")]
        private static void OpenEventsGraphMenu()
        {
            NavigateToPath("Assets/BuildSource/EventsGraph/README.txt");
        }
        
        [MenuItem("项目快速导航/脚本编辑/lua")]
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
            NavigateToPath("Assets/BuildSource/Jyx2Items/0.png");
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
            
            //动态设置keystore的密码
            PlayerSettings.Android.keystorePass = "123456";
            PlayerSettings.Android.keyaliasPass = "123456";
            
            //打包
            BuildPipeline.BuildPlayer(GetScenePaths(), apkPath, BuildTarget.Android, BuildOptions.None);
            
            //强制移动目录
            //System.IO.Directory.Move("StandaloneWindows64", path + "/StandaloneWindows64");

            EditorUtility.DisplayDialog("打包完成", "输出文件:" + apkPath, "确定");
        }
    }
}