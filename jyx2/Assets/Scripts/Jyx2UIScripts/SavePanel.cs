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
using Jyx2.Util;
using UnityEngine.EventSystems;
using Jyx2.UINavigation;

public partial class SavePanel : Jyx2_UIBase
{
	[SerializeField]
	private string m_ArchiveItemPath;

	public override UILayer Layer => UILayer.NormalUI;

	Action<int> m_selectCallback;

	private List<ArchiveItem> m_ArchiveItems = new List<ArchiveItem>();


	protected override void OnCreate()
	{
		InitTrans();
		BindListener(BackButton_Button, OnBackClick, false);
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		m_selectCallback = allParams[0] as Action<int>;
        MainText_Text.text = allParams[1] as string;
        
		RefreshSave();
		SelectFirstItem();

    }

	void RefreshSave()
	{
		Action<int, ArchiveItem, int> OnArchiveItemCreate = (idx, item, ArchiveIdx) =>
		{
			item.SetClickCallBack(OnSaveItemClick);
		};

        var idxList = GetArchiveIndexList();
		MonoUtil.GenerateMonoElementsWithCacheList(m_ArchiveItemPath, idxList, m_ArchiveItems, SaveParent_RectTransform, OnArchiveItemCreate);
		NavigateUtil.SetUpNavigation(m_ArchiveItems, idxList.Count, 1);
		SetUpBackButtonNavigation();

    }

    void SelectFirstItem()
	{
		if (m_ArchiveItems.Count > 0)
			EventSystem.current.SetSelectedGameObject(m_ArchiveItems[0].gameObject);
	}

    private void SetUpBackButtonNavigation()
    {
        var navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        if (m_ArchiveItems.Count > 0)
        {
            navigation.selectOnDown = m_ArchiveItems[0].GetSelectable();
        }
        BackButton_Button.navigation = navigation;
    }

    private List<int> GetArchiveIndexList()
	{
        var idxList = new List<int>(GameConst.SAVE_COUNT);
		for (int i = 0; i < GameConst.SAVE_COUNT; i++) 
			idxList.Add(i);
		return idxList;
    }

	void OnSaveItemClick(int index)
	{
		Action<int> tmpCallback = m_selectCallback;
		m_selectCallback = null;

        Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
        tmpCallback?.Invoke(index);
	}

	public void OnBackClick()
	{
        Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
    }
}
