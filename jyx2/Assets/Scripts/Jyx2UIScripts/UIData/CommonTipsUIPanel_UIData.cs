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
