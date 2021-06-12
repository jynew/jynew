using Jyx2;
using HSUI;
using UnityEngine;
using UnityEngine.UI;

public class SystemPanelUI : BaseUI
{
    public Button m_CoverButton;
    public Button m_BackButton;
    public Button m_SaveButton;
    public Button m_GraphicSettingsButton;
    public Button m_MainMenuButton;

    public Button m_LoadButton;
    public Button m_QuitButton;

    public GameObject m_SavePanel;
    public GameObject m_SelectionPanel;

    /// <summary>
    /// 切换到选择面板还是存读档面板，为true则是关闭存读档，打开选择面板
    /// </summary>
    /// <param name="isOn"></param>
    public void SwitchToSelectionPanel(bool isOn = true)
    {
        m_SavePanel.SetActive(!isOn);
        m_SelectionPanel.SetActive(isOn);
    }

    public void Awake()
    {
        var levelMaster = FindObjectOfType<LevelMaster>();

        BindListener(m_CoverButton, delegate
        {
            gameObject.SetActive(false);
        });

        BindListener(m_BackButton, delegate
        {
            gameObject.SetActive(false);
        });

        BindListener(m_SaveButton, delegate
        {
            savePanelMode = SavePanelMode.Save;
            RefreshSavePanel();
            SwitchToSelectionPanel(false);
        });

        BindListener(m_LoadButton, () => {
            savePanelMode = SavePanelMode.Load;
            RefreshSavePanel();
            SwitchToSelectionPanel(false);
        });

        BindListener(m_GraphicSettingsButton, delegate
        {
            gameObject.SetActive(false);
            //runTimeHelper.ShowGraphicSettingsPanel();
        });

        BindListener(m_MainMenuButton, delegate
        {
            gameObject.SetActive(false);
            levelMaster.QuitToMainMenu();
        });

        BindListener(m_QuitButton, () => {
            Application.Quit();
        });

        SwitchToSelectionPanel(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    enum SavePanelMode
    {
        Save,
        Load,
    }

    SavePanelMode savePanelMode;


    void RefreshSavePanel()
    {
        var container = m_SavePanel.transform.Find("Panel");
        for (int i = 0; i < container.transform.childCount; ++i)
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

    public void OnClickedSaveItem(int index)
    {
        if (savePanelMode == SavePanelMode.Save)
        {
            var levelMaster = FindObjectOfType<LevelMaster>();
            levelMaster.OnManuelSave(index);
        }
        else if (savePanelMode == SavePanelMode.Load)
        {
            StoryEngine.DoLoadGame(index);
        }
        SwitchToSelectionPanel(true);
    }
}
