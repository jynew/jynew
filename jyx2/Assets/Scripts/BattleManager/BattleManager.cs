using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Jyx2;
using HSFrameWork.Common;
using UnityEngine;


public class BattleStartParams 
{
    public Action<BattleResult> callback;//战斗结果
    public List<RoleInstance> roles;//参与战斗的角色
    public Jyx2Battle battleData;//战斗地图数据
    public int range = 16;
    public bool backToBigMap = true;
    public bool playerJoin = true;
}
public class BattleManager:MonoBehaviour
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
        Move,//移动状态
        AI,
        UseItem,//使用物品的状态 主要播放使用物品动画
        BuffSettlement,//buff结算状态 这里主要是用毒
    }

    string[] battleMusics = new string[]
    {
        "Assets/BuildSource/Musics/5.mp3",
        "Assets/BuildSource/Musics/6.mp3",
        "Assets/BuildSource/Musics/7.mp3",
    };

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
                _instance.Init();
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
    static public bool Whosyourdad = false;
    void Init() 
    {

    }

    public bool IsInBattle = false;
    public BattleStartParams m_battleParams;
    private AudioClip lastAudioClip;
    public int m_BattleMoveSpeed = 15;

    public void StartBattle(BattleStartParams customParams)
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

        //状态初始化
        HSUtilsEx.CallWithDelay(this, () =>
        {
            Debug.Log("-----------HSUtilsEx.CallWithDelay");
            BattleboxHelper.Instance.EnterBattle(_player.View.transform.position);

            //地图上所有单位进入战斗
            foreach (var item in m_battleParams.roles)
            {
                if (item.View.m_Behavior == MapRoleBehavior.Enemy)
                {
                    item.EnterBattle(1);
                }
                else 
                {
                    item.EnterBattle(0);
                }
            }
            m_BattleModel.InitBattleModel();//战场初始化 行动顺序排序这些
            BattleStateMechine.Instance.StartStateMechine(OnBattleEnd);//交给战场状态机接管 状态机完成会回调回来
            //提示UI
            Jyx2_UIManager.Instance.ShowUI("CommonTipsUIPanel", TipsType.MiddleTop, "战斗开始");
            Jyx2_UIManager.Instance.ShowUI("BattleMainUIPanel", BattleMainUIState.ShowHUD);//展示角色血条
        }, 0.5f);

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if(brain != null)
        {
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        }
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
                        if (m_battleParams.backToBigMap)
                            LevelLoader.LoadGameMap("Level_BigMap");
                        m_battleParams = null;
                    });
                    break;
                }
        }
    }
    //清扫战场
    public void EndBattle()
    {
        IsInBattle = false;
        Jyx2_UIManager.Instance.HideUI("BattleMainUIPanel");

        //临时，需要调整
        foreach (var role in m_BattleModel.Roles)
        {
            //role.LeaveBattle();
            //非KeyRole死亡2秒后尸体消失
            if (role.IsDead() && role.View != null && role.View.gameObject!=null && !role.View.m_IsKeyRole)
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
        foreach(var role in alive_teammate)
        {
            int expAdd = battleData.Exp / alive_teammate.Count();
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

    #region 操作相关
    //BY CG：战斗中，对格子的操作，需要穿透touchpad（用于旋转屏幕的）
    #endregion

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

    #region 战斗共有方法
    /// <summary>
    /// 获取技能覆盖范围
    /// </summary>
    /// <returns></returns>
    public List<BattleBlockVector> GetSkillCoverBlocks(BattleZhaoshiInstance skill,BattleBlockVector targetPos, BattleBlockVector selfPos) 
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
    public List<BattleBlockVector> GetSkillUseRange(RoleInstance role,BattleZhaoshiInstance zhaoshi) 
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
    public List<RoleInstance> GetRoleInSkillRange(BattleZhaoshiInstance skill,List<BattleBlockVector> range,int team) 
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
