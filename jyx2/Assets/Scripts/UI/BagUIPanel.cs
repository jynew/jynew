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
using System;
using System.Collections.Generic;
using i18n.TranslatorDef;
using System.Linq;

//代码数据结构遗留问题，不太好动道具存档数据结构，这里用别名处理下
using ItemArchiveData = System.Collections.Generic.KeyValuePair<string, (int, int)>; //<道具id，(道具数量，获取时间戳)> 
using Jyx2.Util;
using UnityEngine;
using Jyx2.UINavigation;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.EventSystems;

public partial class BagUIPanel : Jyx2_UIBase
{
    [SerializeField]
    private string m_BagItemPrefabPath;

    [SerializeField]
    private SelectButtonGroup m_TabGroup;

    public override UILayer Layer => UILayer.NormalUI;

    Action<int> m_UseItemCallBack;

    Jyx2ItemUI m_selectItem;
    Func<LItemConfig, bool> m_filter;

    private bool castFromSelectPanel = false;

    private int m_PreSelectItemId;

    private List<ItemArchiveData> m_ItemDatas = new List<ItemArchiveData>();

    private List<Jyx2ItemUI> m_VisibleItems = new List<Jyx2ItemUI>();

    private List<Jyx2ItemUI> m_CachedItems = new List<Jyx2ItemUI>();

    public bool IsUseButtonActive => UseBtn_Button.gameObject.activeInHierarchy;

    enum BagItemTabType
    {
        None = -1,
        All = 0,
        Item,
        Cost,
        Equipment,
        Book,
        Anqi
    }

    private BagItemTabType m_TabType = BagItemTabType.None;

    private void Awake()
    {
        m_TabGroup.OnButtonSelect.AddListener(OnTabSelect);
    }

    private void OnDestroy()
    {
        m_TabGroup.OnButtonSelect.RemoveListener(OnTabSelect);
    }

    protected override void OnCreate()
    {
        InitTrans();
        IsBlockControl = true;
        BindListener(UseBtn_Button, OnUseBtnClick, false);
        BindListener(CloseBtn_Button, OnCloseBtnClick, false);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        castFromSelectPanel = false;
        if (allParams.Length > 0)
            m_UseItemCallBack = (Action<int>)allParams[0];
        if (allParams.Length > 1)
            m_filter = (Func<LItemConfig, bool>)allParams[1];
        if (allParams.Length > 2)
        {
            castFromSelectPanel = true;
            m_PreSelectItemId = (int)allParams[2];
        }
        m_TabGroup.SelectIndex(0);
    }

    private void OnTabSelect(int idx)
    {
        var newTabType = (BagItemTabType)(idx);
        if (m_TabType == newTabType)
            return;
        m_TabType = newTabType;
        ClearSelectItem();
        RefreshItems();
    }

    void ClearSelectItem()
    {
        if(m_selectItem != null)
        {
            m_selectItem.SetSelectState(false, false);
            m_selectItem = null;
        }
    }

    void RefreshItems()
    {
        m_ItemDatas.Clear();
        m_VisibleItems.Clear();

        var visibleItems = GameRuntimeData.Instance.Items.OrderBy(item => item.Value.Item2 /*按时间排序*/)
                                                         .Where(IsItemVisible);
        m_ItemDatas.AddRange(visibleItems);

        Action<int, Jyx2ItemUI, ItemArchiveData> OnBagItemCreate = (idx, item, data) =>
        {   
            item.SetSelectState(m_PreSelectItemId == item.ItemId, false);
            m_VisibleItems.Add(item);
            item.OnItemSelect -= OnItemSelect;
            item.OnItemSelect += OnItemSelect;
        };


        MonoUtil.GenerateMonoElementsWithCacheList(m_BagItemPrefabPath, m_ItemDatas, m_CachedItems, ItemRoot_GridLayoutGroup.transform, OnBagItemCreate);

        int col = ItemRoot_GridLayoutGroup.constraintCount;
        int row = m_VisibleItems.Count % col == 0 ? m_VisibleItems.Count / col : m_VisibleItems.Count / col + 1;
        NavigateUtil.SetUpNavigation(m_VisibleItems, row, col);

        PreSelectItem();
        //以后如果加载项多了用MonoUtil.GenerateMonoElementsAsync
    }

