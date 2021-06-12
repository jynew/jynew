using Jyx2;
using HSFrameWork.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// buff结算状态 主要是每次行动之前 结算上次的中毒效果
/// </summary>
public class BuffSettlementState : IBattleState
{
    RoleInstance currentRole;
    float castAnimLenght = 0.5f;
    public override void OnEnterState()
    {
        currentRole = BattleStateMechine.Instance.CurrentRole;
        if (currentRole.Poison <= 0) 
        {
            OnSettleOver();//没有中毒
            return;
        }
        if (currentRole.View)
        {
            var animator = currentRole.View.GetAnimator();
            animator.SetTrigger("hit");
        }
        PoisonEffect();
    }

    void PoisonEffect() 
    {
        int tmp = currentRole.Hp;
        currentRole.Poison -= currentRole.AntiPoison;
        currentRole.Poison = Tools.Limit(currentRole.Poison, 0, 100);
        currentRole.Hp -= currentRole.Poison / 3;
        if (currentRole.Hp < 1)
            currentRole.Hp = 1;

        currentRole.View?.MarkHpBarIsDirty();

        int effectRst = tmp - currentRole.Hp;
        currentRole.View.ShowAttackInfo($"<color=green>毒发-{effectRst}</color>");
        Observable.TimerFrame(Convert.ToInt32(castAnimLenght * 60), FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                OnSettleOver();
            });
    }

    void OnSettleOver() 
    {
        if (currentRole.isAI)
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.AI);
        else
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.SelectMove);
    }

    public override void OnLeaveState()
    {
        currentRole = null;
    }
}
