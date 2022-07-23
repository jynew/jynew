/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public partial class SystemUIPanel : Jyx2_UIBase
{
	public override UILayer Layer => UILayer.NormalUI;

	private List<Action> ActionList = new List<Action>();
	private List<Button> ButtonList = new List<Button>();

	private void OnEnable()
	{
		GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, HidePanel);
		GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.UpArrow, () =>
		{
			OnDirectionalUp();
		});
		GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.DownArrow, () =>
		{
			OnDirectionalDown();
		});
		GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Space, () =>
		{
			if (current_selection != -1)
			{
				buttonClickAt(current_selection);
			}
		});

		if (GamepadHelper.GamepadConnected)
			changeCurrentSelection(0);
	}



	private void OnDisable()
	{
		current_selection = -1;
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.DownArrow);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.UpArrow);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Space);
	}

	protected override void OnCreate()
	{
		Jyx2_UIManager.Instance.UIVisibilityToggled += onUiVisibilityToggle;
		InitTrans();
		IsBlockControl = true;
		ActionList.Add(Save);
		ActionList.Add(Load);
		ActionList.Add(GraphicSetting);
		ActionList.Add(Quit2MainMenu);
		ActionList.Add(HidePanel);

		ButtonList.Add(SaveButton_Button);
		ButtonList.Add(LoadButton_Button);
		ButtonList.Add(GraphicSettingsButton_Button);
		ButtonList.Add(MainMenuButton_Button);
		ButtonList.Add(QuitGameButton_Button);

		for (int i = 0; i < ButtonList.Count; i++)
		{
			BindListener(ButtonList[i], ActionList[i]);
		}
	}

	private void onUiVisibilityToggle(Jyx2_UIBase arg1, bool arg2)
	{
		if (arg1 is SavePanel && !arg2 && showingSavePanel)
		{
			showingSavePanel = false;
		}
	}

	bool showingSavePanel = false;

	async void Save()
	{
		showingSavePanel = true;

		//---------------------------------------------------------------------------
		//await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), new Action<int>((index) => 
		//{
		//    var levelMaster = FindObjectOfType<LevelMaster>();
		//    levelMaster.OnManuelSave(index);
		//}),"选择存档位");
		//---------------------------------------------------------------------------
		//特定位置的翻译【存档时候的Title显示】
		//---------------------------------------------------------------------------
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), new Action<int>((index) =>
		{
			var levelMaster = FindObjectOfType<LevelMaster>();
			levelMaster.OnManuelSave(index);
		}), "选择存档位".GetContent(nameof(SystemUIPanel)));
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
	}

	async void Load()
	{
		showingSavePanel = true;

		//---------------------------------------------------------------------------
		//await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), new Action<int>((index) =>
		//{
		//    StoryEngine.DoLoadGame(index);
		//}),"选择读档位");
		//---------------------------------------------------------------------------
		//特定位置的翻译【读档时候的Title显示】
		//---------------------------------------------------------------------------
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), new Action<int>((index) =>
		{
			var summary = GameSaveSummary.Load(index);
			if (summary.ModId != null && !summary.ModId.Equals(RuntimeEnvSetup.CurrentModId))
			{
				HidePanel();
				List<string> selectionContent = new List<string>() {"是(Y)", "否(N)"};
				string msg = "该存档MOD不匹配，载入可能导致数据错乱，是否继续？";
				Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, new Action<int>((index) =>
				{
					if (index == 0)
					{
						StoryEngine.DoLoadGame(index);
					}
				})).Forget();
			}
			else
			{
				StoryEngine.DoLoadGame(index);
			}
		}), "选择读档位".GetContent(nameof(SystemUIPanel)));
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
	}

	async void GraphicSetting()
	{
		HidePanel();
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(GameSettingsPanel));
	}

	async void Quit2MainMenu()
	{
		HidePanel();
		List<string> selectionContent = new List<string>() { "是(Y)", "否(N)" };
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", "将丢失未保存进度，是否继续？", selectionContent, new Action<int>((index) =>
		{
			if (index == 0)
			{
				LoadingPanel.Create(null).Forget();
			}
		}));
	}

	void HidePanel()
	{
		this.gameObject.SetActive(false);
		Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
	}

	protected override bool captureGamepadAxis
	{
		get { return true; }
	}

	protected override bool handleDpadMove()
	{
		if (showingSavePanel)
			return false;

		return base.handleDpadMove();
	}

	protected override void handleGamepadButtons()
	{
		if (showingSavePanel)
			return;

		base.handleGamepadButtons();
		if (gameObject.activeSelf)
			if (GamepadHelper.IsCancel())
				HidePanel();
	}
}
