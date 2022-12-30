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
using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.UI;
using Jyx2.Util;
using UnityEngine.EventSystems;
using Jyx2.UINavigation;

public partial class ShopUIPanel : Jyx2_UIBase
{
	int curShopId;
	List<CsShopItem> m_ShopDataItems = new List<CsShopItem>();
    
	Action callback;
	List<ShopUIItem> m_VisibleShopItems = new List<ShopUIItem>();

    List<ShopUIItem> m_CachedShopItems = new List<ShopUIItem>();

    private ShopUIItem m_CurSelectItem;

	public ShopUIItem CurSelectItem => m_CurSelectItem;

    private GameRuntimeData runtime
	{
		get { return GameRuntimeData.Instance; }
	}

	protected override void OnCreate()
	{
		InitTrans();
		IsBlockControl = true;

		BindListener(CloseBtn_Button, OnCloseClick, false);
		BindListener(ConfirmBtn_Button, OnConfirmClick, false);
	}

	int GetHasBuyNum(int id)
	{
		if (!runtime.ShopItems.ContainsKey(id.ToString()))
			return 0;
		return runtime.ShopItems[id.ToString()];
	}

	void AddBuyCount(int itemId, int num)
	{
		if (!runtime.ShopItems.ContainsKey(itemId.ToString()))
			runtime.ShopItems[itemId.ToString()] = 0;
		runtime.ShopItems[itemId.ToString()] += num;
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		m_ShopDataItems.Clear();
		//curShopId = (int)allParams[0];
		curShopId = LevelMaster.GetCurrentGameMap().Id;
		var curShopData = LuaToCsBridge.ShopTable[curShopId];
		var shopItems = curShopData.ShopItems;
		foreach (var aShopItem in shopItems)
		{
			m_ShopDataItems.Add(new CsShopItem(aShopItem));
		}

		LoadShopItems();
		RefreshProperty();
		RefreshMoney();
		if (allParams.Length > 1)
		{
			callback = (Action)allParams[1];
		}
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
		callback?.Invoke();
		callback = null;
	}

	void RefreshMoney()
	{
		int num = runtime.GetMoney();
		//---------------------------------------------------------------------------
		//MoneyNum_Text.text = $"持有银两:{num}";
		//---------------------------------------------------------------------------
		//特定位置的翻译【持有银两的显示翻译】
		//---------------------------------------------------------------------------
		MoneyNum_Text.text = string.Format("持有银两:{0}".GetContent(nameof(ShopUIPanel)), num);
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
	}

	void LoadShopItems()
	{
		m_VisibleShopItems.Clear();
		Action<int, ShopUIItem, CsShopItem> OnShopItemCreate = (idx, item, data) =>
		{
			m_VisibleShopItems.Add(item);
            item.gameObject.BetterSetActive(true);
            int currentNum = GetHasBuyNum(data.Id);
            item.Refresh(data, idx, currentNum);
            item.SetSelect(m_CurSelectItem == item);
			item.OnShopItemSelect += OnItemSelect;
        };      
        MonoUtil.GenerateMonoElementsWithCacheList(ScrollItem_RectTransform.gameObject, m_ShopDataItems, m_CachedShopItems, ItemRoot_RectTransform, OnShopItemCreate);
        if(m_VisibleShopItems.Count > 0)
		{
			int col = ItemRoot_GridLayout.constraintCount;
            int row = m_VisibleShopItems.Count % col == 0 ? m_VisibleShopItems.Count / col : m_VisibleShopItems.Count / col + 1;

            NavigateUtil.SetUpNavigation(m_VisibleShopItems, row, col);
			EventSystem.current.SetSelectedGameObject(m_VisibleShopItems[0].gameObject);
        }
	}

	void RefreshProperty()
	{
		if (m_CurSelectItem == null)
		{
			ItemDes_RectTransform.gameObject.SetActive(false);
			return;
		}
		ItemDes_RectTransform.gameObject.SetActive(true);
		string mainText = UIHelper.GetItemDesText(LuaToCsBridge.ItemTable[m_CurSelectItem.ItemId]);
		DesText_Text.text = mainText;
	}

	void OnItemSelect(ShopUIItem item)
	{
		m_CurSelectItem?.SetSelect(false);

		m_CurSelectItem = item;
		m_CurSelectItem?.SetSelect(true);
		//scrollIntoView(ItemsArea_ScrollReact, item.transform as RectTransform, ItemRoot_GridLayout, 0);
		RefreshProperty();
	}

	public void OnCloseClick()
	{
		Jyx2_UIManager.Instance.HideUI(nameof(ShopUIPanel));
	}

	public void OnConfirmClick()
	{
		if (m_CurSelectItem == null)
			return;
		int count = m_CurSelectItem.GetBuyCount();
		if (count <= 0)
			return;
		CsShopItem shopItem = m_CurSelectItem.ShopItemData;
		if (shopItem == null)
			return;

        LItemConfig itemCfg = LuaToCsBridge.ItemTable[shopItem.Id];
		if (itemCfg == null)
			return;
		int moneyCost = count * shopItem.Price;
		if (runtime.GetMoney() < moneyCost)
		{
			GameUtil.DisplayPopinfo("持有银两不足");
			return;
		}
		runtime.AddItem(itemCfg.Id, count);
		AddBuyCount(itemCfg.Id, count);
		GameUtil.DisplayPopinfo($"购买{itemCfg.Name},数量{count}");
		runtime.AddItem(GameConst.MONEY_ID, -moneyCost);

        m_CurSelectItem.Refresh(shopItem, m_CurSelectItem.GetIndex(), GetHasBuyNum(shopItem.Id));
        RefreshMoney();
	}
}
