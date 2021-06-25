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

public partial class BattleMainUIPanel
{
	private RectTransform BattleHpRoot_RectTransform;
	private RectTransform CurrentRole_RectTransform;
	private Text NameText_Text;
	private Text DetailText_Text;
	private Image PreImage_Image;
	private Toggle AutoBattle_Toggle;
	private RectTransform HUDItem_RectTransform;

	public void InitTrans()
	{
		BattleHpRoot_RectTransform = transform.Find("BattleHpRoot").GetComponent<RectTransform>();
		CurrentRole_RectTransform = transform.Find("CurrentRole").GetComponent<RectTransform>();
		NameText_Text = transform.Find("CurrentRole/NameText").GetComponent<Text>();
		DetailText_Text = transform.Find("CurrentRole/DetailText").GetComponent<Text>();
		PreImage_Image = transform.Find("CurrentRole/HeadAvataPre/Mask/PreImage").GetComponent<Image>();
		AutoBattle_Toggle = transform.Find("AutoBattle").GetComponent<Toggle>();
		HUDItem_RectTransform = transform.Find("Prefabs/HUDItem").GetComponent<RectTransform>();

	}
}
