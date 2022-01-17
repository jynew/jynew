using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Jyx2.MOD
{
    public class DownloadManager
    {
        private UnityWebRequest _uwr;

        public event Action<float> OnProgress;

        /// <summary>
        /// 是否操作完成
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        /// 操作进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        /// 已完成大小
        /// </summary>
        public ulong LoadedSize { get; private set; }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool IsDisposeed => _uwr == null;

        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string SavePath { get; private set; }

        public IEnumerator DownloadFile(string uri, string path, Action<float>onProgress, bool isAutoDeleteWrongFile = true)
        {
            SavePath = path;
            _uwr = UnityWebRequest.Get(uri);
            var dh = new DownloadHandlerFile(path);
            dh.removeFileOnAbort = isAutoDeleteWrongFile;
            _uwr.downloadHandler = dh;
            _uwr.SendWebRequest();
            yield return DownloadProgress(_uwr, onProgress);
            
            IsDone = _uwr.isDone;
            if (_uwr.isNetworkError || _uwr.isHttpError)
            {
                Debug.LogError(_uwr.error);
                Error = _uwr.error;
            }
            else
            {
                Debug.Log("File successfully downloaded and saved to " + path);
                LoadedSize = _uwr.downloadedBytes;
            }
        }

        private IEnumerator DownloadProgress(UnityWebRequest uwr, Action<float>onProgress)
        {
            while (!uwr.isDone)
            {
                Debug.Log(uwr.downloadProgress);
                Progress = uwr.downloadProgress;
                
                if (onProgress != null)
                {
                    Debug.Log("onProgess event not null");
                    onProgress(uwr.downloadProgress);
                }
                
                yield return null;
            }

            if (uwr.isDone)
            {
                if (onProgress != null) onProgress(1);
            }
        }

        public IEnumerator DownloadSprite(string uri, Action<Sprite> onSprite)
        {
            _uwr = UnityWebRequest.Get(uri);
            _uwr.downloadHandler = new DownloadHandlerTexture();
            yield return _uwr.SendWebRequest();
            
            IsDone = _uwr.isDone;
            Texture2D texture = null;
            if (_uwr.isNetworkError || _uwr.isHttpError)
            {
                Debug.LogError(_uwr.error);
                Error = _uwr.error;
            }
            else
            {
                Texture2D texture2D =
                    DownloadHandlerTexture.GetContent(_uwr);
                Sprite sp = Sprite.Create(
                        texture2D,
                        new Rect(0, 0, texture2D.width, texture2D.height),
                        new Vector2(0.5f, 0.5f));
                onSprite(sp);
            }
        }

        /// <summary>
        /// 中断下载
        /// </summary>
        public void Abort()
        {
            if (null != _uwr)
            {
                _uwr.Abort();
            }
        }
        
        /// <summary>
        /// 销毁对象，会停止所有的下载
        /// </summary>
        public void Dispose()
        {
            if (null != _uwr)
            {
                _uwr.Dispose();
                _uwr = null;
            }
        }
    }
}