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

public partial class XiakeUIPanel
{
	private Image PreImage_Image;
	private Text NameText_Text;
	private Text InfoText_Text;
	private Text SkillText_Text;
	private Text ItemsText_Text;
	private Button ButtonSelectWeapon_Button;
	private Button ButtonSelectArmor_Button;
	private Button ButtonSelectBook_Button;
	private Button LeaveButton_Button;
	private RectTransform RoleParent_RectTransform;
	private Button BackButton_Button;
	private Button ButtonHeal_Button;
	private Button ButtonDetoxicate_Button;

	public void InitTrans()
	{
		PreImage_Image = transform.Find("MainContent/HeadAvataPre/Mask/PreImage").GetComponent<Image>();
		NameText_Text = transform.Find("MainContent/NameText").GetComponent<Text>();
		InfoText_Text = transform.Find("MainContent/InfoText").GetComponent<Text>();
		SkillText_Text = transform.Find("MainContent/SkillText").GetComponent<Text>();
		ItemsText_Text = transform.Find("MainContent/ItemsText").GetComponent<Text>();
		ButtonSelectWeapon_Button = transform.Find("MainContent/ButtonSelectWeapon").GetComponent<Button>();
		ButtonSelectArmor_Button = transform.Find("MainContent/ButtonSelectArmor").GetComponent<Button>();
		ButtonSelectBook_Button = transform.Find("MainContent/ButtonSelectBook").GetComponent<Button>();
		LeaveButton_Button = transform.Find("MainContent/LeaveButton").GetComponent<Button>();
		RoleParent_RectTransform = transform.Find("RoleScroll/Viewport/RoleParent").GetComponent<RectTransform>();
		BackButton_Button = transform.Find("BackButton").GetComponent<Button>();
		ButtonHeal_Button = transform.Find("MainContent/ButtonHeal").GetComponent<Button>();
		ButtonDetoxicate_Button = transform.Find("MainContent/ButtonDetoxicate").GetComponent<Button>();

	}
}
