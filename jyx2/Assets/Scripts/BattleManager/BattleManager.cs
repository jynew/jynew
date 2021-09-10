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
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Jyx2;
using HSFrameWork.Common;
using UnityEngine;
using Random = UnityEngine.Random;


public class BattleStartParams
{
    public Action<BattleResult> callback; //战斗结果
    public List<RoleInstance> roles; //参与战斗的角色
    public Jyx2Battle battleData; //战斗地图数据
    public bool backToBigMap = true;
    public bool playerJoin = true;
}

public class BattleManager : MonoBehaviour
{
    public enum BattleViewStates
    {
        None = -1,
        WaitingForNextActiveBattleRole, //等待下一个行动角色
        SelectMove, //选择移动，展现移动范围
        SelectSkill, //选择技能
        SelectSkillTarget, //选择技能攻击目标，展现施展范围
        PreshowSkillCoverRange, //预展现技能覆盖范围
        PlayingAction, //播放当前行动中
        Move, //移动状态
        AI,
        UseItem, //使用物品的状态 主要播放使用物品动画
        BuffSettlement, //buff结算状态 这里主要是用毒
    }
    

    public static BattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("BattleManager");
                go.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(go);
                _instance = GameUtil.GetOrAddComponent<BattleManager>(go.transform);
            }

            return _instance;
        }
    }

    private static BattleManager _instance;

    #region 战场组件

    private BattleFieldModel m_BattleModel;

    public BattleFieldModel GetModel()
    {
        return m_BattleModel;
    }


    private RangeLogic rangeLogic;

    public RangeLogic GetRangeLogic()
    {
        return rangeLogic;
    }

    private RoleInstance _player
    {
        get { return GameRuntimeData.Instance.Player; }
    }

    #endregion

    //是否无敌
    public static bool Whosyourdad = false;


    public bool IsInBattle = false;
    private BattleStartParams m_battleParams;
    private AudioClip lastAudioClip;

    public async UniTask StartBattle(BattleStartParams customParams)
    {
        Debug.Log("StartBattle called");
        if (IsInBattle) return;
        if (!BattleboxHelper.Instance.CanEnterBattle(_player.View.transform.position)) return;

        IsInBattle = true;
        m_battleParams = customParams;
        //初始化战斗model
        m_BattleModel = new BattleFieldModel();
        //初始化范围逻辑
        rangeLogic = new RangeLogic(BattleboxHelper.Instance.IsBlockExists, m_BattleModel.BlockHasRole);

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        }

        //await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        await UniTask.WaitForEndOfFrame();

        BattleboxHelper.Instance.EnterBattle(_player.View.transform.position);

        //地图上所有单位进入战斗
        foreach (var role in m_battleParams.roles)
        {
            role.EnterBattle();
            AddBattleRole(role);
        }

        m_BattleModel.InitBattleModel(); //战场初始化 行动顺序排序这些
        BattleStateMechine.Instance.StartStateMechine(OnBattleEnd); //交给战场状态机接管 状态机完成会回调回来
        Jyx2_UIManager.Instance.ShowUI(nameof(CommonTipsUIPanel), TipsType.MiddleTop, "战斗开始"); //提示UI
        Jyx2_UIManager.Instance.ShowUI(nameof(BattleMainUIPanel), BattleMainUIState.ShowHUD); //展示角色血条
    }

    void OnBattleEnd(BattleResult result)
    {
        switch (result)
        {
            case BattleResult.Win:
            {
                string bonusText = CalExpGot(m_battleParams.battleData);
                GameUtil.ShowFullSuggest(bonusText, "<color=yellow><size=50>战斗胜利</size></color>", delegate
                {
                    EndBattle();
                    m_battleParams.callback?.Invoke(result);
                    m_battleParams = null;
                });
                break;
            }
            case BattleResult.Lose:
            {
                GameUtil.ShowFullSuggest("胜败乃兵家常事，请大侠重新来过。", "<color=red><size=80>战斗失败！</size></color>", delegate
                {
                    EndBattle();
                    m_battleParams.callback?.Invoke(result);
                    //if (m_battleParams.backToBigMap) //由dead指令实现返回主界面逻辑
                    //    LevelLoader.LoadGameMap("Level_BigMap");
                    m_battleParams = null;
                });
                break;
            }
        }
        
        //所有人至少有1HP
        foreach (var role in GameRuntimeData.Instance.Team)
        {
            if (role.Hp == 0)
                role.Hp = 1;
        }
    }

    //清扫战场
    public void EndBattle()
    {
        IsInBattle = false;
        Jyx2_UIManager.Instance.HideUI(nameof(BattleMainUIPanel));

        //临时，需要调整
        foreach (var role in m_BattleModel.Roles)
        {
            //role.LeaveBattle();
            //非KeyRole死亡2秒后尸体消失
            if (role.IsDead() && role.View != null && role.View.gameObject != null && !role.View.m_IsKeyRole)
            {
                role.View.gameObject.SetActive(false);
            }
        }

        rangeLogic = null;
        m_BattleModel.Roles.Clear();
    }


    /// <summary>
    /// 添加角色到战场里面
    /// </summary>
    /// <param name="role"></param>
    /// <param name="team"></param>
    public void AddBattleRole(RoleInstance role)
    {
        int team = role.team;
        //计算NPC应该站的点
        BattleBlockData npcStandBlock = FindNearestBattleBlock(role.View.transform.position);

        if (npcStandBlock == null)
        {
            Debug.LogError($"错误，{role.Key}找不到有效格子");
            return;
        }

        //加入战场
        m_BattleModel.AddBattleRole(role, npcStandBlock.BattlePos, team, (team != 0));

        //待命
        role.View.Idle();
        var enemy = AIManager.Instance.GetNearestEnemy(role);
        if (enemy != null)
        {
            //面向最近的敌人
            role.View.LookAtWorldPosInBattle(enemy.View.transform.position);
        }

        //死亡的关键角色默认晕眩
        if (role.View.m_IsKeyRole && role.IsDead())
        {
            role.Stun(-1);
        }
    }

    string CalExpGot(Jyx2Battle battleData)
    {
        List<RoleInstance> alive_teammate = m_BattleModel.Roles.Where(r => r.team == 0).ToList();
        string rst = "";
        foreach (var role in alive_teammate)
        {
            int expAdd = battleData.Exp / alive_teammate.Count();
            role.ExpGot += expAdd;
            rst += string.Format("{0}获得经验{1}\n", role.Name, role.ExpGot);
        }

        //分配经验
        foreach (var role in alive_teammate)
        {
            var practiseItem = role.GetXiulianItem();

            if (role.Level >= GameConst.MAX_ROLE_LEVEL)
            {
                role.ExpForItem += role.ExpGot;
            }
            else if (practiseItem != null && role.GetWugongLevel(practiseItem.Wugong) <= 10)
            {
                role.Exp += role.ExpGot / 2;
                role.ExpForItem += role.ExpGot / 2;
            }
            else
            {
                role.Exp += role.ExpGot;
            }

            role.ExpForMakeItem += role.ExpGot;
            //避免越界
            role.Exp = Tools.Limit(role.Exp, 0, GameConst.MAX_EXP);
            role.ExpForItem = Tools.Limit(role.ExpForItem, 0, GameConst.MAX_EXP);

            //升级
            int change = 0;
            while (role.CanLevelUp())
            {
                role.LevelUp();
                change++;
                rst += $"{role.Name}升级了！等级{role.Level}\n";
            }

            //TODO：升级的展示

            if (practiseItem != null)
            {
                change = 0;

                //修炼秘籍
                while (role.CanFinishedItem() && role.GetWugongLevel(practiseItem.Wugong) <= 10)
                {
                    role.UseItem(practiseItem);
                    change++;
                    var level = role.GetWugongLevel(practiseItem.Wugong);
                    rst += $"{role.Name} 修炼 {practiseItem.Name} 成功\n";
                    if (level > 1) rst += $"{practiseItem.Name} 升为 " + level.ToString() + " 级\n";
                }

                //炼制物品
                rst += role.LianZhiItem(practiseItem);
            }
        }

        return rst;
    }

    /// <summary>
    /// 找到最近的战斗格子
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="ignoreRole"></param>
    /// <returns></returns>
    BattleBlockData FindNearestBattleBlock(Vector3 pos, bool ignoreRole = false)
    {
        BattleBlockData rst = null;
        var list = BattleboxHelper.Instance.GetBattleBlocks();
        var minDist = float.MaxValue;
        foreach (var data in list)
        {
            //有人占了这一格了
            if (!ignoreRole && m_BattleModel.BlockHasRole(data.BattlePos.X, data.BattlePos.Y)) continue;

            var dist = (data.WorldPos - pos).sqrMagnitude;
            if (minDist > dist)
            {
                minDist = dist;
                rst = data;
            }
        }

        return rst;
    }


    #region 战斗共有方法

    /// <summary>
    /// 获取技能覆盖范围
    /// </summary>
    /// <returns></returns>
    public List<BattleBlockVector> GetSkillCoverBlocks(BattleZhaoshiInstance skill, BattleBlockVector targetPos,
        BattleBlockVector selfPos)
    {
        var coverSize = skill.GetCoverSize();
        var coverType = skill.GetCoverType();
        var sx = selfPos.X;
        var sy = selfPos.Y;
        var tx = targetPos.X;
        var ty = targetPos.Y;
        var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, tx, ty, sx, sy, coverSize);
        return coverBlocks;
    }

    /// <summary>
    /// 寻找移动路径 也就是寻路
    /// </summary>
    /// <returns></returns>
    public List<Vector3> FindMovePath(RoleInstance role, BattleBlockVector block)
    {
        var paths = rangeLogic.GetWay(role.Pos.X, role.Pos.Y,
            block.X, block.Y);
        var posList = new List<Vector3>();
        foreach (var temp in paths)
        {
            var tempBlock = BattleboxHelper.Instance.GetBlockData(temp.X, temp.Y);
            if (tempBlock != null) posList.Add(tempBlock.WorldPos);
        }

        return posList;
    }

    /// <summary>
    /// 获取角色的移动范围
    /// </summary>
    /// <param name="role"></param>
    public List<BattleBlockVector> GetMoveRange(RoleInstance role)
    {
        //获得角色移动能力
        int moveAbility = role.GetMoveAbility();
        //绘制周围的移动格子
        var blockList = rangeLogic.GetMoveRange(role.Pos.X, role.Pos.Y, moveAbility);
        return blockList;
    }

    //获取技能的使用范围
    public List<BattleBlockVector> GetSkillUseRange(RoleInstance role, BattleZhaoshiInstance zhaoshi)
    {
        int castSize = zhaoshi.GetCastSize();
        var coverType = zhaoshi.GetCoverType();
        var sx = role.Pos.X;
        var sy = role.Pos.Y;

        //绘制周围的攻击格子
        var blockList = rangeLogic.GetSkillCastBlocks(sx, sy, zhaoshi, role);
        return blockList;
    }

    //获取范围内的敌人或者友军
    public List<RoleInstance> GetRoleInSkillRange(BattleZhaoshiInstance skill, List<BattleBlockVector> range, int team)
    {
        List<RoleInstance> result = new List<RoleInstance>();
        for (int i = 0; i < range.Count; i++)
        {
            BattleBlockVector pos = range[i];
            RoleInstance rolei = m_BattleModel.GetAliveRole(pos);
            if (rolei == null || rolei.IsDead()) continue;
            //打敌人的招式
            if (skill.IsCastToEnemy() && rolei.team == team) continue;
            if (!skill.IsCastToEnemy() && rolei.team != team) continue;
            result.Add(rolei);
        }

        return result;
    }

    #endregion
}