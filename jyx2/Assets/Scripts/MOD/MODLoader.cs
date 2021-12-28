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

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2.MOD
{
    public static class MODLoader
    {
        /// <summary>
        /// TODO：添加界面可配置和可导入
        /// </summary>
        public static readonly List<string> ModList = new List<string>();// {"D:/jynew/MOD/replace_sprite","D:/jynew/MOD/replace_audio"};

        /// <summary>
        /// 存储所有的重载资源
        /// </summary>
        private static readonly Dictionary<string, Object> _remapResources = new Dictionary<string, Object>();
        
        public static async UniTask Init()
        {
            _remapResources.Clear();//for test
            
            foreach (var modUri in ModList)
            {
                var ab = await AssetBundle.LoadFromFileAsync(modUri);
                if (ab == null)
                {
                    Debug.LogError($"载入MOD失败：{modUri}");
                    continue;
                }

                Jyx2ModInstance modInstance = new Jyx2ModInstance()
                    {uri = modUri, assetBundle = ab};
                
                //记录和复写所有的MOD重载资源
                foreach (var name in ab.GetAllAssetNames())
                {
                    Debug.Log($"mod file:{name}");
                    var obj = ab.LoadAsset(name);
                    string overrideAddr = "assets/" + name.Substring(name.IndexOf("buildsource"));

                    //UI
                    if (obj is Texture2D)
                    {
                        Texture2D t = obj as Texture2D;
                        _remapResources[overrideAddr] = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                    }
                    //声音
                    else if (obj is AudioClip)
                    {
                        AudioClip t = obj as AudioClip;
                        _remapResources[overrideAddr] = t;
                    }
                    else
                    {
                        _remapResources[overrideAddr] = obj;    
                    }
                }
            }
        }

#region 复合MOD加载资源的接口
        public static async UniTask<T> LoadAsset<T>(string uri) where T : Object
        {
            if (_remapResources.ContainsKey(uri.ToLower()))
            {
                var obj = _remapResources[uri.ToLower()];
                if (obj is T)
                {
                    return obj as T;
                }
            }
            return await Addressables.LoadAssetAsync<T>(uri);
        }
#endregion
    }

    public class Jyx2ModInstance
    {
        public string uri;
        public AssetBundle assetBundle;
    }
}
