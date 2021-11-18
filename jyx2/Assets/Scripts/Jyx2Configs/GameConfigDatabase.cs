using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HSFrameWork.ConfigTable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2Configs
{
    [DisallowMultipleComponent]
    public class GameConfigDatabase : MonoBehaviour
    {
        public static GameConfigDatabase Instance
        {
            get;
            private set;
        } 
        
        private readonly Dictionary<Type, Dictionary<int, Jyx2ConfigBase>> _dataBase =
            new Dictionary<Type, Dictionary<int, Jyx2ConfigBase>>();

        public async UniTask Init()
        {
            int total = 0;
            total += await Init<Jyx2ConfigCharacter>();
            total += await Init<Jyx2ConfigItem>();
            total += await Init<Jyx2ConfigSkill>();
            total += await Init<Jyx2ConfigShop>();
            total += await Init<Jyx2ConfigMap>();
            total += await Init<Jyx2ConfigBattle>();
            
            Debug.Log($"载入完成，总数{total}个配置asset");
        }

        public T Get<T>(int id) where T : Jyx2ConfigBase
        {
            var type = typeof(T);
            if (_dataBase.TryGetValue(type, out var db))
            {
                if (db.TryGetValue(id, out var asset))
                {
                    return (T)asset;
                }
            }
            return null;
        }
        public T Get<T>(string id) where T : Jyx2ConfigBase
        {
            return Get<T>(int.Parse(id));
        }

        public bool Has<T>(string id) where T : Jyx2ConfigBase
        {
            return Get<T>(id) != null;
        }
        
        public IEnumerable<T> GetAll<T>() where T : Jyx2ConfigBase
        {
            var type = typeof(T);
            if (_dataBase.TryGetValue(type, out var db))
            {
                foreach (var v in db.Values)
                {
                    yield return (T) v;
                }
            }
        }

        /// <summary>
        /// 初始化指定类型配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async UniTask<int> Init<T>() where T : Jyx2ConfigBase
        {
            if (_dataBase.ContainsKey(typeof(T)))
            {
                throw new Exception("类型" + typeof(T) + "已经创建过了，不允许重复创建！");
            }

            var assets = await Addressables
                .LoadAssetsAsync<T>(new List<object>() {"configs"}, null,
                    Addressables.MergeMode.Union).Task;

            var db = new Dictionary<int, Jyx2ConfigBase>();
            _dataBase[typeof(T)] = db;
            foreach (var asset in assets)
            {
                if (db.ContainsKey(asset.Id))
                {
                    Debug.Log($"ID重复，覆盖写入: {asset.Name}-->{db[asset.Id].Name}");
                }
                db[asset.Id] = asset;
                asset.WarmUp().Forget();
            }

            return db.Count;
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
    
    
}
