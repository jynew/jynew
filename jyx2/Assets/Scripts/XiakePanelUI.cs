/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HanSquirrel.ResourceManager;
using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public class XiakePanelUI : MonoBehaviour
{
    public Image headAvata;

    public Text m_NameText;
    public Text m_InfoText;
    public Text m_SkillText;
    public Text m_ItemsText;
    public Button m_LeaveButton;
    public Button m_BackButton;
    public Dropdown m_RoleDropdown;

    public Button m_SelectWeapon;
    public Button m_SelectArmor;
    public Button m_SelectBook;

    public static void Create(RoleInstance role, List<RoleInstance> roles, Transform parent)
    {
        var obj = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/XiakePanel.prefab");
        obj.transform.SetParent(parent);

        var rt = obj.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        
        var xiakePanelUI = obj.GetComponent<XiakePanelUI>();
        xiakePanelUI.Show(role, roles);
    }
    
    private void OnEnable()
    {
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, Close);
    }

    private void OnDisable()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
    }

    private List<RoleInstance> _roles;
    private RoleInstance _role;

    void Close()
    {
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BackButton.onClick.AddListener(Close);
        m_RoleDropdown.onValueChanged.AddListener(OnSelectDropdown);
        m_SelectWeapon.onClick.AddListener(OnSelectWeapon);
        m_SelectArmor.onClick.AddListener(OnSelectArmor);
        m_SelectBook.onClick.AddListener(OnSelectBook);
    }

    public void Show(RoleInstance role, List<RoleInstance> roles)
    {
        _role = role;
        _roles = roles;

        Jyx2ResourceHelper.GetRoleHeadSprite(role, headAvata);
        
        m_NameText.text = role.Name;
        m_InfoText.text = GetInfoText(role);
        m_SkillText.text = GetSkillText(role);
        m_ItemsText.text = GetItemsText(role);

        if(_roles == null || _roles.Count <= 0)
        {
            m_RoleDropdown.gameObject.SetActive(false);
        }
        else
        {
            m_RoleDropdown.gameObject.SetActive(true);
            ShowDropdown(_roles);
        }
    }

    void Refresh()
    {
        Show(_role, _roles);
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
        foreach(var skill in role.Wugongs)
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
		var xiulianString="修炼：";
		if(xiulianItem != null)
		{
			if(role.GetWugongLevel(xiulianItem.Wugong)<10)
			{
				xiulianString+="-/{role.GetFinishedExpForItem()}";
			}else{
				xiulianString+=xiulianItem.Name + $"({role.ExpForItem}/{role.GetFinishedExpForItem()})";
			}
		}
        sb.AppendLine(xiulianString); 

        return sb.ToString();
    }


    void ShowDropdown(List<RoleInstance> roles)
    {
        m_RoleDropdown.ClearOptions();
        List<string> opts = new List<string>();

        foreach(var role in roles)
        {
            opts.Add(role.Name);
        }
        m_RoleDropdown.AddOptions(opts);
    }

    void OnSelectDropdown(int index)
    {
        if (_roles == null || _roles.Count <= index)
            return;

        Show(_roles[index], _roles);
    }

    GameRuntimeData runtime { get { return GameRuntimeData.Instance; } }

    public void OnSelectWeapon()
    {
        SelectFromBag(
            (itemId) => { _role.Weapon = itemId; },
            () =>
            {
                if (_role.Weapon != -1)
                {
                    // runtime.AddItem(_role.Weapon, 1);
                    _role.Weapon = -1;
                }
            },
            (item) => { return item.EquipmentType == 0;});
    }

    public void OnSelectArmor()
    {
        SelectFromBag(
            (itemId) => { _role.Armor = itemId; },
            () =>
            {
                if (_role.Armor != -1)
                {
                    runtime.AddItem(_role.Armor, 1);
                    _role.Armor = -1;
                }
            },
            (item) => { return item.EquipmentType == 1; });
    }

    public void OnSelectBook()
    {
        SelectFromBag(
            (itemId) => { _role.Xiulianwupin = itemId; },
            () =>
            {
                if (_role.Xiulianwupin != -1)
                {
                    runtime.AddItem(_role.Xiulianwupin, 1);
                    _role.Xiulianwupin = -1;
                }
            },
            (item) => { return item.ItemType == 2; });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback">装备物品回调</param>
    /// <param name="unquipCallback">卸物品回调</param>
    /// <param name="filter">过滤器</param>
    void SelectFromBag(Action<int> callback, Action unquipCallback, Func<Jyx2Item, bool> filter)
    {
        //BagPanel.Create(this.transform, runtime.Items, (itemId) =>
        //{
        //    if(itemId != -1 && !_role.CanUseItem(itemId))
        //    {
        //        MessageBox.Create("该角色不满足使用条件", null);
        //        return;
        //    }

        //    //卸下现有
        //    unquipCallback();
            
        //    if (itemId != -1)
        //    { 
        //        unquipCallback();
        //        runtime.AddItem(itemId, -1);
        //        callback(itemId);
        //    }

        //    Refresh();
        //},filter);
        Jyx2_UIManager.Instance.ShowUI(nameof(BagUIPanel),new Action<int>((itemId) =>
        {
            if (itemId != -1 && !_role.CanUseItem(itemId))
            {
                MessageBox.Create("该角色不满足使用条件", null);
                return;
            }
            
            if (itemId != -1)
            {
                //卸下现有
                unquipCallback();
                //装备选择的武器
                // runtime.AddItem(itemId,-1);
                callback(itemId);
            }

            Refresh();
        }),filter);
    }
}
