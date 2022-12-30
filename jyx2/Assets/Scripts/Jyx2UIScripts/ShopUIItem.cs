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
using Jyx2;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.UI;
using Jyx2.UINavigation;
using UnityEngine.EventSystems;

public class ShopUIItem : Selectable,INavigable
{
	Image iconImg;
	Text desText;
	Transform numContent;
	Button addBtn;
	Button reduceBtn;
	Text numText;
	Transform select;
	Text itemNum;
	Text totalCost;

	CsShopItem shopItem;
	int buyCount;
	int index;
	int leftNum;

	public event Action<ShopUIItem> OnShopItemSelect;

	public int ItemId => shopItem?.Id ?? -1;

	public CsShopItem ShopItemData => shopItem;


    protected override void Awake()
	{
		base.Awake();
		iconImg = transform.Find("Icon").GetComponent<Image>();
		desText = transform.Find("DesText").GetComponent<Text>();
		numContent = transform.Find("NumContent");
		addBtn = numContent.Find("AddBtn").GetComponent<Button>();
		reduceBtn = numContent.Find("ReduceBtn").GetComponent<Button>();
        numText = numContent.Find("NumText").GetComponent<Text>();
		select = transform.Find("Select");
		itemNum = transform.Find("PriceText").GetComponent<Text>();
		totalCost = numContent.Find("TotalCost").GetComponent<Text>();

		addBtn.onClick.AddListener(OnAddBtnClick);
		reduceBtn.onClick.AddListener(OnReduceBtnClick);

    }

	protected override void OnDestroy()
	{
		base.OnDestroy();
        addBtn.onClick.RemoveListener(OnAddBtnClick);
        reduceBtn.onClick.RemoveListener(OnReduceBtnClick);
		OnShopItemSelect = null;
    }

	public async UniTaskVoid Refresh(CsShopItem shopItem, int index, int hasBuyNum)
	{
		this.index = index;
		this.shopItem = shopItem;
		LItemConfig item = LuaToCsBridge.ItemTable[shopItem.Id];

		//---------------------------------------------------------------------------
		//desText.text = $"{item.Name}\n价格：{shopItem.Price}";
		//---------------------------------------------------------------------------
		//特定位置的翻译【价格显示】
		//---------------------------------------------------------------------------
		desText.text = string.Format("{0}\n价格：{1}".GetContent(nameof(ShopUIItem)), item.Name, shopItem.Price);
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
		leftNum = shopItem.Count - hasBuyNum;
		leftNum = Jyx2.Middleware.Tools.Limit(leftNum, 0, shopItem.Count);
		itemNum.text = leftNum.ToString();

		iconImg.LoadAsyncForget(item.GetPic());
	}

	void RefreshCount()
	{
		numText.text = buyCount.ToString();
		int moneyCount = GameRuntimeData.Instance.GetMoney();
		int needCount = shopItem.Price * buyCount;
		Color textColor = moneyCount >= needCount ? Color.white : Color.red;
		//---------------------------------------------------------------------------
		//totalCost.text = "花费："+needCount.ToString();
		//---------------------------------------------------------------------------
		//特定位置的翻译【花费显示】
		//---------------------------------------------------------------------------
		totalCost.text = "花费：".GetContent(nameof(ShopUIItem)) + needCount.ToString();
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
		totalCost.color = textColor;
	}

	bool selected = false;

	public void SetSelect(bool active)
	{
		selected = active;
		numContent.gameObject.SetActive(active);
		select.gameObject.SetActive(active);
		if (active)
		{
			buyCount = leftNum > 0 ? 1 : 0;
			RefreshCount();
		}
	}

	public void OnAddBtnClick()
	{
		if (buyCount >= leftNum)
			return;
		buyCount++;
		RefreshCount();
	}

	public void OnReduceBtnClick()
	{
		if (buyCount <= 0)
			return;
		buyCount--;
		RefreshCount();
	}

    public int GetIndex()
	{
		return index;
	}

	public int GetBuyCount()
	{
		return buyCount;
	}

	public void Connect(INavigable up = null, INavigable down = null, INavigable left = null, INavigable right = null)
	{
		Navigation navigation = new Navigation();
		navigation.mode = Navigation.Mode.Explicit;
		navigation.selectOnUp = up?.GetSelectable();
        navigation.selectOnDown = down?.GetSelectable();
        navigation.selectOnLeft = left?.GetSelectable();
        navigation.selectOnRight = right?.GetSelectable();
        this.navigation = navigation;
    }

	public Selectable GetSelectable()
	{
		return this;
	}
    
	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		Select();
    }

	public void Select(bool notifyEvent = true)
	{
        SetSelect(true);
		if (notifyEvent)
			OnShopItemSelect?.Invoke(this);
    }
}
