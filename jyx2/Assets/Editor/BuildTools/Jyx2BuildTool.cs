using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Jyx2.Editor;
using NPOI.OpenXml4Net.OPC.Internal;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Jyx2Editor.BuildTool
{
    public class Jyx2BuildTool
    {
        public enum EnvOption
        {
            CS_DEF_SYMBOL,
            DEVLOPMENT,
            IS_MONO,
            BUILD_PATH,
            DistributionSign,
        }

        private static Dictionary<EnvOption, string> internalEnvMap = new Dictionary<EnvOption, string>();

        #region 封装打包机BuildPlayer接口

        public static void BuildForAndroid()
        {
            BuildPlayer(BuildTarget.Android);
        }

        public static void BuildForiOS()
        {
            BuildPlayer(BuildTarget.iOS);
        }

        public static void BuildForWindows()
        {
            BuildPlayer(BuildTarget.StandaloneWindows64);
        }

        #endregion

        #region BuildPlayer接口

        /// <summary>
        /// 分步骤构建安装包
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <exception cref="Exception"></exception>
        public static void BuildPlayer(BuildTarget buildTarget)
        {
            //检查BuildSceneList
            if (!CheckScenesInBuildValid())
            {
                return;
            }

            //切换平台
            if (buildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                Debug.Log("Start switch platform to: " + buildTarget);
                var beginTime = System.DateTime.Now;
                var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);
                Debug.Log("End switch platform to: " + buildTarget);
                Debug.Log("=================Build SwitchPlatform Time================ : " +
                          (System.DateTime.Now - beginTime).TotalSeconds);
            }

            //根据buildTarget区分BuildGroup
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            if (BuildTargetGroup.Unknown == buildTargetGroup)
            {
                throw new System.Exception(
                    string.Format("{0} is Unknown Build Platform ! Build Failture!", buildTarget));
            }

            //设置参数
            SetBuildParams(buildTargetGroup);

            //出包
            InternalBuildPkg(buildTargetGroup);
        }

        /// <summary>
        /// 用来设置一些编译的宏和参数等操作
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void SetBuildParams(BuildTargetGroup buildTargetGroup)
        {
            //Android包在这里做签名操作
            // var isDistributionSign = ContainsEnvOption(EnvOption.DistributionSign);
            // if (!isDistributionSign)
            // {
            //     PlayerSettings.Android.keystoreName = "";
            //     PlayerSettings.Android.keystorePass = "";
            //     PlayerSettings.Android.keyaliasName = "";
            //     PlayerSettings.Android.keyaliasPass = "";
            // }
            // else
            // {
            //     PlayerSettings.Android.keystoreName = "./Tools/user.keystore";
            //     PlayerSettings.Android.keystorePass = "password";
            //     PlayerSettings.Android.keyaliasName = "cola";
            //     PlayerSettings.Android.keyaliasPass = "password";
            // }

            var csDefineSymbol = GetEnvironmentVariable(EnvOption.CS_DEF_SYMBOL);
            if (!string.IsNullOrEmpty(csDefineSymbol))
            {
                var oldSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                oldSymbol = oldSymbol + ";" + csDefineSymbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, oldSymbol);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 最后的出包环节
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void InternalBuildPkg(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            var outputPath = GetEnvironmentVariable(EnvOption.BUILD_PATH);
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = EditorUtility.SaveFolderPanel("Choose location of the build game", "", "");
            }

            if (outputPath.Length == 0)
                return;

            var levels = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }

            var targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = levels,
                locationPathName = outputPath + targetName,
                target = EditorUserBuildSettings.activeBuildTarget,
            };
            if (ContainsEnvOption(EnvOption.DEVLOPMENT))
            {
                buildPlayerOptions.options |= BuildOptions.Development;
                buildPlayerOptions.options |= BuildOptions.AllowDebugging;
                buildPlayerOptions.options |= BuildOptions.ConnectWithProfiler;
            }

            if (ContainsEnvOption(EnvOption.IS_MONO))
            {
                PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
            }

            AssetDatabase.SaveAssets();

            //重新生成Addressable相关文件
            AddressableAssetSettings.BuildPlayerContent();

            //强制GENDATA
            GenDataMenuCmd.GenerateDataForce();

            // 处理场景文件
            AddScenesToBuildTool.AddScenesToBuild();

            //打包
            BuildPipeline.BuildPlayer(buildPlayerOptions);

            Debug.Log("=================Build Pkg Time================ : " +
                      (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// 对打包平台进行分组
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        private static BuildTargetGroup HandleBuildGroup(BuildTarget buildTarget)
        {
            var buildTargetGroup = BuildTargetGroup.Unknown;
            if (BuildTarget.Android == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.Android;
            }
            else if (BuildTarget.iOS == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.iOS;
            }
            else if (BuildTarget.StandaloneWindows == buildTarget || BuildTarget.StandaloneWindows64 == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.Standalone;
            }

            return buildTargetGroup;
        }

        private static string GetBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "/" + PlayerSettings.productName + ".apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "/" + PlayerSettings.productName + ".exe";
                case BuildTarget.iOS:
                    return "";
                // Add more build targets for your own.
                default:
                    Debug.LogError("Build Target not implemented.");
                    return null;
            }
        }

        #endregion

        #region 工具方法

        public static bool CheckScenesInBuildValid()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!File.Exists(scene.path))
                {
                    Debug.LogError("Error! Scene In BuildList中有场景丢失！请检查！");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 环境变量

        public static string GetEnvironmentVariable(EnvOption envOption)
        {
            return internalEnvMap.ContainsKey(envOption)
                ? internalEnvMap[envOption]
                : Environment.GetEnvironmentVariable(envOption.ToString()) ?? string.Empty;
        }

        public static void SetEnvironmentVariable(EnvOption envOption, string value, bool isAppend)
        {
            string oldValue = GetEnvironmentVariable(envOption);
            if (!isAppend)
            {
                oldValue = value;
            }
            else
            {
                oldValue = string.IsNullOrEmpty(oldValue) ? value : (oldValue + ";" + value);
            }

            if (!internalEnvMap.ContainsKey(envOption))
            {
                internalEnvMap.Add(envOption, oldValue);
            }
            else
            {
                internalEnvMap[envOption] = oldValue;
            }
        }

        public static bool ContainsEnvOption(EnvOption envOption)
        {
            string envVar = GetEnvironmentVariable(envOption);
            if (string.IsNullOrEmpty(envVar) ||
                0 == string.Compare(envVar, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public static void ClearEnvironmentVariable()
        {
            internalEnvMap.Clear();
        }

        #endregion
    }
}