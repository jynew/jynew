using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Jyx2.MOD
{
    public class MODManager
    {
        #region Singleton

        //单例模式
        public static MODManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MODManager();
                return _instance;
            }
        }

        private static MODManager _instance;

        #endregion

        private Dictionary<string, MODProviderBase> _platforms = new Dictionary<string, MODProviderBase>();

        private bool _isInited = false;

        public void Init()
        {
            if (_isInited) return;
            _isInited = true;

            _platforms.Clear();

            //注册平台
#if UNITY_ANDROID
            RegisterModPlatform(new AndroidMODProvider() { Name = "Android" });
#else
            RegisterModPlatform(new SteamMODProvider() { Name = "Steam" });
            RegisterModPlatform(new PCLocalMODProvider() { Name = "PC" });
#endif
        }

        /// <summary>
        /// 注册Mod平台
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterModPlatform<T>(T t) where T : MODProviderBase
        {
            if (t == null)
            {
                Debug.LogError("Mod平台注册失败，传入的Mod平台为空");
                return;
            }

            if (_platforms.ContainsKey(t.Name))
            {
                Debug.LogError("Mod平台注册失败，已经存在同名的平台");
                return;
            }

            _platforms.Add(t.Name, t);
        }


        /// <summary>
        /// 获取所有的Mod提供器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllModProviders<T>() where T : MODProviderBase
        {
            foreach (var platform in _platforms.Values)
                if (platform is T)
                    yield return platform as T;
        }

        /// <summary>
        /// 用指定的Mod加载器加载Mod，需要记住每个ModId是由哪个ModProvider提供的
        /// </summary>
        /// <param name="modId"></param>
        /// <param name="modProviderName"></param>
        public async UniTask LoadMod(string modId, string modProviderName)
        {
            if (!_platforms.ContainsKey(modProviderName))
            {
                Debug.LogError("Mod加载失败，不存在名为" + modProviderName + "的Mod提供器");
                return;
            }

            await _platforms[modProviderName].LoadMod(modId);
        }

        public string GetCurrentModLuaDirectory()
        {
            var curModId = RuntimeEnvSetup.CurrentModId;
            var modDir = MODProviderBase.GetModDirPath(curModId);
            return $"{modDir}/Lua";
        }


        /// <summary>
        /// 获取平台数量
        /// </summary>
        /// <returns></returns>
        public int GetModPlatformCount()
        {
            return _platforms.Count;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            _platforms.Clear();
            _instance = null;
        }
    }
}