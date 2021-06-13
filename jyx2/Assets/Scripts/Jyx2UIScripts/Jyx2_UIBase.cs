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
    }

    public void Hide() 
    {
        this.gameObject.SetActive(false);
        this.OnHidePanel();
    }

    public void BindListener(Button button, Action callback, BUTTON_TYPE type = BUTTON_TYPE.CLICK)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                switch (type)
                {
                    case BUTTON_TYPE.CLICK:
                        //AudioManager.PlayEffect("音效.点击");
                        break;
                    case BUTTON_TYPE.SWITCH:
                        //AudioManager.PlayEffect("音效.切换界面");
                        break;
                    default:
                        break;
                }
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
