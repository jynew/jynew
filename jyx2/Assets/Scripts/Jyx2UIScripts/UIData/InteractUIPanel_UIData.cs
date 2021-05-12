using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class InteractUIPanel
{
	private RectTransform InteractiveButton_RectTransform;
	private Button MainBg_Button;
	private Text MainText_Text;

	public void InitTrans()
	{
		InteractiveButton_RectTransform = transform.Find("BtnRoot/InteractiveButton").GetComponent<RectTransform>();
		MainBg_Button = transform.Find("BtnRoot/InteractiveButton/MainBg").GetComponent<Button>();
		MainText_Text = transform.Find("BtnRoot/InteractiveButton/MainBg/MainText").GetComponent<Text>();

	}
}
