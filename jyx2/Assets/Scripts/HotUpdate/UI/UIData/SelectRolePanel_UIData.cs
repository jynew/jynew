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

public partial class SelectRolePanel
{
	private GridLayoutGroup RoleParent_GridLayoutGroup;
	private Text TitleText_Text;
	private Button ConfirmBtn_Button;
	private Button CancelBtn_Button;
	private Button AllBtn_Button;

	public void InitTrans()
	{
		RoleParent_GridLayoutGroup = transform.Find("RoleScroll/Viewport/RoleParent").GetComponent<GridLayoutGroup>();
		TitleText_Text = transform.Find("Title/TitleText").GetComponent<Text>();
		ConfirmBtn_Button = transform.Find("Btns/ConfirmBtn").GetComponent<Button>();
		CancelBtn_Button = transform.Find("Btns/CancelBtn").GetComponent<Button>();
		AllBtn_Button = transform.Find("Btns/AllBtn").GetComponent<Button>();

	}
}
