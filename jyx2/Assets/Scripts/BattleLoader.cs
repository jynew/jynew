using System;
using System.Collections;
using System.Collections.Generic;
using Jyx2;
using HSFrameWork.ConfigTable;
using Jyx2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 战斗启动器
/// </summary>
public class BattleLoader : MonoBehaviour
{
    [LabelText("载入战斗ID")]
    public int m_BattleId = 0;


    [HideInInspector]
    public Action<BattleResult> Callback;


    public bool IsTestCase = false;

    public struct BattlePosRole
    {
        public string pos;
        
        public int team;

        public string roleKey;
    }

    public List<BattlePosRole> m_Roles;

    void CycleLoadBattle()
    {
        LevelLoader.LoadBattle(m_BattleId, (ret)=> { CycleLoadBattle(); });
    }

    // Start is called before the first frame update
    async void Start()
    {
        await BeforeSceneLoad.loadFinishTask;

        if (IsTestCase)
        {
            LoadJyx2Battle(m_BattleId, (ret) =>
            {
                CycleLoadBattle();
            });
        }
        else
        {
            LoadJyx2Battle(m_BattleId, Callback);
        }
        
    }


    GameRuntimeData runtime { get { return GameRuntimeData.Instance; } }

    //void OnTest()
    //{
    //    m_Roles = new List<BattlePosRole>();
    //    m_Roles.Add(new BattlePosRole() { pos = "1", team = 0, roleKey = "0" });
    //    m_Roles.Add(new BattlePosRole() { pos = "2", team = 1, roleKey = "1" });
    //    m_Roles.Add(new BattlePosRole() { pos = "1", team = 1, roleKey = "3" });
    //    m_Roles.Add(new BattlePosRole() { pos = "1", team = 1, roleKey = "4" });
    //    m_Roles.Add(new BattlePosRole() { pos = "2", team = 0, roleKey = "5" });
    //    m_Roles.Add(new BattlePosRole() { pos = "2", team = 0, roleKey = "51" });

    //    GameRuntimeData.CreateNew();  //选一个没有用过的id
    //    MapRuntimeData.Instance.Clear();


    //    //测试等级
    //    runtime.TeamLevel = 10;

    //    InitBattle(null);
    //}


    void LoadJyx2Battle(int id, Action<BattleResult> callback)
    {
        Debug.Log("-----------BattleLoader.LoadJyx2Battle");
        if (GameRuntimeData.Instance == null)
        {
            GameRuntimeData.CreateNew();
        }


#if JYX2_TEST
        //临时用于测试
        var player = runtime.Team[0];
        player.MaxHp = 999;
        player.MaxMp = 999;
        player.Zuoyouhubo = 1;
        player.Qinggong = 100;

        player.Attack = 50;
        player.Recover();

        runtime.AddItem(3, 100);//小还丹
#endif


        m_Roles = new List<BattlePosRole>();
        Jyx2Battle battle = ConfigTable.Get<Jyx2Battle>(id);
        if(battle == null)
        {
            Debug.LogError("载入了未定义的战斗，id=" + id);
            return;
        }

        AudioManager.PlayMusic(battle.Music);

        //设置了自动战斗人物
        if(battle.HasAutoTeamMates())
        {
            foreach (var v in battle.AutoTeamMates)
            {
                var roleId = v.Value;
                if (roleId == -1) continue;
                AddRole(roleId, 0); //TODO IS AUTO
            }
            LoadJyx2BattleStep2(battle, null, callback);
        }
        else //否则让玩家选择
        {
            //必选人物
            Func<RoleInstance, bool> mustRoleFunc = (r) => {
                return battle.TeamMates.Exists(t => t.Value.ToString() == r.Key);
            };

            //弹出选择人物面板
            //Jyx2RoleSelector.Create(runtime.Team, mustRoleFunc, (selectRoles) => {
            //    LoadJyx2BattleStep2(battle, selectRoles, callback);
            //});
            SelectRoleParams selectPram = new SelectRoleParams();
            selectPram.roleList = runtime.Team;
            selectPram.mustSelect = mustRoleFunc;
            selectPram.title = "选择上场角色";
            selectPram.maxCount = GameConst.MAX_BATTLE_TEAMMATE_COUNT;//TODO 最大上场人数
            selectPram.canCancel = false;
            selectPram.callback = (cb) =>
            {
                LoadJyx2BattleStep2(battle, cb.selectList, callback);
            };
            Jyx2_UIManager.Instance.ShowUI("SelectRolePanel", selectPram);
        }
    }

