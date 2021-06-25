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

public partial class BattleOKPanel
{
	private RectTransform Root_RectTransform;
	private Button OKBtn_Button;
	private Button CancelBtn_Button;
	private Text DamageText_Text;

	public void InitTrans()
	{
		Root_RectTransform = transform.Find("Root").GetComponent<RectTransform>();
		OKBtn_Button = transform.Find("Root/BtnRoot/OKBtn").GetComponent<Button>();
		CancelBtn_Button = transform.Find("Root/BtnRoot/CancelBtn").GetComponent<Button>();
		DamageText_Text = transform.Find("Root/SkillInfo/DamageText").GetComponent<Text>();

	}
}
