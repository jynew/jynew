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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class FullSuggestUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.PopupUI;

    Action m_callback;
    string m_content = "";
    int curIndex=0;
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
		if(curIndex<m_content.Length){
			Content_Text.text = GetSubContent();
		}else{
			Action cb = m_callback;
			Jyx2_UIManager.Instance.HideUI(nameof(FullSuggestUIPanel));
			cb?.Invoke();
		}
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);

		m_content = (allParams[0] as string).Trim();
        if(allParams.Length > 1)
            m_title = allParams[1] as string;
        if(allParams.Length > 2)
            m_callback = allParams[2] as Action;

        Title_Text.text = m_title;
		Content_Text.text = GetSubContent();
        m_canClickTime = Time.unscaledTime + 0.5f;//0.5s后才能点击
        GameUtil.GamePause(true);
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        m_title = "";
        m_content = "";
		curIndex=0;
        m_callback = null;
        GameUtil.GamePause(false);
    }
	
	private string GetSubContent(){
		int endIndex=curIndex-1;
		for(int i=0;i<GameConst.MAX_BATTLE_RESULT_LINE_NUM;i++){
			endIndex=m_content.IndexOfAny(new char[]{'\r','\n'},endIndex+1);
			if(endIndex==-1){
				endIndex=m_content.Length;
				break;
			}
		}
		var result=m_content.Substring(curIndex,endIndex-curIndex);
		curIndex=endIndex+1;
		return result;
	}
}
