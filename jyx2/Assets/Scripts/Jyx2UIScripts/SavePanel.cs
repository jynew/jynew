using Jyx2;
using Jyx2.Middleware;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SavePanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    Action<int> m_selectCallback;
    protected override void OnCreate()
    {
        InitTrans();

        BindListener(BackButton_Button, OnBackClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);

        m_selectCallback = allParams[0] as Action<int>;
        RefreshSave();
    }

    void RefreshSave() 
    {
        HSUnityTools.DestroyChildren(SaveParent_RectTransform);

        for (int i = 0; i < GameConst.SAVE_COUNT; i++)
        {
            var btn = Instantiate(SaveItem_Button);
            btn.transform.SetParent(SaveParent_RectTransform);
            btn.transform.localScale = Vector3.one;
            btn.name = i.ToString();
            Text title = btn.transform.Find("Title").GetComponent<Text>();
            title.text = "存档" + GameConst.GetUPNumber(i+1);

            var txt = btn.transform.Find("SummaryText").GetComponent<Text>();
            var summaryInfoKey = GameRuntimeData.ARCHIVE_SUMMARY_PREFIX + i;
            if (PlayerPrefs.HasKey(summaryInfoKey))
            {
                txt.text = PlayerPrefs.GetString(summaryInfoKey);
            }
            else
            {
                txt.text = "空档位";
            }

            BindListener(btn, () =>
            {
                OnSaveItemClick(btn);
            });
        }
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();

        HSUnityTools.DestroyChildren(SaveParent_RectTransform);
    }

    void OnSaveItemClick(Button btn) 
    {
        Action<int> cb = m_selectCallback;
        Jyx2_UIManager.Instance.HideUI("SavePanel");
        cb?.Invoke(int.Parse(btn.name));
    }

    private void OnBackClick()
    {
        Jyx2_UIManager.Instance.HideUI("SavePanel");
    }
}
