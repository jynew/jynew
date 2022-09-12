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
using i18n.TranslatorDef;
using Jyx2;

using Jyx2.Battle;
using Jyx2.Middleware;
using Jyx2Configs;
using UnityEngine;
using Random = UnityEngine.Random;


public class BattleStartParams
{
    public Action<BattleResult> callback; //战斗结果
    public List<RoleInstance> roles; //参与战斗的角色
    public Jyx2ConfigBattle battleData; //战斗地图数据
    public bool backToBigMap = true;
    public bool playerJoin = true;
}

public class BattleManager : MonoBehaviour
{
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
        var tempView = _player.View;
        if (tempView == null)
        {
            tempView = customParams.roles[0].View;
        }

        if (!BattleboxHelper.Instance.CanEnterBattle(tempView.transform.position)) return;

        IsInBattle = true;
        
        LevelMaster.Instance.UpdateMobileControllerUI();
        
        m_battleParams = customParams;
        //初始化战斗model
        m_BattleModel = new BattleFieldModel();
        //初始化范围逻辑
        rangeLogic = new RangeLogic(BattleboxHelper.Instance.IsBlockExists, m_BattleModel.BlockHasRole);

        if (Camera.main != null)
        {
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
            }
        }

        //await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        await UniTask.WaitForEndOfFrame();

        BattleboxHelper.Instance.EnterBattle(tempView.transform.position);

        //地图上所有单位进入战斗
        foreach (var role in m_battleParams.roles)
        {
            role.EnterBattle();
            AddBattleRole(role);
        }

        m_BattleModel.InitBattleModel(); //战场初始化 行动顺序排序这些
        //---------------------------------------------------------------------------
        //await Jyx2_UIManager.Instance.ShowUIAsync(nameof(CommonTipsUIPanel), TipsType.MiddleTop, "战斗开始"); //提示UI
        //---------------------------------------------------------------------------
        //特定位置的翻译【战斗开始时候的弹窗提示】
        //---------------------------------------------------------------------------
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(CommonTipsUIPanel), TipsType.MiddleTop, "战斗开始".GetContent(nameof(BattleManager))); //提示UI
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------

        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BattleMainUIPanel), BattleMainUIState.ShowHUD); //展示角色血条
        
        await new BattleLoop(this).StartLoop();
    }
    

    public void OnBattleEnd(BattleResult result)
    {
        switch (result)
        {
            case BattleResult.Win:
            {
                string bonusText = CalExpGot(m_battleParams.battleData);
                //---------------------------------------------------------------------------
                //GameUtil.ShowFullSuggest(bonusText, "<color=yellow><size=50>战斗胜利</size></color>", delegate
                //{
                //    EndBattle();
                //    m_battleParams.callback?.Invoke(result);
                //    m_battleParams = null;
                //});
                //---------------------------------------------------------------------------
                //特定位置的翻译【战斗胜利的提示】
                //---------------------------------------------------------------------------
                GameUtil.ShowFullSuggest(bonusText, "<color=yellow>战斗胜利</color>".GetContent(nameof(BattleManager)), delegate
                {
                    EndBattle();
                    m_battleParams.callback?.Invoke(result);
                    m_battleParams = null;
                });
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
                break;
            }
            case BattleResult.Lose:
            {

                //---------------------------------------------------------------------------
                //GameUtil.ShowFullSuggest("胜败乃兵家常事，请大侠重新来过。", "<color=red><size=80>战斗失败！</size></color>", delegate
                //{
                //    EndBattle();
                //    m_battleParams.callback?.Invoke(result);
                //    //if (m_battleParams.backToBigMap) //由dead指令实现返回主界面逻辑
                //    //    LevelLoader.LoadGameMap("Level_BigMap");
                //    m_battleParams = null;
                //});
                //---------------------------------------------------------------------------
                //特定位置的翻译【战斗失败的提示】
                //---------------------------------------------------------------------------
                GameUtil.ShowFullSuggest("胜败乃兵家常事，请大侠重新来过。".GetContent(nameof(BattleManager)), "<color=red>战斗失败！</color>".GetContent(nameof(BattleManager)), delegate
                {
                    EndBattle();
                    m_battleParams.callback?.Invoke(result);
                    //if (m_battleParams.backToBigMap) //由dead指令实现返回主界面逻辑
                    //    LevelLoader.LoadGameMap("Level_BigMap");
                    m_battleParams = null;
                });
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
                break;
            }
        }
        
        //所有人至少有1HP
        foreach (var role in GameRuntimeData.Instance.GetTeam())
        {
            if (role.Hp <= 0)
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
        //角色初次加入战场时候，上一回合的生命值就是出场生命值
        role.PreviousRoundHp = role.Hp;
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

    string CalExpGot(Jyx2ConfigBattle battleData)
    {
        List<RoleInstance> alive_teammate = m_BattleModel.Teammates;
        var dead_teammates = m_BattleModel.Dead.Where(r => r.team == 0);
        var all_teammates = alive_teammate.Union(dead_teammates);
        var bonusTextBuilder = new System.Text.StringBuilder();

        foreach (var role in alive_teammate)
        {
            int expAdd = battleData.Exp / alive_teammate.Count;
            role.ExpGot += expAdd;
        }

        foreach (var role in all_teammates)
        {
            if (role.ExpGot > 0)
            {
                //---------------------------------------------------------------------------
                //rst += string.Format("{0}获得经验{1}\n", role.Name, role.ExpGot);
                //---------------------------------------------------------------------------
                //特定位置的翻译【战斗胜利角色获得经验的提示】
                //---------------------------------------------------------------------------
                bonusTextBuilder.AppendFormat("{0}获得经验{1}\n".GetContent(nameof(BattleManager)), role.Name, role.ExpGot);
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
                role.Exp += role.ExpGot;

                //避免越界
                role.Exp = Tools.Limit(role.Exp, 0, GameConst.MAX_EXP);
            }
      

            //升级
            int change = 0;
            while (role.CanLevelUp())
            {
                role.LevelUp();
                change++;
                if (change == 1)
                {
                    //---------------------------------------------------------------------------
                    //rst = $"{role.Name}升级了！\n";
                    //---------------------------------------------------------------------------
                    //特定位置的翻译【战斗胜利角色升级的提示】
                    //---------------------------------------------------------------------------
                    bonusTextBuilder.AppendFormat("{0}升级了！\n".GetContent(nameof(BattleManager)), role.Name);
                    //---------------------------------------------------------------------------
                    //---------------------------------------------------------------------------
                }
            }

            //TODO：升级的展示
            var practiseItem = role.GetXiulianItem();
            if (practiseItem != null)
            {
                //武功修炼的经验为人物获得经验的0.8倍
                role.ExpForItem += role.ExpGot * 8 / 10;
                role.ExpForMakeItem += role.ExpGot * 8 / 10;

                role.ExpForItem = Tools.Limit(role.ExpForItem, 0, GameConst.MAX_EXP);
                role.ExpForMakeItem = Tools.Limit(role.ExpForMakeItem, 0, GameConst.MAX_EXP);

                bool isWugongCanUpgrade = role.GetWugongLevel(practiseItem.Skill) < GameConst.MAX_WUGONG_LEVEL;
                //修炼秘籍
                while (role.CanFinishedItem() && isWugongCanUpgrade)
                {
                    role.UseItem(practiseItem);
                    //---------------------------------------------------------------------------
                    //rst += $"{role.Name} 修炼 {practiseItem.Name} 成功\n";
                    //---------------------------------------------------------------------------
                    //特定位置的翻译【战斗胜利角色修炼武功提示】
                    //---------------------------------------------------------------------------
                    bonusTextBuilder.AppendFormat("{0} 修炼 {1} 成功\n".GetContent(nameof(BattleManager)), role.Name, practiseItem.Name);
                    //---------------------------------------------------------------------------
                    //---------------------------------------------------------------------------
                    var level = role.GetWugongLevel(practiseItem.Skill);
                    if (level > 1)
                    {
                        //---------------------------------------------------------------------------
                        //rst += string.Format("{0} 升为 ", practiseItem.SkillCast.Name) + level.ToString() + " 级\n";
                        //---------------------------------------------------------------------------
                        //特定位置的翻译【战斗胜利角色修炼武功升级提示】
                        //---------------------------------------------------------------------------
                        bonusTextBuilder.AppendFormat("{0} 升为 {1}级\n".GetContent(nameof(BattleManager)), GameConfigDatabase.Instance.Get<Jyx2ConfigSkill>(practiseItem.Skill).Name, level);
                        //---------------------------------------------------------------------------
                        //---------------------------------------------------------------------------
                    }
                }

                //炼制物品
                bonusTextBuilder.Append(role.LianZhiItem(practiseItem));
            }
        }

        return bonusTextBuilder.ToString();
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
    public IEnumerable<BattleBlockVector> GetSkillCoverBlocks(SkillCastInstance skill, BattleBlockVector targetPos,
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
    /// <param name="movedStep">移动过的格子数</param>
    public List<BattleBlockVector> GetMoveRange(RoleInstance role, int movedStep)
    {
        //获得角色移动能力
        int moveAbility = role.GetMoveAbility();
        //绘制周围的移动格子
        var blockList = rangeLogic.GetMoveRange(role.Pos.X, role.Pos.Y, moveAbility - movedStep, false, true);
        return blockList;
    }

    //获取技能的使用范围
    public List<BattleBlockVector> GetSkillUseRange(RoleInstance role, SkillCastInstance skillCast)
    {
        int castSize = skillCast.GetCastSize();
        var coverType = skillCast.GetCoverType();
        var sx = role.Pos.X;
        var sy = role.Pos.Y;

        //绘制周围的攻击格子
        var blockList = rangeLogic.GetSkillCastBlocks(sx, sy, skillCast, role);

        return blockList.ToList();
    }

    //获取范围内的敌人或者友军
    public List<RoleInstance> GetRoleInSkillRange(SkillCastInstance skill, IEnumerable<BattleBlockVector> range, int team)
    {
        List<RoleInstance> result = new List<RoleInstance>();

        foreach (var pos in range)
        {
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