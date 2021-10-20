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
		var color1=m_currentRole.Hurt>20?ColorStringDefine.Hp_hurt_heavy:m_currentRole.Hurt>0?ColorStringDefine.Hp_hurt_light:ColorStringDefine.Default;
		var color2=m_currentRole.Poison>0?ColorStringDefine.Hp_posion:ColorStringDefine.Default;
        DetailText_Text.text = ($"体力 {m_currentRole.Tili}/100\n生命 <color={color1}>{m_currentRole.Hp}</color>/<color={color2}>{m_currentRole.MaxHp}</color>\n内力 {m_currentRole.Mp}/{m_currentRole.MaxMp}");

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


        BattleActionUIPanel panel = FindObjectOfType<BattleActionUIPanel>();
        if (panel != null )
        {
            var role = panel.GetCurrentRole();
            if (role != null && active && role.team == 0)
            {
                panel.OnAutoClicked();
            }
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
