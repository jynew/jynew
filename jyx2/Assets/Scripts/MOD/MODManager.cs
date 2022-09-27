using System.Collections;
using System.Collections.Generic;
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
                if (_instance == null)
                {
                    _instance = new MODManager();
                }
                return _instance;
            }
        }
        
        private static MODManager _instance;
        #endregion
        
        private Dictionary<string, MODProviderBase> _platforms = new Dictionary<string, MODProviderBase>();
        
        private bool _isInited = false;
        
        public void Init()
        {
            if (_isInited)
            {
                return;
            }
            _isInited = true;
            
            //注册平台
            RegisterMODPlatform(new SteamMODProvider() { Name = "Steam" });
            RegisterMODPlatform(new PCLocalMODProvider() { Name = "PC" });
#if UNITY_ANDROID
            RegisterMODPlatform(new AndroidMODProvider());
#endif
        }

        /// <summary>
        /// 注册MOD平台
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterMODPlatform<T>(T t) where T : MODProviderBase
        {
            if (t == null)
            {
                Debug.LogError("MOD平台注册失败，传入的MOD平台为空");
                return;
            }
            
            if (_platforms.ContainsKey(t.Name))
            {
                Debug.LogError("MOD平台注册失败，已经存在同名的平台");
                return;
            }
            
            _platforms.Add(t.Name, t);
        }

        
        /// <summary>
        /// 获取所有的MOD提供器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllModProviders<T>() where T : MODProviderBase
        {
            foreach (var platform in _platforms.Values)
            {
                if (platform is T)
                {
                    yield return platform as T;
                }
            }
        }
        
        /// <summary>
        /// 用指定的MOD加载器加载MOD，需要记住每个MODID是由哪个ModProvider提供的
        /// </summary>
        /// <param name="modId"></param>
        /// <param name="modProviderName"></param>
        public void LoadMod(string modId, string modProviderName)
        {
            if (!_platforms.ContainsKey(modProviderName))
            {
                Debug.LogError("MOD加载失败，不存在名为" + modProviderName + "的MOD提供器");
                return;
            }
            
            _platforms[modProviderName].LoadMod(modId);
        }
        
        
        /// <summary>
        /// 获取平台数量
        /// </summary>
        /// <returns></returns>
        public int GetMODPlatformCount()
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