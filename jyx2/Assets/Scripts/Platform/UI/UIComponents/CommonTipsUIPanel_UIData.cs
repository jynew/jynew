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

public partial class CommonTipsUIPanel
{
	private RectTransform CommonTips_RectTransform;
	private RectTransform PopInfoParent_RectTransform;
	private RectTransform MiddleTopMessageSuggest_RectTransform;
	private Text MiddleText_Text;

	public void InitTrans()
	{
		CommonTips_RectTransform = transform.Find("CommonTips").GetComponent<RectTransform>();
		PopInfoParent_RectTransform = transform.Find("CommonTips/PopInfoParent").GetComponent<RectTransform>();
		MiddleTopMessageSuggest_RectTransform = transform.Find("MiddleTopMessageSuggest").GetComponent<RectTransform>();
		MiddleText_Text = transform.Find("MiddleTopMessageSuggest/Bg/MiddleText").GetComponent<Text>();

	}
}
