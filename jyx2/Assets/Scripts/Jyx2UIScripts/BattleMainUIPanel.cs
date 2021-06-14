using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleMainUIState 
{
    None = 0,
    ShowRole = 1,//显示角色
    ShowHUD = 2,//显示血条
}

public partial class BattleMainUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.MainUI;

    ChildGoComponent childMgr;
    RoleInstance m_currentRole;
    protected override void OnCreate()
    {
        InitTrans();
        childMgr = GameUtil.GetOrAddComponent<ChildGoComponent>(BattleHpRoot_RectTransform);
        childMgr.Init(HUDItem_RectTransform, OnHUDCreate);

        AutoBattle_Toggle.isOn = false;//默认取消
        AutoBattle_Toggle.gameObject.SetActive(false);
        AutoBattle_Toggle.onValueChanged.AddListener(OnAutoBattleValueChange);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
		
		if(childMgr==null){
			childMgr = GameUtil.GetOrAddComponent<ChildGoComponent>(BattleHpRoot_RectTransform);
			childMgr.Init(HUDItem_RectTransform, OnHUDCreate);
		}
        BattleMainUIState state = (BattleMainUIState)allParams[0];
        if (state == BattleMainUIState.ShowRole)
        {
            m_currentRole = allParams[1] as RoleInstance;
            ShowRole();
        }
        else if (state == BattleMainUIState.ShowHUD)
        {
            ShowHUDSlider();
        }else
            ShowRole();
    }

    void ShowRole() 
    {
        if (m_currentRole == null)
        {
            CurrentRole_RectTransform.gameObject.SetActive(false);
            AutoBattle_Toggle.gameObject.SetActive(false);
            return;
        }
        AutoBattle_Toggle.gameObject.SetActive(true);
        CurrentRole_RectTransform.gameObject.SetActive(true);
        NameText_Text.text = m_currentRole.Name;
        DetailText_Text.text = string.Format("体力 {0}/100\n生命 {1}/{2}\n内力 {3}/{4}", m_currentRole.Tili, m_currentRole.Hp, m_currentRole.MaxHp, m_currentRole.Mp, m_currentRole.MaxMp);

        Jyx2ResourceHelper.GetRoleHeadSprite(m_currentRole, PreImage_Image);
    }

    void OnAutoBattleValueChange(bool active) 
    {
        var battleModel = BattleManager.Instance.GetModel();
        if (battleModel == null)
            return;
        foreach (var role in battleModel.Teammates)
        {
            role.isAI = active;
        }

        var curRole = BattleStateMechine.Instance.CurrentRole;
        if(active && curRole != null && curRole.team == 0)
        {
            BattleStateMechine.Instance.SwitchState(BattleManager.BattleViewStates.AI);
        }         
    }

    void OnHUDCreate(Transform hudTrans) 
    {
        HUDItem item = GameUtil.GetOrAddComponent<HUDItem>(hudTrans);
        item.Init();
    }

    //显示血条
    void ShowHUDSlider() 
    {
        List<RoleInstance> roles = BattleManager.Instance.GetModel().AliveRoles;
        childMgr.RefreshChildCount(roles.Count);
        List<Transform> childTrans = childMgr.GetUsingTransList();
        for (int i = 0; i < childTrans.Count; i++)
        {
            HUDItem item = GameUtil.GetOrAddComponent<HUDItem>(childTrans[i]);
            RoleInstance role = roles[i];
            if (role == null)
                continue;
            item.BindRole(role);
        }
    }
	
	protected override void OnHidePanel()
    {
        base.OnHidePanel();
        AutoBattle_Toggle.isOn = false;
        AutoBattle_Toggle.gameObject.SetActive(false);
		childMgr=null;
		m_currentRole=null;
	}
}
