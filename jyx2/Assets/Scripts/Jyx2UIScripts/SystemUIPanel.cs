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

    private int current_selection = -1;

    private List<Action> ActionList=new List<Action>();
    private List<Button> ButtonList = new List<Button>();
    private void OnEnable()
    {
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, HidePanel);
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.UpArrow, () =>
        {
            if(current_selection>0) ChangeSelection(-1);
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.DownArrow, () =>
        {
            if(current_selection<4) ChangeSelection(1);
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Space, () =>
        {
            if (current_selection != -1)
            {
                OnItemSelect();
            }
        });
        ChangeSelection(0);
    }

    void ChangeSelection(int num)
    {
        current_selection += num;
        for (int i = 0; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.transform.GetChild(0).GetComponent<Text>().color = i == current_selection
                ? ColorStringDefine.system_item_selected
                : ColorStringDefine.system_item_normal;
        }
    }

    void OnItemSelect()
    {
        ActionList[current_selection]?.Invoke();
    }

    private void OnDisable()
    {
        current_selection = -1;
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.DownArrow);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.UpArrow);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Space);
    }

    protected override void OnCreate()
    {
        InitTrans();
        IsBlockControl = true;
        ActionList.Add(Save);
        ActionList.Add(Load);
        ActionList.Add(GraphicSetting);
        ActionList.Add(Quit2MainMenu);
        ActionList.Add(HidePanel);

        ButtonList.Add(SaveButton_Button);
        ButtonList.Add(LoadButton_Button);
        ButtonList.Add(GraphicSettingsButton_Button);
        ButtonList.Add(MainMenuButton_Button);
        ButtonList.Add(QuitGameButton_Button);

        for(int i=0;i<ButtonList.Count;i++)
        {
            BindListener(ButtonList[i], ActionList[i]);
        }
    }

    void Save()
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(SavePanel), new Action<int>((index) => 
        {
            var levelMaster = FindObjectOfType<LevelMaster>();
            levelMaster.OnManuelSave(index);
        }),"选择存档位");
    }
    
    void Load()
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(SavePanel), new Action<int>((index) =>
        {
            StoryEngine.DoLoadGame(index);
        }),"选择读档位");
    }

    void GraphicSetting()
    {
        HidePanel();
        Jyx2_UIManager.Instance.ShowUI(nameof(GraphicSettingsPanel));
    }    
    
    void Quit2MainMenu()
    {
        HidePanel();
        List<string> selectionContent = new List<string>() { "是", "否" };
        Jyx2_UIManager.Instance.ShowUI(nameof(ChatUIPanel), ChatType.Selection, "主角", "将丢失未保存进度，是否继续？", selectionContent, new Action<int>((index) =>
        {
            if(index == 0)
            {
                LoadingPanel.Create("0_GameStart", () => { });
            }
        }));
    }
    
    void HidePanel()
    {
        Jyx2_UIManager.Instance.HideUI(nameof(SystemUIPanel));
    }
}
