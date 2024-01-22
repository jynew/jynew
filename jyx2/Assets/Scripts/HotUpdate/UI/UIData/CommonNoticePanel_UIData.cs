using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class CommonNoticePanel
{
	private Text TitleText_Text;
	private ScrollRect TextView_ScrollRect;
	private Text ContentText_Text;
	private Button FirstBtn_Button;
	private Text FirstBtn_Text_Text;
	private Button CloseBtn_Button;
	private Text CloseBtn_Text_Text;

	public void InitTrans()
	{
		TitleText_Text = transform.Find("MainBody/TitleText").GetComponent<Text>();
		TextView_ScrollRect = transform.Find("MainBody/TextView").GetComponent<ScrollRect>();
		ContentText_Text = transform.Find("MainBody/TextView/Viewport/Content/ContentText").GetComponent<Text>();
		FirstBtn_Button = transform.Find("MainBody/FirstBtn").GetComponent<Button>();
		FirstBtn_Text_Text = transform.Find("MainBody/FirstBtn/FirstBtn_Text").GetComponent<Text>();
		CloseBtn_Button = transform.Find("MainBody/CloseBtn").GetComponent<Button>();
		CloseBtn_Text_Text = transform.Find("MainBody/CloseBtn/CloseBtn_Text").GetComponent<Text>();

	}
}
