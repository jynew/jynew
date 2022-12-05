/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Jyx2;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Jyx2Configs;
using UnityEngine.Playables;

//待重构
public class StoryEngine : MonoBehaviour
{
    public static StoryEngine Instance;

    public bl_HUDText HUDRoot;
    

    public bool BlockPlayerControl
    {
        get { return _blockPlayerControl; }
        set { _blockPlayerControl = value; }
    }


    private bool _blockPlayerControl;

    private static GameRuntimeData runtime
    {
        get { return GameRuntimeData.Instance; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public static async void DisplayPopInfo(string msg, float duration = 2f)
    {
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(CommonTipsUIPanel), TipsType.Common, msg, duration);
    }

    public static bool DoLoadGame(int index)
    {
        try
        {
            //加载存档
            var r = GameRuntimeData.LoadArchive(index);
            if (r == null)
            {
                return false;
            }

            //初始化角色
            foreach (var role in r.AllRoles.Values)
            {
                role.BindKey();
            }

            var loadPara = new LevelMaster.LevelLoadPara() {loadType = LevelMaster.LevelLoadPara.LevelLoadType.Load};

            //加载地图
            int mapId = -1;
            if (r.SubMapData == null)
            {
                mapId = GameConst.WORLD_MAP_ID;
                loadPara.Pos = r.WorldData.WorldPosition;
                loadPara.Rotate = r.WorldData.WorldRotation;
            }
            else
            {
                mapId = r.SubMapData.MapId;
                loadPara.Pos = r.SubMapData.CurrentPos;
                loadPara.Rotate = r.SubMapData.CurrentOri;
            }

            var map = GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(mapId);

            if (map == null)
            {
                throw new Exception("存档中的地图找不到！");
            }
            LevelMaster.LastGameMap = null;
            LevelLoader.LoadGameMap(map, loadPara,
                () => { LuaExecutor.Clear(); 
                        LevelMaster.Instance.TryBindPlayer().Forget(); });
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.ShowMessage("错误，载入存档失败。请检查版本号和MOD是否匹配。");
            Debug.LogErrorFormat("存档异常:{0}" , ex);
            return true;
        }
    }
}
