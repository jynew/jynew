using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class FullSuggestUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.PopupUI;

    Action m_callback;
    string m_content = "";
    string m_title = "";
    float m_canClickTime = 0;
    protected override void OnCreate()
    {
        InitTrans();
        BindListener(MainBg_Button, OnBgClick);
    }

    private void OnBgClick()
    {
        if (Time.unscaledTime < m_canClickTime)
            return;
        Action cb = m_callback;
        Jyx2_UIManager.Instance.HideUI("FullSuggestUIPanel");
        cb?.Invoke();
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);

        m_content = allParams[0] as string;
        if(allParams.Length > 1)
            m_title = allParams[1] as string;
        if(allParams.Length > 2)
            m_callback = allParams[2] as Action;

        Title_Text.text = m_title;
        Content_Text.text = m_content;
        m_canClickTime = Time.unscaledTime + 0.5f;//0.5s后才能点击
        GameUtil.GamePause(true);
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_title = "";
        m_content = "";
        m_callback = null;
        GameUtil.GamePause(false);
    }
}
