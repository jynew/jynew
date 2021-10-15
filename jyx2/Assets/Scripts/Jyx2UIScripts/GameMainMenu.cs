/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using UnityEngine;
using Jyx2;
using System;
using System.Collections;
using UnityEngine.UI;
using HSFrameWork.Common;

public partial class GameMainMenu : Jyx2_UIBase {

    private enum PanelType
    {
        Home,
        NewGamePage,
        PropertyPage,
        LoadGamePage,
    }
    private RandomPropertyComponent m_randomProperty;

    private PanelType m_panelType;

    private int main_menu_index=0;

    private const int NewGameIndex = 0;
    private const int LoadGameIndex = 1;
    private const int QuitGameIndex = 2;
    
    async void Start()
    {
        //显示loading
        var c = StartCoroutine(ShowLoading());
        
        if (BeforeSceneLoad.loadFinishTask != null)
        {
            await BeforeSceneLoad.loadFinishTask;
        }

        StopCoroutine(c);
        LoadingText.gameObject.SetActive(false);
        homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(true);
    }

    IEnumerator ShowLoading()
    {
        while (true)
        {
            LoadingText.gameObject.SetActive(!LoadingText.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    
    public override UILayer Layer { get => UILayer.MainUI;}
    protected override void OnCreate()
    {
        InitTrans();
        RegisterEvent();
        m_randomProperty = this.StartNewRolePanel_RectTransform.GetComponent<RandomPropertyComponent>();
    }

    private void ChangeSelection(int num)
    {
        if (homeBtnAndTxtPanel_RectTransform.gameObject.active && m_panelType==PanelType.Home)
        {
            main_menu_index += num;
            NewGameButton_Button.gameObject.transform.GetChild(0).GetComponent<Text>().color = (main_menu_index == NewGameIndex)
                ? ColorStringDefine.main_menu_selected
                : ColorStringDefine.main_menu_normal;
            LoadGameButton_Button.gameObject.transform.GetChild(0).GetComponent<Text>().color = (main_menu_index == LoadGameIndex)
                ? ColorStringDefine.main_menu_selected
                : ColorStringDefine.main_menu_normal;
            QuitGameButton_Button.gameObject.transform.GetChild(0).GetComponent<Text>().color = (main_menu_index == QuitGameIndex)
                ? ColorStringDefine.main_menu_selected
                : ColorStringDefine.main_menu_normal;
        }
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        AudioManager.PlayMusic(16);
        m_panelType = PanelType.Home;
        Version_Text.text = allParams[0] as string;
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.DownArrow, () =>
        {
            if(main_menu_index<QuitGameIndex) ChangeSelection(1);
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.UpArrow, () =>
        {
            if(main_menu_index>NewGameIndex) ChangeSelection(-1);
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Space, () =>
        {
            if (main_menu_index == NewGameIndex)
            {
                OnNewGameClicked();
            }else if (main_menu_index == LoadGameIndex)
            {
                OnLoadGameClicked();
            }else if (main_menu_index == QuitGameIndex)
            {
                OnQuitGameClicked();
            }
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, () =>
        {
            if (m_panelType == PanelType.NewGamePage || m_panelType==PanelType.LoadGamePage)//save/ load panel has its own logic to close/ hide themself
            {
                OnBackBtnClicked();
            }
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Return, () =>
        {
            if (m_panelType == PanelType.NewGamePage)
            {
                OnCreateBtnClicked();
            }
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Y, () =>
        {
            if (m_panelType == PanelType.PropertyPage)
            {
                OnCreateRoleYesClick();
            }
        });
        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.N, () =>
        {
            if (m_panelType == PanelType.PropertyPage)
            {
                OnCreateRoleNoClick();
            }
        });
    }
    
    public void OnNewGameClicked()
    {
        OnNewGame();
    }

    // merge to SavePanel.cs
    // modified by eaphone at 2021/05/21
    public void OnLoadGameClicked()
    {
        m_panelType = PanelType.LoadGamePage;
        Jyx2_UIManager.Instance.ShowUI(nameof(SavePanel), new Action<int>((index) =>
        {
            if (!StoryEngine.DoLoadGame(index) && m_panelType==PanelType.LoadGamePage){
                OnNewGame();
            }
        }),"选择读档位", new Action(() =>
        {
            m_panelType = PanelType.Home;
        }));
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
        m_panelType = PanelType.PropertyPage;
        //todo:给玩家提示
        RoleInstance role = GameRuntimeData.Instance.Player;
        role.Name = newName;

        this.InputNamePanel_RectTransform.gameObject.SetActive(false);
        this.StartNewRolePanel_RectTransform.gameObject.SetActive(true);
        m_randomProperty.ShowComponent();
		// generate random property at randomP panel first show
		// added by eaphone at 2021/05/23
        OnCreateRoleNoClick();
    }
    
    void OnNewGame()
    {
        int index = 999;
        var runtime = GameRuntimeData.Create(index);
        m_panelType = PanelType.NewGamePage;
        //默认创建主角

        var player = runtime.AllRoles[0];
        player.Key = "主角";
        player.HeadAvata = "0";
        
        
        //主角初始物品
        foreach (var item in player.Items)
        {
            if(item.IsAdd != 1)
            {
                if (item.Count == 0) item.Count = 1;
                runtime.AddItem(item.Id, item.Count);
                item.IsAdd = 1;
                item.Count = 0;
            }
        }

        player.BindKey();
        runtime.Team.Add(player);

        //开场地图
        var startMap = GameMap.GetGameStartMap();
        runtime.CurrentMap = startMap.Key + "&transport#0";
        runtime.CurrentPos = "";

        this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(false);
        this.InputNamePanel_RectTransform.gameObject.SetActive(true);

        NameInput_InputField.ActivateInputField();
    }

    private void RegisterEvent()
    {
        BindListener(this.NewGameButton_Button,OnNewGameClicked);
        BindListener(this.LoadGameButton_Button,OnLoadGameClicked);
        BindListener(this.QuitGameButton_Button,OnQuitGameClicked);
        BindListener(this.inputSure_Button,OnCreateBtnClicked);
        BindListener(this.inputBack_Button,OnBackBtnClicked);
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
            AudioManager.PlayMusic(GameConst.GAME_START_MUSIC_ID);
            Jyx2_UIManager.Instance.HideUI(nameof(GameMainMenu));
            LevelMaster.Instance.GetPlayer().transform.rotation = Quaternion.Euler(Vector3.zero);
        });
    }
    private void OnCreateRoleNoClick()
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        for (int i = 0; i <= 12; i++)
        {
			GenerateRamdomPro(role, i);
        }
		GenerateRamdomPro(role, 25);//资质
        m_randomProperty.RefreshProperty();
    }
	
	private void GenerateRamdomPro(RoleInstance role, int i)
	{
		string key = i.ToString();
		if (GameConst.ProItemDic.ContainsKey(key)){
			PropertyItem item = GameConst.ProItemDic[key];
			int value = Tools.GetRandomInt(item.DefaulMin, item.DefaulMax);
			role.GetType().GetProperty(item.PropertyName).SetValue(role, value);
		}
	}
	
	private void OnBackBtnClicked()
    {
        this.homeBtnAndTxtPanel_RectTransform.gameObject.SetActive(true);
        this.InputNamePanel_RectTransform.gameObject.SetActive(false);
		m_panelType = PanelType.Home;
	}

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        //释放资源
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.DownArrow);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.UpArrow);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Space);
        //GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Return);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Y);
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.N);
    }

    public void OnOpenURL(string url)
    {
        Tools.openURL(url);
    }
}
