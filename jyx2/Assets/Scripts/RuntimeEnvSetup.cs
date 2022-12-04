using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AClockworkBerry;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2.MOD;
using Jyx2.MOD.ModV2;
using Jyx2.ResourceManagement;
using Jyx2Configs;
using MOD.UI;
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
        public static MODRootConfig CurrentModConfig { get; private set; } = null;
        private static GameModBase _currentMod;

        public static string CurrentModId => _currentMod.Id;

        public static void SetCurrentMod(GameModBase mod)
        {
            _currentMod = mod;
        }

        public static GameModBase GetCurrentMod() => _currentMod;

        public static bool IsLoading { get; private set; } = false;

        public static void ForceClear()
        {
            _isSetup = false;
            CurrentModConfig = null;
            _currentMod = null;
            IsLoading = false;
            LuaManager.Clear();
            GameConfigDatabase.ForceClear();
        }
        
        public static async UniTask<bool> Setup()
        {
            if (_isSetup) return false;

            try
            {
                if (IsLoading)
                {
                    //同时调用了Setup的地方都应该挂起
                    await UniTask.WaitUntil(() => _isSetup);
                    return false;
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

                await ResLoader.Init();
                await ResLoader.LaunchMod(_currentMod);

                CurrentModConfig = await ResLoader.LoadAsset<MODRootConfig>("Assets/ModSetting.asset");
                GameSettingManager.Init();
                await Jyx2ResourceHelper.Init();
                LuaManager.LuaMod_Init();
                _isSetup = true;
                IsLoading = false;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("<color=red>MOD加载出错了，请检查文件是否损坏！</color>");
                Debug.LogError(e.ToString());
                ScreenLogger.Instance.enabled = true;
                ModPanelNew.SwitchSceneTo();
                return false;
            }
        }
    }
}