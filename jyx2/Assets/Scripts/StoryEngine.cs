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
using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using System.Globalization;
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

    public void DisplayPopInfo(string msg, float duration = 2f)
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(CommonTipsUIPanel), TipsType.Common, msg, duration);
    }

    public static bool DoLoadGame(int index)
    {
        //加载存档
        var r = GameRuntimeData.LoadArchive(index);
        if (r == null)
        {
            return false;
        }

        //初始化角色
        foreach (var role in r.Team)
        {
            role.BindKey();

            //因为更改了存储的数据结构，需要检查存档的数据
            if (!runtime.HaveItemBool(role.Weapon) && role.Weapon != -1) runtime.AddItem(role.Weapon, 1);
            if (!runtime.HaveItemBool(role.Armor) && role.Armor != -1) runtime.AddItem(role.Armor, 1);
            if (!runtime.HaveItemBool(role.Xiulianwupin) && role.Xiulianwupin != -1)
                runtime.AddItem(role.Xiulianwupin, 1);
        }

        //CGGG: 2021/9/11 修复老的存档主角没有绑定0号角色主角，导致的增加属性数值指令无效的问题
        if (r.Player != r.AllRoles[0])
        {
            r.AllRoles[0] = r.Player;
        }

        var loadPara = new LevelMaster.LevelLoadPara() {loadType = LevelMaster.LevelLoadPara.LevelLoadType.Load};

        //加载地图
        // fix load game from Main menu will not transport player to last time indoor position 
        // modified by eaphone at 2021/06/01

        int mapId = -1;
        
        //修复老存档
        if (!int.TryParse(r.CurrentMap, out mapId))
        {
            mapId = GameConst.WORLD_MAP_ID;
        }

        LevelLoader.LoadGameMap(GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(mapId), loadPara,
            () => { LevelMaster.Instance.TryBindPlayer(); });
        return true;
    }
}