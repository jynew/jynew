using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jyx2
{
    /// <summary>
    /// 游戏运行时的初始化
    /// </summary>
    public static class RuntimeEnvSetup
    {
        private static bool _isSetup;
        public static MODRootConfig CurrentModConfig { get; set; } = null;
        public static string CurrentModId { get; set; } = "";

        public static bool IsLoading { get; private set; } = false;

        public static void ForceClear()
        {
            _isSetup = false;
            CurrentModConfig = null;
            CurrentModId = "";
            IsLoading = false;
            LuaManager.Clear();
            GameConfigDatabase.ForceClear();
        }
        
        public static async UniTask Setup()
        {
            if (_isSetup) return;

            if(IsLoading)
            {
                //同时调用了Setup的地方都应该挂起
               await UniTask.WaitUntil(() => _isSetup);
               return;
            }

            IsLoading = true;
            
            DebugInfoManager.Init();
            
            //全局配置表
            var t = Resources.Load<GlobalAssetConfig>("GlobalAssetConfig");
            if (t != null)
            {
                GlobalAssetConfig.Instance = t;
                await t.OnLoad();
            }

            //初始化MOD管理器
            MODManager.Instance.Init();

            await LoadMods();
            await LoadCurrentMod();
            
            await ResLoader.Init();

#if UNITY_EDITOR
            await MODManager.Instance.LoadMod(CurrentModId, "PC");
#elif UNITY_STANDALONE
            await MODManager.Instance.LoadMod(CurrentModId, "Steam");
#elif UNITY_ANDROID
            await MODManager.Instance.LoadMod(CurrentModId, "Android");
#elif UNITY_IPHONE
            await MODManager.Instance.LoadMod(CurrentModId, "IOS");
#else
            Debug.LogError("当前平台还不支持Mod");
#endif
            CurrentModConfig = await ResLoader.LoadAsset<MODRootConfig>("Assets/ModSetting.asset");

#if UNITY_EDITOR
            var dirPath = $"Assets/Mods/{CurrentModId}/Configs";
            if (Directory.Exists(dirPath))
            {
                if (!File.Exists($"{dirPath}/Datas.bytes"))
                {
                    CurrentModConfig.GenerateConfigs();
                }
                else
                {
                    ExcelTools.WatchConfig(dirPath, () =>
                    {
                        CurrentModConfig.GenerateConfigs();
                        Debug.Log("File Watcher! Reload success! -> " + dirPath);
                    }); 
                } 
            }
#endif
      
            GameSettingManager.Init();
            await Jyx2ResourceHelper.Init();
            _isSetup = true;
            IsLoading = false;
        }
        
        public static async UniTask LoadMods()
        {
            foreach (var mod in MODManager.Instance.GetAllModProviders<MODProviderBase>())
            {
                await mod.GetInstalledMods();
            }
        }
        

        private static async UniTask LoadCurrentMod()
        {
#if UNITY_EDITOR
            var path = SceneManager.GetActiveScene().path;
            if (path.Contains("Assets/Mods/"))
            {

                CurrentModId = path.Split('/')[2];
            }
            else
            {
                CurrentModId = Jyx2_PlayerPrefs.GetString("CURRENT_MOD_ID", GlobalAssetConfig.Instance.startModId);
            }
#else
                CurrentModId = Jyx2_PlayerPrefs.GetString("CURRENT_MOD_ID", GlobalAssetConfig.Instance.startModId);
#endif
        }


    }
}