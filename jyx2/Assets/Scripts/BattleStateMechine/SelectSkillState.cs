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

//选择技能状态
public class SelectSkillState : IBattleState
{
    BattleZhaoshiInstance zhaoshi;
    RoleInstance role;
    public override void OnEnterState()
    {
        role = BattleStateMechine.Instance.CurrentRole;
        zhaoshi = BattleStateMechine.Instance.CurrentZhaoshi;

        Jyx2_UIManager.Instance.ShowUI(nameof(BattleActionUIPanel), role, BattleManager.BattleViewStates.SelectSkill);
        ShowZhaoshi();
    }

    void ShowZhaoshi() 
    {
        if (zhaoshi == null)
            return;
        var blockList = BattleManager.Instance.GetSkillUseRange(BattleStateMechine.Instance.CurrentRole, zhaoshi);
        BattleboxHelper.Instance.ShowBlocks(blockList, BattleBlockType.AttackZone);
    }

    public override void OnLeaveState()
    {
        zhaoshi = null;
        role = null;
        Jyx2_UIManager.Instance.HideUI(nameof(BattleActionUIPanel));
        BattleboxHelper.Instance.HideAllBlocks();
    }

    void OnSelectPos(BattleBlockData data) 
    {
        //朝向
        role.View.LookAtBattleBlock(data);
        //绑定技能释放位置
        BattleStateMechine.Instance.BindSkillPos(data.BattlePos);
        //切换到技能预展示状态
        //BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.PreshowSkillCoverRange);
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.PlayingAction);//释放技能
    }

    public override void OnUpdate()
    {
        //如果选择招式改变 重新显示选择区域
        if (BattleStateMechine.Instance.CurrentZhaoshi != zhaoshi) 
        {
            zhaoshi = BattleStateMechine.Instance.CurrentZhaoshi;
            ShowZhaoshi();
        }
        if (zhaoshi == null)
            return;
        var block = InputManager.Instance.GetMouseDownBattleBlock();
        if (block == null) return;

        //如果无目标则不使用暗器
        if (zhaoshi is AnqiZhaoshiInstance)
        {
            if (!BattleManager.Instance.GetModel().BlockHasRole(block.BattlePos.X, block.BattlePos.Y))
            {
                GameUtil.DisplayPopinfo($"暗器需指定目标");
                return;
            }
        }
        OnSelectPos(block);
    }
}
