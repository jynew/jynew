using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jyx2;
using HSFrameWork.SPojo;
using System;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;

public partial class GameMainMenu : Jyx2_UIBase {

    private enum PanelType
    {
        Home,
        NewGamePage,
        LoadGamePage,
    }
    private RandomPropertyComponent m_randomProperty;

    private PanelType m_panelType;
    
    
    public override UILayer Layer { get => UILayer.MainUI;}
    protected override void OnCreate()
    {
        InitTrans();
        RegisterEvent();
        m_randomProperty = this.StartNewRolePanel_RectTransform.GetComponent<RandomPropertyComponent>();
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        AudioManager.PlayMusic(16);
        m_panelType = PanelType.Home;
    }
    
    public void OnNewGameClicked()
    {
        OnNewGame();
        //savePanelMode = SavePanelMode.New;
        //RefreshSavePanel();
        //m_savePanel.SetActive(true);
    }

    public void OnLoadGameClicked()
    {
        m_panelType = PanelType.LoadGamePage;
        RefreshSavePanel();
    }

    public void OnQuitGameClicked()
    {
        Application.Quit();
    }

    public void OnCreateBtnClicked()
    {
        string newName = this.NameInput_InputField.text;
        //todo:去掉特殊符号
        if (newName.Equals(""))
            return;
        //todo:给玩家提示
        RoleInstance role = GameRuntimeData.Instance.Player;
        role.Name = newName;

        this.InputNamePanel_RectTransform.gameObject.SetActive(false);
        this.StartNewRolePanel_RectTransform.gameObject.SetActive(true);
        m_randomProperty.ShowComponent();
    }

    void RefreshSavePanel()
    {
        this.SavePanel_RectTransform.gameObject.SetActive(true);
        var container = this.savePanelContainer_RectTransform;
        for(int i = 0; i < container.transform.childCount; ++i)
        {
            var button = container.transform.GetChild(i).GetComponent<Button>();
            var summaryText = button.transform.Find("SummaryText").GetComponent<Text>();
            var summaryInfoKey = GameRuntimeData.ARCHIVE_SUMMARY_PREFIX + i;
            if (PlayerPrefs.HasKey(summaryInfoKey))
            {
                summaryText.text = PlayerPrefs.GetString(summaryInfoKey);
            }
            else
            {
                summaryText.text = "空存档位";
            }
        }
    }

    void OnNewGame()
    {
        int index = 999;
        var runtime = GameRuntimeData.Create(index);

        //默认创建主角
        var player = new RoleInstance()
        {
            Name = "小虾米",
            Key = "主角",
            HeadAvata = "0",
        };
        player.BindKey();
        runtime.Team.Add(player);
        runtime.MapRuntimeData.Clear();

        //m_PlayableDirector.Stop();
        //Camera.main.transform.position = new Vector3(2f, 1.2f, 0);
        //Camera.main.transform.rotation = Quaternion.Euler(new Vector3(2f, -45f, 0));
        //this.gameObject.SetActive(false);
        //m_HeadAvataPanel.gameObject.SetActive(true);
        //SceneManager.LoadScene("BuildRole");

        //开场地图
        var startMap = GameMap.GetGameStartMap();
        runtime.CurrentMap = startMap.Key;

        runtime.CurrentPos = "";

        //var loadPara = new LevelMaster.LevelLoadPara();
        //loadPara.loadType = LevelMaster.LevelLoadPara.LevelLoadType.StartAtTrigger;
        //loadPara.triggerName = "Level/Triggers/0";

        ////加载地图
        //LevelLoader.LoadGameMap(startMap, loadPara, "", () =>
        //{
        //    //首次进入游戏音乐
        //    AudioManager.PlayMusic(16);
        //});
        this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(false);
        this.InputNamePanel_RectTransform.gameObject.SetActive(true);
    }

    

    public void OnClickedSaveItem(int index)
    {
        if (!StoryEngine.DoLoadGame(index))
        {
            OnNewGame();
        }
        this.SavePanel_RectTransform.gameObject.SetActive(false);
    }



    private void RegisterEvent()
    {
        BindListener(this.NewGameButton_Button,OnNewGameClicked);
        BindListener(this.LoadGameButton_Button,OnLoadGameClicked);
        BindListener(this.QuitGameButton_Button,OnQuitGameClicked);
        BindListener(this.inputSure_Button,OnCreateBtnClicked);
        BindListener(this.YesBtn_Button,OnCreateRoleYesClick);
        BindListener(this.NoBtn_Button,OnCreateRoleNoClick);
    }
    private void OnCreateRoleYesClick()
    {
        this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(true);
        this.StartNewRolePanel_RectTransform.gameObject.SetActive(false);
        var loadPara = new LevelMaster.LevelLoadPara();
        loadPara.loadType = LevelMaster.LevelLoadPara.LevelLoadType.StartAtTrigger;
        loadPara.triggerName = "Level/Triggers/0";

        //加载地图
        var startMap = GameMap.GetGameStartMap();
        LevelLoader.LoadGameMap(startMap, loadPara, "", () =>
        {
            //首次进入游戏音乐
            AudioManager.PlayMusic(16);
            Jyx2_UIManager.Instance.HideUI("GameMainMenu");
        });
    }
    private void OnCreateRoleNoClick()
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        for (int i = 1; i <= 12; i++)
        {
            string key = i.ToString();
            if (!GameConst.ProItemDic.ContainsKey(key))
                continue;
            PropertyItem item = GameConst.ProItemDic[key];
            int value = Tools.GetRandomInt(item.DefaulMin, item.DefaulMax);
            role.GetType().GetProperty(item.PropertyName).SetValue(role, value);
        }
        m_randomProperty. RefreshProperty();
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        //释放资源
    }
}