    void LoadJyx2BattleStep2(Jyx2Battle battle, List<RoleInstance> selectRoles, Action<BattleResult> callback)
    {
        if(selectRoles != null)
        {
            foreach (var role in selectRoles)
            {
                AddRole(role.GetJyx2RoleId(), 0);
            }
        }
        
        //预配置队友
        foreach (var v in battle.TeamMates)
        {
            AddRole(v.Value, 0);
        }

        foreach (var v in battle.Enemies)
        {
            AddRole(v.Value, 1);
        }

        InitBattle(callback, battle);
    }

    //用于存储各个队伍已经放置的角色编号
    private Dictionary<int, int> teamRoleIndex = new Dictionary<int, int>();
    
    void AddRole(int id,int team)
    {
        if (id == -1)
            return;

        //已经添加过了
        if (m_Roles.Exists(r => r.roleKey == id.ToString() && r.team == team))
            return;

        if (!teamRoleIndex.ContainsKey(team))
        {
            teamRoleIndex[team] = 0; //编号从0开始
        }

        //命名方式为：战斗地图号/队伍_序号，目前0是己方队伍，1是敌方队伍
        string posKey = $"battle{m_BattleId}/{team}_{teamRoleIndex[team]}";
        teamRoleIndex[team]++;
        
        m_Roles.Add(new BattlePosRole() { pos = posKey, team = team, roleKey = id.ToString() });
    }

    //初始化战斗
    void InitBattle(Action<BattleResult> callback, Jyx2Battle battleData)
    {
        Debug.Log("-----------BattleLoader.InitBattle");
        List<RoleInstance> roles = new List<RoleInstance>();
        foreach (var r in m_Roles)
        {
            RoleInstance roleInstance = null;
            if(r.roleKey == "0")
            {
                roleInstance = runtime.Team[0]; //临时写法，现在是直接取主角
            }
            else
            {
                roleInstance = new RoleInstance(r.roleKey);
            }
            
            if(roleInstance == null)
            {
                Debug.LogError("error roleKey:" + r.roleKey);
                continue;
            }

            var pos = FindSpawnPosition(r.pos, r.team);
            if(pos == null)
            {
                Debug.LogError("未定义的POS:" + r.pos);
                continue;
            }

            roleInstance.ExpGot = 0;

            CreateRole(roleInstance, r.team, pos);
            roles.Add(roleInstance);
        }
        LevelMaster.Instance.TryBindPlayer(); //尝试绑定角色
        StartCoroutine(DelayStartBattle(roles,battleData,callback));
    }

    IEnumerator DelayStartBattle(List<RoleInstance> roles,Jyx2Battle battleData,Action<BattleResult> callback)
    {
        yield return 0;
        BattleStartParams startParam = new BattleStartParams()
        {
            roles = roles,
            battleData = battleData,
            callback = callback,
            range = 100,
        };
        //BattleHelper.Instance.StartBattle(startParam);
        BattleManager.Instance.StartBattle(startParam);
    }

    //寻找定义的出生点
    Transform FindSpawnPosition(string posKey, int team)
    {
        var obj = GameObject.Find("Level/BattlePos/" + posKey);
        
        //如果找不到，则用默认的队伍出生点
        if (obj == null)
        {
            obj = GameObject.Find("Level/BattlePos/" + team.ToString());
        }
        if (obj == null) return null;
        return obj.transform;
    }

    bool setPlayer = false;
    void CreateRole(RoleInstance role, int team, Transform pos)
    {
        Debug.Log($"--------BattleLoader.CreateRole, role={role.Name}, team={team}, pos={pos.name}");
        role.LeaveBattle();
        //find or create
        GameObject npcRoot = GameObject.Find("BattleRoles");
        if(npcRoot == null)
        {
            npcRoot = new GameObject("BattleRoles");
        }

        MapRole roleView;
        //JYX2苟且逻辑：找第一个能找到的角色设置为主角
        if (!setPlayer)
        {
            setPlayer = true;
            roleView = role.CreateRoleView("Player");
        }
        else
        {
            roleView = role.CreateRoleView();
        }

        roleView.IsInBattle = true;

        //敌人的警戒距离和队友的跟随距离，都设置一个巨大的数
        roleView.m_BehaviorParas = new List<float>();
        roleView.m_BehaviorParas.Add(999);

        roleView.transform.SetParent(npcRoot.transform, false);
        roleView.transform.position = pos.position;

        roleView.SetBehavior(team == 0 ? MapRoleBehavior.Teammate : MapRoleBehavior.Enemy);
        roleView.RefreshModel();//刷新模型
    }
}
