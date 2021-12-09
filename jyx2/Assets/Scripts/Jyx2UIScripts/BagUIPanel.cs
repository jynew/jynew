/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2.Middleware;

using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;

public partial class BagUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    Action<int> m_callback;
    Dictionary<string,int> m_itemDatas;
    Jyx2ItemUI m_selectItem;
    Func<Jyx2ConfigItem, bool> m_filter = null;
    private bool castFromSelectPanel=false;
    private int current_item;

    enum BagFilter
    {
        All = 0,
        Item,
        Cost,
        Equipment,
        Book,
        Anqi
    }

    private BagFilter _filter = BagFilter.All;
    
    protected override void OnCreate()
    {
        InitTrans();
        IsBlockControl = true;
        BindListener(UseBtn_Button, OnUseBtnClick);
        BindListener(CloseBtn_Button, OnCloseBtnClick);
    }
    
    private void OnEnable()
    {
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, OnCloseBtnClick);
    }

    private void OnDisable()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_itemDatas = (Dictionary<string,int>)allParams[0];
        if(allParams.Length > 1)
            m_callback = (Action<int>)allParams[1];
        if (allParams.Length > 2)
            m_filter = (Func<Jyx2ConfigItem, bool>)allParams[2];
        if (allParams.Length > 3)
        {
            castFromSelectPanel = true;
            current_item = (int) allParams[3];
        }
        else castFromSelectPanel = false;

        //道具类型过滤器
        int index = 0;
        foreach (var btn in m_Filters)
        {
            btn.onClick.RemoveAllListeners();
            var index1 = index;
            btn.onClick.AddListener(() =>
            {
                _filter = (BagFilter) (index1);
                RefreshFocusFilter();
                RefreshScroll();
            });
            index++;
        }

        _filter = BagFilter.All;
        RefreshFocusFilter();
        RefreshScroll();
    }

    void RefreshScroll() 
    {
        HSUnityTools.DestroyChildren(ItemRoot_RectTransform);
        bool hasSelect = false;
        foreach (var kv in m_itemDatas)
        {
            string id = kv.Key;
            int count = kv.Value;

            var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(id);
            if (item == null)
            {
                Debug.LogError("cannot get item data, id=" + id);
                continue;
            }
            //item filter
            if (m_filter != null && m_filter(item) == false)
                continue;

            if (_filter == BagFilter.Item && item.GetItemType() != Jyx2ItemType.TaskItem) continue;
            if (_filter == BagFilter.Anqi && item.GetItemType() != Jyx2ItemType.Anqi) continue;
            if (_filter == BagFilter.Book && item.GetItemType() != Jyx2ItemType.Book) continue;
            if (_filter == BagFilter.Cost && item.GetItemType() != Jyx2ItemType.Costa) continue;
            if (_filter == BagFilter.Equipment && item.GetItemType() != Jyx2ItemType.Equipment) continue;
            

            var itemUI = Jyx2ItemUI.Create(int.Parse(id), count);
            itemUI.transform.SetParent(ItemRoot_RectTransform);
            itemUI.transform.localScale = Vector3.one;
            var btn = itemUI.GetComponent<Button>();

            BindListener(btn, () => 
            {
                OnItemClick(itemUI);
            });
            if (!hasSelect) 
            {
                m_selectItem = itemUI;
                hasSelect = true;
            }
            itemUI.Select(m_selectItem == itemUI);
        }

        setBtnText();

        ShowItemDes();
    }

    void ShowItemDes() 
    {
        if (m_selectItem == null) 
        {
            UseBtn_Button.gameObject.SetActive(false);
            ItemDes_RectTransform.gameObject.SetActive(false);
            return;
        }

        ItemDes_RectTransform.gameObject.SetActive(true);
        UseBtn_Button.gameObject.SetActive(true);
        var item = m_selectItem.GetItem();
        DesText_Text.text = UIHelper.GetItemDesText(item);
    }

    void OnItemClick(Jyx2ItemUI itemUI) 
    {
        if (m_selectItem == itemUI)
            return;

        if (m_selectItem)
            m_selectItem.Select(false);
        m_selectItem = itemUI;
        m_selectItem.Select(true);
        
        setBtnText();

        ShowItemDes();
    }

    void OnCloseBtnClick() 
    {
        Jyx2_UIManager.Instance.HideUI(nameof(BagUIPanel));
    }

    void OnUseBtnClick() 
    {
        if (m_selectItem == null || m_callback ==null)
            return;
        Action<int> call = m_callback;
        var item = m_selectItem.GetItem();
        
        //if (item.ItemType == 3) //使用未遂，不关闭bag
        //{
            Jyx2_UIManager.Instance.HideUI(nameof(BagUIPanel));
        //}
        call(item.Id);
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_selectItem = null;
        m_callback = null;
        m_filter = null;
        HSUnityTools.DestroyChildren(ItemRoot_RectTransform);
    }

    void setBtnText()
    {
        if (m_selectItem==null)return;
        if (castFromSelectPanel && m_selectItem.GetItem().Id == current_item)
            UseBtn_Text.text = "卸 下";
        else
            UseBtn_Text.text = "使 用";
    }


    void RefreshFocusFilter()
    {
        foreach (var btn in m_Filters)
        {
            btn.GetComponent<Image>().color = Color.white;
        }

        int index = (int) _filter;
        
        //高亮的边框颜色等于文字颜色
        m_Filters[index].GetComponent<Image>().color = m_Filters[index].GetComponentInChildren<Text>().color; 
    }

}
