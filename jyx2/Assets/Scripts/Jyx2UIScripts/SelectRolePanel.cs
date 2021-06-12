using Jyx2;
using Jyx2.Middleware;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //默认选择角色和必须上场的角色
    public void SetDefaltRole() 
    {
        if (selectList.Count > 0 || roleList.Count <= 0)
            return;
        for (int i = 0; i < roleList.Count; i++)
        {
            RoleInstance role = roleList[i];
            if (mustSelect != null && mustSelect(role))
                selectList.Add(role);
        }
        if(selectList.Count <= 0)
            selectList.Add(roleList[0]);
    }
}

public partial class SelectRolePanel:Jyx2_UIBase
{
    SelectRoleParams m_params;

    protected override void OnCreate()
    {
        InitTrans();
        BindListener(ConfirmBtn_Button, OnConfirmClick);
        BindListener(CancelBtn_Button, OnCancelClick);
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

        m_params.SetDefaltRole();//如果没有选择 默认选择一个
        TitleText_Text.text = m_params.title;
        ShowBtns();
        RefreshScroll();
    }

    void ShowBtns() 
    {
        CancelBtn_Button.gameObject.SetActive(m_params.canCancel);
    }

    void RefreshScroll() 
    {
        HSUnityTools.DestroyChildren(RoleParent_RectTransform);
        if (m_params.roleList == null || m_params.roleList.Count <= 0)
            return;
        
        for (int i = 0; i < m_params.roleList.Count; i++)
        {
            var role = m_params.roleList[i];
            var item = RoleUIItem.Create();
            item.transform.SetParent(RoleParent_RectTransform);
            item.transform.localScale = Vector3.one;

            Button btn = item.GetComponent<Button>();
            BindListener(btn, () =>
            {
                OnItemClick(item);
            });
            bool select = m_params.selectList.Contains(role);
            item.SetSelect(select);
            item.ShowRole(role);
        }
    }

    void OnItemClick(RoleUIItem item) 
    {
        RoleInstance role = item.GetShowRole();
        bool hasIt = m_params.selectList.Contains(role);
        if (hasIt)
        {
            if (m_params.selectList.Count <= 1)
                return;
            if (m_params.mustSelect != null && m_params.mustSelect.Invoke(role)) 
            {
                GameUtil.DisplayPopinfo("此角色强制上场");
                return;
            }
            m_params.selectList.Remove(role);
            item.SetSelect(false);
        }
        else 
        {
            if (m_params.IsFull) 
            {
                GameUtil.DisplayPopinfo($"最多只能选择{m_params.maxCount}人");
                return;
            }
            m_params.selectList.Add(role);
            item.SetSelect(true);
        }
    }

    void OnConfirmClick() 
    {
        SelectRoleParams param = m_params;
        if (m_params.callback == null) //说明不需要回调 直接刷新面板
        {
            RefreshScroll();
        }
        else 
        {
            if(m_params.needCloseAfterClickOK)
                Jyx2_UIManager.Instance.HideUI("SelectRolePanel");
            param.callback(param);
        }
    }

    void OnCancelClick() 
    {
        Jyx2_UIManager.Instance.HideUI("SelectRolePanel");
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_params = null;
        HSUnityTools.DestroyChildren(RoleParent_RectTransform);
    }
}
