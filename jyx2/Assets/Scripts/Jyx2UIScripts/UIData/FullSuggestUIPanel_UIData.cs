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

public partial class FullSuggestUIPanel
{
	private Button MainBg_Button;
	private RectTransform ResultRoot_RectTransform;
	private Text Title_Text;
	private Text Content_Text;

	public void InitTrans()
	{
		MainBg_Button = transform.Find("MainBg").GetComponent<Button>();
		ResultRoot_RectTransform = transform.Find("ResultRoot").GetComponent<RectTransform>();
		Title_Text = transform.Find("ResultRoot/Title").GetComponent<Text>();
		Content_Text = transform.Find("ResultRoot/Content").GetComponent<Text>();

	}
}