    private void PreSelectItem()
    {
        var idx = m_VisibleItems.FindIndex(item => item.ItemId == m_PreSelectItemId);
        if (idx == -1) idx = 0;
        var item = m_VisibleItems.SafeGet(idx);
        if (item != null)
        {
            EventSystem.current.SetSelectedGameObject(item.gameObject);
            item.SetSelectState(true, true);
        }
        else
        {
            ClearSelectItem();
            RefreshItemDetail();
            RefreshButtonText();
        }
    }

    private bool IsItemVisible(ItemArchiveData itemData)
    {
        var item = LuaToCsBridge.ItemTable[int.Parse(itemData.Key)];
        if (item == null)
            return false;
        if (m_filter != null && !m_filter(item))
            return false;

        if (m_TabType == BagItemTabType.Item && item.GetItemType() != Jyx2ItemType.TaskItem) return false;
        if (m_TabType == BagItemTabType.Anqi && item.GetItemType() != Jyx2ItemType.Anqi) return false;
        if (m_TabType == BagItemTabType.Book && item.GetItemType() != Jyx2ItemType.Book) return false;
        if (m_TabType == BagItemTabType.Cost && item.GetItemType() != Jyx2ItemType.Costa) return false;
        if (m_TabType == BagItemTabType.Equipment && item.GetItemType() != Jyx2ItemType.Equipment) return false;

        return true;
    }


    void RefreshItemDetail()
    {
        if (m_selectItem == null)
        {
            UseBtn_Button.gameObject.BetterSetActive(false);
            ItemDes_RectTransform.gameObject.BetterSetActive(false);
            return;
        }

        ItemDes_RectTransform.gameObject.BetterSetActive(true);
        UseBtn_Button.gameObject.BetterSetActive(true);
        var item = m_selectItem.GetItemConfigData();
        DesText_Text.text = UIHelper.GetItemDesText(item);
    }

    void OnItemSelect(Jyx2ItemUI itemUI)
    {
        if (m_selectItem == itemUI)
            return;

        if (m_selectItem != null)
            m_selectItem.SetSelectState(false, false);
        m_selectItem = itemUI;
        m_selectItem.SetSelectState(true, false);
        RefreshButtonText();
        RefreshItemDetail();
    }

    public void OnCloseBtnClick()
    {
        m_UseItemCallBack?.Invoke(-1);
        Jyx2_UIManager.Instance.HideUI(nameof(BagUIPanel));
    }

    public void OnUseBtnClick()
    {
        if (m_selectItem == null || m_UseItemCallBack == null)
            return;
        Action<int> callback = m_UseItemCallBack;
        var item = m_selectItem.GetItemConfigData();
        Jyx2_UIManager.Instance.HideUI(nameof(BagUIPanel));
        callback?.Invoke(item.Id);
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_selectItem = null;
        m_UseItemCallBack = null;
        m_filter = null;
        ClearSelectItem();
        m_TabType = BagItemTabType.None;
    }

    void RefreshButtonText()
    {
        if (m_selectItem == null) 
            return;
        if (castFromSelectPanel && m_selectItem.ItemId == m_PreSelectItemId)
            UseText_Text.text = "卸 下".GetContent(nameof(BagUIPanel));
        else
            UseText_Text.text = "使 用".GetContent(nameof(BagUIPanel));
    }

    public void TabLeft()
    {
        m_TabGroup.SelectIndex(m_TabGroup.CurButtonIndex - 1);
    }

    public void TabRight()
    {
        m_TabGroup.SelectIndex(m_TabGroup.CurButtonIndex + 1);
    }
}
