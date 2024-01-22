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
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.UI;

using Jyx2.UINavigation;
using Jyx2.Util;
using System.Linq;
using UnityEngine.EventSystems;

public partial class XiakeUIPanel : Jyx2_UIBase
{
    [SerializeField]
    private string m_RoleItemPrefabPath;

    [SerializeField]
    private List<Selectable> m_RightButtons = new List<Selectable>();

    private List<Selectable> m_VisibleBtns = new List<Selectable>();

    public override UILayer Layer => UILayer.NormalUI;

    List<RoleInstance> m_roleList = new List<RoleInstance>();

    RoleUIItem m_CurSelecRoleItem;

    RoleInstance m_currentRole;

    private List<RoleUIItem> m_CachedRoleItems = new List<RoleUIItem>();

    private List<RoleUIItem> m_AvailableRoleItems = new List<RoleUIItem>();

    public int CurItemIdx => m_AvailableRoleItems.IndexOf(m_CurSelecRoleItem);

    private List<Selectable> m_OperationBtns = new List<Selectable>();

    public bool IsSelectOnOpertaionBtn
    {
        get
        {
            var curSelect = EventSystem.current?.currentSelectedGameObject;
            if (curSelect == null)
                return false;
            if (!curSelect.activeInHierarchy)
                return false;
            if (!m_OperationBtns.Exists(btn => btn.gameObject == curSelect))
                return false;
            return true;
        }
    }


