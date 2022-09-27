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

    public async void Start()
    {
        m_Dropdown.onValueChanged.RemoveAllListeners();
        m_Dropdown.onValueChanged.AddListener(OnValueChanged);
        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(await LoadModList());
        m_Dropdown.value = m_Dropdown.options.FindIndex(o => o.text == RuntimeEnvSetup.CurrentModId);
        
        ModChangedSuggestLabel.gameObject.SetActive(false);
    }


    public async UniTask<List<string>> LoadModList()
    {
        List<string> mods = new List<string>();
        foreach (var mod in MODManager.Instance.GetAllModProviders<MODProviderBase>())
        {
            mods = mods.Union(await mod.GetInstalledMods()).ToList();
        }

        return mods;
    }

    public void OnClose()
    {
        var selectMod = m_Dropdown.options[m_Dropdown.value].text;
        if(selectMod != RuntimeEnvSetup.CurrentModId)
        {
            PlayerPrefs.SetString("CURRENT_MOD", selectMod);
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
