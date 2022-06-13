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
using Jyx2.Middleware;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;

public class ShopUIItem : MonoBehaviour
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

	Jyx2ConfigShopItem shopItem;
	int buyCount;
	int index;
	int leftNum;
	private bool currentlyReleased;

	public void Init()
	{
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

	public async UniTaskVoid Refresh(Jyx2ConfigShopItem shopItem, int index, int hasBuyNum)
	{
		this.index = index;
		this.shopItem = shopItem;
		Jyx2ConfigItem item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(shopItem.Id);

		//---------------------------------------------------------------------------
		//desText.text = $"{item.Name}\n价格：{shopItem.Price}";
		//---------------------------------------------------------------------------
		//特定位置的翻译【价格显示】
		//---------------------------------------------------------------------------
		desText.text = string.Format("{0}\n价格：{1}".GetContent(nameof(ShopUIItem)), item.Name, shopItem.Price);
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
		leftNum = shopItem.Count - hasBuyNum;
		leftNum = Tools.Limit(leftNum, 0, shopItem.Count);
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

	void OnAddBtnClick()
	{
		if (buyCount >= leftNum)
			return;
		buyCount++;
		RefreshCount();
	}

	void OnReduceBtnClick()
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


	private void Update()
	{
		if (gameObject.activeSelf && selected)
		{
			//right tab to add, left tab to remove
			if (Input.GetButtonDown("JL1"))
			{
				OnReduceBtnClick();
			}
			else if (Input.GetButtonDown("JR1"))
			{
				OnAddBtnClick();
			}
		}
	}
}
