using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class ModPanelNew : Jyx2_UIBase
{
    public Dropdown m_Dropdown;
    public GameObject ModChangedSuggestLabel;
    private List<string> _options;

    public async void Start()
    {
        m_Dropdown.onValueChanged.RemoveAllListeners();
        m_Dropdown.onValueChanged.AddListener(OnValueChanged);
        m_Dropdown.ClearOptions();
        _options = RuntimeEnvSetup.ModDic.Values.Select(x => x.Name).ToList();
        m_Dropdown.AddOptions(_options);
        m_Dropdown.value = m_Dropdown.options.FindIndex(o => RuntimeEnvSetup.ModDic.FirstOrDefault(q => q.Value.Name == o.text).Key == RuntimeEnvSetup.CurrentModId);
        
        ModChangedSuggestLabel.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        var selectMod = RuntimeEnvSetup.ModDic.FirstOrDefault(q => q.Value.Name == _options[m_Dropdown.value]).Key;
        if(selectMod != RuntimeEnvSetup.CurrentModId)
        {
            PlayerPrefs.SetString("CURRENT_MOD_ID", selectMod);
            PlayerPrefs.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        this.Hide();
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
