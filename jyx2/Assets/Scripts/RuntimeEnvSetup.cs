using System;
using Cysharp.Threading.Tasks;
using Jyx2.ResourceManagement;
using UnityEngine;

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
                CurrentModId = GlobalAssetConfig.Instance.startModId;
            }
            
            CurrentModConfig = await ResLoader.LoadAsset<MODRootConfig>("ModSetting.asset");
        }

      
    }
}