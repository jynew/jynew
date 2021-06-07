using Jyx2;
using Jyx2.Middleware;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class XiakeUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    RoleInstance m_currentRole;
    List<RoleInstance> m_roleList;
    RoleUIItem m_currentShowItem;
    protected override void OnCreate()
    {
        InitTrans();
        BindListener(BackButton_Button, OnBackClick);
        BindListener(ButtonSelectWeapon_Button, OnWeaponClick);
        BindListener(LeaveButton_Button, OnLeaveClick);
        BindListener(ButtonSelectArmor_Button, OnArmorClick);
        BindListener(ButtonSelectBook_Button, OnXiulianClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_currentRole = allParams[0] as RoleInstance;
        if (allParams.Length > 1)
            m_roleList = allParams[1] as List<RoleInstance>;

        RefreshScrollView();
        RefreshCurrent();
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        HSUnityTools.DestroyChildren(RoleParent_RectTransform);
    }

    void RefreshCurrent() 
    {
        if (m_currentRole == null) 
        {
            Debug.LogError("has not current role");
            return;
        }
        Jyx2ResourceHelper.GetRoleHeadSprite(m_currentRole, PreImage_Image);
        NameText_Text.text = m_currentRole.Name;

        InfoText_Text.text = GetInfoText(m_currentRole);
        SkillText_Text.text = GetSkillText(m_currentRole);
        ItemsText_Text.text = GetItemsText(m_currentRole);
    }

    void RefreshScrollView() 
    {
        HSUnityTools.DestroyChildren(RoleParent_RectTransform);
        if (m_roleList == null || m_roleList.Count <= 0)
            return;
        RoleInstance role;
        for (int i = 0; i < m_roleList.Count; i++)
        {
            role = m_roleList[i];
            var item = RoleUIItem.Create();
            item.transform.SetParent(RoleParent_RectTransform);
            item.transform.localScale = Vector3.one;

            Button btn = item.GetComponent<Button>();
            BindListener(btn, () => 
            {
                OnItemClick(item);
            });
            bool isSelect = (m_currentRole == role);
            if (isSelect)
                m_currentShowItem = item;
            item.SetSelect(isSelect);
            item.ShowRole(role);
        }
    }

    void OnItemClick(RoleUIItem item) 
    {
        if (m_currentShowItem != null && m_currentShowItem == item)
            return;
        if (m_currentShowItem)
            m_currentShowItem.SetSelect(false);

        m_currentShowItem = item;
        m_currentShowItem.SetSelect(true);

        m_currentRole = m_currentShowItem.GetShowRole();
        RefreshCurrent();
    }

    string GetInfoText(RoleInstance role)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"等级 {role.Level}");
        sb.AppendLine($"体力 {role.Tili}/{GameConst.MaxTili}");
        sb.AppendLine($"生命 {role.Hp}/{role.MaxHp}");
        sb.AppendLine($"内力 {role.Mp}/{role.MaxMp}");
        sb.AppendLine($"经验 {role.Exp}/{role.GetLevelUpExp()}");
        sb.AppendLine();
        sb.AppendLine($"攻击 {role.Attack}");
        sb.AppendLine($"防御 {role.Defence}");
        sb.AppendLine($"轻功 {role.Qinggong}");
        sb.AppendLine($"医疗 {role.Heal}");
        sb.AppendLine($"解毒 {role.DePoison}");
        sb.AppendLine($"用毒 {role.UsePoison}");
        sb.AppendLine();
        sb.AppendLine($"拳掌 {role.Quanzhang}");
        sb.AppendLine($"御剑 {role.Yujian}");
        sb.AppendLine($"耍刀 {role.Shuadao}");
        sb.AppendLine($"特殊 {role.Qimen}");
        sb.AppendLine($"暗器 {role.Anqi}");
        return sb.ToString();
    }

    string GetSkillText(RoleInstance role)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var skill in role.Wugongs)
        {
            sb.AppendLine(skill.Name + " " + skill.GetLevel());
        }
        return sb.ToString();
    }

    string GetItemsText(RoleInstance role)
    {
        StringBuilder sb = new StringBuilder();
        var weapon = role.GetWeapon();
        sb.AppendLine("武器：" + (weapon == null ? "" : weapon.Name));

        var armor = role.GetArmor();
        sb.AppendLine("防具：" + (armor == null ? "" : armor.Name));

        var xiulianItem = role.GetXiulianItem();
        sb.AppendLine("修炼：" + (xiulianItem == null ? "" : xiulianItem.Name + $"({role.ExpForItem}/{role.GetFinishedExpForItem()})"));

        return sb.ToString();
    }

    void OnBackClick() 
    {
        Jyx2_UIManager.Instance.HideUI("XiakeUIPanel");
    }

	// added handle leave chat logic
	// by eaphone at 2021/6/6
    void OnLeaveClick() 
    {
        if (m_currentRole == null)
            return;
        if (!m_roleList.Contains(m_currentRole))
            return;
        if (m_currentRole.GetJyx2RoleId() == GameRuntimeData.Instance.Player.GetJyx2RoleId()) 
        {
            GameUtil.DisplayPopinfo("主角不能离开队伍");
            return;
        }
		var eventLuaPath=HSFrameWork.ConfigTable.ConfigTable.Get<Jyx2Role>(m_currentRole.GetJyx2RoleId()).Dialogue;
		if(eventLuaPath!=null && eventLuaPath!=""){
			
			Jyx2.LuaExecutor.Execute("jygame/ka"+eventLuaPath,new Action(()=>{
				RefreshView();
			}));
		}
		else{
			GameRuntimeData.Instance.LeaveTeam(m_currentRole.GetJyx2RoleId());
			RefreshView();
		}
    }
	void RefreshView(){
		m_roleList.Remove(m_currentRole);
		m_currentRole = GameRuntimeData.Instance.Player;
		RefreshScrollView();
		RefreshCurrent();
	}

    GameRuntimeData runtime { get { return GameRuntimeData.Instance; } }
    void OnWeaponClick() 
    {
        SelectFromBag(
            (itemId) => 
            { 
                m_currentRole.Weapon = itemId;
                m_currentRole.UseItem(m_currentRole.GetWeapon());
            },
            () =>
            {
                if (m_currentRole.Weapon != -1)
                {
                    runtime.AddItem(m_currentRole.Weapon, 1);
                    m_currentRole.UnequipItem(m_currentRole.GetWeapon());
                    m_currentRole.Weapon = -1;
                }
            },
            (item) => { return item.EquipmentType == 0; });
    }

    void OnArmorClick() 
    {
        SelectFromBag(
            (itemId) => 
            { 
                m_currentRole.Armor = itemId;
                m_currentRole.UseItem(m_currentRole.GetArmor());
            },
            () =>
            {
                if (m_currentRole.Armor != -1)
                {
                    runtime.AddItem(m_currentRole.Armor, 1);
                    m_currentRole.UnequipItem(m_currentRole.GetArmor());
                    m_currentRole.Armor = -1;
                }
            },
            (item) => { return item.EquipmentType == 1; });
    }

    void OnXiulianClick() 
    {
        SelectFromBag(
            (itemId) => 
            { 
                m_currentRole.Xiulianwupin = itemId;
                m_currentRole.UseItem(m_currentRole.GetXiulianItem());
            },
            () =>
            {
                if (m_currentRole.Xiulianwupin != -1)
                {
                    runtime.AddItem(m_currentRole.Xiulianwupin, 1);
                    m_currentRole.UnequipItem(m_currentRole.GetXiulianItem()); // Maybe this shouldn't be unequipped? Still need to change
                    m_currentRole.Xiulianwupin = -1;
                }
            },
            (item) => { return item.ItemType == 2; });
    }

    void SelectFromBag(Action<int> callback, Action unquipCallback, Func<Jyx2Item, bool> filter)
    {
        Jyx2_UIManager.Instance.ShowUI("BagUIPanel",runtime.Items ,new Action<int>((itemId) =>
        {
            if (itemId != -1 && !m_currentRole.CanUseItem(itemId))
            {
                //MessageBox.Create("该角色不满足使用条件", null);
                GameUtil.DisplayPopinfo("该角色不满足使用条件");
                return;
            }

            //卸下现有
            unquipCallback();

            if (itemId != -1)
            {
                unquipCallback();
                runtime.AddItem(itemId, -1);
                callback(itemId);
            }

            RefreshCurrent();
        }), filter);
    }
}
