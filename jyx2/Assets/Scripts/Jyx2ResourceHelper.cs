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
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
        var handler = await Addressables.LoadAssetAsync<TextAsset>("Assets/BuildSource/PreCachedPrefabs.txt").Task;
        
        cachedPrefabs = new Dictionary<string, GameObject>();

        foreach (var path in handler.text.Split('\n'))
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
        var task = await Addressables.LoadAssetsAsync<Jyx2SkillDisplayAsset>("skills", null).Task;
        if (task != null)
        {
            Jyx2SkillDisplayAsset.All = task;
        }

        //全局配置表
        var t = await Addressables.LoadAssetAsync<GlobalAssetConfig>("Assets/BuildSource/Configs/GlobalAssetConfig.asset").Task;
        if (t != null)
        {
            GlobalAssetConfig.Instance = t;
        }
    }

    public static GameObject GetCachedPrefab(string path)
    {
        if (cachedPrefabs.ContainsKey(path))
            return cachedPrefabs[path];

        Debug.LogError($"载入缓存的Prefab失败：{path}(是否没写入Assets/BuildSource/PreCachedPrefabs.txt?)");

        return null;
    }

    public static GameObject CreatePrefabInstance(string path)
    {
        var obj = GetCachedPrefab(path);
        return Object.Instantiate(obj);
        //return LeanPool.Spawn(obj);
    }

    public static void ReleasePrefabInstance(GameObject obj)
    {
        Object.Destroy(obj);
        //LeanPool.Despawn(obj);
    }

    public static async UniTask<Sprite> GetRoleHeadSprite(string path)
    {
        string p = "Assets/BuildSource/head/" + path + ".png";
        return await Addressables.LoadAssetAsync<Sprite>(p).Task;
    }

    [Obsolete("待修改为tilemap")]
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

    [Obsolete("待修改为tilemap")]
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

    public static UniTask<Sprite> GetSprite(RoleInstance role)
    {
        if (role.Key == GameRuntimeData.Instance.Player.Key)
        {
            return GetRoleHeadSprite(GameRuntimeData.Instance.Player.HeadAvata);
        }
        else
        {
            return GetRoleHeadSprite(role.HeadAvata);
        }
    }


    public static void GetRoleHeadSprite(RoleInstance role, Image setImage)
    {
        DoGetRoleHeadSprite(role,setImage).Forget();
    }

    private static async UniTaskVoid DoGetRoleHeadSprite(RoleInstance role, Image setImage)
    {
        var sprite = await GetSprite(role);
        setImage.sprite = sprite;
    }

    public static async UniTask<Sprite> LoadItemSprite(int itemId)
    {
        string p = ("Assets/BuildSource/Jyx2Items/" + itemId + ".png");
        return await Addressables.LoadAssetAsync<Sprite>(p).Task;
    }

    public static void SpawnPrefab(string path, Action<GameObject> callback)
    {
        Addressables.InstantiateAsync(path).Completed += r => { callback(r.Result); };
    }

    public static void LoadAsset<T>(string path, Action<T> callback)
    {
        Addressables.LoadAssetAsync<T>(path).Completed += r => { callback(r.Result); };
    }

    public static async UniTask<Jyx2NodeGraph> LoadEventGraph(int id)
    {
        string url = $"Assets/BuildSource/EventsGraph/{id}.asset";
        var rst = await Addressables.LoadResourceLocationsAsync(url).Task;
        if (rst.Count == 0)
        {
            return null;
        }

        return await Addressables.LoadAssetAsync<Jyx2NodeGraph>(url).Task;
    }
}