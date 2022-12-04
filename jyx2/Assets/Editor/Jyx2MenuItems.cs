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

    }
}