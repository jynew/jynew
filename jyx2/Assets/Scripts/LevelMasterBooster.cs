using System;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using Jyx2;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class LevelMasterBooster : MonoBehaviour
{
    [ProgressBar(1, 50)]
    [LabelText("测试等级")]
    public int m_TestLevel = 1;

    [LabelText("测试队友（角色Key）")]
    public string[] m_TestTeammate;

    [LabelText("模拟数据")]
    public bool m_RuntimeDataSimulate = true;

    [LabelText("模拟移动端")]
    public bool m_MobileSimulate = false;


    GameRuntimeData runtime { get { return GameRuntimeData.Instance; } }

    async private void Awake()
    {
        await BeforeSceneLoad.loadFinishTask;

        //实例化LevelMaster
        LevelMaster levelMaster = Jyx2ResourceHelper.CreatePrefabInstance(ConStr.LevelMaster).GetComponent<LevelMaster>();
        DontDestroyOnLoad(levelMaster);
        levelMaster.name = "LevelMaster";
        levelMaster.transform.SetParent(transform, false);
        levelMaster.RuntimeDataSimulate = m_RuntimeDataSimulate;
        levelMaster.MobileSimulate = m_MobileSimulate;
    }

    async private void Start()
    {
        await BeforeSceneLoad.loadFinishTask;
        if (GameRuntimeData.Instance == null)
        {
            GameRuntimeData.CreateNew();
        }

        //设置所有的宝箱
        foreach(var chest in GameObject.FindObjectsOfType<MapChest>())
        {
            chest.Init();
        }


        //设置所有的场景变更
        RefreshSceneObjects();


        //if (m_RuntimeDataSimulate && GameRuntimeData.Instance == null)
        //{
        //    var runtime = GameRuntimeData.CreateNew();
        //    var playerRoleView = RoleHelper.FindPlayer();
        //    playerRoleView.BindRoleInstance(runtime.Player);

        //    //测试队友
        //    //foreach (var teammate in m_TestTeammate)
        //    //{
        //    //    var role = RoleHelper.CreateRoleInstance(teammate);
        //    //    runtime.Team.Add(role);
        //    //    MapRuntimeData.Instance.AddToExploreTeam(role);
        //    //}

        //    //随机增加物品
        //    //for(int i = 0; i < 5; ++i)
        //    //{
        //    //    runtime.AddItem(UnityEngine.Random.Range(0, 197), UnityEngine.Random.Range(1, 10));
        //    //}

        //    //测试物品
        //    //List<string> itemList = new List<string>() {
        //    //    "皮甲", "皮手套", "生锈的宝剑", "凤凰琴", "桃花华裳", "杨家宝刀",
        //    //    "杨家宝甲", "林冲虎啸枪", "摇光", "摇光剑", "摇光枪", "摇光拳套",
        //    //    "寒铁宝甲", "老成皮甲", "名剑白虹", "昆吾神剑", "青莲古衣", "道济古帽",
        //    //    "道济葫芦", "朔雪头冠", "朔雪飞靴", "大内金靴", "大内头冠", "星移长衣",
        //    //    "星移斗笠", "香神链", "蔓陀长靴", "蔓陀束带", "飞流饰带", "四象饰带",
        //    //    "吟龙带", "吟龙履", "南山守则", "阴阳幡", "天道束腰", "任侠环"
        //    //};

        //    //foreach (var item in itemList)
        //    //{
        //    //    runtime.AddItem(ItemInstance.Generate(item, true));
        //    //}

        //    //探索技能值
        //    //GameConst.MapSkillPoint = 100;
        //}
        //场景准备完成 显示主界面
        Jyx2_UIManager.Instance.ShowMainUI();
    }

    public void ReplaceSceneObject(string scene, string path, string replace)
    {
        string sceneName = "";
        //当前场景
        if (string.IsNullOrEmpty(scene))
        {
            //sceneName = SceneManager.GetActiveScene().name;
            sceneName = LevelMaster.Instance.GetCurrentGameMap().Jyx2MapId.ToString();
        }
        else
        {
            sceneName = scene;
        }
        
        var dict = runtime.GetSceneInfo(sceneName);
        if (dict == null)
            dict = new Dictionary<string, string>();
        dict[path] = replace;
        runtime.SetSceneInfo(sceneName, dict);

        //如果是当前场景
        if (string.IsNullOrEmpty(scene))
        {
            RefreshSceneObjects();
        }
    }

    public bool IsBattle()
    {
        var battleLoader = FindObjectOfType<BattleLoader>();
        return battleLoader != null;
    }

    public void RefreshSceneObjects()
    {
        if (IsBattle())
            return;

        if (LevelMaster.Instance == null) return;

        var currentGameMap = LevelMaster.Instance.GetCurrentGameMap();
        if (currentGameMap == null)
            return;

        string sceneName = currentGameMap.Jyx2MapId.ToString();
        var dict = runtime.GetSceneInfo(sceneName);
        if (dict == null)
            return;
        foreach(var kv in dict)
        {
            string objPath = "Level/" + kv.Key;

            GameObject obj = GameObject.Find(objPath);
            if (obj == null)
            {
                Debug.LogError("RefreshSceneObjects错误：找不到对象:" + objPath);
                continue;
            }

            if (string.IsNullOrEmpty(kv.Value) || kv.Value.Equals("0"))
            {
                obj.SetActive(false);
            }
            else if (kv.Value.Equals("1"))
            {
                obj.SetActive(true);
            }
        }
    }
}