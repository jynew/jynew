using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移动状态
public class MoveState : IBattleState
{
    int m_BattleMoveSpeed = 5;//移动速度

    BattleBlockVector m_targetPos;
    RoleInstance m_role;
    List<Vector3> m_posList;
    public override void OnEnterState()
    {
        m_targetPos = BattleStateMechine.Instance.CurrentToPos;
        m_role = BattleStateMechine.Instance.CurrentRole;
        if (m_role == null || m_targetPos == null)
        {
            GameUtil.LogError("enter move state failed");
            return;
        }
        m_posList = BattleManager.Instance.FindMovePath(m_role, m_targetPos);
        if (m_posList == null || m_posList.Count <= 0)
        {
            OnEndMove();
            return;
        }
        m_moveIndex = 0;
        m_needMove = false;
        m_role.View.Run();//播放奔跑动画
        DoMove();
    }

    void DoMove() 
    {
        if (m_moveIndex >= m_posList.Count) 
        {
            OnEndMove();
            return;
        }
        m_currentTarget = m_posList[m_moveIndex];
        //面向目标
        m_role.View.transform.LookAt(new Vector3(m_currentTarget.x, m_role.View.transform.position.y, m_currentTarget.z));
        m_needMove = true;
    }

    void OnEndMove()
    {
        m_role.View.Idle();//idle动画
        m_role.Pos = m_targetPos;//设置逻辑位置
        var enemy = AIManager.Instance.GetNearestEnemy(m_role);
        if (enemy != null)
        {
            //面向最近的敌人
            m_role.View.LookAtWorldPosInBattle(enemy.View.transform.position);
        }
        if (m_role.isAI)
        {
            //ai不能放招式 休息 下一步
            if (BattleStateMechine.Instance.CurrentZhaoshi == null && BattleStateMechine.Instance.CurrentUseItem == null)
            {
                //m_role.Reset();应该是OnRest
                m_role.OnRest();
                BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.WaitingForNextActiveBattleRole);
                return;
            }
            //ai用道具
            else if(BattleStateMechine.Instance.CurrentUseItem != null)
            {
                BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.UseItem);
                return;
            }
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.PlayingAction);
        }
        else
        {
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.SelectSkill);
        }
    }

    public override void OnLeaveState()
    {
        m_posList = null;
        m_role = null;
        m_targetPos = null;
    }

    int m_moveIndex = -1;
    bool m_needMove = false;
    Vector3 m_currentTarget;
    public override void OnUpdate()
    {
        if (!m_needMove)
            return;
        Vector3 dis = m_currentTarget - m_role.View.transform.position;
        if ((dis).magnitude < 0.1f) 
        {
            m_needMove = false;
            m_moveIndex++;
            DoMove();
            return;
        }
        m_role.View.transform.position += dis.normalized * m_BattleMoveSpeed * Time.deltaTime;
    }
}
