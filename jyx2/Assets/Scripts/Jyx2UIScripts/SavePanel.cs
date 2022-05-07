/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using Jyx2.Middleware;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;

public partial class SavePanel : Jyx2_UIBase
{

#if UNITY_STANDALONE_WIN
	private const string testDirectory = @"C:\Users";
#elif UNITY_STANDALONE_OSX
	private const string testDirectory = @"~/Desktop";
#else
	private const string testDirectory = "";
#endif

	public override UILayer Layer => UILayer.NormalUI;

	Action<int> m_selectCallback;

	private Action closeCallback;

	protected override void OnCreate()
	{
		InitTrans();
		BindListener(BackButton_Button, OnBackClick, false);
	}

	private void OnEnable()
	{
		if (!IsInGameOverPage)
		{
			GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, OnBackClick);
		}

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
				OnSaveItemClick(current_selection);
			}
		});
	}

	private void OnDisable()
	{
		current_selection = -1;
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.DownArrow);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.UpArrow);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Space);
	}

	public void Show()
	{
		InitTrans();
		this.OnShowPanel(new Action<int>((index) =>
		{
			StoryEngine.DoLoadGame(index);
		}), "");
	}

	volatile static int _current_selection = 0;

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		m_selectCallback = allParams[0] as Action<int>;
		Main_Text.text = allParams[1] as string;
		if (allParams.Length > 2)
		{
			closeCallback = allParams[2] as Action;
		}

		var curScene = SceneManager.GetActiveScene().name;
		var isHouse = (curScene != "0_GameStart" && curScene != "0_BigMap");
		(ImButton_Button.gameObject).SetActive(!isHouse);
		(ExButton_Button.gameObject).SetActive(!isHouse);
		RefreshSave();
		current_selection = _current_selection;
		hiliteSaveItem();
	}

	protected override bool resetCurrentSelectionOnShow => false;

	void ChangeSelection(int num)
	{
		current_selection += num;
		_current_selection = current_selection;
		hiliteSaveItem();
	}

	private void hiliteSaveItem()
	{
		for (int i = 0; i < GameConst.SAVE_COUNT; i++)
		{
			var btn = SaveParent_RectTransform.gameObject.transform.GetChild(i).GetComponent<Button>();
			var text = btn.transform.Find("SummaryText").GetComponent<Text>();
			text.color = i == current_selection
				? ColorStringDefine.save_selected
				: ColorStringDefine.save_normal;
			text.fontStyle = FontStyle.Bold;
		}
	}

	protected override bool captureGamepadAxis
	{
		get { return true; }
	}

	protected override void OnDirectionalDown()
	{
		if (current_selection < 2) ChangeSelection(1);

	}

	protected override void OnDirectionalUp()
	{
		if (current_selection > 0) ChangeSelection(-1);
	}

	protected override void handleGamepadButtons()
	{
		if (gameObject.activeSelf)
			if (GamepadHelper.IsConfirm())
			{
				if (current_selection != -1)
				{
					OnSaveItemClick(current_selection);
				}
			}
			else if (GamepadHelper.IsCancel())
			{
				OnBackClick();
			}
	}

	void RefreshSave()
	{
		HSUnityTools.DestroyChildren(SaveParent_RectTransform);
		cleanupDestroyedButtons();

		for (int i = 0; i < GameConst.SAVE_COUNT; i++)
		{
			var btn = Instantiate(SaveItem_Button, SaveParent_RectTransform);
			//btn.transform.SetParent(SaveParent_RectTransform);
			btn.transform.localScale = Vector3.one;
			btn.name = i.ToString();
			Text title = btn.transform.Find("Title").GetComponent<Text>();
			//---------------------------------------------------------------------------
			//title.text = "存档" + GameConst.GetUPNumber(i+1);
			//---------------------------------------------------------------------------
			//特定位置的翻译【存档界面存档一、存档二、存档三的显示】
			//---------------------------------------------------------------------------
			title.text = "存档".GetContent(nameof(SavePanel)) + GameConst.GetUPNumber(i + 1).GetContent(nameof(SavePanel));
			//---------------------------------------------------------------------------
			//---------------------------------------------------------------------------

			var txt = btn.transform.Find("SummaryText").GetComponent<Text>();

			var summary = GameSaveSummary.Load(i);
			
			string summaryInfo = summary.GetBrief();

			//---------------------------------------------------------------------------
			//txt.text = string.IsNullOrEmpty(summaryInfo) ? "空档位" : summaryInfo;
			//---------------------------------------------------------------------------
			//特定位置的翻译【SavePanel中没有存档显示空档位的显示问题】
			//---------------------------------------------------------------------------
			txt.text = string.IsNullOrEmpty(summaryInfo) ? "空档位".GetContent(nameof(SavePanel)) : summaryInfo;
			//---------------------------------------------------------------------------
			//---------------------------------------------------------------------------

			var date = btn.transform.Find("DateTime").GetComponent<Text>();

			var dateText = GameRuntimeData.GetSaveDate(i)
				?.ToLocalTime() //save time is utc, need to convert to local time first
				.ToString("yyyy年M月d日 H时m分") ?? "";

			date.text = dateText;

			BindListener(btn, new Action(() =>
			{
				OnSaveItemClick(int.Parse(btn.name));
			}), false);
		}
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
		HSUnityTools.DestroyChildren(SaveParent_RectTransform);
	}

	void OnSaveItemClick(int index)
	{
		Action<int> cb = m_selectCallback;
		Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
		cb?.Invoke(index);
	}

	private void OnBackClick()
	{
		if (!IsInGameOverPage)
		{
			Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
			closeCallback?.Invoke();
		}
	}
}
