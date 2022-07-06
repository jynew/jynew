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

using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using i18n.TranslatorDef;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopUIPanel : Jyx2_UIBase
{
	ChildGoComponent childMgr;
	int curShopId;
	List<Jyx2ConfigShopItem> curShopItemList = new List<Jyx2ConfigShopItem>();
	ShopUIItem curSelectItem
	{
		get
		{
			return (current_selection >= 0 && current_selection < visibleItems.Count) ? visibleItems[current_selection] : null;
		}
	}
	Action callback;
	List<ShopUIItem> visibleItems = new List<ShopUIItem>();

	private GameRuntimeData runtime
	{
		get { return GameRuntimeData.Instance; }
	}

	protected override void OnCreate()
	{
		InitTrans();
		IsBlockControl = true;
		childMgr = GameUtil.GetOrAddComponent<ChildGoComponent>(ItemRoot_RectTransform);
		childMgr.Init(ScrollItem_RectTransform, (trans) =>
		{
			ShopUIItem item = GameUtil.GetOrAddComponent<ShopUIItem>(trans);
			item.Init();
			visibleItems.Add(item);
			BindListener(trans.GetComponent<Button>(), () =>
			{
				OnItemSelect(item, false);
			});
		});

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
		curShopItemList.Clear();
		//curShopId = (int)allParams[0];
		curShopId = LevelMaster.GetCurrentGameMap().Id;
		var curShopData = GameConfigDatabase.Instance.Get<Jyx2ConfigShop>(curShopId);
		var shopItems = curShopData.ShopItems.Split('|');
		foreach (var shopItemStr in shopItems)
		{
			var shopItemArr = shopItemStr.Split(',');
			if (shopItemArr.Length != 3) continue;
			var shopItem = new Jyx2ConfigShopItem();
			shopItem.Id = int.Parse(shopItemArr[0]);
			shopItem.Count = int.Parse(shopItemArr[1]);
			shopItem.Price = int.Parse(shopItemArr[2]);
			curShopItemList.Add(shopItem);
		}
		
		//only change to first item, when first time showing
		if (visibleItems.Count > 0 && GamepadHelper.GamepadConnected)
			changeCurrentSelection(0);

		RefreshChild();
		RefreshProperty();
		RefreshMoney();
		if (allParams.Length > 1)
		{
			callback = (Action)allParams[1];
		}
	}

	protected override void OnHidePanel()
	{
		itemX = 0;
		itemY = 0;
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

	void RefreshChild()
	{
		childMgr.RefreshChildCount(curShopItemList.Count);
		List<Transform> childList = childMgr.GetUsingTransList();

		float itemHeight = 0;

		for (int i = 0; i < childList.Count; i++)
		{
			Transform trans = childList[i];
			var data = curShopItemList[i];
			ShopUIItem uiItem = trans.GetComponent<ShopUIItem>();
			int currentNum = GetHasBuyNum(data.Id);
			uiItem.Refresh(data, i, currentNum);
			uiItem.SetSelect(current_selection == i);

			if (itemHeight == 0)
				itemHeight = uiItem.rectTransform().rect.height;
		}

		//setAreasHeightForItemCompleteView(itemHeight, new[] {
		//	ItemsArea_ScrollReact.rectTransform(),
		//	ItemDes_RectTransform
		//});
	}

	void RefreshProperty()
	{
		if (current_selection < 0 || current_selection >= curShopItemList.Count)
		{
			ItemDes_RectTransform.gameObject.SetActive(false);
			return;
		}
		ItemDes_RectTransform.gameObject.SetActive(true);
		string mainText = UIHelper.GetItemDesText(GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(curShopItemList[current_selection].Id));
		DesText_Text.text = mainText;
	}

	void OnItemSelect(ShopUIItem item, bool scroll)
	{
		curSelectItem?.SetSelect(false);

		int index = item.GetIndex();
		current_selection = index;
		curSelectItem?.SetSelect(true);
		if (scroll)
			scrollIntoView(ItemsArea_ScrollReact, item.transform as RectTransform, ItemRoot_GridLayout, 0);
		RefreshProperty();
	}

	void OnCloseClick()
	{
		Jyx2_UIManager.Instance.HideUI(nameof(ShopUIPanel));
	}

	void OnConfirmClick()
	{
		if (curSelectItem == null)
			return;
		int count = curSelectItem.GetBuyCount();
		if (count <= 0)
			return;
		Jyx2ConfigShopItem item = curShopItemList[curSelectItem.GetIndex()];
		Jyx2ConfigItem itemCfg = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(item.Id);
		if (itemCfg == null)
			return;
		int moneyCost = count * item.Price;
		if (runtime.GetMoney() < moneyCost)
		{
			GameUtil.DisplayPopinfo("持有银两不足");
			return;
		}
		runtime.AddItem(itemCfg.Id, count);
		AddBuyCount(itemCfg.Id, count);
		GameUtil.DisplayPopinfo($"购买{itemCfg.Name},数量{count}");
		runtime.AddItem(GameConst.MONEY_ID, -moneyCost);

		RefreshChild();
		RefreshMoney();
	}
	#region 手柄支持代码

	protected override int axisReleaseDelay
	{
		get
		{
			return 200;
		}
	}


	protected override bool captureGamepadAxis
	{
		get
		{
			return true;
		}
	}

	private int itemX = 0;
	private int itemY = 0;

	protected override void changeCurrentSelection(int num)
	{
		if (num >= 0 && num < visibleItems.Count)
		{
			OnItemSelect(visibleItems[num], true);
		}
		else
		{
			if (curSelectItem != null)
			{
				curSelectItem.SetSelect(false);
			}
		}
	}

	private int getSelectedItemIndex()
	{
		if (visibleItems.Count == 0)
			return -1;

		int horizontalItemsCount = getColCount();
		return itemY * horizontalItemsCount + itemX;
	}

	private int getColCount()
	{
		if (visibleItems.Count == 0)
			return 1;

		return (int)Math.Floor(ItemRoot_RectTransform.rect.width / visibleItems[0].rectTransform().rect.width);
	}

	private int getRowCount()
	{
		return (int)Math.Ceiling((float)visibleItems.Count / (float)getColCount());
	}

	protected override void OnDirectionalLeft()
	{
		if (itemX > 0)
			itemX--;
		else if (itemY > 0)
		{
			itemX = getColCount() - 1;
			OnDirectionalUp();
		}

		changeCurrentSelectionWithAxis();
	}

	private bool changeCurrentSelectionWithAxis()
	{
		var itemIndex = getSelectedItemIndex();
		var validMove = (itemIndex > -1 && itemIndex < visibleItems.Count);

		if (validMove)
			changeCurrentSelection(itemIndex);

		return validMove;
	}

	protected override void OnDirectionalUp()
	{
		if (itemY > 0)
			itemY--;

		changeCurrentSelectionWithAxis();
	}

	protected override void OnDirectionalRight()
	{
		if (itemX < getColCount() - 1)
		{
			itemX++;
			if (!changeCurrentSelectionWithAxis())
				itemX--;
		}
		else if (itemY < getRowCount() - 1)
		{
			itemX = 0;
			OnDirectionalDown();
		}
	}

	protected override void OnDirectionalDown()
	{
		if (itemY < getRowCount() - 1)
			itemY++;

		if (!changeCurrentSelectionWithAxis())
			itemY--;
	}


	protected override void handleGamepadButtons()
	{
		if (GamepadHelper.IsConfirm())
		{
			OnConfirmClick();
		}
		else if (GamepadHelper.IsCancel())
		{
			OnCloseClick();
		}
	}

	#endregion
}
