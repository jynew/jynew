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

public partial class InteractUIPanel
{
	private RectTransform InteractiveButton_RectTransform1;
	private Button MainBg_Button1;
	private Text MainText_Text1;

	private RectTransform InteractiveButton_RectTransform2;
	private Button MainBg_Button2;
	private Text MainText_Text2;

	public void InitTrans()
	{
		InteractiveButton_RectTransform1 = transform.Find("BtnRoot/InteractiveButton1").GetComponent<RectTransform>();
		MainBg_Button1 = transform.Find("BtnRoot/InteractiveButton1/MainBg").GetComponent<Button>();
		MainText_Text1 = transform.Find("BtnRoot/InteractiveButton1/MainBg/MainText").GetComponent<Text>();

		InteractiveButton_RectTransform2 = transform.Find("BtnRoot/InteractiveButton2").GetComponent<RectTransform>();
		MainBg_Button2 = transform.Find("BtnRoot/InteractiveButton2/MainBg").GetComponent<Button>();
		MainText_Text2 = transform.Find("BtnRoot/InteractiveButton2/MainBg/MainText").GetComponent<Text>();
	}
}
