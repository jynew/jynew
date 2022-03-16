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

using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleOKPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.PopupUI;

    Vector3 followPos;
    Action okCallback;
    Action cancelCallback;
    protected override void OnCreate()
    {
        InitTrans();
        BindListener(OKBtn_Button, OnOKClick);
        BindListener(CancelBtn_Button, OnCancelClick);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        followPos = (Vector3)allParams[0];
        if (allParams.Length > 1)
            okCallback = allParams[1] as Action;
        if (allParams.Length > 2)
            cancelCallback = allParams[2] as Action;
        ResetPos();
        ShowSkillDamage();
    }

    void ResetPos() 
    {
        Camera uiCamera = Jyx2_UIManager.Instance.GetUICamera();
        Vector3 pos = uiCamera.WorldToScreenPoint(followPos);
        Root_RectTransform.position = pos;
    }

    void ShowSkillDamage() 
    {
        //NOTHING
    }

    void OnOKClick() 
    {
        Jyx2_UIManager.Instance.HideUI(nameof(BattleOKPanel));
        if (okCallback != null) 
        {
            okCallback();
            okCallback = null;
        }
    }

    void OnCancelClick() 
    {
        Jyx2_UIManager.Instance.HideUI(nameof(BattleOKPanel));
        if (cancelCallback != null)
        {
            cancelCallback();
            cancelCallback = null;
        }
    }

	protected override void handleGamepadButtons()
	{
		if (GamepadHelper.IsConfirm())
            OnOKClick();
        else if (GamepadHelper.IsCancel())
            OnCancelClick();
	}
}
