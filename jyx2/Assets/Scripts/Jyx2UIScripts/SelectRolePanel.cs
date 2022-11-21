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
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Jyx2.Util;
using Jyx2.UINavigation;
using UnityEngine.EventSystems;

public class SelectRoleParams
{
    public List<RoleInstance> roleList = new List<RoleInstance>();
    public List<RoleInstance> selectList = new List<RoleInstance>();
    public Func<RoleInstance, bool> mustSelect;
    public Action<SelectRoleParams> callback;
    public int maxCount = 1;//最大选择人数
    public string title = "选择角色";
    public bool canCancel = true;//默认可以取消选择
    public bool needCloseAfterClickOK = true;//点击确认之后是否需要关闭 如果不需要关闭 那么刷新下面板
    public List<int> showPropertyIds = new List<int>() { 13, 15, 14 };//要显示的属性 默认是生命 体力 内力
    public bool IsFull { get { return selectList.Count >= maxCount; } }
    public bool isDefaultSelect = true;
    //默认选择角色和必须上场的角色
    public bool isCancelClick = false;//是否点击取消
    public void SetDefaltRole()
    {
        if (selectList.Count > 0 || roleList.Count <= 0)
            return;
        for (int i = 0; i < roleList.Count; i++)
        {
            RoleInstance role = roleList[i];
            if (mustSelect != null && mustSelect(role))
            {
                if (role.Hp <= 0) role.Hp = 1;
                selectList.Add(role);
            }
        }
        if (selectList.Count <= 0)
            selectList.Add(roleList[0]);
    }
}

public partial class SelectRolePanel : Jyx2_UIBase
{
    [SerializeField]
    private string m_RoleItemPrefabPath;
    
    private List<RoleUIItem> m_AvailableRoleItems = new List<RoleUIItem>();

    private List<RoleUIItem> m_CachedRolesItems = new List<RoleUIItem>();

    public static UniTask<List<RoleInstance>> Open(SelectRoleParams paras)
    {
        var t = new UniTaskCompletionSource<List<RoleInstance>>();
        paras.callback = (ret) =>
        {
            t.TrySetResult(ret.selectList);
        };
        return t.Task;
    }


    SelectRoleParams m_params;

    protected override void OnCreate()
    {
        InitTrans();
        BindListener(ConfirmBtn_Button, OnConfirmClick, false);
        BindListener(CancelBtn_Button, OnCancelClick, false);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_params = allParams[0] as SelectRoleParams;
        if (m_params == null || m_params.roleList.Count <= 0)
        {
            Debug.LogError("selectRolePanel params is null");
            return;
        }

        if (m_params.isDefaultSelect)
        {
            m_params.SetDefaltRole();//如果没有选择 默认选择一个
        }
        TitleText_Text.text = m_params.title;
        ShowBtns();

        RefreshRoleItems();
    }

    void ShowBtns()
    {
        CancelBtn_Button.gameObject.SetActive(m_params.canCancel);
    }

    void RefreshRoleItems()
    {
        m_AvailableRoleItems.Clear();
        if (m_params.roleList == null || m_params.roleList.Count <= 0)
            return;

        int aliveRoleCount = m_params.roleList.Count(role => role.Hp > 0);
        if (aliveRoleCount == 0 && m_params.roleList.Count > 0)
            m_params.roleList[0].Hp = 1;

        Action<int, RoleUIItem, RoleInstance> onRoleItemCreate = (idx, item, data) =>
        {
            m_AvailableRoleItems.Add(item);
            item.SetState(m_params.selectList.Contains(data), false);
            item.OnSelectStateChange -= OnItemClick;
            item.OnSelectStateChange += OnItemClick;
        };

        MonoUtil.GenerateMonoElementsWithCacheList(m_RoleItemPrefabPath, m_params.roleList, m_CachedRolesItems, RoleParent_GridLayoutGroup.transform, onRoleItemCreate);
        int col = RoleParent_GridLayoutGroup.constraintCount;
        int row = m_AvailableRoleItems.Count % col == 0 ? m_AvailableRoleItems.Count / col : m_AvailableRoleItems.Count / col + 1;
        NavigateUtil.SetUpNavigation(m_AvailableRoleItems, row, col);
        SelectDefaultRoleItem();
    }

    void SelectDefaultRoleItem()
    {
        var role = m_params.selectList.FirstOrDefault();
        var item = m_AvailableRoleItems.Find(item => role == item.GetShowRole());
        if (item == null && m_AvailableRoleItems.Count > 0)
            item = m_AvailableRoleItems[0];
        if(item != null)
        {
            EventSystem.current.SetSelectedGameObject(item.gameObject);
        }
    }

    void OnItemClick(RoleUIItem item, bool willBeSelected)
    {
        RoleInstance role = item.GetShowRole();
        if (!willBeSelected)
        {
            bool isRoleMusSelect = m_params.mustSelect?.Invoke(role) ?? false;
            if (isRoleMusSelect)
            {
                item.SetState(true, false);
                GameUtil.DisplayPopinfo("此角色强制上场");
                return;
            }
            m_params.selectList.Remove(role);
        }
        else
        {
            if (m_params.IsFull)
            {
                item.SetState(false, false);
                GameUtil.DisplayPopinfo($"最多只能选择{m_params.maxCount}人");
                return;
            }
            m_params.selectList.Add(role);
        }
    }

    public void OnConfirmClick()
    {
        if (m_params.selectList.Count == 0)
        {
            GameUtil.DisplayPopinfo($"未选择任何人");
            return;
        }
        SelectRoleParams param = m_params;
        if (m_params.callback == null) //说明不需要回调 直接刷新面板
        {
            RefreshRoleItems();
        }
        else
        {
            if (m_params.needCloseAfterClickOK)
                Jyx2_UIManager.Instance.HideUI(nameof(SelectRolePanel));
            param.callback(param);
        }
    }

    public void OnCancelClick()
    {
        m_params.isCancelClick = true;
        SelectRoleParams param = m_params;
        Jyx2_UIManager.Instance.HideUI(nameof(SelectRolePanel));
        param.callback(param);
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_params = null;
    }
}
