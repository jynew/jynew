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
using System.Reflection;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public partial class SystemUIPanel : Jyx2_UIBase
{
	public override UILayer Layer => UILayer.NormalUI;


	protected override void OnCreate()
	{
		InitTrans();

		SaveButton_Button.onClick.AddListener(Save);
		LoadButton_Button.onClick.AddListener(Load);
		GraphicSettingsButton_Button.onClick.AddListener(OnSettingBtnClick);
		MainMenuButton_Button.onClick.AddListener(Quit2MainMenu);
		ResumeGameBtn_Button.onClick.AddListener(HidePanel);

    }

	private void OnDestroy()
	{
        SaveButton_Button.onClick.RemoveListener(Save);
        LoadButton_Button.onClick.RemoveListener(Load);
        GraphicSettingsButton_Button.onClick.RemoveListener(OnSettingBtnClick);
        MainMenuButton_Button.onClick.RemoveListener(Quit2MainMenu);
        ResumeGameBtn_Button.onClick.RemoveListener(HidePanel);
    }


	async void Save()
	{
		if (!LevelMaster.IsInWorldMap && RuntimeEnvSetup.CurrentModConfig != null && RuntimeEnvSetup.CurrentModConfig.EnableSaveBigMapOnly)
		{
			StoryEngine.DisplayPopInfo("本MOD只允许在大地图进行存档");
			return;
		}

		Action<int> OnSave = (idx) =>
		{
			if (LevelMaster.Instance != null)
				LevelMaster.Instance.OnManuelSave(idx);
		};

		var titleText = "选择存档位".GetContent(nameof(SystemUIPanel));


        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), OnSave, titleText);
	}

	async void Load()
	{
		Action<int> OnLoadSelect = (archiveIndex) =>
		{
			var summary = GameSaveSummary.Load(archiveIndex);
			if (summary.IsEmpty())
			{
                StoryEngine.DisplayPopInfo("存档为空");
				return;
            }
			if (!summary.ModId.ToLower().Equals(RuntimeEnvSetup.CurrentModId.ToLower()))
			{
				HidePanel();
				List<string> selectionContent = new List<string>() { "是", "否" };
				string msg = "该存档MOD不匹配，载入可能导致数据错乱，是否继续？";
                Action<int> onChatSelect = (selection) =>
                {
                    if (selection == 0)
                    {
                        StoryEngine.DoLoadGame(archiveIndex);
                    }
                };

                Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, onChatSelect).Forget();
			}
			else
			{
				StoryEngine.DoLoadGame(archiveIndex);
			}
		};

		var titleText = "选择读档位".GetContent(nameof(SystemUIPanel));

        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), OnLoadSelect, titleText);

	}

	async void OnSettingBtnClick()
	{
		HidePanel();
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(GameSettingsPanel));
	}

	async void Quit2MainMenu()
	{
		HidePanel();
		List<string> selectionContent = new List<string>() { "是", "否" };
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
		Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
	}
}