    protected override void OnCreate()
    {
        InitTrans();
        IsBlockControl = true;
        BindListener(BackButton_Button, OnBackClick, false);

        BindListener(LeaveButton_Button, OnLeaveClick);
        BindListener(ButtonHeal_Button, OnHealClick);
        BindListener(ButtonDetoxicate_Button, OnDetoxicateClick);
        BindListener(ButtonSelectWeapon_Button, OnWeaponClick);
        BindListener(ButtonSelectArmor_Button, OnArmorClick);
        BindListener(ButtonSelectBook_Button, OnXiulianClick);
        m_OperationBtns.Add(LeaveButton_Button);
        m_OperationBtns.Add(ButtonHeal_Button);
        m_OperationBtns.Add(ButtonDetoxicate_Button);
        m_OperationBtns.Add(ButtonSelectWeapon_Button);
        m_OperationBtns.Add(ButtonSelectArmor_Button);
        m_OperationBtns.Add(ButtonSelectBook_Button);
    }


    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_currentRole = allParams[0] as RoleInstance;
        if (allParams.Length > 1)
            m_roleList = allParams[1] as List<RoleInstance>;
        DoRefresh();
    }

    void DoRefresh()
    {
        RefreshRoleItems();
        RefreshCurrentRole();
    }

    void RefreshCurrentRole()
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

        bool canDepoison = m_currentRole.DePoison >= 20 && m_currentRole.Tili >= 10;
        ButtonDetoxicate_Button.gameObject.SetActive(canDepoison);
        bool canHeal = m_currentRole.Heal >= 20 && m_currentRole.Tili >= 50;
        ButtonHeal_Button.gameObject.SetActive(canHeal);
        PreImage_Image.LoadAsyncForget(m_currentRole.GetPic());

        AdjustRightButtonNavigation();
        TryFocusOnRightButton();
    }

    void RefreshRoleItems()
    {
        m_AvailableRoleItems.Clear();
        Action<int, RoleUIItem, RoleInstance> onRoleItemCreate = (idx, item, data) =>
        {
            m_AvailableRoleItems.Add(item);
            item.SetState(m_currentRole == data, false);
            item.OnSelectStateChange -= OnItemClick;
            item.OnSelectStateChange += OnItemClick;
        };

        MonoUtil.GenerateMonoElementsWithCacheList(m_RoleItemPrefabPath, m_roleList, m_CachedRoleItems, RoleParent_RectTransform, onRoleItemCreate);
        NavigateUtil.SetUpNavigation(m_AvailableRoleItems, m_AvailableRoleItems.Count, 1);
        SetUpRoleItemRightNavigation();
        PreSelectRoleItem();
    }

    void SetUpRoleItemRightNavigation()
    {
        foreach (var role in m_AvailableRoleItems)
        {
            var nav = role.navigation;
            nav.selectOnRight = LeaveButton_Button;
        }
    }

    void AdjustRightButtonNavigation()
    {
        m_VisibleBtns.Clear();
        var visibleBtns = m_RightButtons.Where(btn => btn.gameObject.activeInHierarchy);
        m_VisibleBtns.AddRange(visibleBtns);
        NavigateUtil.SetUpNavigation(m_VisibleBtns, m_VisibleBtns.Count, 1);
    }

    public void TryFocusOnRightButton()
    {
        if (!IsSelectOnOpertaionBtn)
            EventSystem.current.SetSelectedGameObject(LeaveButton_Button.gameObject);
    }


    void PreSelectRoleItem()
    {
        if (m_AvailableRoleItems.Count == 0)
            return;
        int idx = -1;
        if (m_currentRole != null)
            idx = m_AvailableRoleItems.FindIndex(item => item.GetShowRole() == m_currentRole);
        if (idx == -1) idx = 0;
        m_AvailableRoleItems[idx].SetState(true);
    }

    void OnItemClick(RoleUIItem item, bool willBeSelected)
    {
        if (m_CurSelecRoleItem == item)
            return;

        if (m_CurSelecRoleItem != null)
            m_CurSelecRoleItem.SetState(false, false);
        m_CurSelecRoleItem = item;
        m_CurSelecRoleItem.SetState(true, false);
        m_currentRole = m_CurSelecRoleItem.GetShowRole();
        RefreshCurrentRole();
        if (NavigateUtil.IsNavigateInputLastFrame())
        {
            NavigateUtil.TryFocusInScrollRect(m_CurSelecRoleItem);
        }
    }

    string GetInfoText(RoleInstance role)
    {
        StringBuilder sb = new StringBuilder();
        var color = role.GetMPColor();
        var color1 = role.GetHPColor1();
        var color2 = role.GetHPColor2();
        //---------------------------------------------------------------------------
        //sb.AppendLine($"等级 {role.Level}");
        //sb.AppendLine($"体力 {role.Tili}/{GameConst.MAX_ROLE_TILI}");
        //sb.AppendLine($"生命 <color={color1}>{role.Hp}</color>/<color={color2}>{role.MaxHp}</color>");
        //sb.AppendLine($"内力 <color={color}>{role.Mp}/{role.MaxMp}</color>");
        //sb.AppendLine($"经验 {role.Exp}/{role.GetLevelUpExp()}");
        //sb.AppendLine();
        //sb.AppendLine($"攻击 {role.Attack}");
        //sb.AppendLine($"防御 {role.Defence}");
        //sb.AppendLine($"轻功 {role.Qinggong}");
        //sb.AppendLine($"医疗 {role.Heal}");
        //sb.AppendLine($"解毒 {role.DePoison}");
        //sb.AppendLine($"用毒 {role.UsePoison}");
        //sb.AppendLine();
        //sb.AppendLine($"拳掌 {role.Quanzhang}");
        //sb.AppendLine($"御剑 {role.Yujian}");
        //sb.AppendLine($"耍刀 {role.Shuadao}");
        //sb.AppendLine($"特殊 {role.Qimen}");
        //sb.AppendLine($"暗器 {role.Anqi}");
        //---------------------------------------------------------------------------
        //特定位置的翻译【XiakePanel角色信息显示大框的信息】
        //---------------------------------------------------------------------------
        sb.AppendLine(string.Format("等级 {0}".GetContent(nameof(XiakeUIPanel)), role.Level));
        sb.AppendLine(string.Format("体力 {0}/{1}".GetContent(nameof(XiakeUIPanel)), role.Tili, GameConst.MAX_ROLE_TILI));
        sb.AppendLine(string.Format("生命 <color={0}>{1}</color>/<color={2}>{3}</color>".GetContent(nameof(XiakeUIPanel)), color1, role.Hp, color2,
            role.MaxHp));
        sb.AppendLine(string.Format("内力 <color={0}>{1}/{2}</color>".GetContent(nameof(XiakeUIPanel)), color, role.Mp, role.MaxMp));
        sb.AppendLine(string.Format("经验 {0}/{1}".GetContent(nameof(XiakeUIPanel)), role.Exp, role.GetLevelUpExp()));
        sb.AppendLine();
        sb.AppendLine(string.Format("攻击 {0}".GetContent(nameof(XiakeUIPanel)), role.Attack));
        sb.AppendLine(string.Format("防御 {0}".GetContent(nameof(XiakeUIPanel)), role.Defence));
        sb.AppendLine(string.Format("轻功 {0}".GetContent(nameof(XiakeUIPanel)), role.Qinggong));
        sb.AppendLine(string.Format("医疗 {0}".GetContent(nameof(XiakeUIPanel)), role.Heal));
        sb.AppendLine(string.Format("解毒 {0}".GetContent(nameof(XiakeUIPanel)), role.DePoison));
        sb.AppendLine(string.Format("用毒 {0}".GetContent(nameof(XiakeUIPanel)), role.UsePoison));
        sb.AppendLine();
        sb.AppendLine(string.Format("拳掌 {0}".GetContent(nameof(XiakeUIPanel)), role.Quanzhang));
        sb.AppendLine(string.Format("御剑 {0}".GetContent(nameof(XiakeUIPanel)), role.Yujian));
        sb.AppendLine(string.Format("耍刀 {0}".GetContent(nameof(XiakeUIPanel)), role.Shuadao));
        sb.AppendLine(string.Format("特殊 {0}".GetContent(nameof(XiakeUIPanel)), role.Qimen));
        sb.AppendLine(string.Format("暗器 {0}".GetContent(nameof(XiakeUIPanel)), role.Anqi));
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------

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
        sb.AppendLine("武器：".GetContent(nameof(XiakeUIPanel)) + (weapon == null ? "" : weapon.Name));

        var armor = role.GetArmor();
        sb.AppendLine("防具：".GetContent(nameof(XiakeUIPanel)) + (armor == null ? "" : armor.Name));

        var xiulianItem = role.GetXiulianItem();
        sb.AppendLine("修炼：".GetContent(nameof(XiakeUIPanel)) + (xiulianItem == null
            ? ""
            : xiulianItem.Name + $"({role.ExpForItem}/{role.GetFinishedExpForItem()})"));

        return sb.ToString();
    }

    public void OnBackClick()
    {
        Jyx2_UIManager.Instance.HideUI(nameof(XiakeUIPanel));
    }

    // added handle leave chat logic
    // by eaphone at 2021/6/6
    void OnLeaveClick()
    {
        var curMap = LevelMaster.GetCurrentGameMap();
        if (!curMap.Tags.Contains("WORLDMAP") && RuntimeEnvSetup.CurrentModConfig.EnableKickTeammateBigMapOnly)
        {
            GameUtil.DisplayPopinfo("必须在大地图才可以角色离队");
            return;
        }

        if (m_currentRole == null)
            return;
        if (!m_roleList.Contains(m_currentRole))
            return;
        if (m_currentRole.IsPlayerRole)
        {
            GameUtil.DisplayPopinfo("主角不能离开队伍");
            return;
        }

        Action onCanceled = () => EventSystem.current.SetSelectedGameObject(LeaveButton_Button.gameObject);


        MessageBox.ConfirmOrCancel("确定让该角色离开队伍?", DoCharacterLeaveTeam, onCanceled);
    }

    private void DoCharacterLeaveTeam()
    {
        if (m_currentRole == null)
            return;
        var eventLuaPath = LuaToCsBridge.CharacterTable[m_currentRole.GetJyx2RoleId()].LeaveStoryId;
        if (!string.IsNullOrEmpty(eventLuaPath))
        {
            PlayLeaveStory(eventLuaPath).Forget();

        }
        else
        {
            GameRuntimeData.Instance.LeaveTeam(m_currentRole.GetJyx2RoleId());
            RefreshView();
        }
    }


    async UniTask PlayLeaveStory(string story)
    {
        gameObject.SetActive(false);

        var eventPath = string.Format(RuntimeEnvSetup.CurrentModConfig.LuaFilePatten, story);

        await Jyx2.LuaExecutor.Execute(eventPath);
        gameObject.SetActive(true);
        RefreshView();
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

    async void OnWeaponClick()
    {
        Action<int> onItemSelect = (itemId) =>
        {
            var item = LuaToCsBridge.ItemTable[itemId];

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
        };

        Func<LItemConfig, bool> itemFilterFunc = item => item.IsWeapon() && (item.IsBeingUsedBy(m_currentRole) || item.NoItemUser());

        await SelectFromBag(onItemSelect, itemFilterFunc, m_currentRole.Weapon);
    }

    async void OnArmorClick()
    {
        Action<int> onItemSelect = (itemId) =>
        {
            var item = LuaToCsBridge.ItemTable[itemId];
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
        };

        Func<LItemConfig, bool> itemFilterFunc = item => item.IsArmor() && (item.IsBeingUsedBy(m_currentRole) || item.NoItemUser());

        await SelectFromBag(onItemSelect, itemFilterFunc, m_currentRole.Armor);
    }

    async void OnXiulianClick()
    {
        async void onItemSelect(int itemId)
        {
            var item = LuaToCsBridge.ItemTable[itemId];
            if (m_currentRole.Xiulianwupin == itemId)
            {
                runtime.SetItemUser(item.Id, -1);
                m_currentRole.ExpForItem = 0;
                m_currentRole.Xiulianwupin = -1;
            }
            else
            {
                if (item.NeedCastration == 1) //辟邪剑谱和葵花宝典
                {
                    await GameUtil.ShowYesOrNoCastrate(m_currentRole, () =>
                    {
                        if (m_currentRole.GetXiulianItem() != null)
                        {
                            runtime.SetItemUser(m_currentRole.Xiulianwupin, -1);
                            m_currentRole.ExpForItem = 0;
                        }

                        m_currentRole.Xiulianwupin = itemId;
                        runtime.SetItemUser(item.Id, m_currentRole.GetJyx2RoleId());

                        RefreshCurrentRole();
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
        }

        Func<LItemConfig, bool> itemFilterFunc = item => item.IsBook() && (item.IsBeingUsedBy(m_currentRole) || item.NoItemUser());

        await SelectFromBag(onItemSelect, itemFilterFunc, m_currentRole.Xiulianwupin);
    }

    async UniTask SelectFromBag(Action<int> Callback, Func<LItemConfig, bool> filter, int current_itemId)
    {
        this.gameObject.SetActive(false);
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), new Action<int>((itemId) =>
        {
            this.gameObject.SetActive(true);
            if (itemId != -1 && !m_currentRole.CanUseItem(itemId))
            {
                var item = LuaToCsBridge.ItemTable[itemId];
                GameUtil.DisplayPopinfo((int)item.ItemType == 1 ? "此人不适合配备此物品" : "此人不适合修炼此物品");
                return;
            }

            if (itemId != -1)
            {
                //卸下或使用选择装备
                Callback(itemId);
            }

            RefreshCurrentRole();
        }), filter, current_itemId);
    }

    async void OnHealClick()
    {
        SelectRoleParams selectParams = new SelectRoleParams();
        selectParams.roleList = m_roleList;
        selectParams.title = "选择需要医疗的人";
        selectParams.isDefaultSelect = false;
        selectParams.callback = (cbParam) =>
        {
            this.gameObject.SetActive(true);
            StoryEngine.BlockPlayerControl = false;
            if (cbParam.isCancelClick == true)
            {
                return;
            }
            if (cbParam.selectList.Count <= 0)
            {
                return;
            }

            var selectRole = cbParam.selectList[0]; //默认只会选择一个
            var skillCast = new HealSkillCastInstance(m_currentRole.Heal);
            var result =
                LuaExecutor.CallLua<SkillCastResult, RoleInstance, RoleInstance, SkillCastInstance, BattleBlockVector>("Jyx2.Battle.DamageCaculator.GetSkillResult", m_currentRole, selectRole, skillCast, new BattleBlockVector(0, 0));
            result.Run();
            if (result.heal > 0)
            {
                m_currentRole.Tili -= 2;
            }
            DoRefresh();
        };

        this.gameObject.SetActive(false);
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SelectRolePanel), selectParams);
    }

    async void OnDetoxicateClick()
    {
        SelectRoleParams selectParams = new SelectRoleParams();
        selectParams.roleList = m_roleList;
        selectParams.title = "选择需要解毒的人";
        selectParams.isDefaultSelect = false;
        selectParams.callback = (cbParam) =>
        {
            this.gameObject.SetActive(true);
            StoryEngine.BlockPlayerControl = false;
            if (cbParam.isCancelClick == true)
            {
                return;
            }
            if (cbParam.selectList.Count <= 0)
            {
                return;
            }

            var selectRole = cbParam.selectList[0]; //默认只会选择一个
            var skillCast = new DePoisonSkillCastInstance(m_currentRole.DePoison);
            var result =
                LuaExecutor.CallLua<SkillCastResult, RoleInstance, RoleInstance, SkillCastInstance, BattleBlockVector>("Jyx2.Battle.DamageCaculator.GetSkillResult", m_currentRole, selectRole, skillCast, new BattleBlockVector(0, 0));
            result.Run();
            if (result.depoison < 0)
            {
                m_currentRole.Tili -= 2;
            }
            DoRefresh();
        };

        this.gameObject.SetActive(false);
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SelectRolePanel), selectParams);
    }


    public void TabLeft()
    {
        SelectRoleItem(CurItemIdx - 1);
    }

    public void TabRight()
    {
        SelectRoleItem(CurItemIdx + 1);
    }

    public void SelectRoleItem(int idx)
    {
        if (idx < 0 || idx >= m_AvailableRoleItems.Count)
            return;
        m_AvailableRoleItems[idx].Select(true);
    }

}
