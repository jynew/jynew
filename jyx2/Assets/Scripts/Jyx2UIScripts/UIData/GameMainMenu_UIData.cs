/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System.Collections.Generic;
using EZ4i18n;
using UnityEngine;
using UnityEngine.UI;

public partial class GameMainMenu
{
    private RectTransform mainPanel_RectTransform;
    private RectTransform homeBtnAndTxtPanel_RectTransform;
    private Button NewGameButton_Button;
    private Button LoadGameButton_Button;
    private Button QuitGameButton_Button;
    private RectTransform SavePanel_RectTransform;
    private RectTransform savePanelContainer_RectTransform;
    private Button BackButton_Button;
    private RectTransform InfoPanel_RectTransform;
    private RectTransform InputNamePanel_RectTransform;
    private InputField NameInput_InputField;
    private Button inputSure_Button;
    private Button inputBack_Button;
    private RectTransform StartNewRolePanel_RectTransform;
    private Button NoBtn_Button;
    private Button YesBtn_Button;
    private RectTransform PropertyItem_RectTransform;
    private RectTransform PropertyRoot_RectTransform;
    private Text LoadingText;
    private ReleaseNotePanel ReleaseNote_Panel;

    private List<Button> bottomButtons = new List<Button>();

    public void InitTrans()
    {
        mainPanel_RectTransform = transform.Find("mainPanel").GetComponent<RectTransform>();
        homeBtnAndTxtPanel_RectTransform = transform.Find("mainPanel/homeBtnAndTxtPanel").GetComponent<RectTransform>();
        NewGameButton_Button = transform.Find("mainPanel/homeBtnAndTxtPanel/NewGameButton").GetComponent<Button>();
        LoadGameButton_Button = transform.Find("mainPanel/homeBtnAndTxtPanel/LoadGameButton").GetComponent<Button>();
        QuitGameButton_Button = transform.Find("mainPanel/homeBtnAndTxtPanel/QuitGameButton").GetComponent<Button>();
        SavePanel_RectTransform = transform.Find("SavePanel").GetComponent<RectTransform>();
        savePanelContainer_RectTransform = transform.Find("SavePanel/savePanelContainer").GetComponent<RectTransform>();
        BackButton_Button = transform.Find("SavePanel/BackButton").GetComponent<Button>();
        InfoPanel_RectTransform = transform.Find("InfoPanel").GetComponent<RectTransform>();
        InputNamePanel_RectTransform = transform.Find("InputNamePanel").GetComponent<RectTransform>();
        NameInput_InputField = transform.Find("InputNamePanel/NameInput").GetComponent<InputField>();
        inputSure_Button = transform.Find("InputNamePanel/inputSure").GetComponent<Button>();
        inputBack_Button = transform.Find("InputNamePanel/inputBack").GetComponent<Button>();
        StartNewRolePanel_RectTransform = transform.Find("StartNewRolePanel").GetComponent<RectTransform>();
        NoBtn_Button = transform.Find("StartNewRolePanel/NoBtn").GetComponent<Button>();
        YesBtn_Button = transform.Find("StartNewRolePanel/YesBtn").GetComponent<Button>();
        PropertyItem_RectTransform = transform.Find("StartNewRolePanel/PropertyItem").GetComponent<RectTransform>();
        PropertyRoot_RectTransform = transform.Find("StartNewRolePanel/PropertyRoot").GetComponent<RectTransform>();
        LoadingText = transform.Find("mainPanel/LoadingText").GetComponent<Text>();
        ReleaseNote_Panel = transform.Find("ReleaseNotePanel").GetComponent<ReleaseNotePanel>();

        //bottom buttons
        foreach (Transform child in transform.Find("mainPanel/ExtendPanel"))
        {
            var btn = child.GetComponent<Button>();
            bottomButtons.Add(btn);
            // 主界面上方按鈕的翻譯
            btn.GetComponentInChildren<Text>().text = btn.GetComponentInChildren<Text>().text.Translate();
        }

        // 主界面菜單翻譯部分
        NewGameButton_Button.GetComponentInChildren<Text>().text =
            NewGameButton_Button.GetComponentInChildren<Text>().text.Translate(); //重新开始
        LoadGameButton_Button.GetComponentInChildren<Text>().text =
            LoadGameButton_Button.GetComponentInChildren<Text>().text.Translate(); //载入游戏
        transform.Find("mainPanel/homeBtnAndTxtPanel/GameSettingsButton").GetComponentInChildren<Text>().text =
            transform.Find("mainPanel/homeBtnAndTxtPanel/GameSettingsButton").GetComponentInChildren<Text>().text
                .Translate(); //游戏设置
        QuitGameButton_Button.GetComponentInChildren<Text>().text =
            QuitGameButton_Button.GetComponentInChildren<Text>().text.Translate(); //离开游戏
    }
}