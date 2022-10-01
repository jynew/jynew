using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
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
        public static Dictionary<string, MODProviderBase.ModItem> ModDic { get; set; } = new Dictionary<string, MODProviderBase.ModItem>();
        public static string CurrentModId { get; set; } = "";
        
        public static async UniTask Setup()
        {
            if (_isSetup) return;
            
            _isSetup = true;
            
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
            
            //TODO: 此处还需进行修改
            if (Application.isMobilePlatform)
            {
                await MODManager.Instance.LoadMod(CurrentModId, "Android");
            }
            else{
                await MODManager.Instance.LoadMod(CurrentModId, "Steam");
            }
            
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
        }
        
        public static async UniTask LoadMods()
        {
            foreach (var mod in MODManager.Instance.GetAllModProviders<MODProviderBase>())
            {
                var dic = await mod.GetInstalledMods();
                //合并到总的mod字典
                foreach (var kv in dic)
                {
                    ModDic[kv.Key] = kv.Value;
                }
            }
        }
        

        private static async UniTask LoadCurrentMod()
        {
            if (PlayerPrefs.HasKey("CURRENT_MOD_ID"))
            {
                CurrentModId = PlayerPrefs.GetString("CURRENT_MOD_ID");
            }
            else
            {
#if UNITY_EDITOR
                var path = SceneManager.GetActiveScene().path;
                if (path.Contains("Assets/Mods/"))
                {

                    CurrentModId = path.Split('/')[2];
                }
                else
                {
                    CurrentModId = GlobalAssetConfig.Instance.startModId;
                }
#else
                CurrentModId = GlobalAssetConfig.Instance.startModId;
#endif
            }
        }

      
    }
}