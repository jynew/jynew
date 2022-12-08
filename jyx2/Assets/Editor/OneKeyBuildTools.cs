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
#if UNITY_STANDALONE_OSX
using UnityEditor.OSXStandalone;
#endif
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

        
        [MenuItem("一键打包/Windows64")]
        private static void BuildWindows()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneWindows64, "windowsbuild", "wuxia_launch.exe");
        }
        [MenuItem("一键打包/Windows64 Dev")]
        private static void BuildWindowsDev()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneWindows64, "windowsbuild", "wuxia_launch.exe", BuildOptions.Development);
        }
        [MenuItem("一键打包/Android")]
        private static void BuildAndroid()
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string apkName = $"jynew-Android-{currentDate}.apk";
            //动态设置keystore的密码
            PlayerSettings.Android.keystorePass = "123456";
            PlayerSettings.Android.keyaliasPass = "123456";
            new JynewBuilder().Build(BuildTarget.Android, "", apkName);
        }
        [MenuItem("一键打包/Android Dev")]
        private static void BuildAndroidDev()
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string apkName = $"jynew-Android-{currentDate}-dev.apk";
            //动态设置keystore的密码
            PlayerSettings.Android.keystorePass = "123456";
            PlayerSettings.Android.keyaliasPass = "123456";
            new JynewBuilder().Build(BuildTarget.Android, "", apkName, BuildOptions.Development);
        }

        [MenuItem("一键打包/MacOS")]
        private static void BuildMac()
        {
#if UNITY_STANDALONE_OSX
            //支持m1芯片
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = MacOSArchitecture.x64ARM64;
#endif
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string outputName = $"jyxOSXBuild-{currentDate}.app";
            new JynewBuilder().Build(BuildTarget.StandaloneOSX, "", outputName);
        }
        
        [MenuItem("一键打包/MacOS Dev")]
        private static void BuildMacDev()
        {
#if UNITY_STANDALONE_OSX
            //支持m1芯片
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = MacOSArchitecture.x64ARM64;
#endif
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string outputName = $"jyxOSXBuild-{currentDate}.app";
            new JynewBuilder().Build(BuildTarget.StandaloneOSX, "", outputName, BuildOptions.Development);
        }
        
        [MenuItem("一键打包/iOS")]
        private static void BuildIOS()
        {
            new JynewBuilder().Build(BuildTarget.iOS, "iOSBuild", "");
        }
        
        [MenuItem("一键打包/iOS Dev")]
        private static void BuildIOSDev()
        {
            new JynewBuilder().Build(BuildTarget.iOS, "iOSBuild", "", BuildOptions.Development);
        }
    }
}