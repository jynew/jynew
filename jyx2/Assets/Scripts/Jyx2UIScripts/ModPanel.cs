using System.Diagnostics;
using Jyx2;
using Jyx2.Middleware;
using Jyx2.MOD;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class ModPanel : Jyx2_UIBase
{
    protected override void OnCreate()
    {
        InitTrans();
        RefreshScroll();
        
        BindListener(CloseBtn_Button, OnCloseClick, false);
    }

    async void RefreshScroll()
    {
        HSUnityTools.DestroyChildren(ModParent_RectTransform);
        foreach (var modEntry in MODManager.ModEntries)
        {
            var item = ModItem.Create();
            Transform transform1;
            (transform1 = item.transform).SetParent(ModParent_RectTransform);
            transform1.localScale = Vector3.one;
            
            await item.ShowMod(modEntry);
        }
    }
    
    void OnCloseClick()
    {
        Jyx2_UIManager.Instance.HideUI(nameof(ModPanel));
        Application.Quit();
    }

    #region 手柄支持代码
    
    bool gamepadOn;

    void Update()
    {
        var gamepadButtonIcon = CloseBtn_Button.gameObject.transform
            .GetChild(1).GetComponentInChildren<Image>();

        if (gamepadOn != GamepadHelper.GamepadConnected)
        {
            gamepadOn = GamepadHelper.GamepadConnected;
            gamepadButtonIcon.gameObject.SetActive(gamepadOn);
        }

        if (GamepadHelper.IsConfirm()
            || GamepadHelper.IsCancel())
            OnCloseClick();
    }
    #endregion
}