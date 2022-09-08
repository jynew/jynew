using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsPanel : Jyx2_UIBase
{
    public Transform GeneralSettingsPanel;
    public Transform GraphicPanel;
    public Transform ControlSettingsPanel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        GeneralSettingsPanel.gameObject.SetActive(true);        
        GraphicPanel.gameObject.SetActive(false);
        ControlSettingsPanel.gameObject.SetActive(false);


        GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape,
            GeneralSettingsPanel.GetComponent<GeneralSettingsPanel>().Close);
    }

    private void OnDestroy()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
    }

    protected override void OnCreate()
    {
       
    }
}
