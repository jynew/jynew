
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using HanSquirrel.ResourceManager;
using Jyx2;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using HSUI;
using Jyx2;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Jyx2.BattleFieldModel;

//WeaponType映射表
/*
 * BigSword 0
 * Bow 1
 * Dagger 2 匕首  -跑动动作用剑的
 * DoubleKnife 3
 * Gudgel 4
 * Gun 5
 * HFH 6（黄飞鸿？)
 * HidWea 7 暗器 -跑动动作用空手
 * Leg 8  -跑动动作用空手的，受击也是
 * Lute 9 琴/琵琶
 * Palm 10
 * Scourge 11 鞭子
 * Shield 12 盾牌
 * Sinknif 13 单刀
 * SinSword 14 长剑
 * Spear 15 长矛
 * 
 * 
 */


[Obsolete("已经用状态机实现，待删除")]
public class BattleHelper : BaseUI
{
    string[] battleMusics = new string[]
    {
        "Assets/BuildSource/Musics/5.mp3",
        "Assets/BuildSource/Musics/6.mp3",
        "Assets/BuildSource/Musics/7.mp3",
    };

    public static BattleHelper Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<BattleHelper>();
            return _instance;
        }
    }
    private static BattleHelper _instance;

    public enum BattleViewStates
    {
        WaitingForNextActiveBattleRole, //等待下一个行动角色
        SelectMove, //选择移动，展现移动范围
        SelectSkill, //选择技能
        SelectSkillTarget, //选择技能攻击目标，展现施展范围
        PreshowSkillCoverRange, //预展现技能覆盖范围
        PlayingAction, //播放当前行动中
        AI,
    }

    public GameObject m_BattleActionPanel;
    public CurrentBattleRolePanel m_CurBattleRolePanel;

    public Toggle m_AutoBattleButton;

    [HideInInspector]
    public bool IsInBattle = false;

    public float m_BattleMoveSpeed = 15;

    private BattleFieldModel BattleModel;

    public BattleFieldModel GetModel()
    {
        return BattleModel;
    }

    //是否无敌
    static public bool Whosyourdad = false;

    //FollowCamera2 followCamera;
    private RangeLogic rangeLogic;

    private AudioClip lastAudioClip;

    private RoleInstance _player
    {
        get { return GameRuntimeData.Instance.Player; }
    }

    private void Awake()
    {
        //followCamera = GetComponent<LevelMaster>().m_PlayerFollowCamera;
        //BindListener(m_AutoBattleButton, delegate (bool isOn) { EnableAutoBattle(isOn); });

        m_CurBattleRolePanel.Hide();
    }

    // Use this for initialization
    void Start()
    {
        m_BattleActionPanel.gameObject.SetActive(false);
        //m_AutoBattleButton.gameObject.SetActive(false);
    }

    Action<BattleResult> _callback;
    public Action<BattleResult> GetCallback() { return _callback; }

    public Jyx2Battle _battleData;

    public void StartBattle(Action<BattleResult> callback = null, int range = 16, bool backToBigMap = true, bool playerJoin = true)
    {
        Debug.Log("StartBattle called");
        if (IsInBattle) return;

        _callback = callback;

        if (!BattleboxHelper.Instance.CanEnterBattle(_player.View.transform.position)) return;

        IsInBattle = true;

        //StoryEngine.Instance.middleTopMessageSuggestPanel.Show("战斗开始");
        Jyx2_UIManager.Instance.ShowUI("CommonTipsUIPanel", TipsType.MiddleTop, "战斗开始");

        //m_AutoBattleButton.isOn = false; //默认取消自动战斗
        //m_AutoBattleButton.gameObject.SetActive(true);

        LevelMaster.Instance.UpdateMobileControllerUI();

        /*if (CameraHelper.Instance.m_BattleCamPoint == null)
        {
            CameraHelper.Instance.m_BattleCamPoint = new GameObject()
            {
                name = "BattlePoint",
            };
        }
        CameraHelper.Instance.m_BattleCamPoint.transform.position = _player.View.transform.position;*/

        //初始化战斗model
        BattleModel = new BattleFieldModel
        {
            Callback = rst =>
            {
                switch (rst)
                {
                    case BattleResult.Win:
                        {
                            //GetBonus(out string bonusText);
                            string bonusText = calExpGot();
                            //StoryEngine.Instance.fullSuggestPanel.Show($"<color=yellow><size=80>战斗胜利！</size></color>\n\n{bonusText}", delegate
                            //{
                            //    EndBattle();
                            //    callback?.Invoke(rst);
                            //});
                            GameUtil.ShowFullSuggest(bonusText, "<color=yellow><size=50>战斗胜利</size></color>", delegate
                             {
                                 EndBattle();
                                 callback?.Invoke(rst);
                             });
                            break;
                        }
                    case BattleResult.Lose:
                        {
                            //StoryEngine.Instance.fullSuggestPanel.Show("<color=red><size=80>战斗失败！</size></color>\n\n胜败乃兵家常事，请大侠重新来过。", delegate
                            //{
                            //    EndBattle();
                            //    callback?.Invoke(rst);
                            //    if (backToBigMap)
                            //        LevelLoader.LoadGameMap("Level_BigMap");
                            //});
                            GameUtil.ShowFullSuggest("胜败乃兵家常事，请大侠重新来过。", "<color=red><size=80>战斗失败！</size></color>", delegate
                            {
                                EndBattle();
                                callback?.Invoke(rst);
                                if (backToBigMap)
                                    LevelLoader.LoadGameMap("Level_BigMap");
                            });
                            break;
                        }
                }
            }
        };

        //初始化范围逻辑
        rangeLogic = new RangeLogic(BattleboxHelper.Instance.IsBlockExists, BattleModel.BlockHasRole);



        //if (MapRuntimeData.Instance.ExploreTeam == null)
        //{
        //    //没有探索队伍时，只添加主角
        //    _player.EnterBattle(0);
        //}
        //else
        //{
        //    //添加探索队伍（包含主角）
        //    foreach (var teammate in MapRuntimeData.Instance.ExploreTeam)
        //    {
        //        teammate.EnterBattle(0);
        //    }
        //}

        ////搜索并添加附近的引导（老成）
        //foreach (var npc in SearchNPC(range, MapRoleBehavior.Guide))
        //{
        //    npc.EnterBattle(0);
        //}

        ////搜索并添加附近的敌人
        //foreach (var npc in SearchNPC(range, MapRoleBehavior.Enemy))
        //{
        //    npc.EnterBattle(1);
        //}

        //状态初始化
        HSUtilsEx.CallWithDelay(this, () =>
        {
            Debug.Log("-----------HSUtilsEx.CallWithDelay");

            BattleboxHelper.Instance.EnterBattle(_player.View.transform.position);

            //地图上所有单位进入战斗
            foreach (var role in GameObject.FindObjectsOfType<MapRole>())
            {
                if (role.m_Behavior == MapRoleBehavior.Enemy)
                {
                    role.DataInstance.EnterBattle(1);
                }
                else
                {
                    role.DataInstance.EnterBattle(0);
                }
            }

            SwitchStatesTo(BattleViewStates.WaitingForNextActiveBattleRole);
        }, 0.5f);


        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if(brain != null)
        {
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        }
    }

    string calExpGot()
    {

        List<RoleInstance> alive_teammate = BattleModel.Roles.Where(r => r.team == 0).ToList();
        string rst = "";
        foreach(var role in alive_teammate)
        {
            int expAdd = _battleData.Exp / alive_teammate.Count();
            role.ExpGot += expAdd;
            rst += string.Format("{0}获得经验{1}\n", role.Name, role.ExpGot);
        }

        //分配经验
        foreach(var role in alive_teammate)
        {
            var practiseItem = role.GetXiulianItem();

            if (role.Level >= GameConst.MAX_ROLE_LEVEL)
            {
                role.ExpForItem += role.ExpGot;
            }else if(practiseItem != null)
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

            //修炼秘籍
            if(practiseItem != null)
            {
                change = 0;
                while (role.CanFinishedItem())
                {
                    role.UseItem(practiseItem);
                    change++;
                    rst += $"{role.Name}学会{practiseItem.Name}\n";
                }

                //TODO：炼制物品kyscpp BattleScene.cpp 1995行
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
            if (!ignoreRole && BattleModel.BlockHasRole(data.BattlePos.X, data.BattlePos.Y)) continue;

            var dist = (data.WorldPos - pos).sqrMagnitude;
            if (minDist > dist)
            {
                minDist = dist;
                rst = data;
            }
        }
        return rst;
    }

    /// <summary>
    /// 搜索范围内的NPC
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    private List<RoleInstance> SearchNPC(int range = 16, MapRoleBehavior mapRoleBehaviour = MapRoleBehavior.Enemy)
    {
        List<RoleInstance> npcList = new List<RoleInstance>();

        foreach (var role in MapRuntimeData.Instance.Roles)
        {
            //排除RoleInstance为空
            if (role == null) continue;

            //排除RoleView为空
            if (role.View == null) continue;

            //排除隐藏角色
            if (!role.View.gameObject.activeInHierarchy) continue;

            //排除其他MapRoleBehaviour
            if (role.View.m_Behavior != mapRoleBehaviour) continue;

            //排除战斗状态角色
            if (role.IsInBattle()) continue;

            //排除死亡的非关键角色
            if (!role.View.m_IsKeyRole && role.IsDead()) continue;

            if ((role.View.transform.position - _player.View.transform.position).magnitude < range) //附近所有角色，默认16
            {
                npcList.Add(role);
            }
        }
        return npcList;
    }

    /// <summary>
    /// 添加角色到战场里面
    /// </summary>
    /// <param name="role"></param>
    /// <param name="team"></param>
    public void AddBattleRole(RoleInstance role, int team)
    {
        //计算NPC应该站的点
        BattleBlockData npcStandBlock = FindNearestBattleBlock(/*team == 0 ? _player.View.transform.position :*/ role.View.transform.position);

        if (npcStandBlock == null)
        {
            Debug.LogError($"错误，{role.Key}找不到有效格子");
            return;
        }

        //加入战场
        BattleModel.AddBattleRole(role, npcStandBlock.BattlePos, team, (team != 0));

        //刷新角色状态面板
        RefreshRoleStatusPanel();

        //移动到那个点
        StartCoroutine(DoPlayerMove(role.View, new List<Vector3>() { npcStandBlock.WorldPos }, () =>
        {
            //待命
            role.View.Idle();

            var enemy = GetNearestEnemy(role);
            if(enemy != null)
            {
                //面向最近的敌人
                role.View.LookAtWorldPosInBattle(enemy.View.transform.position);
            }

            //死亡的关键角色默认晕眩
            if (role.View.m_IsKeyRole && role.IsDead())
            {
                role.Stun(-1);
            }
        }));
    }

    public void EndBattle()
    {
        IsInBattle = false;

        //m_AutoBattleButton.gameObject.SetActive(false);
        Jyx2_UIManager.Instance.HideUI("BattleMainUIPanel");

        LevelMaster.Instance.UpdateMobileControllerUI();

        //临时，需要调整
        foreach (var role in BattleModel.Roles)
        {
            //role.LeaveBattle();
            //非KeyRole死亡2秒后尸体消失
            if (role.IsDead() && !role.View.m_IsKeyRole)
            {
                role.View.gameObject.SetActive(false);
            }
        }

        //清理所有格子
        BattleboxHelper.Instance.ClearAllBlocks();
        rangeLogic = null;

        HideSkillUIPanel(); //隐藏技能面板

        //隐藏角色状态面板
        HideRoleStatusPanel();

        CameraHelper.Instance.ChangeFollow(_player.View.transform);
        //CameraHelper.Instance.ChangeBattleCamFOV(60f, 0.2f);

        BattleModel.Roles.Clear();

        //StoryEngine.Instance.m_AudioSource.clip = lastAudioClip;
        //StoryEngine.Instance.m_AudioSource.Play();

        //游戏存档
        //if (GameRuntimeData.Instance != null)
        //{
        //    GameRuntimeData.Instance.GameSave();
        //}

        if (_currentRoleFocusRing != null)
        {
            Jyx2ResourceHelper.ReleasePrefabInstance(_currentRoleFocusRing);
        }
    }

    void HideSkillUIPanel()
    {
        m_BattleActionPanel.SetActive(false);
    }

    void SetCurrentSkill(int index)
    {
        var content = m_BattleActionPanel.transform.Find("SkillSelection/Content");
        OnSetCurrentSkillButton(content.GetChild(0).GetComponent<Button>());
    }

    //设置技能为选中状态
    void OnSetCurrentSkillButton(Button btn)
    {
        if (btn != null)
        {
            btn.transform.Find("CurrentTag").gameObject.SetActive(true);
            btn.transform.Find("CurrentTag").localScale = Vector3.one * 3;
            btn.transform.Find("CurrentTag").DOScale(1f, 0.5f);
        }

        //隐藏其他的
        var content = m_BattleActionPanel.transform.Find("SkillSelection/Content");
        for (int i = 0; i < content.childCount; ++i)
        {
            var b = content.transform.GetChild(i).GetComponent<Button>();
            if (b != btn)
            {
                b.transform.Find("CurrentTag").gameObject.SetActive(false);
            }
        }
    }

    void ShowBattleActionPanel(RoleInstance role)
    {
        m_BattleActionPanel.SetActive(true);

        var menuRoot = m_BattleActionPanel.transform.Find("BattleActionMenu");
        for(int i=0;i< menuRoot.childCount; ++i)
        {
            menuRoot.GetChild(i).gameObject.SetActive(true);
        }

        if (role.UsePoison <= 0 || role.Tili < 30)
        {
            menuRoot.Find("PoisonButton").gameObject.SetActive(false);
        }
        if(role.DePoison <= 0 || role.Tili < 30)
        {
            menuRoot.Find("DepoisonButton").gameObject.SetActive(false);
        }
        if(role.Heal <= 0 || role.Tili < 10)
        {
            menuRoot.Find("HealButton").gameObject.SetActive(false);
        }
        //TODO：暗器、药品，在物品中实现
    }

    void SelectVirtualSkill(BattleZhaoshiInstance zhaoshi)
    {
        //隐藏技能面板
        HideSkillUIPanel();

        //隐藏角色状态面板
        HideRoleStatusPanel();

        //隐藏所有移动格子
        BattleboxHelper.Instance.HideAllBlocks();

        //存储选择的招式
        _currentSelectZhaoshi = zhaoshi;

        //选择招式了
        OnSelectZhaoshi(_currentRole, zhaoshi);
    }

    public void OnSelectUsePoison()
    {
        var zhaoshi = new PoisonZhaoshiInstance(_currentRole.UsePoison);
        SelectVirtualSkill(zhaoshi);
    }

    public void OnSelectDePosion()
    {
        var zhaoshi = new DePoisonZhaoshiInstance(_currentRole.DePoison);
        SelectVirtualSkill(zhaoshi);
    }

    public void OnSelectHeal()
    {
        var zhaoshi = new HealZhaoshiInstance(_currentRole.Heal);
        SelectVirtualSkill(zhaoshi);
    }

    public void OnSelectAnqi()
    {
        //var zhaoshi = new AnqiZhaoshiInstance(_currentRole.Anqi);
        //SelectVirtualSkill(zhaoshi);
    }

    void ShowSkillUIPanel(RoleInstance role)
    {
        //行动面板
        ShowBattleActionPanel(role);

        //显示名字
        //m_BattleActionPanel.transform.Find("SkillSelection/NameText").GetComponent<Text>().text = "TO DELETE";

        //招式列表
        var zhaoshis = role.GetZhaoshis(true).ToList();

        //按钮集合父节点
        var content = m_BattleActionPanel.transform.Find("SkillSelection/Content");

        //刷新招式按钮
        for (int i = 0; i < content.childCount; ++i)
        {
            var btn = content.transform.GetChild(i).GetComponent<Button>();

            if (i < zhaoshis.Count)
            {
                btn.gameObject.SetActive(true);
                var zhaoshi = zhaoshis[i];

                btn.transform.Find("Text").GetComponent<Text>().text = zhaoshi.Data.Name;

                //图标
                //if (zhaoshi.Data.GetIcon() != null)
                //    btn.transform.Find("Icon").GetComponent<Image>().sprite = zhaoshi.Data.GetIcon();

                var cdmaskImage = btn.transform.Find("CdMask").GetComponent<Image>();

                //判断技能状态
                if (zhaoshi.GetStatus() == BattleZhaoshiInstance.ZhaoshiStatus.OK)
                {
                    btn.transform.Find("Text").GetComponent<Text>().text +=
                        $"\n<size=18>冷却:{zhaoshi.Data.GetCoolDown()}</size>";

                    cdmaskImage.fillAmount = 0;

                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {

                        //当前选中
                        OnSetCurrentSkillButton(btn);

                        //隐藏技能面板
                        HideSkillUIPanel();

                        //隐藏角色状态面板
                        HideRoleStatusPanel();

                        //隐藏所有移动格子
                        BattleboxHelper.Instance.HideAllBlocks();

                        //存储选择的招式
                        _currentSelectZhaoshi = zhaoshi;

                        //选择招式了
                        OnSelectZhaoshi(role, zhaoshi);
                    });
                }
                else if (zhaoshi.GetStatus() == BattleZhaoshiInstance.ZhaoshiStatus.CD)
                {
                    btn.transform.Find("Text").GetComponent<Text>().text +=
                        $"\n<size=18><color=red>冷却中:{(int)(zhaoshi.CurrentCooldown / BattleZhaoshiInstance.TimeTickCoolDown + 1)}</color></size>";

                    cdmaskImage.fillAmount = (float)zhaoshi.CurrentCooldown / (zhaoshi.Data.GetCoolDown() * BattleZhaoshiInstance.TimeTickCoolDown);

                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        StoryEngine.Instance.DisplayPopInfo("<color=red>技能冷却中..</color>");
                    });
                }
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    void ShowRoleStatusPanel()
    {
        //m_BattleRoleStatusPanel.SetActive(true);
    }


    void RefreshRoleStatusPanel()
    {

    }


    void HideRoleStatusPanel()
    {
    }

    void OnSelectZhaoshi(RoleInstance role, BattleZhaoshiInstance zhaoshi)
    {
        role.SwitchAnimationToSkill(zhaoshi.Data); //切换招式动作
        SwitchStatesTo(BattleViewStates.SelectSkillTarget);
    }

    void OnCastZhaoshi(RoleInstance role, BattleZhaoshiInstance zhaoshi, BattleBlockData target, int attackTwiceCounter = 0)
    {
        //JYX2 切换武功动作
        role.SwitchAnimationToSkill(zhaoshi.Data);

        zhaoshi.CastCD();
        zhaoshi.CastCost(role);

        List<RoleInstance> beHitAnimationList = new List<RoleInstance>();
        double baseAttack = role.Attack;
        double finalAttack = role.Attack * ToolsShared.GetRandom(0.8, 1.2); //todo
        var coverSize = zhaoshi.GetCoverSize();
        var coverType = zhaoshi.GetCoverType();
        var sx = _currentSelectMoveTo.X;
        var sy = _currentSelectMoveTo.Y;
        var tx = _currentSkillTo.X;
        var ty = _currentSkillTo.Y;
        var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, tx, ty, sx, sy, coverSize);
        //处理掉血
        foreach (var blockVector in coverBlocks)
        {
            var rolei = BattleModel.GetAliveRole(blockVector);
            //还活着
            if (rolei == null || rolei.IsDead()) continue;
            //打敌人的招式
            if(zhaoshi.IsCastToEnemy() && rolei.team == role.team) continue;
            //“打”自己人的招式
            if (!zhaoshi.IsCastToEnemy() && rolei.team != role.team) continue;
            //受伤逻辑
            //double baseDefence = rolei.Defence; //todo
            //int damage = Convert.ToInt32(finalAttack * 0.03 / (1 + baseDefence / baseAttack));
            //if (damage < 1)
            //    damage = 1;

            var result = GetSkillResult(role, rolei, zhaoshi, blockVector);

            result.Run();

            if(result.IsDamage())
            {
                //加入到受击动作List
                beHitAnimationList.Add(rolei);
            }
        }

        //切换播放状态
        SwitchStatesTo(BattleViewStates.PlayingAction);

        //隐藏技能面板
        HideSkillUIPanel();


        SkillCastHelper castHelper = new SkillCastHelper
        {
            Source = role.View,
            CoverBlocks = coverBlocks.ToTransforms(),
            Zhaoshi = zhaoshi,
            Targets = beHitAnimationList.ToMapRoles(),
        };

        if(attackTwiceCounter < role.Zuoyouhubo)
        {
            castHelper.Play(()=> {
                OnCastZhaoshi(role, zhaoshi, target, attackTwiceCounter + 1);
            });
        }
        else
        {
            castHelper.Play(DoCallbackBattleAction);
        }
    }


    IEnumerator CastTexiaoAndWaitSkill(GameObject pre, float time, Transform parent, Action callback = null)
    {
        GameObject obj = null;
        if (pre != null)
        {
            obj = GameObject.Instantiate(pre);
            obj.transform.SetParent(parent.transform, false);
        }
        yield return new WaitForSeconds(time);
        if (pre != null)
        {
            GameObject.Destroy(obj);
        }

        callback?.Invoke();
        RefreshRoleStatusPanel();
    }

    void Update()
    {
        if (!IsInBattle)
            return;

        //移动
        if (GetCurrentStates() == BattleViewStates.SelectMove)
        {
            var block = GetMouseUpBattleBlock();
            if (block == null) return;

            if (block.gameObject.activeSelf == false) return;
            //站人了
            if (BattleModel.BlockHasRole(block.BattlePos.X, block.BattlePos.Y)) return;

            //Debug.LogFormat("on move to pos ({0},{1})", block.BattlePos.X, block.BattlePos.Y);

            var paths = rangeLogic.GetWay(_currentRole.Pos.X, _currentRole.Pos.Y,
                block.BattlePos.X, block.BattlePos.Y);

            //隐藏所有格子
            BattleboxHelper.Instance.HideAllBlocks();

            //开始移动
            var posList = new List<Vector3>();
            foreach (var temp in paths)
            {
                var tempBlock = BattleboxHelper.Instance.GetBlockData(temp.X, temp.Y);
                posList.Add(tempBlock.WorldPos);
            }

            StartCoroutine(DoPlayerMove(_currentRole.View, posList, () =>
            {
                _currentRole.View.Idle();

                _currentSelectMoveTo = block.BattlePos;
                SwitchStatesTo(BattleViewStates.SelectSkill);

                //默认选择第一个技能
                var zhaoshis = _currentRole.GetZhaoshis(true).ToList();
                if(zhaoshis.Count > 0)
                {
                    _currentSelectZhaoshi = zhaoshis.First();

                    OnSelectZhaoshi(_currentRole, _currentSelectZhaoshi);

                    //第一个技能
                    SetCurrentSkill(0);
                }
            }));
        }
        else if (GetCurrentStates() == BattleViewStates.SelectSkillTarget)
        {
            var block = GetMouseDownBattleBlock();
            if (block == null) return;

            //朝向
            _currentRole.View.LookAtBattleBlock(block);
            //施展位置
            _currentSkillTo = block.BattlePos;
            SwitchStatesTo(BattleViewStates.PreshowSkillCoverRange);
        }
        //攻击
        else if (GetCurrentStates() == BattleViewStates.PreshowSkillCoverRange)
        {
            if (!Input.GetMouseButtonUp(0)) return;

            var block = GetMouseUpBattleBlock();
            if (block == null)
            {
                SwitchStatesTo(BattleViewStates.SelectSkillTarget);
                return;
            }

            //隐藏所有格子
            BattleboxHelper.Instance.HideAllBlocks();

            //施展招式
            if (_currentSelectZhaoshi.GetType() == typeof(AnqiZhaoshiInstance))
            {
                //暗器必须攻击指定目标，无目标不执行攻击
                if (BattleModel.BlockHasRole(block.BattlePos.X, block.BattlePos.Y))
                {
                    _currentRole.Pos = _currentSelectMoveTo;
                    OnCastZhaoshi(_currentRole, _currentSelectZhaoshi, block);
                    runtime.AddItem(_currentAnqiID, -1);
                    _currentRole.View.ShowAttackInfo("<color=orange>使用" + ConfigTable.Get<Jyx2Item>(_currentAnqiID).Name + "</color>");
                }
                else
                {
                    _currentRole.View.ShowAttackInfo("无攻击对象");
                }
            }
            else
            {
                _currentRole.Pos = _currentSelectMoveTo;
                OnCastZhaoshi(_currentRole, _currentSelectZhaoshi, block);
            }
        }
    }

    //BY CG：战斗中，对格子的操作，需要穿透touchpad（用于旋转屏幕的）
    bool IsPointerOverUIObjectExceptTouchpad()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        if (results == null || results.Count == 0)
            return false;

        //排除掉touchpad
        if (results.Count == 1)
        {
            var hit = results[0];
            if (hit.gameObject == LevelMaster.Instance.m_TouchPad.gameObject)
                return false;
        }

        return true;
    }

    BattleBlockData GetMouseUpBattleBlock()
    {
        if (Input.GetMouseButtonUp(0) && !IsPointerOverUIObjectExceptTouchpad() && !IsBattleMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            //待调整为格子才可以移动
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("Ground")))
            {
                var block = BattleboxHelper.Instance.GetLocationBattleBlock(hitInfo.point);
                if (block != null && block.IsActive)
                {
                    return block;
                }
            }
        }
        return null;
    }

    BattleBlockData GetMouseDownBattleBlock()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObjectExceptTouchpad() && !IsBattleMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //待调整为格子才可以移动
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("Ground")))
            {
                var block = BattleboxHelper.Instance.GetLocationBattleBlock(hitInfo.point);
                if (block != null && block.IsActive)
                {
                    return block;
                }
            }
        }
        return null;
    }

    BattleBlockData GetAIBattleBlock(Vector3 vector)
    {
        if (!IsBattleMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(vector);


            //待调整为格子才可以移动
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("Ground")))
            {
                var block = BattleboxHelper.Instance.GetLocationBattleBlock(hitInfo.point);
                if (block != null && block.IsActive)
                {
                    return block;
                }
            }
        }
        return null;
    }

    bool IsBattleMoving = false;

    IEnumerator DoPlayerMove(MapRole player, List<Vector3> path, Action callback)
    {
        IsBattleMoving = true;
        var count = path.Count;

        player.IsPlayingMovingAnimation = false;
        for (int i = 0; i < count; i++)
        {
            var pos = path[i];
            var temp = new Vector3(pos.x, pos.y, pos.z);
            yield return DoPlayerMove(player, temp);
        }

        //yield return new WaitForSeconds(0.2f); //有个切换动作的时间？否则在漂移？
        //player.m_Animator.SetTrigger("Idle");
        IsBattleMoving = false;
        //yield return new WaitForSeconds(0.05f); //有个切换动作的时间？否则在漂移？

        callback?.Invoke();
    }

    IEnumerator DoPlayerMove(MapRole player, Vector3 target)
    {
        var dist = (target - player.transform.position).magnitude;
        if (dist < 0.1f) //eqaul zero
            yield break;

        float time = (float)(dist / m_BattleMoveSpeed);

        if (!player.IsPlayingMovingAnimation)
        {
            player.Run(); //如果不是这样，则每次移动一格，移动动画会重新开始播放
            player.IsPlayingMovingAnimation = true;
        }

        player.transform.LookAt(new Vector3(target.x, player.transform.position.y, target.z)); //转身
        var tweener = player.transform.DOMove(target, time).SetEase(Ease.Linear).OnComplete(() => { });
        yield return new WaitForSeconds(time);
    }

    public void AIMove(RoleInstance role, BattleBlockVector block, Action callback)
    {
        _currentSelectMoveTo = block;
        var paths = rangeLogic.GetWay(role.Pos.X, role.Pos.Y,
                block.X, block.Y);
        var posList = new List<Vector3>();
        foreach (var temp in paths)
        {
            var tempBlock = BattleboxHelper.Instance.GetBlockData(temp.X, temp.Y);
            if (tempBlock != null) posList.Add(tempBlock.WorldPos);
        }
        StartCoroutine(DoPlayerMove(role.View, posList, callback));
    }

    public void AIAttack(RoleInstance role, BattleZhaoshiInstance zhaoshi, BattleBlockVector targetBlockVector)
    {
        _currentSelectZhaoshi = zhaoshi;
        var block = BattleboxHelper.Instance.GetBlockData(targetBlockVector.X, targetBlockVector.Y);
        //朝向
        role.View.LookAtBattleBlock(block);
        //施展位置
        _currentSkillTo = block.BattlePos;
        OnCastZhaoshi(role, zhaoshi, block);
    }
    #region BattleView相关

    BattleViewStates _currentStates;
    Action<BattleAction> _currentBattleActionCallback;
    BattleBlockVector _currentSelectMoveTo;
    BattleBlockVector _currentSkillTo;
    BattleZhaoshiInstance _currentSelectZhaoshi;
    RoleInstance _currentRole;
    Vector3 _currentRoleWorldPos;
    int _currentAnqiID;

    public BattleViewStates GetCurrentStates()
    {
        return _currentStates;
    }

    //切换当前战场状态
    public void SwitchStatesTo(BattleViewStates states)
    {
        _currentStates = states;
        CameraHelper.Instance.BattleFieldLockRole();

        if (states == BattleViewStates.SelectMove)
        {
            //获得角色移动能力
            int moveAbility = _currentRole.GetMoveAbility();

            //绘制周围的移动格子
            var blockList = rangeLogic.GetMoveRange(_currentRole.Pos.X, _currentRole.Pos.Y, moveAbility);
            BattleboxHelper.Instance.ShowBlocks(blockList);

            //显示技能面板，开始选择招式
            ShowSkillUIPanel(_currentRole);

            //当前没有选择技能，不设置高亮
            OnSetCurrentSkillButton(null);

            //显示角色状态面板
            ShowRoleStatusPanel();
        }
        else if (states == BattleViewStates.SelectSkillTarget)
        {
            //攻击距离
            int castSize = _currentSelectZhaoshi.GetCastSize();
            var coverType = _currentSelectZhaoshi.GetCoverType();
            var sx = _currentSelectMoveTo.X;
            var sy = _currentSelectMoveTo.Y;

            //绘制周围的攻击格子
            var blockList = rangeLogic.GetSkillCastBlocks(sx, sy, _currentSelectZhaoshi, _currentRole);
            BattleboxHelper.Instance.ShowBlocks(blockList, BattleBlockType.AttackZone);

            //显示技能面板，可以切换当前招式
            ShowSkillUIPanel(_currentRole);

            //显示角色状态面板
            ShowRoleStatusPanel();
        }
        else if (states == BattleViewStates.PreshowSkillCoverRange)
        {
            var coverSize = _currentSelectZhaoshi.GetCoverSize();
            var coverType = _currentSelectZhaoshi.GetCoverType();
            var sx = _currentSelectMoveTo.X;
            var sy = _currentSelectMoveTo.Y;
            var tx = _currentSkillTo.X;
            var ty = _currentSkillTo.Y;

            //绘制技能覆盖范围
            var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, tx, ty, sx, sy, coverSize);
            BattleboxHelper.Instance.ShowBlocks(coverBlocks, BattleBlockType.AttackZone);
        }
        else if (states == BattleViewStates.WaitingForNextActiveBattleRole)
        {
            //战斗下一步
            //BattleModel.NextStep();
            RefreshRoleStatusPanel();
        }
    }

    //等待玩家下达角色指令
    public void GetBattleActionFromPlayer(RoleInstance role, Action<BattleAction> callback)
    {
        _currentRoleWorldPos = role.View.transform.position; //预存当前世界坐标系位置
        _currentSelectMoveTo = role.Pos; //默认停留本地
        _currentSelectZhaoshi = null; //清空技能选择
        _currentSkillTo = null;
        _currentBattleActionCallback = callback;

        CameraHelper.Instance.ChangeFollow(role.View.transform); //切换摄像机跟随角色

        SwitchStatesTo(BattleViewStates.SelectMove);
    }

    public RoleInstance GetCurrentRole()
    {
        return _currentRole;
    }

    public void SetCurrentRole(RoleInstance role)
    {
        _currentRole = role;
        if (_currentRoleFocusRing == null)
        {
            _currentRoleFocusRing = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/CurrentBattleRoleTag.prefab");
        }
        _currentRoleFocusRing.transform.SetParent(_currentRole.View.transform, false);
        _currentRoleFocusRing.transform.localPosition = new Vector3(0, 0.15f, 0); //略微比地面高一点


        //m_CurBattleRolePanel.ShowRole(role);
        Jyx2_UIManager.Instance.ShowUI("BattleMainUIPanel", BattleMainUIState.ShowRole, role);
        //if (role.isAI)
        //{
        //    m_CurBattleRolePanel.ShowRole(role);
        //}
        //else
        //{
        //    m_CurBattleRolePanel.Hide();
        //}
    }

    GameObject _currentRoleFocusRing; //当前角色指示器

    public void GetBattleActionFromAI(RoleInstance role, Action<BattleAction> callback)
    {
        _currentSelectMoveTo = role.Pos; //默认停留本地
        _currentSelectZhaoshi = null; //清空技能选择
        _currentSkillTo = null;
        _currentBattleActionCallback = callback;

        //TODO 判断是否在摄像机显示内，如果为否，则切换
        //if (role.team == 0)
        {
            CameraHelper.Instance.ChangeFollow(role.View.transform); //切换摄像机跟随角色
        }
    }

    public void OnMoveButtonClicked()
    {
        var states = GetCurrentStates();
        if (states == BattleViewStates.SelectSkill || states == BattleViewStates.SelectSkillTarget)
        {
            //位置还原
            _currentRole.View.transform.position = _currentRoleWorldPos;

            //选择技能还原
            _currentSelectZhaoshi = null;
            _currentSkillTo = null;

            //还原角色位置
            _currentSelectMoveTo = _currentRole.Pos;

            //状态转移
            SwitchStatesTo(BattleViewStates.SelectMove);
        }
    }

    public void OnRestButtonClicked()
    {
        _currentRole.View.Idle();
        _currentRole.OnRest();
        DoCallbackBattleAction();
    }

    GameRuntimeData runtime { get { return GameRuntimeData.Instance; } }

    public void OnItemButtonClicked()
    {
        Func<Jyx2Item, bool> filter = (item) => {
            //药物或暗器
            return item.ItemType == 3 || item.ItemType == 4;
        };
        //BagPanel.Create(this.transform.Find("UI"), runtime.Items, (itemId) => {

        //    if (itemId == -1)
        //        return;

        //    var item = ConfigTable.Get<Jyx2Item>(itemId);

        //    //药品
        //    if(item.ItemType == 3)
        //    {
        //        _currentRole.UseItem(item);
        //        runtime.AddItem(itemId, -1);
        //        _currentRole.View.ShowAttackInfo("<color=orange>使用" + item.Name + "</color>");
        //        DoCallbackBattleAction();
        //    }
        //    //暗器
        //    else if(item.ItemType == 4)
        //    {

        //    }

        //}, filter);
        Jyx2_UIManager.Instance.ShowUI("BagUIPanel", runtime.Items,new Action<int>((itemId) =>
        {

            if (itemId == -1)
                return;

            var item = ConfigTable.Get<Jyx2Item>(itemId);

            //药品
            if (item.ItemType == 3)
            {
                _currentRole.UseItem(item);
                runtime.AddItem(itemId, -1);
                _currentRole.View.ShowAttackInfo("<color=orange>使用" + item.Name + "</color>");
                DoCallbackBattleAction();
            }
            //暗器
            else if (item.ItemType == 4)
            {
                OnSelectAnqi();
                _currentAnqiID = itemId;
                Jyx2_UIManager.Instance.HideUI("BagUIPanel");
            }

        }), filter);
    }

    public void EnableAutoBattle(bool isEnable)
    {
        m_AutoBattleButton.transform.Find("Text").GetComponent<Text>().text = isEnable ? "取消自动" : "自动战斗";

        foreach (var role in BattleModel.Teammates)
        {
            role.isAI = isEnable;
        }

        //if (isEnable && _currentRole.team == 0)
        //    _currentRole.BattleAction();

        /*if(isEnable)
            CameraHelper.Instance.ChangeBattleCamFOV(70f, 0.2f);
        else
            CameraHelper.Instance.ChangeBattleCamFOV(60f, 0.2f);*/
    }

    void DoCallbackBattleAction()
    {
        _currentBattleActionCallback?.Invoke(new BattleAction()
        {
            MoveTo = _currentSelectMoveTo,
            Skill = _currentSelectZhaoshi,
            SkillTo = _currentSkillTo
        });

        SwitchStatesTo(BattleViewStates.WaitingForNextActiveBattleRole);
    }

    #endregion

    public void GetAIResult(RoleInstance role, Action<AIResult> callback)
    {
        GetAIResultClassic(role, callback);
    }

    public void CreateAIAction(RoleInstance role, Action<BattleAction> callback)
    {
        //AI走过去丢技能
        GetAIResult(role, result =>
        {
            BattleAction action = new BattleAction
            {
                MoveTo = new BattleBlockVector(result.MoveX, result.MoveY),
                SkillTo = new BattleBlockVector(result.AttackX, result.AttackY),
                Skill = result.Zhaoshi
            };
            callback(action);
        });
        //var heavyMethod = Observable.Start(() =>
        //{
        //    //AI走过去丢技能
        //    GetAIResult(role, result =>
        //    {
        //        BattleAction action = new BattleAction
        //        {
        //            MoveTo = new BattleBlockVector(result.MoveX, result.MoveY),
        //            SkillTo = new BattleBlockVector(result.AttackX, result.AttackY),
        //            Skill = result.Zhaoshi
        //        };
        //        MainThreadDispatcher.Post(delegate (object obj)
        //        {
        //            callback(action);
        //        }, null);
        //    });
        //});
        //Observable.WhenAll(heavyMethod)
        //    .ObserveOnMainThread()
        //    .Subscribe();
    }

    public void GetAIResultClassic(RoleInstance role, Action<AIResult> callback)
    {
        //初始化范围逻辑
        rangeLogic = new RangeLogic(BattleboxHelper.Instance.IsBlockExists, BattleModel.BlockHasRole);

        //获得角色移动能力
        int moveAbility = role.GetMoveAbility();

        //行动范围
        var range = rangeLogic.GetMoveRange(role.Pos.X, role.Pos.Y, moveAbility);

        //可使用招式
        var zhaoshis = role.GetZhaoshis(false);

        //TODO:攻击计算缓存 by Cherubinxxx
        //AttackResultCache cache = new AttackResultCache(currentSprite, Field);

        //AI算法：穷举每个点，使用招式，取最大收益
        AIResult result = null;
        double maxscore = 0;

        foreach (var zhaoshi in zhaoshis)
        {
            if (zhaoshi.GetStatus() != BattleZhaoshiInstance.ZhaoshiStatus.OK)
                continue;

            GetMoveAndCastPos(role, zhaoshi, range);

            BattleBlockVector[] tmp = m_GetMoveAndCastPosResult;
            if (tmp != null && tmp.Length == 2 && tmp[0] != null)
            {
                BattleBlockVector movePos = tmp[0];
                BattleBlockVector castPos = tmp[1];
                double score = GetSkillCastResultScore(role, zhaoshi, movePos.X, movePos.Y, castPos.X, castPos.Y, true);
                // yield return 0; //分帧
                //if (score <= 0 && rst.HitEnemyCount > 0)
                //{
                //    score = ToolsShared.GetRandom(0, 1);
                //}
                if (score > maxscore)
                {
                    maxscore = score;
                    result = new AIResult
                    {
                        AttackX = castPos.X,
                        AttackY = castPos.Y,
                        MoveX = movePos.X,
                        MoveY = movePos.Y,
                        Zhaoshi = zhaoshi,
                        IsRest = false
                    };
                }
            }
        }

        //Debug.Log(Time.realtimeSinceStartup);

        if (result != null)
        {
            callback(result);
            return;
        }

        //否则靠近自己最近的敌人
        result = MoveToNearestEnemy(role, range);
        if (result != null)
        {
            callback(result);
            return;
        }

        //否则原地休息
        callback(Rest(role));
        return;
    }

    public double GetSkillCastResultScore(RoleInstance caster, BattleZhaoshiInstance skill,
            int movex, int movey, int castx, int casty, bool isAIComputing)
    {
        double score = 0;
        var coverSize = skill.GetCoverSize();
        var coverType = skill.GetCoverType();
        var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, castx, casty, movex, movey, coverSize);

        foreach (var blockVector in coverBlocks)
        {
            var targetRole = BattleModel.GetAliveRole(blockVector);
            //还活着
            if (targetRole == null || targetRole.IsDead()) continue;
            //打敌人的招式
            if (skill.IsCastToEnemy() && caster.team == targetRole.team) continue;
            //“打”自己人的招式
            if (!skill.IsCastToEnemy() && caster.team != targetRole.team) continue;

            var result = GetSkillResult(caster, targetRole, skill, blockVector);
            score += result.GetTotalScore();
        }
        return score;
    }

    /// <summary>
    /// 靠近自己最近的敌人
    /// </summary>
    /// <returns>The to nearest enemy.</returns>
    /// <param name="sprite">Sprite.</param>
    /// <param name="moverange">Moverange.</param>
    public AIResult MoveToNearestEnemy(RoleInstance sprite, List<BattleBlockVector> range)
    {
        var tmp = GetNearestEnemyBlock(sprite, range);
        if (tmp == null) return null;

        AIResult rst = new AIResult
        {
            Zhaoshi = null,
            MoveX = tmp.X,
            MoveY = tmp.Y,
            IsRest = true //靠近对手
        };
        return rst;
    }

    /// <summary>
    /// 原地休息
    /// </summary>
    /// <param name="sprite">Sprite.</param>
    public AIResult Rest(RoleInstance sprite)
    {
        AIResult rst = new AIResult
        {
            MoveX = sprite.Pos.X,
            MoveY = sprite.Pos.Y,
            IsRest = true
        };
        return rst;
    }

    private BattleBlockVector[] m_GetMoveAndCastPosResult = new BattleBlockVector[2];

    public void GetMoveAndCastPos(RoleInstance role, BattleZhaoshiInstance zhaoshi, List<BattleBlockVector> moveRange)
    {
        //clear
        m_GetMoveAndCastPosResult[0] = null;
        m_GetMoveAndCastPosResult[1] = null;

        //丢给自己的，随便乱跑一个地方丢
        if (zhaoshi.GetCoverType() == SkillCoverType.POINT && zhaoshi.GetCastSize ()== 0 && zhaoshi.GetCoverSize()==0)
        {
            BattleBlockVector targetBlock = null;
            if ((float)role.Hp / role.MaxHp > 0.5)
            {
                targetBlock = GetNearestEnemyBlock(role, moveRange); //生命大于50%前进
            }
            else
            {
                targetBlock = GetFarestEnemyBlock(role, moveRange); //生命小于50%后退
            }
            m_GetMoveAndCastPosResult[0] = targetBlock;
            m_GetMoveAndCastPosResult[1] = targetBlock;
        }

        bool isAttack = zhaoshi.IsCastToEnemy();
        double maxScore = 0;

        //带攻击范围的，找最多人丢
        foreach (var moveBlock in moveRange)
        {
            int castSize = zhaoshi.GetCastSize();
            var coverType = zhaoshi.GetCoverType();
            var sx = moveBlock.X;
            var sy = moveBlock.Y;
            var castBlocks = rangeLogic.GetSkillCastBlocks(sx, sy, zhaoshi, role);

            int splitFrame = 0;//分帧
            foreach (var castBlock in castBlocks)
            {
                double score = 0;
                var coverSize = zhaoshi.GetCoverSize();
                var tx = castBlock.X;
                var ty = castBlock.Y;
                var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, tx, ty, sx, sy, coverSize);

                foreach (var coverBlock in coverBlocks)
                {
                    var targetSprite = BattleModel.GetAliveRole(coverBlock);
                    //位置没人
                    if (targetSprite == null) continue;

                    //如果判断是施展给原来的自己，但自己已经不在原位置了,相当于没打中
                    if (targetSprite == role && !(targetSprite.Pos.X == moveBlock.X && targetSprite.Pos.Y == moveBlock.Y)) continue;
                    //如果是自己的新位置，则相当于施展给自己
                    if (targetSprite.Pos.X == moveBlock.X && targetSprite.Pos.Y == moveBlock.Y)
                    {
                        continue;
                        //targetSprite = sprite;
                    }
                    else if (targetSprite.team != role.team && targetSprite.Hp > 0)
                    {
                        score += 0.1;
                    }
                }

                if (score > maxScore)
                {
                    maxScore = score;

                    m_GetMoveAndCastPosResult[0] = new BattleBlockVector(moveBlock.X, moveBlock.Y);
                    m_GetMoveAndCastPosResult[1] = new BattleBlockVector(castBlock.X, castBlock.Y);
                }
            }
            if (splitFrame++ > 5)//分帧
            {
                // yield return 0;
                splitFrame = 0;
            }
        }
        if (maxScore > 0)
        {

        }
        else
        {
            m_GetMoveAndCastPosResult[0] = null;
            m_GetMoveAndCastPosResult[1] = null;
        }
    }

    public RoleInstance GetNearestEnemy(RoleInstance role)
    {
        int minDistance = int.MaxValue;
        RoleInstance targetRole = null;
        //寻找离自己最近的敌人
        foreach (var sp in BattleModel.AliveRoles)
        {
            if (sp == role) continue;

            if (sp.team == role.team) continue;

            int distance = BattleBlockVector.GetDistance(sp.Pos.X, sp.Pos.Y, role.Pos.X, role.Pos.Y);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetRole = sp;
            }
        }
        return targetRole;
    }

    public BattleBlockVector GetNearestEnemyBlock(RoleInstance sprite, List<BattleBlockVector> moverange = null)
    {
        var targetRole = GetNearestEnemy(sprite);
        if (targetRole == null)
            return null;

        int minDis2 = int.MaxValue;
        int movex = sprite.Pos.X, movey = sprite.Pos.Y;
        //寻找离对手最近的一点
        foreach (var mr in moverange)
        {
            int distance = BattleBlockVector.GetDistance(mr.X, mr.Y, targetRole.Pos.X, targetRole.Pos.Y);

            if (distance <= minDis2)
            {
                minDis2 = distance;
                movex = mr.X;
                movey = mr.Y;
            }
        }
        BattleBlockVector rst = new BattleBlockVector
        {
            X = movex,
            Y = movey
        };
        return rst;
    }

    public BattleBlockVector GetFarestEnemyBlock(RoleInstance sprite, List<BattleBlockVector> range)
    {
        int max = 0;
        BattleBlockVector rst = new BattleBlockVector();
        //寻找一个点离敌人最远
        foreach (var r in range)
        {
            int min = int.MaxValue;
            foreach (RoleInstance sp in BattleModel.AliveRoles)
            {
                int distance = BattleBlockVector.GetDistance(sp.Pos.X, sp.Pos.Y, r.X, r.Y);
                if (sp.team != sprite.team && distance < min)
                {
                    min = distance;
                }
            }
            if (min > max)
            {
                max = min;
                rst = r;
            }
        }
        return rst;
    }

    //public void GetBonus(out string bonusText)
    //{
    //    int totalExp = 0;
    //    int totalMoney = 0;
    //    List<ItemInstance> bonusItemList = new List<ItemInstance>();
    //    foreach (var enemy in BattleModel.Enemys)
    //    {
    //        if (enemy.Data.Tag == "BOSS")
    //        {
    //            totalExp += enemy.Level * 50;
    //            totalMoney += enemy.Level * 50;
    //            if (ToolsShared.ProbabilityTest(0.5))
    //                bonusItemList.Add(RuntimeHelper.Instance.AddRandomItemByLevel(enemy.Level + 2));
    //        }
    //        else
    //        {
    //            totalExp += enemy.Level * 10;
    //            totalMoney += enemy.Level * 10;
    //            if (ToolsShared.ProbabilityTest(0.05))
    //                bonusItemList.Add(RuntimeHelper.Instance.AddRandomItemByLevel(enemy.Level));
    //        }
    //    }
    //    RuntimeHelper.Instance.AddTeamExp(totalExp);
    //    RuntimeHelper.Instance.AddMoney(totalMoney);

    //    bonusText = $"获得：{totalExp}经验，{totalMoney}银两";
    //    foreach (var bonusItem in bonusItemList)
    //    {
    //        bonusText += $"\n获得：{bonusItem.Key}";
    //    }
    //}

    #region 金庸群侠传适配

    /// <summary>
    /// 中毒的效果，对应BattleScene::poisonEffect(Role* r)
    /// </summary>
    /// <param name="r"></param>
    public void poisonEffect(RoleInstance role)
    {
        if (role == null)
            return;
        if (role.Poison == 0)
            return;
        int tmp = role.Hp;
        role.Poison -= role.AntiPoison;
        role.Poison = Tools.Limit(role.Poison, 0, 100);
        role.Hp -= role.Poison / 3;
        if (role.Hp < 1)
            role.Hp = 1;
        int effectRst = tmp - role.Hp;
        role.View.ShowAttackInfo($"<color=green>毒发-{effectRst}</color>");
    }


    //对应kys-cpp:BattleScene::calMagicHurt
    public SkillCastResult GetSkillResult(RoleInstance r1, RoleInstance r2, BattleZhaoshiInstance skill, BattleBlockVector blockVector)
    {
        SkillCastResult rst = new SkillCastResult(r1, r2, skill, blockVector.X, blockVector.Y);
        var magic = skill.Data.GetSkill();
        int level_index = skill.Data.GetLevel();
        level_index = skill.calMaxLevelIndexByMP(r1.Mp, level_index);
        //普通攻击
        if (magic.DamageType == 0)
        {
            if (r1.Mp <= 10)
            {
                rst.damage = 1 + UnityEngine.Random.Range(0, 10);
                return rst;
            }
            int attack = r1.Attack + skill.Data.GetSkillLevelInfo(level_index).Attack / 3;
            int defence = r2.Defence;
            if (r1.Weapon >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r1.Weapon);
                attack += i.Attack;
            }
            if (r1.Armor >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r1.Armor);
                attack += i.Attack;
            }
            if (r2.Weapon >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r2.Weapon);
                defence += i.Defence;
            }
            if (r2.Armor >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r2.Armor);
                defence += i.Defence;
            }

            int v = attack - defence;
            int dis = r1.Pos.GetDistance(r2.Pos);
            v = (int)(v / Math.Exp((dis - 1) / 10));
            v += UnityEngine.Random.Range(0, 10) - UnityEngine.Random.Range(0, 10);
            if (v < 10)
            {
                v = 1 + UnityEngine.Random.Range(0, 10);
            }
            rst.damage = v;
            return rst;
        }
        else if(magic.DamageType == 1) //吸内
        {
            int v = skill.Data.GetSkillLevelInfo().KillMp;
            v += UnityEngine.Random.Range(0, 10) - UnityEngine.Random.Range(0, 10);
            if (v < 10)
            {
                v = 1 + UnityEngine.Random.Range(0, 10);
            }
            rst.damageMp = v;
            return rst;
        }else if(magic.DamageType == 2) //用毒 -GameUtil::usePoison
        {
            rst.poison = usePoison(r1, r2);
            return rst;
        }else if(magic.DamageType == 3) //解毒
        {
            rst.depoison = detoxification(r1, r2);
            return rst;
        }
        else if(magic.DamageType == 4) //治疗
        {
            rst.heal = medicine(r1, r2);
            return rst;
        }
        return null;
    }

    //用毒
    int usePoison(RoleInstance r1, RoleInstance r2)
    {
        int add = r1.UsePoison / 3;
        int rst = Tools.Limit(r2.Poison + add, 0, 100);
        return rst - r2.Poison;
    }

    //医疗的效果
    int medicine(RoleInstance r1, RoleInstance r2)
    {
        int add = r1.Heal;
        int rst = Tools.Limit(r2.Hp+add, 0, r2.MaxHp);
        return rst - r2.Hp;
    }

    //解毒
    //注意这个返回值通常应为负
    int detoxification(RoleInstance r1, RoleInstance r2)
    {
        int mius = r1.DePoison / 3;
        int rst = Tools.Limit(r2.Poison - mius, 0, 100);
        return r2.Poison - rst;
    }

    #endregion
}
