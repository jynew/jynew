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
using Jyx2.Middleware;
using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleActionUIPanel:Jyx2_UIBase
{
    RoleInstance m_currentRole;
    BattleManager.BattleViewStates m_currentState;
    SkillUIItem m_selectItem;
    bool m_chooseBtn = false;
    List<SkillUIItem> m_curItemList = new List<SkillUIItem>();
    ChildGoComponent childMgr;
    protected override void OnCreate()
    {
        InitTrans();
        childMgr = GameUtil.GetOrAddComponent<ChildGoComponent>(Skills_RectTransform);
        childMgr.Init(SkillItem_RectTransform);

        BindListener(Move_Button, OnMoveClick);
        BindListener(UsePoison_Button, OnUsePoisonClick);
        BindListener(Depoison_Button, OnDepoisonClick);
        BindListener(Heal_Button, OnHealClick);
        BindListener(Item_Button, OnUseItemClick);
        BindListener(Wait_Button, OnWaitClick);
        BindListener(Rest_Button, OnRestClick);
        BindListener(Cancel_Button, OnCancelClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_currentRole = allParams[0] as RoleInstance;
        if (m_currentRole == null)
            return;
        if (allParams.Length > 1)
            m_currentState = (BattleManager.BattleViewStates)allParams[1];
        Cancel_Button.gameObject.SetActive(false);
        SetActionBtnState();
        RefreshSkill();
        SetPanelState();
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_currentState = BattleManager.BattleViewStates.None;
        m_currentRole = null;
        m_selectItem = null;
        m_curItemList.Clear();
    }

    void SetActionBtnState() 
    {
        bool canPoison = m_currentRole.UsePoison > 0 && m_currentRole.Tili >= 30;
        UsePoison_Button.gameObject.SetActive(canPoison);
        bool canDepoison = m_currentRole.DePoison > 0 && m_currentRole.Tili >= 30;
        Depoison_Button.gameObject.SetActive(canDepoison);
        bool canHeal = m_currentRole.Heal > 0 && m_currentRole.Tili >= 10;
        Heal_Button.gameObject.SetActive(canHeal);

        bool lastRole = BattleManager.Instance.GetModel().IsLastRole(m_currentRole);
        Wait_Button.gameObject.SetActive(!lastRole);

        Cancel_Button.gameObject.SetActive(m_currentState == BattleManager.BattleViewStates.SelectMove
            || m_currentState == BattleManager.BattleViewStates.SelectSkill);
    }

    void RefreshSkill()
    {
        m_curItemList.Clear();
        var zhaoshis = m_currentRole.GetZhaoshis(true).ToList();
        childMgr.RefreshChildCount(zhaoshis.Count);
        List<Transform> childTransList = childMgr.GetUsingTransList();
        for (int i = 0; i < zhaoshis.Count; i++)
        {
            SkillUIItem item = GameUtil.GetOrAddComponent<SkillUIItem>(childTransList[i]);
            item.RefreshSkill(zhaoshis[i]);
            item.SetSelect(m_selectItem == item);

            Button btn = item.GetComponent<Button>();
            BindListener(btn, () => 
            {
                OnItemClick(item);
            });
            m_curItemList.Add(item);
        }
    }


    void SetPanelState() 
    {
        if (m_currentState == BattleManager.BattleViewStates.SelectMove)
        {
            m_selectItem = null;
            m_chooseBtn = false;
        } else if (m_currentState == BattleManager.BattleViewStates.SelectSkill) 
        {
            if (BattleStateMechine.Instance.CurrentZhaoshi == null) 
            {
                if (m_curItemList.Count > 0)
                {
                    m_selectItem = m_curItemList[0];
                    BattleStateMechine.Instance.BindSkill(m_selectItem.GetSkill());
                }
                m_chooseBtn = false;
                UpdateSelect();
                return;
            }
            for (int i = 0; i < m_curItemList.Count; i++)
            {
                if (m_curItemList[i].GetSkill().Key == BattleStateMechine.Instance.CurrentZhaoshi.Key) 
                {
                    m_selectItem = m_curItemList[i];
                    break;
                }
            }
            m_chooseBtn = (m_selectItem == null);
            if (m_chooseBtn && m_curItemList.Count > 0)
                m_selectItem = m_curItemList[0];
        }
        UpdateSelect();
    }

    void UpdateSelect() 
    {
        LeftActions_RectTransform.gameObject.SetActive(!m_chooseBtn);
        Skills_RectTransform.gameObject.SetActive(!m_chooseBtn);
        if (m_chooseBtn)
            return;
        if(m_selectItem)
            m_selectItem.SetSelect(true);
    }

    void OnItemClick(SkillUIItem item) 
    {
        if (m_selectItem == item)
            return;
        if (m_selectItem != null)
            m_selectItem.SetSelect(false);
        m_selectItem = item;
        m_chooseBtn = false;
        BattleStateMechine.Instance.BindSkill(m_selectItem.GetSkill());

        m_currentRole.SwitchAnimationToSkill(m_selectItem.GetSkill().Data);
        CheckNeedChangeState();
    }

    void OnCancelClick() 
    {
        if (m_currentState == BattleManager.BattleViewStates.SelectMove)
        {
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.SelectMove);
            Debug.Log("cancel");
        }
        else if (m_currentState == BattleManager.BattleViewStates.SelectSkill) 
        {
            if (m_chooseBtn)
            {
                m_chooseBtn = false;
                BattleStateMechine.Instance.BindSkill(m_selectItem.GetSkill());
                UpdateSelect();
                Debug.Log("cm");
            }
            else
            {
                BattleStateMechine.Instance.BindSkill(null);
                BattleStateMechine.Instance.IsCanceling = true;
                BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.SelectMove);
                BattleStateMechine.Instance.ResetRolePos();//让角色回到原来位置
            }
        }
    }

    void OnMoveClick() 
    {

    }

    void CheckNeedChangeState() 
    {
        if (m_currentState == BattleManager.BattleViewStates.SelectMove)
        {
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.SelectSkill);
        }
        else 
        {
            UpdateSelect();
        }
    }

    void OnUsePoisonClick() 
    {
        var zhaoshi = new PoisonZhaoshiInstance(m_currentRole.UsePoison);
        m_chooseBtn = true;
        BattleStateMechine.Instance.BindSkill(zhaoshi);
        CheckNeedChangeState();
    }

    void OnDepoisonClick() 
    {
        var zhaoshi = new DePoisonZhaoshiInstance(m_currentRole.DePoison);
        m_chooseBtn = true;
        BattleStateMechine.Instance.BindSkill(zhaoshi);
        CheckNeedChangeState();
    }

    void OnHealClick() 
    {
        var zhaoshi = new HealZhaoshiInstance(m_currentRole.Heal);
        m_chooseBtn = true;
        BattleStateMechine.Instance.BindSkill(zhaoshi);
        CheckNeedChangeState();
    }

    void OnUseItemClick()
    {
        bool Filter(Jyx2Item item) => item.ItemType == 3 || item.ItemType == 4;

        Jyx2_UIManager.Instance.ShowUI(nameof(BagUIPanel), GameRuntimeData.Instance.Items, new Action<int>((itemId) =>
        {

            if (itemId == -1)
                return;

            var item = ConfigTable.Get<Jyx2Item>(itemId);
            if (item.ItemType == 3) //使用道具逻辑
            {
                if (m_currentRole.CanUseItem(itemId))
                {
                    BattleStateMechine.Instance.BindItem(item);
                    BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.UseItem);
                }
            }
            else if (item.ItemType == 4) //使用暗器逻辑
            {
                var zhaoshi = new AnqiZhaoshiInstance(m_currentRole.Anqi, item);
                m_chooseBtn = true;
                BattleStateMechine.Instance.BindSkill(zhaoshi);
                CheckNeedChangeState();
            }

        }), (Func<Jyx2Item, bool>) Filter);
    }

    void OnWaitClick() 
    {
        BattleManager.Instance.GetModel().ActWait(m_currentRole);
        BattleStateMechine.Instance.ResetRolePos();
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.WaitingForNextActiveBattleRole);
    }

    void OnRestClick() 
    {
        m_currentRole.OnRest();
        BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.WaitingForNextActiveBattleRole);
    }
}
