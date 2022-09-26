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
    }
}