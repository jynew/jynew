using HanSquirrel.ResourceManager;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using HSFrameWork.Common;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lean.Pool;

static public class Jyx2ResourceHelper
{
    static Dictionary<string, GameObject> cachedPrefabs;

    async static public Task Init()
    {
        //已经初始化过了
        if(cachedPrefabs != null)
        {
            return;
        }

        var handler = Addressables.LoadAssetAsync<TextAsset>("Assets/BuildSource/PreCachedPrefabs.txt").Task;
        await handler;

        cachedPrefabs = new Dictionary<string, GameObject>();

        foreach (var path in handler.Result.text.Split('\n'))
        {
            if (string.IsNullOrEmpty(path)) continue;

            var p = path.Replace("\r", "");
            var h = Addressables.LoadAssetAsync<GameObject>(p).Task;
            await h;
            if(h.Result != null)
            {
                cachedPrefabs[p] = h.Result;
                Debug.Log("cached prefab:" + p);
            }
        }
    }

    static public GameObject GetCachedPrefab(string path)
    {
        if (cachedPrefabs.ContainsKey(path))
            return cachedPrefabs[path];

        Debug.LogError($"载入缓存的Prefab失败：{path}(是否没写入Assets/BuildSource/PreCachedPrefabs.txt?)");

        return null;
    }

    static public GameObject CreatePrefabInstance(string path)
    {
        var obj = GetCachedPrefab(path);
        return LeanPool.Spawn(obj);
    }

    static public void ReleasePrefabInstance(GameObject obj)
    {
        LeanPool.Despawn(obj);
    }

    static public void GetRoleHeadSprite(string path, Action<Sprite> callback)
    {
        string p = ("Assets/BuildSource/head/" + path + ".png");
        Addressables.LoadAssetAsync<Sprite>(p).Completed += r => { callback(r.Result); };
    }

    static public void GetSceneCoordDataSet(string sceneName, Action<SceneCoordDataSet> callback)
    {
        string path = $"{ConStr.BattleBlockDatasetPath}{sceneName}_coord_dataset.bytes";
        Addressables.LoadAssetAsync<TextAsset>(path).Completed += r =>
        {
            if (r.Result == null)
                callback(null);
            var obj = r.Result.bytes.Deserialize<SceneCoordDataSet>();
            callback(obj);
        };
    }
    
    static public void GetBattleboxDataset(string fullPath, Action<BattleboxDataset> callback)
    {
        Addressables.LoadAssetAsync<TextAsset>(fullPath).Completed += r =>
        {
            if (r.Result == null)
                callback(null);
            var obj = r.Result.bytes.Deserialize<BattleboxDataset>();
            callback(obj);
        };
    }
    

    static public void GetSprite(RoleInstance role, Action<Sprite> callback)
    {
        if (role.Key == GameRuntimeData.Instance.Player.Key)
        {
            GetRoleHeadSprite(GameRuntimeData.Instance.Player.HeadAvata, callback);
        }
        else
        {
            GetRoleHeadSprite(role.HeadAvata, callback);
        }
    }

    static public void GetRoleHeadSprite(string path, Image setImage)
    {
        GetRoleHeadSprite(path, r => setImage.sprite = r);
    }
    
    static public void GetRoleHeadSprite(RoleInstance role, Image setImage)
    {
        GetSprite(role, r => setImage.sprite = r);
    }

    static public void GetItemSprite(int itemId, Image setImage)
    {
        string p = ("Assets/BuildSource/Jyx2Items/" + itemId + ".png");
        Addressables.LoadAssetAsync<Sprite>(p).Completed += r => {
            setImage.sprite = r.Result;
        };
    }

    static public void GetSprite(string iconName, string atlasName, Action<Sprite> cb) 
    {
        string path = $"Assets/BuildSource/UI/{atlasName}/{iconName}.png";
        Addressables.LoadAssetAsync<Sprite>(path).Completed += r =>
        {
            cb?.Invoke(r.Result);
        };
    }


    static public void SpawnPrefab(string path, Action<GameObject> callback)
    {
        Addressables.InstantiateAsync(path).Completed += r => { callback(r.Result); };
    }


    static public void LoadPrefab(string path, Action<GameObject> callback)
    {
        Addressables.LoadAssetAsync<GameObject>(path).Completed += r => { callback(r.Result); };
    }

    static public void LoadAsset<T>(string path, Action<T> callback)
    {
        Addressables.LoadAssetAsync<T>(path).Completed += r => { callback(r.Result); };
    }

    static public void ReleaseInstance(GameObject obj)
    {
        if(obj != null)
        {
            Addressables.ReleaseInstance(obj);
        }
    }
}
