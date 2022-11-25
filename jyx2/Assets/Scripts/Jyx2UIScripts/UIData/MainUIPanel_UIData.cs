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

public partial class MainUIPanel
{
	private RectTransform AnimRoot_RectTransform;
	private Text Name_Text;
	private Text Level_Text;
	private Text Exp_Text;
	private Text MapName_Text;
	private Image SystemButton_Image;
	private Button SystemButton_Button;
	private Button BagButton_Button;
	private Button XiakeButton_Button;
	private Text ComassText_Text;

	public void InitTrans()
	{
		AnimRoot_RectTransform = transform.Find("AnimRoot").GetComponent<RectTransform>();
		Name_Text = transform.Find("AnimRoot/PlayerStatus/Name").GetComponent<Text>();
		Level_Text = transform.Find("AnimRoot/PlayerStatus/Level").GetComponent<Text>();
		Exp_Text = transform.Find("AnimRoot/PlayerStatus/Exp").GetComponent<Text>();
		MapName_Text = transform.Find("AnimRoot/PlayerStatus/MapName").GetComponent<Text>();
		SystemButton_Image = transform.Find("AnimRoot/Image-right/BtnRoot/SystemButton").GetComponent<Image>();
		SystemButton_Button = transform.Find("AnimRoot/Image-right/BtnRoot/SystemButton").GetComponent<Button>();
		BagButton_Button = transform.Find("AnimRoot/Image-right/BtnRoot/BagButton").GetComponent<Button>();
		XiakeButton_Button = transform.Find("AnimRoot/Image-right/BtnRoot/XiakeButton").GetComponent<Button>();
		ComassText_Text = transform.Find("AnimRoot/Compass/ComassText").GetComponent<Text>();

	}
}
