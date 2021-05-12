using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopUIPanel
{
	private RectTransform ItemRoot_RectTransform;
	private RectTransform ItemDes_RectTransform;
	private Text DesText_Text;
	private RectTransform ScrollItem_RectTransform;
	private Button ConfirmBtn_Button;
	private Button CloseBtn_Button;
	private Text MoneyNum_Text;

	public void InitTrans()
	{
		ItemRoot_RectTransform = transform.Find("ShopScroll/Viewport/ItemRoot").GetComponent<RectTransform>();
		ItemDes_RectTransform = transform.Find("ItemDes").GetComponent<RectTransform>();
		DesText_Text = transform.Find("ItemDes/Viewport/DesText").GetComponent<Text>();
		ScrollItem_RectTransform = transform.Find("ScrollItem").GetComponent<RectTransform>();
		ConfirmBtn_Button = transform.Find("Btns/ConfirmBtn").GetComponent<Button>();
		CloseBtn_Button = transform.Find("Btns/CloseBtn").GetComponent<Button>();
		MoneyNum_Text = transform.Find("MoneyNum").GetComponent<Text>();

	}
}
