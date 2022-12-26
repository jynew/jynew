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
    public static class JynewBuildTools
    {
        [MenuItem("一键打包/Windows64")]
        private static void BuildWindows()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneWindows64, "windowsbuild", "wuxia_launch.exe");
        }
        [MenuItem("一键打包/Windows64 Dev", false, 2000)]
        private static void BuildWindowsDev()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneWindows64, "windowsbuild", "wuxia_launch.exe", BuildOptions.Development | BuildOptions.AllowDebugging);
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
        
        [MenuItem("一键打包/TapTap Android")]
        private static void BuildTapTapAndroid()
        {
            var currentDate = DateTime.Now.ToString("yyyyMMdd");
            var apkName = $"jynew-TapTap-Android-{currentDate}.apk";
            //动态设置keystore的密码
            PlayerSettings.Android.keystorePass = "123456";
            PlayerSettings.Android.keyaliasPass = "123456";
            if (!ReadyForTapBuild())
            {
                return;
            }
            new JynewBuilder().Build(BuildTarget.Android, "", apkName);
            RecoverAfterTapBuild();
        }

        /// 打TapTap包之前，将TapTap Sdk相关文件拷入工程，并进行相应配置
        private static bool ReadyForTapBuild()
        {
            const string paramsFile = "Assets/Resources/TAPTAP_BUILD_PARAMS.txt";
            if (!File.Exists(paramsFile))
            {
                Debug.LogError("Resources目录下没有Tap参数文件TAPTAP_BUILD_PARAMS.txt");
                return false;
            }
            
            const string tapDir = "TapSdkFiles/TapTap";
            if (!Directory.Exists(tapDir))
            {
                Debug.LogError("未找到Tap Sdk的 tap文件夹");
                return false;
            }

            const string pluginsDir = "TapSdkFiles/Plugins";
            if (!Directory.Exists(pluginsDir))
            {
                Debug.LogError("未找到Tap Sdk的 Plugins文件夹");
                return false;
            }
            
            CopyDirectory(tapDir, "Assets/TapTap");
            CopyDirectory(pluginsDir, "Assets/Plugins");
            
            // 增加Tap闪屏
            string[] logos = {"Assets/TapTapSlogan.png"};
            ChangeSplashLogo(logos);
            PlayerSettings.SplashScreen.showUnityLogo = true;
            PlayerSettings.SplashScreen.backgroundColor = Color.black;
            PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
            PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
            
            // 增加宏
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, out var defines);
            if (!defines.Contains("DEVELOP_TAPTAP"))
            {
                var defineList = defines.ToList();
                defineList.Add("DEVELOP_TAPTAP");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineList.ToArray());
            }
            
            AssetDatabase.Refresh();
            Debug.Log("打Tap包前的准备完成");
            
            return true;
        }

        /// 打TapTap包之后，将TapTap Sdk相关文件移出工程
        private static void RecoverAfterTapBuild()
        {
            // 删除宏
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, out var defines);
            if (defines.Contains("DEVELOP_TAPTAP"))
            {
                var defineList = defines.ToList();
                defineList.Remove("DEVELOP_TAPTAP");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineList.ToArray());
            }
            // 删除文件
            const string paramsFile = "Assets/Resources/TAPTAP_BUILD_PARAMS.txt";
            if (File.Exists(paramsFile)) File.Delete(paramsFile);
            
            const string tapDir = "Assets/TapTap";
            if (Directory.Exists(tapDir)) Directory.Delete(tapDir, true);
            
            const string pluginsDir = "TapSdkFiles/Plugins";
            var files = Directory.GetFiles(pluginsDir);
            foreach (var file in files)
            {
                var dFile = $"Assets/Plugins/{Path.GetFileName(file)}";
                if (File.Exists(dFile)) File.Delete(dFile);
            }
            // 删除闪屏及文件
            PlayerSettings.SplashScreen.logos = null;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = false;
            
            AssetDatabase.Refresh();
            Debug.Log("Tap打包后工程恢复完成");
        }
        
        /// 文件夹拷贝
        private static void CopyDirectory(string sourceDirPath, string saveDirPath)
        {
            try
            {
                if (!Directory.Exists(saveDirPath))
                {
                    Directory.CreateDirectory(saveDirPath);
                }
                var files = Directory.GetFiles(sourceDirPath);
                foreach (var file in files)
                {
                    var pFilePath = Path.Combine(saveDirPath, Path.GetFileName(file));
                    File.Copy(file, pFilePath, true);
                }

                var dirs = Directory.GetDirectories(sourceDirPath);
                foreach (var dir in dirs)
                {
                    CopyDirectory(dir, Path.Combine(saveDirPath, Path.GetFileName(dir)));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("拷贝文件出错：" + ex);
            }
        }
        
        // 改闪屏，将闪屏图片按顺序组成数组，然后传入
        private static void ChangeSplashLogo(string[] logos)
        {
            PlayerSettings.SplashScreen.show = true;
            
            var size = logos.Length;
            var newLogos = new PlayerSettings.SplashScreenLogo[size];
            for (var i = 0; i < size; i++)
            {
                newLogos[i].duration = 2f;
                newLogos[i].logo = AssetDatabase.LoadAssetAtPath<Sprite>(logos[i]);
            }
            PlayerSettings.SplashScreen.logos = newLogos;
        }
        
        [MenuItem("一键打包/Android Dev", false, 2000)]
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
        
        [MenuItem("一键打包/MacOS Dev", false, 2000)]
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
        
        [MenuItem("一键打包/iOS Dev", false, 2000)]
        private static void BuildIOSDev()
        {
            new JynewBuilder().Build(BuildTarget.iOS, "iOSBuild", "", BuildOptions.Development);
        }
        
        [MenuItem("一键打包/Linux")]
        private static void BuildLinux()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneLinux64, "LinuxBuild", "");
        }
        
        [MenuItem("一键打包/Linux Dev", false, 2000)]
        private static void BuildLinuxDev()
        {
            new JynewBuilder().Build(BuildTarget.StandaloneLinux64, "LinuxBuild", "", BuildOptions.Development);
        }
    }
}