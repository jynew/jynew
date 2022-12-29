/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using Jyx2;
using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jyx2.EventsGraph;
using Jyx2.Middleware;
using Jyx2.MOD.ModV2;
using Jyx2.ResourceManagement;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Jyx2
{
    public static class ImageLoadHelper
    {
        async static UniTask<Sprite> GetPicTask(string path)
        {
            var _sprite = await ResLoader.LoadAsset<Sprite>(path);
            return _sprite;
        }
        //public static void LoadAsyncForget(this Image image, UniTask<Sprite> task)
        public static void LoadAsyncForget(this Image image, string path)
        {   
            LoadAsync(image,GetPicTask(path)).Forget();
        }
        
        public static async UniTask LoadAsync(this Image image, UniTask<Sprite> task)
        {
            image.gameObject.SetActive(false);
            image.sprite = await task;
            image.gameObject.SetActive(true);
        }
    }
}

public static class Jyx2ResourceHelper
{
    public static async UniTask Init()
    {
        //模型池
        var allModels = await ResLoader.LoadAssets<ModelAsset>("Assets/Models/");
        if (allModels != null)
        {
            ModelAsset.All = allModels;
        }
        
        //技能池
        var allSkills = await ResLoader.LoadAssets<Jyx2SkillDisplayAsset>("Assets/Skills/");
        if (allSkills != null)
        {
            Jyx2SkillDisplayAsset.All = allSkills;
        }

        //lua
        await LuaManager.InitLuaMapper();
        
        //执行lua根文件
        LuaManager.Init(GlobalAssetConfig.Instance.rootLuaFile.text);

        //初始化LuaScripts
        await InitLuaScripts();
        
        //基础配置表
        await InitConfigTables();

        //初始化基础配置
        GameSettings.Refresh();
        
        //如果有热更新文件，执行热更新
        LuaManager.PreloadLua();
        
        //IFix热更新文件
        await IFixManager.LoadPatch();
    }

    private static async Task InitLuaScripts()
    {
        var initFile = await ResLoader.LoadAsset<TextAsset>("Assets/LuaScripts/InitLuaScripts.lua");
        var luaEnv = LuaManager.GetLuaEnv();
        luaEnv.DoString(System.Text.Encoding.UTF8.GetBytes(initFile.text));
    }

    private static async Task InitConfigTables()
    {
        var mod = RuntimeEnvSetup.GetCurrentMod();

        //编辑器模式下直接从excel中载入，不需要再打包
        /*if (mod is GameModEditor editor)
        {
            var excelDir = Path.Combine(editor.RootConfig.ModRootDir, "Configs");
            var data = ExcelTools.LoadFromExcels(excelDir);
            GameConfigDatabase.Instance.Init(data);   
        }
        else //否则从打包载入
        {
            var config = await ResLoader.LoadAsset<TextAsset>($"Assets/Configs/Datas.bytes");
            GameConfigDatabase.Instance.Init(config.bytes);    

        }
        */
        //从Lua表读取配置
        var configs = await ResLoader.LoadAssets<TextAsset>("Assets/Configs/Lua/");
        Debug.Log("load configs num: "+configs.Count);
        var luaEnv = LuaManager.GetLuaEnv();
        foreach (var file in configs)
        {
            try{
                luaEnv.DoString(System.Text.Encoding.UTF8.GetBytes(file.text));
            }
            catch{
                Debug.LogError($"{file.name} 运行出错");
                throw;
            }
        }
        LuaToCsBridge.LuaConfRef();
    }

    public static GameObject GetCachedPrefab(string path)
    {
        if(GlobalAssetConfig.Instance.CachePrefabDict.TryGetValue(path, out var prefab))
        {
            return prefab;
        }

        Debug.LogError($"载入缓存的Prefab失败：{path}(是否没填入GlobalAssetConfig.CachedPrefabs?)");
        return null;
    }

    public static GameObject CreatePrefabInstance(string path)
    {
        var obj = GetCachedPrefab(path);
        return Object.Instantiate(obj);
    }

    public static void ReleasePrefabInstance(GameObject obj)
    {
        Object.Destroy(obj);
    }

    [Obsolete("待修改为tilemap")]
    public static async UniTask<SceneCoordDataSet> GetSceneCoordDataSet(string sceneName)
    {
        string path = $"{ConStr.BattleBlockDatasetPath}{sceneName}_coord_dataset.bytes";
        var result = await ResLoader.LoadAsset<TextAsset>(path);
        using var memory = new MemoryStream(result.bytes);
        return Serializer.Deserialize<SceneCoordDataSet>(memory);
    }

    public static async UniTask<Jyx2NodeGraph> LoadEventGraph(string id)
    {
        string url = $"Assets/BuildSource/EventsGraph/{id}.asset";

        return await ResLoader.LoadAsset<Jyx2NodeGraph>(url);
    }
}
