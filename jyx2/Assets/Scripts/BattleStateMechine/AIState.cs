/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AI计算状态 专门给AI使用
public class AIState : IBattleState
{
    AIResult m_aiResult;
    RoleInstance m_currentRole;
    public override void OnEnterState()
    {
        BattleboxHelper.Instance.HideAllBlocks();
        BattleStateMechine.Instance.BindBattleAction(null);

        m_currentRole = BattleStateMechine.Instance.CurrentRole;
        if (m_currentRole == null) 
        {
            GameUtil.LogError("ai当前角色为空");
            return;
        }
        AIManager.Instance.GetAIResult(BattleStateMechine.Instance.CurrentRole, (result) => 
        {
            m_aiResult = result;
            BattleBlockVector MoveTo = new BattleBlockVector(result.MoveX, result.MoveY);
            BattleBlockVector SkillTo = new BattleBlockVector(result.AttackX, result.AttackY);
            BattleZhaoshiInstance Skill = result.Zhaoshi;
            Jyx2Item Item = result.Item;
            DoAIAction(MoveTo, SkillTo, Skill, Item);

        }); 
    }

    void DoAIAction(BattleBlockVector MoveTo, BattleBlockVector SkillTo,BattleZhaoshiInstance Skill, Jyx2Item Item)
    {
        //将ai行为储存
        BattleStateMechine.Instance.BindBattleAction(Skill, SkillTo);
        BattleStateMechine.Instance.BindItem(Item);
        if (MoveTo != null)
        {
            BattleStateMechine.Instance.CurrentToPos = MoveTo;
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.Move);
        }
        else
        {
            m_currentRole.View.Idle();
            m_currentRole.OnRest();
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.WaitingForNextActiveBattleRole);
        }
    }

    public override void OnLeaveState()
    {
        m_aiResult = null;
        m_currentRole = null;
    }
}
