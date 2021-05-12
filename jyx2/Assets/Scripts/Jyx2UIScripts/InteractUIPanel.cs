using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class InteractUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    Action m_callback;
    protected override void OnCreate()
    {
        InitTrans();

        BindListener(MainBg_Button, OnBtnClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);

        string mainText = allParams[0] as string;
        m_callback = allParams[1] as Action;

        MainText_Text.text = mainText;
    }

    void OnBtnClick() 
    {
        Action temp = m_callback;
        Jyx2_UIManager.Instance.HideUI("InteractUIPanel");
        temp?.Invoke();
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_callback = null;
    }
}
