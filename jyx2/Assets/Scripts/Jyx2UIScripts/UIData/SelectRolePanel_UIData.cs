using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SelectRolePanel
{
	private RectTransform RoleParent_RectTransform;
	private Text TitleText_Text;
	private Button ConfirmBtn_Button;
	private Button CancelBtn_Button;

	public void InitTrans()
	{
		RoleParent_RectTransform = transform.Find("RoleScroll/Viewport/RoleParent").GetComponent<RectTransform>();
		TitleText_Text = transform.Find("Title/TitleText").GetComponent<Text>();
		ConfirmBtn_Button = transform.Find("Btns/ConfirmBtn").GetComponent<Button>();
		CancelBtn_Button = transform.Find("Btns/CancelBtn").GetComponent<Button>();

	}
}
