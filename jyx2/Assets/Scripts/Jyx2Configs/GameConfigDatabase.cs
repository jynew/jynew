using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.Networking;

namespace Jyx2Configs
{
    public class GameConfigDatabase
    {
        #region Singleton
        public static GameConfigDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameConfigDatabase();
                return _instance;
            }
        }

        private static GameConfigDatabase _instance;
        #endregion

        private Dictionary<Type, Dictionary<int, Jyx2ConfigBase>> _dataBase =
            new Dictionary<Type, Dictionary<int, Jyx2ConfigBase>>();

        private bool _isInited = false;


        /// <summary>
        /// 载入配置表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Init(byte[] data)
        {
            if (_isInited)
                return;
            
            _isInited = true;
            _dataBase.Clear();
            
            _dataBase = ExcelTools.ProtobufDeserialize<Dictionary<Type, Dictionary<int, Jyx2ConfigBase>>>(data);
        }
        
        /// <summary>
        /// 根据ID获取Config
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(string id) where T : Jyx2ConfigBase
        {
            return Get<T>(int.Parse(id));
        }
        
        /// <summary>
        /// 根据ID获取Config
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(int id) where T : Jyx2ConfigBase
        {
            if(_dataBase.TryGetValue(typeof(T), out var configMap))
            {
                if (configMap.TryGetValue(id, out var v))
                {
                    return (T)v;
                }
            }

            return null;
        }

        /// <summary>
        /// 是否包含一个值
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Has<T>(string id) where T : Jyx2ConfigBase
        {
            return Get<T>(id) != null;
        }
        
        /// <summary>
        /// 获取所有的Config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>() where T : Jyx2ConfigBase
        {
            if (_dataBase.TryGetValue(typeof(T), out var configMap))
            {
                foreach (var v in configMap)
                {
                    yield return (T) v.Value;
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            _dataBase.Clear();
            _instance = null;
        }

        /// <summary>
        /// 获取Config的种类数量
        /// </summary>
        /// <returns></returns>
        public int GetConfigTypeCount()
        {
            return _dataBase.Count;
        }

        /// <summary>
        /// 拷贝文件到指定目录
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        async UniTask CopyFile(string sourcePath, string destPath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
                {
                    var dh = new DownloadHandlerFile(sourcePath);
                    dh.removeFileOnAbort = true;
                    request.downloadHandler = dh;
                    await request.SendWebRequest();
                }
            }
            else
            {
                File.Copy(sourcePath, destPath, true);
            }
        }
    }
}
