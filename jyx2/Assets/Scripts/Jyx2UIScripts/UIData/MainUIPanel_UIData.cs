using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MainUIPanel
{
	private RectTransform AnimRoot_RectTransform;
	private Text Level_Text;
	private Text Exp_Text;
	private Text Name_Text;
	private Text MapName_Text;
	private Button XiakeButton_Button;
	private Button BagButton_Button;
	private Button MapButton_Button;
	private Button SystemButton_Button;
	private Image Image_Right;

	public void InitTrans()
	{
		AnimRoot_RectTransform = transform.Find("AnimRoot").GetComponent<RectTransform>();
		Level_Text = transform.Find("AnimRoot/PlayerStatus/Level").GetComponent<Text>();
		Exp_Text = transform.Find("AnimRoot/PlayerStatus/Exp").GetComponent<Text>();
		Name_Text = transform.Find("AnimRoot/PlayerStatus/Name").GetComponent<Text>();
		MapName_Text = transform.Find("AnimRoot/PlayerStatus/MapName").GetComponent<Text>();
		XiakeButton_Button = transform.Find("AnimRoot/BtnRoot/XiakeButton").GetComponent<Button>();
		BagButton_Button = transform.Find("AnimRoot/BtnRoot/BagButton").GetComponent<Button>();
		MapButton_Button = transform.Find("AnimRoot/BtnRoot/MapButton").GetComponent<Button>();
		SystemButton_Button = transform.Find("AnimRoot/BtnRoot/SystemButton").GetComponent<Button>();
		// for change main ui panel background image
		// added by eaphone at 2021/05/27
		Image_Right = transform.Find("AnimRoot/Image-right").GetComponent<Image>();
		
	}
}
