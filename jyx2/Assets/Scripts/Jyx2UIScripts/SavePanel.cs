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

	public void Show()
	{
		InitTrans();
		this.OnShowPanel(new Action<int>((index) =>
		{
			StoryEngine.DoLoadGame(index);
		}), "");
	}

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

			int btnIdx = i;
			BindListener(btn, () => OnSaveItemClick(btnIdx), false);
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

	public void OnBackClick()
	{
		if (!IsInGameOverPage)
		{
			Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
			closeCallback?.Invoke();
		}
	}
}
