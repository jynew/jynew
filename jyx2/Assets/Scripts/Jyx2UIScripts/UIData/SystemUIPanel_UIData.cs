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
