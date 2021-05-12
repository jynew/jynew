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
