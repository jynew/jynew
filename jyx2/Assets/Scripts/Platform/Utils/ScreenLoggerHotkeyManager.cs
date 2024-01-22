using System;
using System.Collections;
using System.Collections.Generic;
using AClockworkBerry;
using UnityEngine;

public class ScreenLoggerHotkeyManager : MonoBehaviour
{
    public ScreenLogger screenLogger;
    public bool isloggerOn;

    
    // Start is called before the first frame update
    void Start()
    {
        screenLogger.ShowLog = isloggerOn;
        if(ScreenLogger.IsPersistent)
            DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
            SwitchLoggerOnAndOff();
    }


    void SwitchLoggerOnAndOff()
    {
        isloggerOn = !isloggerOn;
        screenLogger.ShowLog = isloggerOn;
    }
}
