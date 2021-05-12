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
