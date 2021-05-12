using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BagUIPanel
{
	private RectTransform ItemDes_RectTransform;
	private Text DesText_Text;
	private RectTransform ItemRoot_RectTransform;
	private Button UseBtn_Button;
	private Button CloseBtn_Button;

	public void InitTrans()
	{
		ItemDes_RectTransform = transform.Find("ItemDes").GetComponent<RectTransform>();
		DesText_Text = transform.Find("ItemDes/Viewport/DesText").GetComponent<Text>();
		ItemRoot_RectTransform = transform.Find("ItemScroll/Viewport/ItemRoot").GetComponent<RectTransform>();
		UseBtn_Button = transform.Find("Btns/UseBtn").GetComponent<Button>();
		CloseBtn_Button = transform.Find("Btns/CloseBtn").GetComponent<Button>();

	}
}
