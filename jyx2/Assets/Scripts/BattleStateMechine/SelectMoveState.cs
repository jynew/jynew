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

public class SelectMoveState : IBattleState
{
    RoleInstance role;
    BattleFieldModel battleModel;
    public override void OnEnterState()
    {
        role = BattleStateMechine.Instance.CurrentRole;
        battleModel = BattleManager.Instance.GetModel();

        //显示移动范围
        BattleboxHelper.Instance.ShowBlocks(BattleStateMechine.Instance.CurrentMoveList);
        //显示技能动作面板
        Jyx2_UIManager.Instance.ShowUI(nameof(BattleActionUIPanel),role,BattleManager.BattleViewStates.SelectMove);
    }

    public override void RefreshState()
    {
        base.RefreshState();
        BattleStateMechine.Instance.ResetRolePos();//还原到初始位置
    }

    public override void OnLeaveState()
    {
        role = null;
        BattleboxHelper.Instance.HideAllBlocks();
        Jyx2_UIManager.Instance.HideUI(nameof(BattleActionUIPanel));
    }

    void OnEndSelect(BattleBlockData data) 
    {
        if (data == null)
            return;

        //隐藏所有格子
        BattleboxHelper.Instance.HideAllBlocks();
        BattleStateMechine.Instance.CurrentToPos = data.BattlePos;
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.Move);
        Debug.Log("moved");
        Jyx2_UIManager.Instance.HideUI(nameof(BattleActionUIPanel));
    }

    public override void OnUpdate()
    {
        var block = InputManager.Instance.GetMouseUpBattleBlock();
        if (block == null) return;

        if (block.gameObject.activeSelf == false) return;
        //站人了
        if (battleModel.BlockHasRole(block.BattlePos.X, block.BattlePos.Y)) return;

        //判断是否按了取消按钮，防止ui消失过快，鼠标点到格子触发移动的问题
        if (BattleStateMechine.Instance.IsCanceling == true)
        {
            BattleStateMechine.Instance.IsCanceling = false;
            return;
        }

        OnEndSelect(block);
    }
}
