/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2.Middleware;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUIAnimator 
{
    void DoShowAnimator();
    void DoHideAnimator();
}

public abstract class Jyx2_UIBase : MonoBehaviour
{
    public virtual UILayer Layer { get; } = UILayer.NormalUI;
    public virtual bool IsOnly { get; } = false;//同一层只能单独存在
    public virtual bool IsBlockControl { get; set; } = false;
    private bool IsChangedBlockControl = false;
    protected abstract void OnCreate();

    protected virtual void OnShowPanel(params object[] allParams) { }
    protected virtual void OnHidePanel() { }

    public void Init() 
    {
        var rt = GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        OnCreate();
    }

    public void Show(params object[] allParams) 
    {
        this.gameObject.SetActive(true);
        this.transform.SetAsLastSibling();
        this.OnShowPanel(allParams);
        if (this is IUIAnimator) 
        {
            (this as IUIAnimator).DoShowAnimator();
        }

        if (IsBlockControl && !IsChangedBlockControl && !BattleManager.Instance.IsInBattle && LevelMaster.Instance.IsPlayerCanControl())
        {
            IsChangedBlockControl = true;
            LevelMaster.Instance.SetPlayerCanController(false);
        }
    }

    public void Hide() 
    {
        this.gameObject.SetActive(false);
        this.OnHidePanel();
        if (IsBlockControl && IsChangedBlockControl)
        {
            IsChangedBlockControl = false;
            LevelMaster.Instance.SetPlayerCanController(true);
        }
    }

    public void BindListener(Button button, Action callback)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(()=>
            {
                callback();
            });
            var nav = Navigation.defaultNavigation;
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
        }
    }

    public void ClearChildren(Transform transform)
    {
        HSUnityTools.DestroyChildren(transform);
    }
}
