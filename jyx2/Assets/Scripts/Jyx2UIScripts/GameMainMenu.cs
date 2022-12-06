/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using UnityEngine;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using i18n.TranslatorDef;
using Jyx2.Middleware;
using UnityEngine.UI;

using Jyx2Configs;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.ResourceManagement;
using MOD;
using MOD.UI;
using UnityEngine.SceneManagement;

public partial class GameMainMenu : Jyx2_UIBase
{
	private enum PanelType
	{
		Home,
		NewGamePage,
		PropertyPage,
		LoadGamePage,
	}
	private RandomPropertyComponent m_randomProperty;

	private PanelType m_panelType;

	private string m_newName;

	async void OnStart()
	{
		await RuntimeEnvSetup.Setup();
		LoadMainMenuBackGround().Forget();
    }


	public override UILayer Layer => UILayer.MainUI;

    protected override void OnCreate()
	{
		InitTrans();
		RegisterEvent();
		m_randomProperty = this.StartNewRolePanel_RectTransform.GetComponent<RandomPropertyComponent>();
	}

    private async UniTask LoadMainMenuBackGround()
	{
        DefaultBackGround_RectTransform.gameObject.BetterSetActive(false);
        homeBtnAndTxtPanel_RectTransform.gameObject.BetterSetActive(false);

        var res = await ResLoader.LoadAsset<GameObject>("MainMenuBg.prefab");
        if (res != null)
        {
            var newMainMenuBg = Instantiate(res, transform, false);
            newMainMenuBg.transform.SetAsFirstSibling();
        }
        else
        {
            DefaultBackGround_RectTransform.gameObject.BetterSetActive(true);
        }
        homeBtnAndTxtPanel_RectTransform.gameObject.BetterSetActive(true);
        ReleaseNotePanel.ShowReleaseNoteIfPossible(ReleaseNoteType.Mod);
    }


	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		OnStart();
		AudioManager.PlayMusic(16);
		m_panelType = PanelType.Home;
	}


	public void OnNewGameClicked()
	{
		 
		OnNewGame();
	}

	// merge to SavePanel.cs
	// modified by eaphone at 2021/05/21
	public async void OnLoadGameClicked()
	{
		m_panelType = PanelType.LoadGamePage;
		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SavePanel), new Action<int>((archiveIndex) =>
		{
			var summary = GameSaveSummary.Load(archiveIndex);
            if (summary.IsEmpty())
            {
                StoryEngine.DisplayPopInfo("存档为空");
                return;
            }
            var modId = summary.ModId.ToLower();
			if (!modId.Equals(RuntimeEnvSetup.CurrentModId.ToLower()))
			{
				List<string> selectionContent = new List<string>() {"是", "否"};
				string msg = "该存档MOD不匹配，载入可能导致数据错乱，是否继续？";
				Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, new Action<int>((selection) =>
				{
					if (selection == 0)
					{
						StoryEngine.DoLoadGame(archiveIndex);
					}
				})).Forget();
			}
			else
			{
				if (!StoryEngine.DoLoadGame(archiveIndex) && m_panelType == PanelType.LoadGamePage)
				{
					OnNewGame();
				}
			}
		}), "选择读档位".GetContent(nameof(GameMainMenu)), new Action(() =>
		 {
			 m_panelType = PanelType.Home;
		 }));
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
	}

	public void OnQuitGameClicked()
	{
		ModPanelNew.SwitchSceneTo();
	}

	private void setPlayerName()
	{
		//todo:去掉特殊符号
		if (string.IsNullOrWhiteSpace(m_newName))
			return;

		m_panelType = PanelType.PropertyPage;
		//todo:给玩家提示
		RoleInstance role = GameRuntimeData.Instance.Player;
		role.Name = m_newName;
		m_randomProperty.ShowComponent();
		DoGeneratePlayerRole();
	}

	public void OnCreateBtnClicked()
	{
		if (m_newName == null)
		{
			m_newName = this.NameInput_InputField.text;
		}
		setPlayerName();
		
		this.InputNamePanel_RectTransform.gameObject.SetActive(false);
		this.StartNewRolePanel_RectTransform.gameObject.SetActive(true);
		// generate random property at randomP panel first show
		// added by eaphone at 2021/05/23
		OnCreateRoleNoClick();
	}

	void OnNewGame()
	{
		GameRuntimeData.CreateNew();

		m_panelType = PanelType.NewGamePage;
		this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(false);

		Debug.Log(RuntimeEnvSetup.CurrentModConfig.PlayerName);
		if (!string.IsNullOrEmpty(RuntimeEnvSetup.CurrentModConfig.PlayerName))
		{
			m_newName = RuntimeEnvSetup.CurrentModConfig.PlayerName;
			setPlayerName();
			this.StartNewRolePanel_RectTransform.gameObject.SetActive(true);
		}
		else
		{
			this.InputNamePanel_RectTransform.gameObject.SetActive(true);
			NameInput_InputField.ActivateInputField();
		}
	}

	private void RegisterEvent()
	{
		BindListener(this.NewGameButton_Button, OnNewGameClicked);
		BindListener(this.LoadGameButton_Button, OnLoadGameClicked);
		BindListener(this.GameSettingsButton_Button, OpenSettingsPanel);
		BindListener(this.QuitGameButton_Button, OnQuitGameClicked);
		
		BindListener(this.inputSure_Button, OnCreateBtnClicked);
		BindListener(this.inputBack_Button, OnBackBtnClicked);
		BindListener(this.YesBtn_Button, OnCreateRoleYesClick);
		BindListener(this.NoBtn_Button, OnCreateRoleNoClick);
	}
	public void OnCreateRoleYesClick()
	{
		//reset mode, fix bug or quit game and new game again on main menu goes straight to property panel
		m_panelType = PanelType.Home;
		this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(true);
		this.StartNewRolePanel_RectTransform.gameObject.SetActive(false);
		var loadPara = new LevelMaster.LevelLoadPara();
		loadPara.loadType = LevelMaster.LevelLoadPara.LevelLoadType.StartAtTrigger;
		loadPara.triggerName = "0";
		GameRuntimeData.Instance.startDate = DateTime.Now;
		//加载地图
		var startMap = Jyx2ConfigMap.GetGameStartMap();

		string startTrigger = startMap.GetTagValue("START");
		if (!string.IsNullOrEmpty(startTrigger))
		{
			loadPara.triggerName = startTrigger;
		}
		
		LevelLoader.LoadGameMap(startMap, loadPara, () =>
		{
			//首次进入游戏音乐
			AudioManager.PlayMusic(GameConst.GAME_START_MUSIC_ID);
			Jyx2_UIManager.Instance.HideUI(nameof(GameMainMenu));

			var player = LevelMaster.Instance.GetPlayer();
			player.OnSceneLoad().Forget();
		});
	}
	public void OnCreateRoleNoClick()
	{
		DoGeneratePlayerRole();
	}

	/// <summary>
	/// 参考:https://github.com/jynew/jynew/issues/688
	/// </summary>
	public void DoGeneratePlayerRole(bool cheating = false)
	{
		RoleInstance role = GameRuntimeData.Instance.Player;
		
		//生成基础属性
		for (int i = 0; i <= 12; i++)
		{
			GenerateRamdomPro(role, i, cheating);
		}

		//特殊
		GenerateRamdomPro(role, 20, cheating);
		role.HpInc = cheating ? 7 : Tools.GetRandomInt(3, 7);
		role.MaxHp = role.HpInc * 3 + 29;
		int seed = cheating ? 9 : Tools.GetRandomInt(0, 9);
		if (seed < 2)
		{
			role.IQ = Tools.GetRandomInt(0, 35) + 30;
		}else if (seed <= 7)
		{
			role.IQ = Tools.GetRandomInt(0, 20) + 60;
		}
		else
		{
			role.IQ = Tools.GetRandomInt(0, 20) + 75;
		}

		m_randomProperty.RefreshProperty();
	}

	private void GenerateRamdomPro(RoleInstance role, int i, bool cheating)
	{
		string key = i.ToString();
		if (GameConst.ProItemDic.ContainsKey(key))
		{
			PropertyItem item = GameConst.ProItemDic[key];

			int value = 0;
			if (cheating) //秘籍
			{
				if (i == 0) //作弊状况下内力性质永远中性调和
					value = (int)Jyx2_MpType.Neutral;
				else
					value = item.DefaulMax;
			}
			else
			{
				value = Tools.GetRandomInt(item.DefaulMin, item.DefaulMax);
			}
			role.GetType().GetField(item.PropertyName).SetValue(role, value);	
		}
	}
	

	public void OnBackBtnClicked()
	{
		this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(true);
		this.InputNamePanel_RectTransform.gameObject.SetActive(false);
		m_panelType = PanelType.Home;
		
		
	}

    public void OnReleaseNoteBtnClick()
	{
		ReleaseNotePanel.ShowReleaseNoteAnyway(ReleaseNoteType.Mod);
	}
    

	public void OnOpenURL(string url)
	{
		Tools.openURL(url);
	}

	/// <summary>
	/// 打开设置界面
	/// </summary>
	public void OpenSettingsPanel()
	{
		Jyx2_UIManager.Instance.ShowUIAsync(nameof(GameSettingsPanel)).Forget();
	}

	/// <summary>
	/// 打开mod界面
	/// </summary>
	public void OpenModPanel()
	{
		ModPanelNew.SwitchSceneTo();
	}
}
