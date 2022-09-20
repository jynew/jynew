using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Jyx2;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class ModPanelNew : Jyx2_UIBase
{
    public Dropdown m_Dropdown;
    public GameObject ModChangedSuggestLabel;

    public void Start()
    {
        m_Dropdown.onValueChanged.RemoveAllListeners();
        m_Dropdown.onValueChanged.AddListener(OnValueChanged);
        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(LoadModList());
        m_Dropdown.value = m_Dropdown.options.FindIndex(o => o.text == RuntimeEnvSetup.CurrentModId);
        
        ModChangedSuggestLabel.gameObject.SetActive(false);
    }


    public List<string> LoadModList()
    {
        if (Application.isEditor || !Application.isMobilePlatform)
        {
            if (File.Exists("modlist.txt"))
            {
                List<string> rst = new List<string>();
                var lines = File.ReadAllLines("modlist.txt");
                foreach (var line in lines)
                {
                    if (line.IsNullOrWhitespace()) continue;
                    rst.Add(line);
                }
                return rst;
            }
        }
        
        //暂不支持自由扩展MOD
        return new List<string> {"JYX2", "SAMPLE"};
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
