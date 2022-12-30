using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace Jyx2
{
    /// <summary>
    /// 游戏设置
    ///
    /// 对应配置表“游戏设置.xlsx”
    /// </summary>
    public static class GameSettings
    {
        public static void Refresh()
        {
            _cacheFloat.Clear();
            _cacheInt.Clear();
            _cache.Clear();

            TryInit();
        }
        
        public static int GetInt(string key, int defaultVal = 0)
        {
            if (!_cacheInt.ContainsKey(key))
            {
                if (defaultVal != 0)
                {
                    _cacheInt[key] = int.Parse(GetValue(key, defaultVal.ToString()));    
                }
                else
                {
                    _cacheInt[key] = int.Parse(GetValue(key));
                }
                
            }

            return _cacheInt[key];
        }

        public static float GetFloat(string key, float defaultVal = 0)
        {
            if (!_cacheFloat.ContainsKey(key))
            {
                if (defaultVal != 0)
                {
                    _cacheFloat[key] = float.Parse(GetValue(key, defaultVal.ToString("F2")));
                }
                else
                {
                    _cacheFloat[key] = float.Parse(GetValue(key));
                }
            }

            return _cacheFloat[key];
        }

        public static string Get(string key)
        {
            return GetValue(key);
        }

        private static readonly Dictionary<string, float> _cacheFloat = new Dictionary<string, float>();
        private static readonly Dictionary<string, int> _cacheInt = new Dictionary<string, int>();
        private static readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        private static void TryInit()
        {
            if (_cache.Count == 0)
            {
                var all = LuaToCsBridge.SettingsTable.Values;
                foreach (var kv in all)
                {
                    _cache.Add(kv.Name, kv.Value);
                }
            }
        }
        
        private static string GetValue(string key, string defaultVal = "")
        {
            TryInit();

            if (!_cache.ContainsKey(key))
            {
                Debug.LogError($"调用了未定义的游戏设置：{key}");
                if(!defaultVal.IsNullOrWhitespace())
                    _cache[key] = defaultVal;
            }
            return _cache[key];
        }

    }
}
