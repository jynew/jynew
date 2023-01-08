using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.UINavigation;
using Jyx2.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOver : Jyx2_UIBase
{
    [SerializeField]
    private Text name_text;

    [SerializeField]
    private Text date_text;

    [SerializeField]
    private Text note_text;

    [SerializeField]
    private string m_ArchiveItemPath;

    [SerializeField]
    private LayoutGroup m_ItemLayout;

    [SerializeField]
    private Button m_BackButton;
    
    private List<ArchiveItem> m_ArchiveItems = new List<ArchiveItem>();

    protected override void OnCreate()
    {
        m_BackButton.onClick.AddListener(BackToMainMenu);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        IsBlockControl = true;
        name_text.text = GameRuntimeData.Instance.Player.Name;
        DateTime nowTime = DateTime.Now;
        date_text.text = nowTime.Subtract(GameRuntimeData.Instance.startDate).Days.ToString()+"天前";
        note_text.text = nowTime.ToLongDateString().ToString()+"\n在地球某处\n当地失踪人口又增加了\n一例……";
        LoadArchiveItems();
        SelectFirstItem();
    }
  
    private void LoadArchiveItems()
    {
        Action<int, ArchiveItem, int> OnArchiveItemCreate = (idx, item, ArchiveIdx) =>
        {
            item.SetClickCallBack(OnSaveItemClick);
        };

        var idxList = GetArchiveIndexList();
        MonoUtil.GenerateMonoElementsWithCacheList(m_ArchiveItemPath, idxList, m_ArchiveItems, m_ItemLayout.transform, OnArchiveItemCreate);
        NavigateUtil.SetUpNavigation(m_ArchiveItems, idxList.Count, 1);
        SetUpBackButtonNavigation();
    }
    
    private void SetUpBackButtonNavigation()
    {
        var navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        if(m_ArchiveItems.Count > 0)
        {
            navigation.selectOnDown = m_ArchiveItems[0].GetSelectable();
        }
        m_BackButton.navigation = navigation;
    }

    void SelectFirstItem()
    {
        if (m_ArchiveItems.Count > 0)
            EventSystem.current.SetSelectedGameObject(m_ArchiveItems[0].gameObject);
    }

    private void OnSaveItemClick(int idx)
    {
        StoryEngine.DoLoadGame(idx);
    }

    private List<int> GetArchiveIndexList()
    {
        var idxList = new List<int>(GameConst.SAVE_COUNT);
        for (int i = 0; i < GameConst.SAVE_COUNT; i++)
            idxList.Add(i);
        return idxList;
    }


    public void BackToMainMenu()
    {
        LoadingPanel.Create(null).Forget();
    }
}
