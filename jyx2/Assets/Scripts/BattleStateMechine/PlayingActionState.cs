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
using HSFrameWork.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSFrameWork.ConfigTable;
using System;

public class PlayingActionState : IBattleState
{
    RoleInstance m_role;
    BattleZhaoshiInstance m_skill;
    BattleBlockVector m_skillPos;
    public override void OnEnterState()
    {
        m_role = BattleStateMechine.Instance.CurrentRole;
        m_skill = BattleStateMechine.Instance.CurrentZhaoshi;
        m_skillPos = BattleStateMechine.Instance.CurrentSkillPos;
        if (m_role == null || m_skill == null || m_skillPos == null) 
        {
            GameUtil.LogError("进入PlayingAction状态失败");
            return;
        }
        var block = BattleboxHelper.Instance.GetBlockData(m_skillPos.X, m_skillPos.Y);
        m_role.View.LookAtBattleBlock(block);//先面向目标
        attackTwiceCounter = 0;
        CastSkill();
    }

    int attackTwiceCounter = 0;
    void CastSkill(bool twiceAttack = false) 
    {
        m_role.SwitchAnimationToSkill(m_skill.Data);

        //左右互播，只扣除一次体力和内力等
        if (!twiceAttack)
        {
            m_skill.CastCD();
            m_skill.CastCost(m_role);    
        }

        List<RoleInstance> beHitAnimationList = new List<RoleInstance>();
        //获取攻击范围
        var coverBlocks = BattleManager.Instance.GetSkillCoverBlocks(m_skill, m_skillPos, m_role.Pos);
        //处理掉血
        foreach (var blockVector in coverBlocks)
        {
            var rolei = BattleManager.Instance.GetModel().GetAliveRole(blockVector);
            //还活着
            if (rolei == null || rolei.IsDead()) continue;
            //打敌人的招式
            if (m_skill.IsCastToEnemy() && rolei.team == m_role.team) continue;
            //“打”自己人的招式
            if (!m_skill.IsCastToEnemy() && rolei.team != m_role.team) continue;

            var result = AIManager.Instance.GetSkillResult(m_role, rolei, m_skill, blockVector);

            result.Run();

			//当需要播放受攻击动画时，不直接刷新血条，延后到播放受攻击动画时再刷新。其他情况直接刷新血条。
            if (result.IsDamage())
            {
                //加入到受击动作List
                beHitAnimationList.Add(rolei);
            }
            else
            {
                rolei.View.MarkHpBarIsDirty();
            }
        }

        SkillCastHelper castHelper = new SkillCastHelper
        {
            Source = m_role.View,
            CoverBlocks = coverBlocks.ToTransforms(),
            Zhaoshi = m_skill,
            Targets = beHitAnimationList.ToMapRoles(),
        };
		
		
        if (attackTwiceCounter < m_role.Zuoyouhubo && m_skill.Data.GetSkill().DamageType==0 && BattleManager.Instance.GetModel().GetBattleResult()==BattleResult.InProgress)//非战斗技能不适用左右互博
        {
            castHelper.Play(() => {
                attackTwiceCounter ++;
                CastSkill(true);
            });
        }
        else
        {
            castHelper.Play(()=> {

                OnCaskSkillOver();
            });
        }
		
    }


    //技能释放结束
    void OnCaskSkillOver() 
    {
        BattleStateMechine.Instance.BindBattleAction(null);
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.WaitingForNextActiveBattleRole);
    }

    public override void OnLeaveState()
    {
        m_role = null;
        m_skill = null;
        m_skillPos = null;
    }
}
