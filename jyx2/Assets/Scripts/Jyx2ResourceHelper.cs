/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using HSFrameWork.Common;
using Jyx2;
using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

static public class Jyx2ResourceHelper
{
    private static Dictionary<string, GameObject> cachedPrefabs;

    public static async Task Init()
    {
        //已经初始化过了
        if (cachedPrefabs != null)
        {
            return;
        }

        //所有需要预加载的资源
        var handler = Addressables.LoadAssetAsync<TextAsset>("Assets/BuildSource/PreCachedPrefabs.txt").Task;
        await handler;

        cachedPrefabs = new Dictionary<string, GameObject>();

        foreach (var path in handler.Result.text.Split('\n'))
        {
            if (string.IsNullOrEmpty(path)) continue;

            var p = path.Replace("\r", "");
            var h = Addressables.LoadAssetAsync<GameObject>(p).Task;
            await h;
            if (h.Result != null)
            {
                cachedPrefabs[p] = h.Result;
                Debug.Log("cached prefab:" + p);
            }
        }

        //技能池
        var task = Addressables.LoadAssetsAsync<Jyx2SkillDisplayAsset>("skills", null).Task;
        await task;
        if (task.Result != null)
        {
            Jyx2SkillDisplayAsset.All = task.Result;
        }

        //全局配置表
        var t = Addressables.LoadAssetAsync<GlobalAssetConfig>("Assets/BuildSource/Configs/GlobalAssetConfig.asset").Task;
        await t;
        if (t.Result != null)
        {
            GlobalAssetConfig.Instance = t.Result;
        }
    }

    static public GameObject GetCachedPrefab(string path)
    {
        if (cachedPrefabs.ContainsKey(path))
            return cachedPrefabs[path];

        Debug.LogError($"载入缓存的Prefab失败：{path}(是否没写入Assets/BuildSource/PreCachedPrefabs.txt?)");

        return null;
    }

    public static GameObject CreatePrefabInstance(string path)
    {
        var obj = GetCachedPrefab(path);
        return LeanPool.Spawn(obj);
    }

    public static void ReleasePrefabInstance(GameObject obj)
    {
        LeanPool.Despawn(obj);
    }

    public static void GetRoleHeadSprite(string path, Action<Sprite> callback)
    {
        string p = ("Assets/BuildSource/head/" + path + ".png");
        Addressables.LoadAssetAsync<Sprite>(p).Completed += r => { callback(r.Result); };
    }

    public static void GetSceneCoordDataSet(string sceneName, Action<SceneCoordDataSet> callback)
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

    public static void GetBattleboxDataset(string fullPath, Action<BattleboxDataset> callback)
    {
        Addressables.LoadAssetAsync<TextAsset>(fullPath).Completed += r =>
        {
            if (r.Result == null)
                callback(null);
            var obj = r.Result.bytes.Deserialize<BattleboxDataset>();
            callback(obj);
        };
    }

    public static void GetSprite(RoleInstance role, Action<Sprite> callback)
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

    public static void GetRoleHeadSprite(string path, Image setImage)
    {
        GetRoleHeadSprite(path, r => setImage.sprite = r);
    }

    public static void GetRoleHeadSprite(RoleInstance role, Image setImage)
    {
        GetSprite(role, r => setImage.sprite = r);
    }

    public static void GetItemSprite(int itemId, Image setImage)
    {
        string p = ("Assets/BuildSource/Jyx2Items/" + itemId + ".png");
        Addressables.LoadAssetAsync<Sprite>(p).Completed += r =>
        {
            setImage.sprite = r.Result;
        };
    }

    public static void GetSprite(string iconName, string atlasName, Action<Sprite> cb)
    {
        string path = $"Assets/BuildSource/UI/{atlasName}/{iconName}.png";
        Addressables.LoadAssetAsync<Sprite>(path).Completed += r =>
        {
            cb?.Invoke(r.Result);
        };
    }

    public static void SpawnPrefab(string path, Action<GameObject> callback)
    {
        Addressables.InstantiateAsync(path).Completed += r => { callback(r.Result); };
    }

    public static void LoadPrefab(string path, Action<GameObject> callback)
    {
        Addressables.LoadAssetAsync<GameObject>(path).Completed += r => { callback(r.Result); };
    }

    public static void LoadAsset<T>(string path, Action<T> callback)
    {
        Addressables.LoadAssetAsync<T>(path).Completed += r => { callback(r.Result); };
    }

    public static void ReleaseInstance(GameObject obj)
    {
        if (obj != null)
        {
            Addressables.ReleaseInstance(obj);
        }
    }

    public static async void LoadEventGraph(int id, Action<Jyx2NodeGraph> successCallback, Action failed)
    {
        string url = $"Assets/BuildSource/EventsGraph/{id}.asset";
        var handle = Addressables.LoadResourceLocationsAsync(url);
        await handle.Task;

        if (handle.Result.Count == 0)
        {
            failed();
            return;
        }

        var task = Addressables.LoadAssetAsync<Jyx2NodeGraph>(url).Task;
        await task;
        if (task.Result != null)
        {
            successCallback(task.Result);
        }
        else
        {
            failed();
        }
    }
}