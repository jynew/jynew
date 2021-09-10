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

public partial class SystemUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;

    private void OnEnable()
    {
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, Close);
    }

    private void OnDisable()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
    }

    protected override void OnCreate()
    {
        InitTrans();

        BindListener(MainMenuButton_Button, delegate 
        {
            Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
            List<string> selectionContent = new List<string>() { "是", "否" };
            Jyx2_UIManager.Instance.ShowUI(nameof(ChatUIPanel), ChatType.Selection, "主角", "将丢失未保存进度，是否继续？", selectionContent, new Action<int>((index) =>
            {
                if(index == 0)
                {
                    LoadingPanel.Create("0_GameStart", () => { });
                }
            }));
        });


        //返回游戏
        BindListener(QuitGameButton_Button, () =>
        {
            Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
            //Application.Quit();
        });

        BindListener(SaveButton_Button, delegate
        {
            Jyx2_UIManager.Instance.ShowUI("SavePanel", new Action<int>((index) => 
            {
                var levelMaster = FindObjectOfType<LevelMaster>();
                levelMaster.OnManuelSave(index);
            }),"选择存档位");
        });

        BindListener(LoadButton_Button, () => 
        {
            Jyx2_UIManager.Instance.ShowUI(nameof(SavePanel), new Action<int>((index) =>
            {
                StoryEngine.DoLoadGame(index);
                Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
            }),"选择读档位");
        });

        BindListener(GraphicSettingsButton_Button, delegate
        {
            Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
            //runTimeHelper.ShowGraphicSettingsPanel();

            Jyx2_UIManager.Instance.ShowUI(nameof(GraphicSettingsPanel));
        });

        BindListener(MainBg_Button, Close);

    }

    void Close()
    {
        Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
    }
}
