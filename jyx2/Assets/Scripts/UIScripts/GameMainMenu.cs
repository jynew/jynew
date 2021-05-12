using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jyx2;
using HSFrameWork.SPojo;
using System;
using HSFrameWork.ConfigTable;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameMainMenu : Jyx2_UIBase {

    enum SavePanelMode
    {
        New,
        Load,
    }
    public GameObject m_savePanel;
    public PlayableDirector m_PlayableDirector;

    public Transform m_inputName;
    public Transform m_middleBtns;
    public InputField m_nameInput;

    public RandomPropertyComponent m_randomProperty;

    SavePanelMode savePanelMode;

    public void OnNewGameClicked()
    {
        OnNewGame();
        //savePanelMode = SavePanelMode.New;
        //RefreshSavePanel();
        //m_savePanel.SetActive(true);
    }

    public void OnLoadGameClicked()
    {
        savePanelMode = SavePanelMode.Load;
        RefreshSavePanel();
        m_savePanel.SetActive(true);
    }

    public void OnQuitGameClicked()
    {
        Application.Quit();
    }

    public void OnCreateBtnClicked() 
    {
        string newName = m_nameInput.text;
        if (newName.Equals(""))
            return;
        RoleInstance role = GameRuntimeData.Instance.Player;
        role.Name = newName;

        m_inputName.gameObject.SetActive(false);
        m_randomProperty.gameObject.SetActive(true);
        m_randomProperty.ShowComponent();
    }

    void RefreshSavePanel()
    {
        var container = m_savePanel.transform.Find("Panel");
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
        m_middleBtns.gameObject.SetActive(false);
        m_inputName.gameObject.SetActive(true);
    }

    

    public void OnClickedSaveItem(int index)
    {
        if(savePanelMode == SavePanelMode.New)
        {
            OnNewGame();
        }
        else if(savePanelMode == SavePanelMode.Load)
        {
            StoryEngine.DoLoadGame(index);
        }
        m_savePanel.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        //HSFrameWork.ConfigTable.Inner.ConfigTableBasic.V2Mode = true;
        //AudioManager.PlayMusic(16);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override UILayer Layer { get => UILayer.MainUI;}
    protected override void OnCreate()
    {
        
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        AudioManager.PlayMusic(16);
    }
}
