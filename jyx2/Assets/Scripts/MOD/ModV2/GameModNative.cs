
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Tools = Jyx2.Middleware.Tools;

namespace Jyx2.MOD.ModV2
{
    /// <summary>
    /// 游戏原生MOD
    ///
    /// 来自StreamingAssets，随包发布
    /// 将在MOD列表中置顶显示，并标记 [原生] 标签
    /// </summary>
    public class GameModNative : GameModBase
    {
        public override async UniTask<AssetBundle> LoadModAb()
        {
            var path = GetUrlFromStreamingAssets($"{Id}_mod");
            return await AssetBundle.LoadFromFileAsync(path);
        }

        public override async UniTask<AssetBundle> LoadModMap()
        {
            var path = GetUrlFromStreamingAssets($"{Id}_maps");
            return await AssetBundle.LoadFromFileAsync(path);
        }

        protected override string Tag => "原生";

        private static string GetUrlFromStreamingAssets(string file)
        {
#if UNITY_ANDROID
            var path = "jar:file://" + Application.dataPath + "!/assets/" + file;
#else
            var path = Path.Combine(Application.streamingAssetsPath, file);
#endif
            return path;
        }
    }
    
    public class GameModNativeLoader : GameModLoader
    {
        /// <summary>
        /// 获取所有的原生MOD
        /// </summary>
        /// <returns></returns>
        public override async UniTask<List<GameModBase>> LoadMods()
        {
            var allMods = new List<GameModBase>();
            string samples = await GetTextForStreamingAssets("native_mods.txt");
            
            foreach (var line in samples.Split(','))
            {
                if (line.StartsWith("#")) continue;
                if (string.IsNullOrEmpty(line)) continue;
                var modId = line.Trim();

                var mod = await TryLoadModIndexer(modId);
                if (mod != null)
                {
                    allMods.Add(mod);
                }
            }

            return allMods;
        }
        
        //尝试载入mod索引
        static async UniTask<GameModNative> TryLoadModIndexer(string id)
        {
            //载入XML文件
            var content = await GetTextForStreamingAssets($"{id}.xml");
            if (content.IsNullOrWhitespace()) return null;
            var modInfo = Tools.DeserializeXML<GameModInfo>(content);
            if (modInfo == null) return null;
            var mod = new GameModNative() {Info = modInfo};
            return mod;
        } 
        
        static async UniTask<string> GetTextForStreamingAssets(string path)
        {
            var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, path));
            UnityWebRequest request = UnityWebRequest.Get(uri);
            try
            {
                await request.SendWebRequest();//读取数据
                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                //文件不存在会导致404错误码，相关http错误WebRequest直接抛的异常
                Debug.LogError(ex);
                return null;
            }
        }

    }
}