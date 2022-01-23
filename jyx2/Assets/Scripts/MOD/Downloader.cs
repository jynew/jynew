using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Jyx2.MOD
{
    public class ResourceLoadCompletedEventArgs :  EventArgs
    {
        public string ResourceName { get; set; }

        public ResourceLoadCompletedEventArgs(string name)
        {
            ResourceName = name;
        }
    }
    public class Downloader
    {
        public delegate void OnResourceDownloadedEventHandler(object sender, ResourceLoadCompletedEventArgs args);
        /// <summary>
        /// 下载的进度。(-1表示当前不在下载过程。)
        /// </summary>
        public float DownloadProgress
        {
            get
            {
                if (_isProgressing && _requestCache != null)
                    return _requestCache.downloadProgress;
                else
                    // UnityWebRequest对象是一次性的，下载完成后就被销毁，所以不能再去访问，否则会出现错误。
                    return -1;
            }
        }

        public event OnResourceDownloadedEventHandler ResourceLoadingCompleted;
        
        /// <summary>
        /// 当前是否在下载的指示器。
        /// </summary>
        private bool _isProgressing;

        /// <summary>
        /// 由于UnityWebRequest是使用using方式创建，需要缓存一下以便能够访问。
        /// </summary>
        private UnityWebRequest _requestCache;

        public async UniTask<Sprite> DownloadSprite(string uri)
        {
            using (var request = UnityWebRequestTexture.GetTexture(uri))
            {
                _requestCache = request;
                _isProgressing = true;

                await request.SendWebRequest();
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.Log(request.error);
                    throw new InvalidOperationException();
                }
                else
                {
                    var texture2D =
                        DownloadHandlerTexture.GetContent(request);
                    var sprite = Sprite.Create(
                        texture2D,
                        new Rect(0, 0, texture2D.width, texture2D.height),
                        new Vector2(0.5f, 0.5f));
                    _isProgressing = false;
                    if (ResourceLoadingCompleted != null)
                        ResourceLoadingCompleted(sprite, new ResourceLoadCompletedEventArgs(sprite.name));
                    return sprite;
                }
            }
        }

        public async UniTask DownloadFile(string uri, string path, bool isAutoDeleteWrongFile = true)
        {
            using (var request = UnityWebRequest.Get(uri))
            {
                _requestCache = request;
                _isProgressing = true;

                var dh = new DownloadHandlerFile(path);
                dh.removeFileOnAbort = isAutoDeleteWrongFile;
                request.downloadHandler = dh;
                await request.SendWebRequest();
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.Log(request.error);
                    throw new InvalidOperationException();
                }
                else
                {
                    _isProgressing = false;
                    Debug.Log("File successfully downloaded and saved to " + path);
                    if (ResourceLoadingCompleted != null)
                        ResourceLoadingCompleted(null, new ResourceLoadCompletedEventArgs(null));
                }
            }
        }
    }
}
