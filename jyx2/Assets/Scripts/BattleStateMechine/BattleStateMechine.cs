/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using HanSquirrel.ResourceManager;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景管理状态机 同一时间只控制一个角色行为 类似opengl状态机，控制其他角色需要先绑定
public class BattleStateMechine:MonoBehaviour
{
    static BattleStateMechine _instance;
    public static BattleStateMechine Instance 
    {
        get 
        {
            if (_instance == null) 
            {
                GameObject go = new GameObject("BattleStateMechine");
                go.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(go);
                _instance = GameUtil.GetOrAddComponent<BattleStateMechine>(go.transform);
                _instance.Init();
            }
            return _instance;
        }
    }

    private Dictionary<BattleManager.BattleViewStates, IBattleState> m_stateDic;
    
    private BattleManager.BattleViewStates m_currentState = BattleManager.BattleViewStates.None;

    //当前状态机控制的角色
    public RoleInstance CurrentRole { get; private set; }
    //当前角色使用的招式
    public BattleZhaoshiInstance CurrentZhaoshi { get; private set; }
    public BattleBlockVector CurrentSkillPos { get; private set; }
    //要移动到的点
    public BattleBlockVector CurrentToPos { get; set; }
    //当次操作可以一定的点的列表
    public List<BattleBlockVector> CurrentMoveList { get;private set; }
    //当前使用的物品
    public Jyx2Item CurrentUseItem { get; private set; }
    //标记是否按了取消按钮
    public bool IsCanceling { get; set; }

    void Init() 
    {
        m_stateDic = new Dictionary<BattleManager.BattleViewStates, IBattleState>();

        //等待下一个角色
        m_stateDic[BattleManager.BattleViewStates.WaitingForNextActiveBattleRole] = new WaitForNextBattleRoleState();
        //ai的决策状态 仅ai
        m_stateDic[BattleManager.BattleViewStates.AI] = new AIState();
        //角色选择移动目标状态 仅角色
        m_stateDic[BattleManager.BattleViewStates.SelectMove] = new SelectMoveState();
        //移动状态 ai也会有
        m_stateDic[BattleManager.BattleViewStates.Move] = new MoveState();
        //释放技能状态 ai 角色
        m_stateDic[BattleManager.BattleViewStates.PlayingAction] = new PlayingActionState();
        //选择技能状态 仅角色
        m_stateDic[BattleManager.BattleViewStates.SelectSkill] = new SelectSkillState();
        //技能范围显示 技能确认状态 仅角色
        m_stateDic[BattleManager.BattleViewStates.PreshowSkillCoverRange] = new SkillShowCoverState();
        //使用物品状态 使用物品后 播放使用物品动画 加血
        m_stateDic[BattleManager.BattleViewStates.UseItem] = new UseItemState();
        //毒 结算
        m_stateDic[BattleManager.BattleViewStates.BuffSettlement] = new BuffSettlementState();
    }

    IBattleState GetState(BattleManager.BattleViewStates state) 
    {
        IBattleState obj;
        if (m_stateDic.TryGetValue(state, out obj))
            return obj;
        return null;
    }

    public BattleManager.BattleViewStates GetCurrentState() 
    {
        return m_currentState;
    }

    private BattleBlockVector m_oriPos;
    private GameObject m_roleFocusRing;
    private Action<BattleResult> m_callback;
    //绑定一个角色
    public void BindRole(RoleInstance role) 
    {
        if (role == null)
        {
            CurrentRole = null;
            CurrentMoveList = null;
            return;
        }
        CurrentRole = role;
        m_oriPos = role.Pos;//预存当前位置
        m_roleFocusRing.transform.SetParent(role.View.transform, false);//选中框 跟随目标
        m_roleFocusRing.transform.localPosition = new Vector3(0, 0.15f, 0); //略微比地面高一点
        CameraHelper.Instance.ChangeFollow(role.View.transform); //切换摄像机跟随角色
        Jyx2_UIManager.Instance.ShowUI(nameof(BattleMainUIPanel), BattleMainUIState.ShowRole, role);//展示UI头像

        //记录本次 可以移动的点的集合 除非是有技能可以二次移动 否则本次移动范围不可更改
        CurrentMoveList = BattleManager.Instance.GetMoveRange(role);
    }

    //绑定当前使用招式 一次绑定技能和技能释放位置 ai使用
    public void BindBattleAction(BattleZhaoshiInstance zhaoshi, BattleBlockVector data = null) 
    {
        CurrentZhaoshi = zhaoshi;
        CurrentSkillPos = data;
    }

    //只绑定招式 玩家指挥使用
    public void BindSkill(BattleZhaoshiInstance zhaoshi) 
    {
        CurrentZhaoshi = zhaoshi;
    }
    //只绑定 招式位置 玩家指挥使用
    public void BindSkillPos(BattleBlockVector data) 
    {
        CurrentSkillPos = data;
    }

    //绑定使用的物品数据
    public void BindItem(Jyx2Item useItem) 
    {
        CurrentUseItem = useItem;
    }
    /// <summary>
    /// 还原角色的位置
    /// </summary>
    public void ResetRolePos() 
    {
        if (CurrentRole == null)
            return;

        CurrentRole.Pos = m_oriPos;
    }

    public void StartStateMechine(Action<BattleResult> cb) 
    {
        if (m_roleFocusRing == null)
        {
            m_roleFocusRing = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/CurrentBattleRoleTag.prefab");
        }
        m_callback = cb;
        SwitchState(BattleManager.BattleViewStates.WaitingForNextActiveBattleRole);//选择下一个 角色
    }

    public void SwitchState(BattleManager.BattleViewStates toState) 
    {
        if (toState == m_currentState)
        {
            IBattleState temp = GetState(toState);
            temp.RefreshState();//说明是刷新状态
            return;
        }
        IBattleState toObj = GetState(toState);
        if (toObj == null) 
        {
            Debug.LogError("不存在状态" + toState.ToString() + ",无法完成转换");
            return;
        }
        IBattleState curState = GetState(m_currentState);
        if (curState != null) 
        {
            curState.OnLeaveState();
        }
        m_currentState = toState;
        toObj.OnEnterState();
    }

    private IBattleState currentBattleState;
    private void Update()
    {
        currentBattleState = GetState(m_currentState);
        if (currentBattleState != null) 
        {
            currentBattleState.OnUpdate();
        }
    }

    public void StopStateMechine(BattleResult result)
    {
        //清理所有格子
        BattleboxHelper.Instance.ClearAllBlocks();
        //相机照射主角
		if(CurrentRole!=null && CurrentRole.View!=null)
		{
			CameraHelper.Instance.ChangeFollow(CurrentRole.View.transform);
		}
        //Jyx2_UIManager.Instance.HideUI("BattleActionOrderPanel");
        IBattleState curState = GetState(m_currentState);
        if (curState != null)
        {
            curState.OnLeaveState();
        }
        if (m_roleFocusRing != null)
        {
            Jyx2ResourceHelper.ReleasePrefabInstance(m_roleFocusRing);
            m_roleFocusRing = null;
        }
        m_currentState = BattleManager.BattleViewStates.None;
        CurrentRole = null;
        m_callback?.Invoke(result);
    }
}
