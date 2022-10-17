using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using Jyx2Configs;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModPanelNew : Jyx2_UIBase
{
    public Dropdown m_Dropdown;
    public GameObject ModChangedSuggestLabel;
    private List<string> _options;

    public void Start()
    {
        m_Dropdown.onValueChanged.RemoveAllListeners();
        m_Dropdown.onValueChanged.AddListener(OnValueChanged);
        m_Dropdown.ClearOptions();
        _options = MODProviderBase.Items.Values.Select(x => x.Name).ToList();
        m_Dropdown.AddOptions(_options);
        m_Dropdown.value = m_Dropdown.options.FindIndex(o => MODProviderBase.Items.FirstOrDefault(q => q.Value.Name == o.text).Key == RuntimeEnvSetup.CurrentModId);
        
        ModChangedSuggestLabel.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        var selectMod = MODProviderBase.Items.FirstOrDefault(q => q.Value.Name == _options[m_Dropdown.value]).Key;
        if(selectMod != RuntimeEnvSetup.CurrentModId)
        {
            Jyx2_PlayerPrefs.SetString("CURRENT_MOD_ID", selectMod);
            Jyx2_PlayerPrefs.Save();
            RebootGame();
#if UNITY_EDITOR
            //UnityEditor.EditorApplication.isPlaying = false;
#else
            //Application.Quit();
#endif
        }
        this.Hide();
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

    void OnValueChanged(int index)
    {
        string selectMod = m_Dropdown.options[m_Dropdown.value].text;
        ModChangedSuggestLabel.gameObject.SetActive(selectMod != RuntimeEnvSetup.CurrentModId);
    }
    
    
    public void OnUploadMod()
    {
        
    }

    public void OnOpenURL(string url)
    {
        Jyx2.Middleware.Tools.openURL(url);
    }


    public void OnOpenSteamWorkshop()
    {
        
    }
}
