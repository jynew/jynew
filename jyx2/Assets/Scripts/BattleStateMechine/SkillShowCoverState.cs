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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技能展示范围的状态 这里会显示一个确定取消面板 点确定才会真的释放技能
//如果点取消 那么重新选择技能
public class SkillShowCoverState : IBattleState
{
    RoleInstance role;
    BattleZhaoshiInstance zhaoshi;
    BattleBlockVector skillPos;
    List<GameObject> hitGoList = new List<GameObject>();//当前技能可以打到的角色
    public override void OnEnterState()
    {
        role = BattleStateMechine.Instance.CurrentRole;
        zhaoshi = BattleStateMechine.Instance.CurrentZhaoshi;
        skillPos = BattleStateMechine.Instance.CurrentSkillPos;
        if (role == null || zhaoshi == null || skillPos == null) 
        {
            GameUtil.LogError("进入技能展示范围状态失败");
            return;
        }
        var blockList = BattleManager.Instance.GetSkillCoverBlocks(zhaoshi, skillPos,role.Pos);
        BattleboxHelper.Instance.ShowBlocks(blockList);//显示技能范围
        FillBeHitEnemeyGameObject(blockList);
        // 将要受到攻击的敌人描边
        ShowEnemyOutLine();

        BattleBlockData blockData = BattleboxHelper.Instance.GetBlockData(skillPos.X, skillPos.Y);
        //面板需要根据世界坐标算出ui坐标 显示在格子上方 方便点击
        Jyx2_UIManager.Instance.ShowUI(nameof(BattleOKPanel), blockData.WorldPos, new Action(OnConfirm),new Action(OnCancel));
    }

    void FillBeHitEnemeyGameObject(List<BattleBlockVector> rangeList) 
    {
        hitGoList.Clear();
        List<RoleInstance> result = BattleManager.Instance.GetRoleInSkillRange(zhaoshi, rangeList, role.team);
        for (int i = 0; i < result.Count; i++)
        {
            RoleInstance role = result[i];
            hitGoList.Add(role.View.gameObject);
        }
    }

    void ShowEnemyOutLine() 
    {
        Color highColor = zhaoshi.IsCastToEnemy() ? Color.red : Color.green;
        UnityTools.HighLightObjects(hitGoList.ToArray(), highColor);
    }

    void HideEnemyOutLine() 
    {
        UnityTools.DisHighLightObjects(hitGoList.ToArray());
    }

    //确认
    void OnConfirm()
    {
        //使用暗器
        if (zhaoshi is AnqiZhaoshiInstance)
        {
            GameRuntimeData.Instance.AddItem(zhaoshi.Anqi.Id, -1);
            GameUtil.DisplayPopinfo($"{role.Name}使用了{zhaoshi.Anqi.Name}");
        }
        //取消描边 
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.PlayingAction);//释放技能

    }

    //取消
    void OnCancel() 
    {
        //取消描边 
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.SelectSkill);//重新选择技能
    }

    public override void OnLeaveState()
    {
        HideEnemyOutLine();
        role = null;
        zhaoshi = null;
        skillPos = null;
        hitGoList.Clear();
    }
}
