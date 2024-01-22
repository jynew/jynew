using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Jyx2.Util
{
    public static class SpriteLoaderUtil
    {
        public static async UniTask<Sprite> DownloadSprite(this string url)
        {
            try
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var tex2d = DownloadHandlerTexture.GetContent(www);
                    Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height),
                        new Vector2(0.5f, 0.5f));
                    return sprite;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                Debug.LogError("加载远程图片资源失败, url:" + url);
                return null;
            }
        }
    }
}