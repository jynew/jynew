using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
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

            
            await LoadCurrentMod();
            
            await ResLoader.Init();
            await ResLoader.LoadMod(CurrentModId); 
            
            CurrentModConfig = await ResLoader.LoadAsset<MODRootConfig>("Assets/ModSetting.asset");
            
#if UNITY_EDITOR
            var dirPath = $"Assets/Mods/{CurrentModId}/Configs";
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
#endif
      
            GameSettingManager.Init();
            await Jyx2ResourceHelper.Init();
        }
        
        

        private static async UniTask LoadCurrentMod()
        {
            if (PlayerPrefs.HasKey("CURRENT_MOD"))
            {
                CurrentModId = PlayerPrefs.GetString("CURRENT_MOD");
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