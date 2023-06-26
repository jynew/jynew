/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using UnityEngine;

public partial class InteractUIPanel : Jyx2_UIBase
{
	public override UILayer Layer => UILayer.NormalUI;

	Action m_callback1;
	Action m_callback2;
	private int buttonCount;
	private float lastDpadY;
	private int focusButtonPos
		= 0;

	protected override void OnCreate()
	{
		InitTrans();

		BindListener(MainBg_Button1, () => OnBtnClick(0), false);
		BindListener(MainBg_Button2, () => OnBtnClick(1), false);
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);

		if (allParams == null) return;

		this.buttonCount = allParams.Length / 2;
		MainBg_Button2.gameObject.SetActive(buttonCount == 2);

		MainText_Text1.text = allParams[0] as string;
		m_callback1 = allParams[1] as Action;

		if (MainBg_Button2.gameObject.activeInHierarchy)
		{
			MainText_Text2.text = allParams[2] as string;
			m_callback2 = allParams[3] as Action;
		}
	}

	public void OnBtnClick(int buttonIndex)
	{
		Action temp = buttonIndex == 0 ? m_callback1 : m_callback2;
		Jyx2_UIManager.Instance.HideUI(nameof(InteractUIPanel));
		temp?.Invoke();
	}

	public bool IsButtonAvailable(int buttonIndex)
	{
		if (buttonIndex == 0)
			return MainBg_Button1.gameObject.activeSelf;
		if (buttonIndex == 1)
			return MainBg_Button2.gameObject.activeSelf;
		return false;
	}


	protected override void OnHidePanel()
	{
		base.OnHidePanel();
		m_callback1 = null;
		m_callback2 = null;
	}
}
