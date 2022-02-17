/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

//TODO：MOD载入框架尚未完成
// - 配置表的载入
// - 场景、战斗场景载入
// - lua的载入
// - UI的载入
// - 其他相关资源的载入
// - MOD配置相关UI界面
// - 各种MODSample

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Jyx2.MOD
{
    public static class MODLoader
    { 
        public struct AssetBundleItem
        {
            public string Name;
            public AssetBundle Ab;
        }

        /// <summary>
        /// 存储所有的重载资源
        /// </summary>
        public static readonly Dictionary<string, AssetBundleItem> Remap = new Dictionary<string, AssetBundleItem>();

        public static async UniTask Init()
        {
            Remap.Clear();//for test
            
            var modList = MODManager.ModEntries.Where(modEntry => modEntry.Active);
            
            foreach (var mod in modList)
            {
                var ab = await AssetBundle.LoadFromFileAsync(mod.Path);
                if (ab == null)
                {
                    Debug.LogError($"载入MOD失败：{mod.Path}");
                    continue;
                }

                //记录和复写所有的MOD重载资源
                foreach (var name in ab.GetAllPaths())
                {
                    Debug.Log($"mod file:{name}");
                    Remap[name] = new AssetBundleItem() { Name = name, Ab = ab };
                }
            }
        }

        private static string[] GetAllPaths(this AssetBundle ab)
        {
            return ab.GetAllScenePaths().Concat(ab.GetAllAssetNames()).ToArray();
        }

#region 复合MOD加载资源的接口
        public static async UniTask<T> LoadAsset<T>(string uri) where T : Object
        {
            if (Remap.ContainsKey(uri.ToLower()))
            {
                var assetBundleItem = Remap[uri.ToLower()];
                return assetBundleItem.Ab.LoadAsset<T>(assetBundleItem.Name);
            }
            return await Addressables.LoadAssetAsync<T>(uri);
        }

        public static async UniTask<List<T>> LoadAssets<T>(List<string> uris) where T : Object
        {
            var allAssets = await Addressables.LoadAssetsAsync<T>(uris, null, Addressables.MergeMode.Union);
            var commonKeys = uris.Select(uri => uri.ToLower()).Intersect(Remap.Keys);
            var assets = commonKeys.Select(key => Remap[key].Ab.LoadAsset<T>(Remap[key].Name));
            return allAssets.Union(assets).ToList();
        }
#endregion

        public static void SaveOverrideList(string path, string filter)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "OverrideList.txt");
            var fileContentsList = GetOverridePaths(path, filter);
#if UNITY_EDITOR
            File.AppendAllLines(filePath, fileContentsList.ToArray());
#endif
        }

        private static List<string> GetOverridePaths(string path, string filter)
        {
            var fileList = new List<string>();
            var overrideList = new List<string>();
            FileTools.GetAllFilePath(path, fileList, new List<string>() { filter });

            foreach (var filePath in fileList)
            {
                var overridePath = filePath.Substring(filePath.IndexOf("Assets", StringComparison.Ordinal));
                overrideList.Add(overridePath);
            }
            
            return overrideList;
        }

        public static List<string> LoadOverrideList(string path)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "OverrideList.txt");
            List<string> fileContentsList;
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest request = UnityWebRequest.Get(filePath);
                request.SendWebRequest();
                while (!request.isDone) { }
                string textString = request.downloadHandler.text;
                fileContentsList = textString.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            }
            else
            {
                fileContentsList = File.ReadAllLines(filePath).ToList(); 
            }
            
            var lineList = fileContentsList.Where(line => line.StartsWith(path)).ToList();
            
            return lineList;
        }
    }
}
