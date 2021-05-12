using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SystemUIPanel:Jyx2_UIBase
{
    public override UILayer Layer => UILayer.NormalUI;
    protected override void OnCreate()
    {
        InitTrans();

        BindListener(MainMenuButton_Button, delegate 
        {
            Jyx2_UIManager.Instance.HideUI("SystemUIPanel");
            List<string> selectionContent = new List<string>() { "是", "否" };
            Jyx2_UIManager.Instance.ShowUI("ChatUIPanel", ChatType.Selection, "主角", "将丢失未保存进度，是否继续？", selectionContent, new Action<int>((index) =>
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
            Jyx2_UIManager.Instance.HideUI("SystemUIPanel");
            //Application.Quit();
        });

        BindListener(SaveButton_Button, delegate
        {
            Jyx2_UIManager.Instance.ShowUI("SavePanel", new Action<int>((index) => 
            {
                var levelMaster = FindObjectOfType<LevelMaster>();
                levelMaster.OnManuelSave(index);
            }));
        });

        BindListener(LoadButton_Button, () => 
        {
            Jyx2_UIManager.Instance.ShowUI("SavePanel", new Action<int>((index) =>
            {
                StoryEngine.DoLoadGame(index);
                Jyx2_UIManager.Instance.HideUI("SystemUIPanel");
            }));
        });

        BindListener(GraphicSettingsButton_Button, delegate
        {
            Jyx2_UIManager.Instance.HideUI("SystemUIPanel");
            //runTimeHelper.ShowGraphicSettingsPanel();
        });

        BindListener(MainBg_Button, delegate
        {
            Jyx2_UIManager.Instance.HideUI("SystemUIPanel");
            
        });

    }
}
