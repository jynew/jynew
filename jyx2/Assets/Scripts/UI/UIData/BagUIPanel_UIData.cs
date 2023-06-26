using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BagUIPanel
{
	private RectTransform ItemDes_RectTransform;
	private Text DesText_Text;
	private GridLayoutGroup ItemRoot_GridLayoutGroup;
	private Button UseBtn_Button;
	private Text UseText_Text;
	private Button CloseBtn_Button;
	private Text CloseText_Text;

	public void InitTrans()
	{
		ItemDes_RectTransform = transform.Find("ItemDes").GetComponent<RectTransform>();
		DesText_Text = transform.Find("ItemDes/Viewport/DesText").GetComponent<Text>();
		ItemRoot_GridLayoutGroup = transform.Find("ItemScroll/Viewport/ItemRoot").GetComponent<GridLayoutGroup>();
		UseBtn_Button = transform.Find("Btns/UseBtn").GetComponent<Button>();
		UseText_Text = transform.Find("Btns/UseBtn/UseText").GetComponent<Text>();
		CloseBtn_Button = transform.Find("Btns/CloseBtn").GetComponent<Button>();
		CloseText_Text = transform.Find("Btns/CloseBtn/CloseText").GetComponent<Text>();

	}
}
