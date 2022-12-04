using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.MOD.ModV2;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Jyx2.ResourceManagement
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class ResLoader
    {
        /// <summary>
        /// 调试接口，打开即在editor下使用assetbundle调试，需要先build
        /// </summary>
        private const bool UseAbInEditor = false;
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
        private static readonly List<string> _modList = new List<string>();


        public static bool IsEditor()
        {
            if (UseAbInEditor) return false;
            return Application.isEditor;
        }
        
        /// <summary>
        /// 初始化资源
        /// </summary>
        public static async UniTask Init()
        {
            foreach (var ab in _modAssets.Values)
            {
                if (ab == null) continue;
                ab.Unload(true);
            }

            foreach (var ab in _modScenes.Values)
            {
                if (ab == null) continue;
                ab.Unload(true);
            }
                
            _modAssets.Clear();
            _modScenes.Clear();
            _assetsMap.Clear();
            _scenesMap.Clear();
            _modList.Clear();

            await Resources.UnloadUnusedAssets();

            if (IsEditor())
            {
                
            }
            else
            {
                //加载基础包
                var ab = await LoadAssetBundleFromStreamingAssets(BaseAbName);
                foreach (var assetName in ab.GetAllAssetNames())
                {
                    _assetsMap[assetName.ToLower()] = ("", assetName.ToLower());
                }
                _modAssets[""] = ab;
            }
        }

        public static async UniTask LaunchMod(GameModBase mod)
        {
            var modId = mod.Id.ToLower();
            if (mod is GameModEditor)
            {
                _modList.Add(modId);
            }
            else
            {
                AssetBundle modAssetsAb = await mod.LoadModAb();
                AssetBundle modScenesAb = await mod.LoadModMap();
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
        }
        

        private static async UniTask<AssetBundle> LoadAssetBundleFromStreamingAssets(string filename)
        {
            string path = Path.Combine(Application.streamingAssetsPath, filename);
            
#if UNITY_ANDROID && !UNITY_EDITOR
            path = "jar:file://" + Application.dataPath + "!/assets/" + filename;
#endif
            return await AssetBundle.LoadFromFileAsync(path);
        } 

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="uri"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> LoadAsset<T>(string uri) where T : Object
        {
            if (IsEditor())
            {
                return LoadAssetInEditor<T>(uri);
            }
            
            var path = ToAssetPath(uri);
            if (!_assetsMap.ContainsKey(path))
                return default(T);

            var ab = _assetsMap[path];
            var assetBundle = _modAssets[ab.Item1];
            
            var ret = await assetBundle.LoadAssetAsync<T>(ab.Item2);

            return (T) ret;
        }

        public static async UniTask<List<T>> LoadAssets<T>(string prefix) where T : Object
        {
            if (IsEditor())
            {
                return await LoadAssetsInEditor<T>(prefix);
            }
            
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


        public static T LoadAssetSync<T>(string uri) where T:Object
        {
            if (IsEditor())
            {
                return LoadAssetInEditor<T>(uri);
            }
            var path = ToAssetPath(uri);
            if (!_assetsMap.ContainsKey(path))
                return default(T);

            var ab = _assetsMap[path];
            var assetBundle = _modAssets[ab.Item1];

            var result = assetBundle.LoadAsset<T>(ab.Item2);

            return result;
        }

        private static string ToAssetPath(string uri)
        {
            var path = uri.ToLower();
            if (!path.StartsWith("assets/"))
            {
                path = "assets/" + path;
            }
            return path;
        }


        public static async UniTask LoadScene(string path)
        {
            if (IsEditor())
            {
                await LoadSceneInEditor(path);
                return;
            }

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

        #region editor mode


        private static IEnumerable<string> GetFixedModPath(string uri)
        {
            //先检查MOD
            for (int i=_modList.Count-1;i>=0;--i)
            {
                var modId = _modList[i];
                yield return uri.ToLower().Replace("assets/",$"assets/mods/{modId}/");
            }
        }

        private static T LoadAssetInEditor<T>(string uri) where T : Object
        {
            #if UNITY_EDITOR

            uri = ToAssetPath(uri);

            foreach (var fixedUri in GetFixedModPath(uri))
            {
                var loadAsset = AssetDatabase.LoadAssetAtPath<T>(fixedUri);
                if (loadAsset != null)
                    return loadAsset;
            }

            //再本体
            return AssetDatabase.LoadAssetAtPath<T>(uri);
            
            #else
            return null;
            #endif
        }

        private static async UniTask<List<T>> LoadAssetsInEditor<T>(string prefix) where T : Object
        {
            #if UNITY_EDITOR
            prefix = ToAssetPath(prefix);

            List<T> rst = new List<T>();
            foreach (var fixedUri in GetFixedModPath(prefix))
            {
                foreach (var asset in LoadAllAssetsInEditor<T>(fixedUri))
                { 
                    rst.Add(asset);
                }
            }
            
            foreach (var asset in LoadAllAssetsInEditor<T>(prefix))
            {
                rst.Add(asset);
            }

            return rst;
            
            #else
            return null;
            #endif
        }

        #if UNITY_EDITOR
        private static T[] LoadAllAssetsInEditor<T>(string path)
        {
            if (!Directory.Exists(path)) return new T[0];
            ArrayList al = new ArrayList();
            
            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
            {
                int index = fileName.LastIndexOf("/");
                string localPath = path;

                if (index > 0)
                    localPath += fileName.Substring(index);

                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

            T[] result = new T[al.Count];
            for (int i = 0; i < al.Count; i++)
                result[i] = (T) al[i];

            return result;
        }
        #endif

        private static async UniTask LoadSceneInEditor(string path)
        {
            #if UNITY_EDITOR
            path = ToAssetPath(path);
            foreach (var fixedUri in GetFixedModPath(path))
            {
                if (File.Exists(fixedUri))
                {
                    await EditorSceneManager.LoadSceneAsyncInPlayMode(fixedUri,
                        new LoadSceneParameters() {loadSceneMode = LoadSceneMode.Single});
                    return;
                }
            }
            #endif
        }
        #endregion
    }
}