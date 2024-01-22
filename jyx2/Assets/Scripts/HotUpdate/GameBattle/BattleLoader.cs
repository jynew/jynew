/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;

using Jyx2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 战斗启动器
/// </summary>
public class BattleLoader : MonoBehaviour
{
    [LabelText("载入战斗ID")] public int m_BattleId = 0;

    public LBattleConfig CurrentBattleConfig { get; set; } = null;

    [HideInInspector] public Action<BattleResult> Callback;


    public bool IsTestCase = false;

    public struct BattlePosRole
    {
        public string pos;

        public int team;

        public int roleKey;

        public RoleInstance role; //直接使用绑定的role，这时候roleKey应设置为-1
    }

    public List<BattlePosRole> m_Roles;



    void CycleLoadBattle()
    {
        LevelLoader.LoadBattle(m_BattleId, (ret) => { CycleLoadBattle(); });
    }

    // Start is called before the first frame update
    async void Start()
    {

        await RuntimeEnvSetup.Setup();

        if (CurrentBattleConfig == null)
        {
            CurrentBattleConfig = LuaToCsBridge.BattleTable[m_BattleId];
        }
        else
        {
            m_BattleId = CurrentBattleConfig.Id;
        }

        if (IsTestCase)
        {
            await LoadJyx2Battle(CurrentBattleConfig, (ret) => { CycleLoadBattle(); });
        }
        else
        {
            await LoadJyx2Battle(CurrentBattleConfig, Callback);
        }
    }


    GameRuntimeData runtime
    {
        get { return GameRuntimeData.Instance; }
    }

