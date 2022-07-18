using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jyx2.ResourceManagement
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class ResLoader
    {
        private const string AbDir = @"D:/jynew/build_ab_test";
        
        private const string BaseAbName = "base_assets";
        
        private static readonly Dictionary<string, AssetBundle> _modAssets = new Dictionary<string, AssetBundle>();
        private static readonly Dictionary<string, AssetBundle> _modScenes = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 资产映射标
        ///
        /// 
        /// 保存数据结构，（下同）
        /// 逻辑路径：（AB包ID，AB包中真实路径）
        /// </summary>
        private static readonly Dictionary<string, (string,string)> _assetsMap = new Dictionary<string, (string,string)>();
        private static readonly Dictionary<string, (string,string)> _scenesMap = new Dictionary<string, (string,string)>();

        /// <summary>
        /// 初始化资源
        /// </summary>
        public static async UniTask Init()
        {
            _modAssets.Clear();
            _modScenes.Clear();
            _assetsMap.Clear();
            _scenesMap.Clear();
            
            //加载基础包
            var ab = await AssetBundle.LoadFromFileAsync(Path.Combine(AbDir, BaseAbName));
            foreach (var assetName in ab.GetAllAssetNames())
            {
                _assetsMap[assetName.ToLower()] = ("", assetName.ToLower());
            }
            _modAssets[""] = ab;
        }


        /// <summary>
        /// 加载MOD
        /// </summary>
        /// <param name="modId"></param>
        public static async UniTask LoadMod(string modId)
        {
            AssetBundle modAssetsAb = await AssetBundle.LoadFromFileAsync(Path.Combine(AbDir, $"{modId}_mod"));
            AssetBundle modScenesAb = await AssetBundle.LoadFromFileAsync(Path.Combine(AbDir, $"{modId}_maps"));
            _modAssets[modId] = modAssetsAb;
            _modScenes[modId] = modScenesAb;
            
            foreach (var assetName in modAssetsAb.GetAllAssetNames())
            {
                string prefix = $"assets/mods/{modId}/";
                var url = assetName.Replace(prefix, "assets/");

                _assetsMap[url] = (modId, assetName);
            }
            
            
            foreach (var sceneName in modScenesAb.GetAllScenePaths())
            {
                var lowSceneName = sceneName.ToLower();
                string prefix = $"assets/mods/{modId}/";

                var url = lowSceneName.Replace(prefix, "assets/");
                _scenesMap[url] = (modId, sceneName);
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="uri"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> LoadAsset<T>(string uri) where T : Object
        {
            if (!uri.ToLower().StartsWith("assets/"))
            {
                uri = "assets/" + uri;
            }
            
            var path = uri.ToLower();
            if (!_assetsMap.ContainsKey(path))
                return default(T);

            var ab = _assetsMap[path];
            var assetBundle = _modAssets[ab.Item1];
            
            var ret = await assetBundle.LoadAssetAsync<T>(ab.Item2);

            return (T) ret;
        }

        public static async UniTask<List<T>> LoadAssets<T>(string prefix) where T : Object
        {
            List<T> rst = new List<T>();
            foreach (var kv in _assetsMap)
            {
                if (!kv.Key.StartsWith(prefix.ToLower())) continue;

                var ab = kv.Value;
                var assetBundle = _modAssets[ab.Item1];
                var ret = await assetBundle.LoadAssetAsync<T>(ab.Item2);
                if (ret is T o)
                {
                    rst.Add(o);
                }
            }

            return rst;
        }

        public static async UniTask LoadScene(string path)
        {
            path = path.ToLower();
            if (_scenesMap.ContainsKey(path))
            {
                await SceneManager.LoadSceneAsync(_scenesMap[path].Item2);
                await UniTask.WaitForEndOfFrame();
            }
            else
            {
                Debug.LogError($"不存在的scene：{path}");
            }
        }
    }
}