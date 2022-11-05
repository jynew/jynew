using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModPanelNew : Jyx2_UIBase
{
    [SerializeField]
    private Dropdown m_Dropdown;

    [SerializeField]
    private GameObject ModChangedSuggestLabel;

    [SerializeField]
    private Button m_UploadBtn;

    [SerializeField]
    private Button m_SteamWorkShopBtn;

    [SerializeField]
    private Button m_ModWikiBtn;

    [SerializeField]
    private Button m_CloseBtn;

    private List<string> m_ModIds = new List<string>();


     void Awake()
    {
        ModChangedSuggestLabel.gameObject.SetActive(false);
        m_Dropdown.ClearOptions();
        var _options = MODProviderBase.Items.Values.Select(x => x.Name).ToList();
        m_ModIds.AddRange(MODProviderBase.Items.Values.Select(m => m.ModId));
        m_Dropdown.AddOptions(_options);
        var idx = m_ModIds.IndexOf(RuntimeEnvSetup.CurrentModId);
        if (idx == -1) idx = 0;
        m_Dropdown.SetValueWithoutNotify(idx);
        m_Dropdown.onValueChanged.AddListener(OnSelectModChange);
        m_UploadBtn.onClick.AddListener(OnUploadMod);
        m_SteamWorkShopBtn.onClick.AddListener(OnOpenSteamWorkshop);
        m_ModWikiBtn.onClick.AddListener(OnOpenModWiki);
        m_CloseBtn.onClick.AddListener(OnCloseBtnClick);
    }

    void OnDestroy()
    {
        m_Dropdown.onValueChanged.RemoveListener(OnSelectModChange);
        m_UploadBtn.onClick.RemoveListener(OnUploadMod);
        m_SteamWorkShopBtn.onClick.RemoveListener(OnOpenSteamWorkshop);
        m_ModWikiBtn.onClick.RemoveListener(OnOpenModWiki);
        m_CloseBtn.onClick.RemoveListener(OnCloseBtnClick);
    }


    public void OnCloseBtnClick()
    {
        var selectMod = m_ModIds[m_Dropdown.value];
        if(selectMod != RuntimeEnvSetup.CurrentModId)
        {
            Jyx2_PlayerPrefs.SetString("CURRENT_MOD_ID", selectMod);
            Jyx2_PlayerPrefs.Save();
            RebootGame();
        }
        else
            Hide();
    }


    void RebootGame()
    {
        RuntimeEnvSetup.ForceClear();
        Jyx2_UIManager.Instance.CloseAllUI();
        SceneManager.LoadScene("0_MainMenu");
    }

    protected override void OnCreate()
    {
        
    }

    void OnSelectModChange(int index)
    {
        string selectMod = m_Dropdown.options[m_Dropdown.value].text;
        ModChangedSuggestLabel.gameObject.SetActive(selectMod != RuntimeEnvSetup.CurrentModId);
    }
    
    
    public void OnUploadMod()
    {
        Jyx2.Middleware.Tools.openURL("https://steamcommunity.com/sharedfiles/filedetails/?id=2874293059");
    }

    public void OnOpenSteamWorkshop()
    {
        Jyx2.Middleware.Tools.openURL("https://steamcommunity.com/app/2098790/workshop/");
    }

    public void OnOpenModWiki()
    {
        Jyx2.Middleware.Tools.openURL("https://github.com/jynew/jynew/wiki");
    }
}