    async UniTask LoadJyx2Battle(LBattleConfig battle, Action<BattleResult> callback)
    {
        Debug.Log("-----------BattleLoader.LoadJyx2Battle");
        if (GameRuntimeData.Instance == null)
        {
            GameRuntimeData.CreateNew();
        }

        m_Roles = new List<BattlePosRole>();

        if (battle == null)
        {
            Debug.LogError("载入了未定义的战斗:" + m_BattleId);
            return;
        }

        var teamMates = battle.TeamMates;
        var autoTeamMates = battle.AutoTeamMates;

        AudioManager.PlayMusic(battle.Music);

        //设置了自动战斗人物
        // 自动队友不等于-1则表示有自己或队友
        if (autoTeamMates[0] != -1)
        {
            foreach (var v in autoTeamMates)
            {
                var roleId = v;
                if (roleId == -1) continue;
                AddRole(roleId, 0); //TODO IS AUTO
                for (var i = 0; i < m_Roles.Count; i++)
                {
                    if (m_Roles[i].roleKey == roleId)
                    {
                        RoleInstance roleInstance = runtime.GetRoleInTeam(roleId);
                        if (roleInstance != null && roleInstance.Hp <= 0) roleInstance.Hp = 1;
                    }
                }
            }

            await LoadJyx2BattleStep2(battle, null, callback);
        }
        else //否则让玩家选择
        {
            //必选人物
            bool MustRoleFunc(RoleInstance r)
            {
                return teamMates.Exists(id => id == r.Key);
            }

            SelectRoleParams selectPram = new SelectRoleParams();
            selectPram.roleList = runtime.GetTeam().ToList();
            selectPram.mustSelect = MustRoleFunc;
            //---------------------------------------------------------------------------
            //selectPram.title = "选择上场角色";
            //---------------------------------------------------------------------------
            //特定位置的翻译【战斗中选择上场角色的文字显示】
            //---------------------------------------------------------------------------
            selectPram.title = "选择上场角色".GetContent(nameof(BattleLoader));
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            selectPram.maxCount = GameConst.MAX_BATTLE_TEAMMATE_COUNT; //TODO 最大上场人数
            selectPram.canCancel = false;

            //弹出选择人物面板
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SelectRolePanel), selectPram);
            var rst = await SelectRolePanel.WaitForSelectConfirm(selectPram);
            await LoadJyx2BattleStep2(battle, rst, callback);
        }
    }

    UniTask LoadJyx2BattleStep2(LBattleConfig battle, List<RoleInstance> selectRoles, Action<BattleResult> callback)
    {
        //玩家选择的人物
        if (selectRoles != null)
        {
            foreach (var role in selectRoles)
            {
                AddRole(role.GetJyx2RoleId(), 0);
            }
        }

        //通过ID添加队友
        var teamMates = battle.TeamMates;
        //预配置队友
        foreach (var v in teamMates)
        {
            AddRole(v, 0);
        }

        //通过ID添加敌人
        var enemies = battle.Enemies;

        //敌人
        foreach (var v in enemies)
        {
            AddRole(v, 1);
        }

        //动态生成队友
        if (battle.DynamicTeammate != null)
        {
            foreach (var r in battle.DynamicTeammate)
            {
                AddDynamicRole(r, 0);
            }
        }

        //Debug.Log("Battle Loader 动态生成敌人");
        //动态生成敌人
        if (battle.DynamicEnemies != null)
        {
            foreach (var r in battle.DynamicEnemies)
            {
                AddDynamicRole(r, 1);
            }
        }

        return InitBattle(callback, battle);
    }

    //用于存储各个队伍已经放置的角色编号
    private Dictionary<int, int> teamRoleIndex = new Dictionary<int, int>();

    void AddRole(int id, int team)
    {
        if (id == -1)
            return;

        //已经添加过了
        if (m_Roles.Exists(r => r.roleKey == id && r.team == team))
            return;

        if (!teamRoleIndex.ContainsKey(team))
        {
            teamRoleIndex[team] = 0; //编号从0开始
        }

        //命名方式为：战斗地图号/队伍_序号，目前0是己方队伍，1是敌方队伍
        string posKey = $"battle{m_BattleId}/{team}_{teamRoleIndex[team]}";
        teamRoleIndex[team]++;

        m_Roles.Add(new BattlePosRole() {pos = posKey, team = team, roleKey = id});
    }

    void AddDynamicRole(RoleInstance r, int team)
    {
        if (!teamRoleIndex.ContainsKey(team))
        {
            teamRoleIndex[team] = 0; //编号从0开始
        }

        //命名方式为：战斗地图号/队伍_序号，目前0是己方队伍，1是敌方队伍
        string posKey = $"battle{m_BattleId}/{team}_{teamRoleIndex[team]}";
        teamRoleIndex[team]++;
        m_Roles.Add(new BattlePosRole() {pos = posKey, team = team, roleKey = -1, role = r});
    }

    RoleInstance GetRoleFromBattleRole(BattlePosRole r)
    {
        if (r.role != null)
        {
            return r.role;
        }
        else
        {
            //从存档中取，否则新生成
            RoleInstance roleInstance = runtime.GetRole(r.roleKey);

            //如果不是队伍中的，则从存档中拷贝一份
            if (!runtime.IsRoleInTeam(r.roleKey))
            {
                roleInstance = roleInstance.Clone();
                roleInstance.Recover();
            }

            //如果存档中没有记录（理论上不可能？除非是存档过期了，与当前配置表不匹配）
            if (roleInstance == null)
            {
                Debug.LogError($"载入角色出错，存档中没有该角色，强行从配置表生成。key={r.roleKey}");
                roleInstance = new RoleInstance(r.roleKey);
            }

            return roleInstance;
        }
    }


    //初始化战斗
    async UniTask InitBattle(Action<BattleResult> callback, LBattleConfig battleData)
    {
        Debug.Log("-----------BattleLoader.InitBattle");
        List<RoleInstance> roles = new List<RoleInstance>();
        foreach (var r in m_Roles)
        {
            var roleInstance = GetRoleFromBattleRole(r);

            //开始位置
            var pos = FindSpawnPosition(r.pos, r.team);
            if (pos == null)
            {
                Debug.LogError("未定义的POS:" + r.pos);
                continue;
            }

            roleInstance.ExpGot = 0;

            await CreateRole(roleInstance, r.team, pos);
            roles.Add(roleInstance);
        }

        //LevelMaster.Instance.TryBindPlayer(); //尝试绑定角色
        //await UniTask.WaitForEndOfFrame();
        BattleStartParams startParam = new BattleStartParams()
        {
            roles = roles,
            battleData = battleData,
            callback = callback,
        };

        //测试LuaEvent 用于Lua侧做拓展逻辑
        //Jyx2LuaBridge.DispatchLuaEvent("OnBeforeBattle", startParam);

        await BattleManager.Instance.StartBattle(startParam);
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

    UniTask CreateRole(RoleInstance role, int team, Transform pos)
    {
        //Debug.Log($"--------BattleLoader.CreateRole, role={role.Name}, team={team}, pos={pos.name}");
        role.LeaveBattle();
        //find or create
        GameObject npcRoot = GameObject.Find("BattleRoles");
        if (npcRoot == null)
        {
            npcRoot = new GameObject("BattleRoles");
        }

        BattleRole roleView;
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

        roleView.transform.SetParent(npcRoot.transform, false);
        roleView.transform.position = pos.position;
        role.team = team;
        return roleView.RefreshModel(); //刷新模型
    }
}
