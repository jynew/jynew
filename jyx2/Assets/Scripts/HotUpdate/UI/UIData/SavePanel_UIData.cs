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

public partial class SavePanel
{
	private Button BackButton_Button;
	private Text MainText_Text;
	private RectTransform SaveParent_RectTransform;
	private Button Export_Button;
	private Button Import_Button;

	public void InitTrans()
	{
		BackButton_Button = transform.Find("TopbarUI/BackButton").GetComponent<Button>();
		MainText_Text = transform.Find("MainText").GetComponent<Text>();
		SaveParent_RectTransform = transform.Find("SaveParent").GetComponent<RectTransform>();
		Export_Button = transform.Find("FileIO/Export").GetComponent<Button>();
		Import_Button = transform.Find("FileIO/Import").GetComponent<Button>();

	}
}
