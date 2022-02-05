/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
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
	private ScrollRect ItemsArea_ScrollReact;
	private GridLayoutGroup ItemRoot_GridLayout;

	public void InitTrans()
	{
		ItemRoot_RectTransform = transform.Find("ShopScroll/Viewport/ItemRoot").GetComponent<RectTransform>();
		ItemDes_RectTransform = transform.Find("ItemDes").GetComponent<RectTransform>();
		DesText_Text = transform.Find("ItemDes/Viewport/DesText").GetComponent<Text>();
		ScrollItem_RectTransform = transform.Find("ScrollItem").GetComponent<RectTransform>();
		ConfirmBtn_Button = transform.Find("Btns/ConfirmBtn").GetComponent<Button>();
		CloseBtn_Button = transform.Find("Btns/CloseBtn").GetComponent<Button>();
		MoneyNum_Text = transform.Find("MoneyNum").GetComponent<Text>();
		ItemsArea_ScrollReact = transform.Find("ShopScroll").GetComponent<ScrollRect>();
		ItemRoot_GridLayout = transform.Find("ShopScroll/Viewport/ItemRoot").GetComponent<GridLayoutGroup>();
	}
}
