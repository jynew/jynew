using Jyx2.Middleware;
using HSFrameWork.ConfigTable;
using HSFrameWork.SPojo;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class BagUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    Action<int> m_callback;
    SaveableNumberDictionary<int> m_itemDatas;
    Jyx2ItemUI m_selectItem;
    Func<Jyx2Item, bool> m_filter = null;
    protected override void OnCreate()
    {
        InitTrans();

        BindListener(UseBtn_Button, OnUseBtnClick);
        BindListener(CloseBtn_Button, OnCloseBtnClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        m_itemDatas = (SaveableNumberDictionary<int>)allParams[0];
        if(allParams.Length > 1)
            m_callback = (Action<int>)allParams[1];
        if (allParams.Length > 2)
            m_filter = (Func<Jyx2Item, bool>)allParams[2];

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

            var item = ConfigTable.Get<Jyx2Item>(id);
            if (item == null)
            {
                Debug.LogError("cannot get item data, id=" + id);
                continue;
            }
            //item filter
            if (m_filter != null && m_filter(item) == false)
                continue;

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
        DesText_Text.text = UIHelper.GetItemDesText(Convert.ToInt32(item.Id));
    }

    void OnItemClick(Jyx2ItemUI itemUI) 
    {
        if (m_selectItem == itemUI)
            return;

        if (m_selectItem)
            m_selectItem.Select(false);
        m_selectItem = itemUI;
        m_selectItem.Select(true);
        ShowItemDes();
    }

    void OnCloseBtnClick() 
    {
        Jyx2_UIManager.Instance.HideUI("BagUIPanel");
    }

    void OnUseBtnClick() 
    {
        if (m_selectItem == null || m_callback ==null)
            return;
        Action<int> call = m_callback;
        var item = m_selectItem.GetItem();
        string selectId = item.Id;

        //if (item.ItemType == 3) //使用未遂，不关闭bag
        //{
            Jyx2_UIManager.Instance.HideUI("BagUIPanel");
        //}
        call(int.Parse(selectId));
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_selectItem = null;
        m_callback = null;
        m_filter = null;
        HSUnityTools.DestroyChildren(ItemRoot_RectTransform);
    }
}
