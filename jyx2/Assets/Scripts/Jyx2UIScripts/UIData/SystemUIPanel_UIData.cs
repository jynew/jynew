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

public partial class SystemUIPanel
{
	private Button MainBg_Button;
	private Button SaveButton_Button;
	private Button LoadButton_Button;
	private Button GraphicSettingsButton_Button;
	private Button MainMenuButton_Button;
	private Button QuitGameButton_Button;

	public void InitTrans()
	{
		MainBg_Button = transform.Find("MainBg").GetComponent<Button>();
		SaveButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/SaveButton").GetComponent<Button>();
		LoadButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/LoadButton").GetComponent<Button>();
		GraphicSettingsButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/GraphicSettingsButton").GetComponent<Button>();
		MainMenuButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/MainMenuButton").GetComponent<Button>();
		QuitGameButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/QuitGameButton").GetComponent<Button>();

	}
}
