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

	private int main_menu_index => current_selection;

	private const int NewGameIndex = 0;
	private const int LoadGameIndex = 1;
	private const int SettingsIndex = 2;
	private const int QuitGameIndex = 3;

	private string m_newName;

	async void OnStart()
	{
		MainMenuTitles.SetActive(false);
		//显示loading
		var c = StartCoroutine(ShowLoading());
		StopCoroutine(c);
		await RuntimeEnvSetup.Setup();
		
		LoadingText.gameObject.SetActive(false);
		homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(true);

		var res = await ResLoader.LoadAsset<GameObject>("MainMenuBg.prefab");
		if (res != null)
		{
			var newMainMenuBg = Instantiate(res, this.transform, false);
			newMainMenuBg.transform.SetAsFirstSibling();
		}
		else
		{
			MainMenuTitles.gameObject.SetActive(true);
		}

		JudgeShowReleaseNotePanel();
	}

	private void OnEnable()
	{
		transform.Find("mainPanel/ExtendPanel")?.gameObject.SetActive(true); 
	}

	void JudgeShowReleaseNotePanel()
	{
		//每个更新显示一次 这里就不用Jyx2_PlayerPrefs了
		string key = "RELEASENOTE_" + Application.version;
		if (!PlayerPrefs.HasKey(key))
		{
			ReleaseNote_Panel.gameObject.SetActive(true);
			PlayerPrefs.SetInt(key, 1);
			PlayerPrefs.Save();
		}
	}

	IEnumerator ShowLoading()
	{
		while (true)
		{
			LoadingText.gameObject.SetActive(!LoadingText.gameObject.activeSelf);
			yield return new WaitForSeconds(0.5f);
		}
	}


	public override UILayer Layer { get => UILayer.MainUI; }
	protected override void OnCreate()
	{
		InitTrans();
		RegisterEvent();
		m_randomProperty = this.StartNewRolePanel_RectTransform.GetComponent<RandomPropertyComponent>();
	}



	protected override Color normalButtonColor()
	{
		return ColorStringDefine.main_menu_normal;
	}

	protected override Color selectedButtonColor()
	{
		return ColorStringDefine.main_menu_selected;
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
		transform.Find("mainPanel/ExtendPanel")?.gameObject.SetActive(false); 
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
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
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
		BindListener(this.SettingsButton_Button, OpenSettingsPanel);
		BindListener(this.QuitGameButton_Button, OnQuitGameClicked);
		
		BindListener(this.inputSure_Button, OnCreateBtnClicked, false);
		BindListener(this.inputBack_Button, OnBackBtnClicked, false);
		BindListener(this.YesBtn_Button, OnCreateRoleYesClick, false);
		BindListener(this.NoBtn_Button, OnCreateRoleNoClick, false);
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
		
		transform.Find("mainPanel/ExtendPanel")?.gameObject.SetActive(true);
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
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
		Jyx2_UIManager.Instance.ShowUIAsync(nameof(ModPanelNew)).Forget();
	}
}
