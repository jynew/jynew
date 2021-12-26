using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2.MOD
{
    public static class MODLoader
    {
        public static List<string> ModList = new List<string>();// {"D:/jynew/MOD/replace_sprite"};

        public static Dictionary<string, Object> reloadRes = new Dictionary<string, Object>();
        
        public static async UniTask Init()
        {
            reloadRes.Clear();//for test
            
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

                    if (obj is Texture2D)
                    {
                        Texture2D t = obj as Texture2D;
                        reloadRes[overrideAddr] = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                    }
                    else
                    {
                        reloadRes[overrideAddr] = obj;    
                    }
                }
            }
        }

#region 复合MOD加载资源的接口
        public static async UniTask<Sprite> LoadSprite(string uri)
        {
            if (reloadRes.ContainsKey(uri.ToLower()))
            {
                var obj = reloadRes[uri.ToLower()];
                if (obj is Sprite)
                {
                    return obj as Sprite;
                }
            }
            return await Addressables.LoadAssetAsync<Sprite>(uri);
        }
#endregion
    }

    public class Jyx2ModInstance
    {
        public string uri;
        public AssetBundle assetBundle;
    }
}
