using System;
using System.Collections;
using System.Collections.Generic;
using AClockworkBerry;
using UnityEngine;

public class ScreenLoggerHotkeyManager : MonoBehaviour
{
    public ScreenLogger screenLogger;
    public bool isloggerOn;

    /// <summary>
    /// 呼出日志控制台的快捷键
    /// </summary>
    private const KeyCode HotKey = KeyCode.F12;
    
    // Start is called before the first frame update
    void Start()
    {
        screenLogger.ShowLog = isloggerOn;
        GlobalHotkeyManager.Instance.RegistHotkey(this, HotKey, SwitchLoggerOnAndOff);
        if(ScreenLogger.IsPersistent)
            DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        GlobalHotkeyManager.Instance.UnRegistHotkey(this, HotKey);
    }
    

    void SwitchLoggerOnAndOff()
    {
        isloggerOn = !isloggerOn;
        screenLogger.ShowLog = isloggerOn;
    }
}
