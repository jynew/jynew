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
using DG.Tweening;


using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum TipsType 
{
    Common = 0,
    MiddleTop = 1,
}

public partial class CommonTipsUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.PopupUI;
    protected override void OnCreate()
    {
        InitTrans();
        MiddleTopMessageSuggest_RectTransform.gameObject.SetActive(false);
    }
    const float POPINFO_FADEOUT_TIME = 1f;
    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);

        TipsType currentType = (TipsType)allParams[0];
        string text = allParams[1] as string;
        float duration = 2f;
        if (allParams.Length > 2)
            duration = (float)allParams[2];

        switch (currentType) 
        {
            case TipsType.Common:
                ShowInfo(text, duration).Forget();
                break;
            case TipsType.MiddleTop:
                ShowMiddleInfo(text, duration).Forget();
                break;
        }
    }

    async UniTaskVoid ShowInfo(string msg, float duration) 
    {
        //初始化
        var popinfoItem = Jyx2ResourceHelper.CreatePrefabInstance("Popinfo");
        popinfoItem.transform.SetParent(PopInfoParent_RectTransform, false);
        popinfoItem.GetComponentInChildren<Text>().text = msg;
        Text mainText = popinfoItem.GetComponentInChildren<Text>();
        Image mainImg = popinfoItem.GetComponent<Image>();
        mainText.color = Color.white;
        mainImg.color = Color.white;

        if (duration > POPINFO_FADEOUT_TIME)
        {
            //FADE相关逻辑
            await Task.Delay(TimeSpan.FromSeconds(POPINFO_FADEOUT_TIME));
            mainText.DOFade(0, POPINFO_FADEOUT_TIME);
            mainImg.DOFade(0, POPINFO_FADEOUT_TIME);
            await Task.Delay(TimeSpan.FromSeconds(duration - POPINFO_FADEOUT_TIME));
        }
        else
        {
            await Task.Delay(TimeSpan.FromSeconds(duration));
        }
        
        Jyx2ResourceHelper.ReleasePrefabInstance(popinfoItem);
    }

    async UniTaskVoid ShowMiddleInfo(string msg, float duration = 2)
    {
        MiddleText_Text.text = msg;
        
        CanvasGroup cg = MiddleTopMessageSuggest_RectTransform.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        
        MiddleTopMessageSuggest_RectTransform.gameObject.SetActive(true);
        MiddleTopMessageSuggest_RectTransform.DOScale(1.2f, duration / 2);
        cg.DOFade(1, duration / 2);
        await Task.Delay(TimeSpan.FromSeconds(duration/2));
        
        MiddleTopMessageSuggest_RectTransform.DOScale(1f, duration / 2);
        cg.DOFade(0, duration / 2);
        await Task.Delay(TimeSpan.FromSeconds(duration));
        MiddleTopMessageSuggest_RectTransform.gameObject.SetActive(false);
    }

    public static void ShowPopInfo(string msg, TipsType tipsType = TipsType.Common)
    {
        Jyx2_UIManager.Instance.ShowUIAsync(nameof(CommonTipsUIPanel), tipsType, msg).Forget();
    }
}
