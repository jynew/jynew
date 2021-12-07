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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using Jyx2Configs;

public partial class XiakeUIPanel : Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    RoleInstance m_currentRole;
    List<RoleInstance> m_roleList;
    RoleUIItem m_currentShowItem;

    protected override void OnCreate()
    {
        InitTrans();
        IsBlockControl = true;
        BindListener(BackButton_Button, OnBackClick);
        BindListener(ButtonSelectWeapon_Button, OnWeaponClick);
        BindListener(LeaveButton_Button, OnLeaveClick);
        BindListener(ButtonSelectArmor_Button, OnArmorClick);
        BindListener(ButtonSelectBook_Button, OnXiulianClick);
        BindListener(ButtonHeal_Button, OnHealClick);
        BindListener(ButtonDetoxicate_Button, OnDetoxicateClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_currentRole = allParams[0] as RoleInstance;
        if (allParams.Length > 1)
            m_roleList = allParams[1] as List<RoleInstance>;

        /*var curMap=GameRuntimeData.Instance.CurrentMap;
        (LeaveButton_Button.gameObject).SetActive("0_BigMap"==curMap);*/
        DoRefresh();
    }

    void DoRefresh()
    {
        RefreshScrollView();
        RefreshCurrent();
    }
    
    private void OnEnable()
    {
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, OnBackClick);
    }

    private void OnDisable()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
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

        NameText_Text.text = m_currentRole.Name;

        InfoText_Text.text = GetInfoText(m_currentRole);
        SkillText_Text.text = GetSkillText(m_currentRole);
        ItemsText_Text.text = GetItemsText(m_currentRole);

        bool canDepoison = m_currentRole.DePoison > 20 && m_currentRole.Tili >= 10;
        ButtonDetoxicate_Button.gameObject.SetActive(canDepoison);
        bool canHeal = m_currentRole.Heal > 20 && m_currentRole.Tili >= 50;
        ButtonHeal_Button.gameObject.SetActive(canHeal);
        
        PreImage_Image.LoadAsyncForget(m_currentRole.Data.GetPic());
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
            BindListener(btn, () => { OnItemClick(item); });
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
        var color = role.GetMPColor();
        var color1 = role.GetHPColor1();
        var color2 = role.GetHPColor2();
        sb.AppendLine($"等级 {role.Level}");
        sb.AppendLine($"体力 {role.Tili}/{GameConst.MAX_ROLE_TILI}");
        sb.AppendLine($"生命 <color={color1}>{role.Hp}</color>/<color={color2}>{role.MaxHp}</color>");
        sb.AppendLine($"内力 <color={color}>{role.Mp}/{role.MaxMp}</color>");
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
        sb.AppendLine("修炼：" + (xiulianItem == null
            ? ""
            : xiulianItem.Name + $"({role.ExpForItem}/{role.GetFinishedExpForItem()})"));

        return sb.ToString();
    }

    void OnBackClick()
    {
        Jyx2_UIManager.Instance.HideUI(nameof(XiakeUIPanel));
    }

    // added handle leave chat logic
    // by eaphone at 2021/6/6
    void OnLeaveClick()
    {

        var curMap = LevelMaster.GetCurrentGameMap();
        if (!curMap.IsWorldMap())
        {
            GameUtil.DisplayPopinfo("必须在大地图才可以角色离队");
            return;
        }
 
        if (m_currentRole == null)
            return;
        if (!m_roleList.Contains(m_currentRole))
            return;
        if (m_currentRole.GetJyx2RoleId() == GameRuntimeData.Instance.Player.GetJyx2RoleId())
        {
            GameUtil.DisplayPopinfo("主角不能离开队伍");
            return;
        }

        var eventLuaPath = GameConfigDatabase.Instance.Get<Jyx2ConfigCharacter>(m_currentRole.GetJyx2RoleId()).LeaveStoryId;
        if (!string.IsNullOrEmpty(eventLuaPath))
        {
            Jyx2.LuaExecutor.Execute("jygame/ka" + eventLuaPath, RefreshView);
        }
        else
        {
            GameRuntimeData.Instance.LeaveTeam(m_currentRole.GetJyx2RoleId());
            RefreshView();
        }
    }

    void RefreshView()
    {
        m_roleList.Remove(m_currentRole);
        m_currentRole = GameRuntimeData.Instance.Player;
        DoRefresh();
    }

    GameRuntimeData runtime
    {
        get { return GameRuntimeData.Instance; }
    }

    void OnWeaponClick()
    {
        SelectFromBag(
            (itemId) =>
            {
                var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
                
                //选择了当前使用的装备，则卸下
                if (m_currentRole.Weapon == itemId)
                {
                    m_currentRole.UnequipItem(m_currentRole.GetWeapon());
                    m_currentRole.Weapon = -1;
                }
                //否则更新
                else
                {
                    m_currentRole.UnequipItem(m_currentRole.GetWeapon());
                    m_currentRole.Weapon = itemId;
                    m_currentRole.UseItem(m_currentRole.GetWeapon());
                    runtime.SetItemUser(item.Id, m_currentRole.GetJyx2RoleId());
                }
            },
            (item) => { return item.EquipmentType == 0 && (runtime.GetItemUser(item.Id) == m_currentRole.GetJyx2RoleId() || runtime.GetItemUser(item.Id) == -1); },
            m_currentRole.Weapon);
    }

    void OnArmorClick()
    {
        SelectFromBag(
            (itemId) =>
            {
                var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
                if (m_currentRole.Armor == itemId)
                {
                    m_currentRole.UnequipItem(m_currentRole.GetArmor());
                    m_currentRole.Armor = -1;
                }
                else
                {
                    m_currentRole.UnequipItem(m_currentRole.GetArmor());
                    m_currentRole.Armor = itemId;
                    m_currentRole.UseItem(m_currentRole.GetArmor());
                    runtime.SetItemUser(item.Id, m_currentRole.GetJyx2RoleId());
                }
            },
            (item) => { return (int)item.EquipmentType == 1 && (runtime.GetItemUser(item.Id) == m_currentRole.GetJyx2RoleId() || runtime.GetItemUser(item.Id) == -1); },
            m_currentRole.Armor);
    }

    void OnXiulianClick()
    {
        SelectFromBag(
            (itemId) =>
            {
                var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
                if (m_currentRole.Xiulianwupin == itemId)
                {
                    runtime.SetItemUser(item.Id, -1);
                    m_currentRole.ExpForItem = 0;
                    m_currentRole.Xiulianwupin = -1;
                }
                else
                {
                    if (item.NeedCastration)//辟邪剑谱和葵花宝典
                    {
                        GameUtil.ShowYesOrNoCastrate(m_currentRole, () =>
                        {
                            if (m_currentRole.GetXiulianItem() != null)
                            {
                                runtime.SetItemUser(m_currentRole.Xiulianwupin, -1);
                                m_currentRole.ExpForItem = 0;
                            }

                            m_currentRole.Xiulianwupin = itemId;
                            runtime.SetItemUser(item.Id, m_currentRole.GetJyx2RoleId());

                            RefreshCurrent();
                        });
                    }
                    else
                    {
                        if (m_currentRole.GetXiulianItem() != null)
                        {
                            runtime.SetItemUser(m_currentRole.Xiulianwupin, -1);
                            m_currentRole.ExpForItem = 0;
                        }

                        m_currentRole.Xiulianwupin = itemId;
                        runtime.SetItemUser(item.Id, m_currentRole.GetJyx2RoleId());
                    }
                }
            },
            (item) =>
            {
                return (int) item.ItemType == 2 && (runtime.GetItemUser(item.Id) == m_currentRole.GetJyx2RoleId() || runtime.GetItemUser(item.Id) == -1);
            },
            m_currentRole.Xiulianwupin);
    }

    void SelectFromBag(Action<int> Callback, Func<Jyx2ConfigItem, bool> filter, int current_itemId)
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(BagUIPanel), runtime.Items, new Action<int>((itemId) =>
        {
            if (itemId != -1 && !m_currentRole.CanUseItem(itemId))
            {
                var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
                GameUtil.DisplayPopinfo((int)item.ItemType == 1 ? "此人不适合配备此物品" : "此人不适合修炼此物品");
                return;
            }

            if (itemId != -1)
            {
                //卸下或使用选择装备
                Callback(itemId);
            }

            RefreshCurrent();
        }), filter, current_itemId);
    }

    void OnHealClick()
    {
        SelectRoleParams selectParams = new SelectRoleParams();
        selectParams.roleList = m_roleList;
        selectParams.title = "选择需要医疗的人";
        selectParams.isDefaultSelect = false;
        selectParams.callback = (cbParam) =>
        {
            StoryEngine.Instance.BlockPlayerControl = false;
            if (cbParam.selectList.Count <= 0)
            {
                return;
            }

            var selectRole = cbParam.selectList[0]; //默认只会选择一个
            var zhaoshi = new HealZhaoshiInstance(m_currentRole.Heal);
            var result =
                AIManager.Instance.GetSkillResult(m_currentRole, selectRole, zhaoshi, new BattleBlockVector(0, 0));
            result.Run();
            if (result.heal > 0)
            {
                m_currentRole.Tili -= 2;
            }

            DoRefresh();
        };

        Jyx2_UIManager.Instance.ShowUI(nameof(SelectRolePanel), selectParams);
    }

    void OnDetoxicateClick()
    {
        SelectRoleParams selectParams = new SelectRoleParams();
        selectParams.roleList = m_roleList;
        selectParams.title = "选择需要解毒的人";
        selectParams.isDefaultSelect = false;
        selectParams.callback = (cbParam) =>
        {
            StoryEngine.Instance.BlockPlayerControl = false;
            if (cbParam.selectList.Count <= 0)
            {
                return;
            }

            var selectRole = cbParam.selectList[0]; //默认只会选择一个
            var zhaoshi = new DePoisonZhaoshiInstance(m_currentRole.DePoison);
            var result =
                AIManager.Instance.GetSkillResult(m_currentRole, selectRole, zhaoshi, new BattleBlockVector(0, 0));
            result.Run();
            if (result.depoison < 0)
            {
                m_currentRole.Tili -= 2;
            }

            DoRefresh();
        };

        Jyx2_UIManager.Instance.ShowUI(nameof(SelectRolePanel), selectParams);
    }
}