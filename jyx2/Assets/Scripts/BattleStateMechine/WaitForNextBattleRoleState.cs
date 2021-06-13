using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Jyx2.BattleFieldModel;

public class WaitForNextBattleRoleState : IBattleState
{
    BattleFieldModel model;
    public override void OnEnterState()
    {
        model = BattleManager.Instance.GetModel();
        if (BattleStateMechine.Instance.CurrentRole != null)
        {
            BattleStateMechine.Instance.CurrentRole.View.Idle();//上一个播放Idle动画
            model.OnActioned(BattleStateMechine.Instance.CurrentRole);
        }
        BattleStateMechine.Instance.BindRole(null);//先清除绑定
        if (model == null) 
        {
            GameUtil.LogError ("状态错误,BattleFieldModel数据不存在");
            return;
        }
        var result = model.GetBattleResult();
        if (result != BattleResult.InProgress) 
        {
            BattleStateMechine.Instance.StopStateMechine(result);
            return;
        }
    }

    public override void OnLeaveState()
    {
        model = null;
    }

    int maxShowCount = 8;//最大显示行动条人数
    //显示行动条
    void ShowActOrder() 
    {
        List<RoleInstance> roles = model.Roles;
        List<RoleInstance> aliveRoles = new List<RoleInstance>();
        int curCount = 0;
        for (int i = 0; i < roles.Count; i++)
        {
            RoleInstance role = roles[i];
            if (role.IsDead())//死亡的排除掉
                continue;
            aliveRoles.Add(role);
            curCount++;
            if (curCount >= maxShowCount)
                break;
        }
        //Jyx2_UIManager.Instance.ShowUI("BattleActionOrderPanel", aliveRoles);
    }

    public override void OnUpdate()
    {
        if (model == null)
            return;
        RoleInstance role = model.GetNextActiveRole();
        if (role!=null) 
        {
            ShowActOrder();
            BattleStateMechine.Instance.BindRole(role);
            //每次开始 先结算上次的buff效果
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.BuffSettlement);
        }
    }
}
