/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using Jyx2;
using Rewired;
using System.Text;

public class GeneralSettingsPanel : Jyx2_UIBase,ISettingChildPanel
{
    public Dropdown resolutionDropdown;
    public Dropdown windowDropdown;
    public Dropdown difficultyDropdown;
    public Dropdown viewportDropdown;
    public Dropdown languageDropdown;
    public Dropdown debugModeDropdown;
    public Dropdown mobileMoveModeDropdown;

    public Slider volumeSlider;
    public Slider soundEffectSlider;

    public Button JoyStickTestButton;


    private GraphicSetting _graphicSetting;
    Resolution[] resolutions;

    private UnityEvent<float> OnVolumeChange;
    
    private Dictionary<GameSettingManager.Catalog, UnityEvent<object>> _gameSettingEvents;

    private Dictionary<GameSettingManager.Catalog, object> gameSetting => GameSettingManager.settings;

    private void Awake()
    {
        //读取语言文件
        var langPath = Path.Combine(Application.streamingAssetsPath, "Language");
        if (Directory.Exists(langPath)) Directory.CreateDirectory(langPath);//安全性检查
        var languageOptions = new List<Dropdown.OptionData>();
        //绑定到指定的文件夹目录
        var langDir = new DirectoryInfo(langPath);

        if (!langDir.Exists)
            return;

        //检索表示当前目录的文件和子目录
        var fsinfos = langDir.GetFileSystemInfos();
        //遍历检索的文件和子目录
        for (var index = 0; index < fsinfos.Length; index++)
        {
            var fsinfo = fsinfos[index];
            if (fsinfo is FileInfo && fsinfo.Extension == ".json")
            {
                languageOptions.Add(new Dropdown.OptionData(fsinfo.Name.Replace(".json", "")));
            }
        }

        languageDropdown.AddOptions(languageOptions);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GeneralSettingsPanel Start()");

        InitWindowDropdown();
        InitResolutionDropdown();
        InitVolumeSlider();
        InitSoundEffectSlider();
        InitViewportSetting();
        InitDifficultyDropdown();
        InitLanguageSetting();
        InitDebugModeSetting();
        InitMobileMoveModeSetting();
        
        windowDropdown.onValueChanged.AddListener(SetFullscreen);
        windowDropdown.gameObject.SetActive(!Application.isMobilePlatform);
        
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        
        volumeSlider.onValueChanged.AddListener(SetVolume);
        soundEffectSlider.onValueChanged.AddListener(SetSoundEffect);
        viewportDropdown.onValueChanged.AddListener(SetViewport);
        difficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        languageDropdown.onValueChanged.AddListener(SetLanguage);
        debugModeDropdown.onValueChanged.AddListener(SetDebugMode);
        mobileMoveModeDropdown.onValueChanged.AddListener(SetMobileMoveMode);
        mobileMoveModeDropdown.gameObject.SetActive(Application.isMobilePlatform);

        JoyStickTestButton.onClick.AddListener(OnJoyStickTestBtnClick);
        JoyStickTestButton.gameObject.SetActive(!Application.isMobilePlatform);
        Debug.Log("GeneralSettingsPanel Start() END");
    }

    public void ApplySetting()
    {
        
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void InitResolutionDropdown()
    {
        var setting = (string) gameSetting[GameSettingManager.Catalog.Resolution];

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;

            if (!options.Contains(option))
            {
                //如果是当前的分辨率，则记下来
                if (option.Equals(setting))
                {
                    currentIndex = i;
                }
                
                options.Add(option);
            }
                
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitWindowDropdown()
    {
        var setting = (int) gameSetting[GameSettingManager.Catalog.Fullscreen];
        windowDropdown.value = setting;
        windowDropdown.RefreshShownValue();
    }

    public void InitDifficultyDropdown()
    {
        var setting = gameSetting[GameSettingManager.Catalog.Difficulty];
        if (setting is int value)
        {
            difficultyDropdown.value = value;
        }
    }

    void InitVolumeSlider()
    {
        var volume = gameSetting[GameSettingManager.Catalog.Volume];
        if (volume is float value)
        {
            volumeSlider.value = value;
        }
    }

    public void InitSoundEffectSlider()
    {
        var volume = gameSetting[GameSettingManager.Catalog.SoundEffect];
        if (volume is float value)
        {
            soundEffectSlider.value = value;
        }
    }
    
   private void InitViewportSetting()
    {
        var setting = gameSetting[GameSettingManager.Catalog.Viewport];
        if (setting is int value)
        {
            viewportDropdown.value = value;
        }
    }

   
   private void InitLanguageSetting()
   {
       var setting = gameSetting[GameSettingManager.Catalog.Language];
       if (setting is int value)
       {
           languageDropdown.value = value;
       }
   }

   private void InitDebugModeSetting()
   {
       var setting = gameSetting[GameSettingManager.Catalog.DebugMode];
       if (setting is int value)
       {
           debugModeDropdown.value = value;
       }
   }

   private void InitMobileMoveModeSetting()
   {
       var setting = gameSetting[GameSettingManager.Catalog.MobileMoveMode];
       if (setting is int value)
       {
           mobileMoveModeDropdown.value = value;
       }
   }

    private void SetResolution(int index)
    {
        string resolutionText = resolutionDropdown.options[index].text;
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Resolution, resolutionText);
    }

    private void SetVolume(float volume)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Volume, volume);
    }

    private void SetSoundEffect(float volume)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.SoundEffect, volume);
    }

    private void SetFullscreen(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Fullscreen, index);
    }

    private void SetViewport(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Viewport, index);
    }
    
    private void SetDifficulty(int index)
    {
        GameSettingManager.SetGameDifficulty(index);
    }
    
    private void SetLanguage(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Language, languageDropdown.options[index].text);
    }

    private void SetDebugMode(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.DebugMode, index);
    }

    private void SetMobileMoveMode(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.MobileMoveMode, index);
    }

    private void OnJoyStickTestBtnClick()
    {
        Action onCopy = () =>
        {
            var ui = Jyx2_UIManager.Instance.GetUI<CommonNoticePanel>();
            if (ui == null)
                return;
            GUIUtility.systemCopyBuffer = ui.Content;
            CommonTipsUIPanel.ShowPopInfo("调试信息已复制到剪贴版");
        };

        var sb = new StringBuilder(); 
        var allJoySticks = ReInput.controllers.Joysticks;
        
        sb.AppendLine("<color=red>以下为手柄测试输出日志, 如有问题请点击复制后发送给开发者</color>");
        sb.AppendLine();
        foreach (var joyStick in allJoySticks)
        {
            sb.AppendFormat("<color=yellow>[设备名:{0}]</color>  <color=green>识别码:{1}</color>\n", joyStick.hardwareName, joyStick.hardwareTypeGuid);
        }
        
        sb.AppendLine("=======按键列表======");
        var json = Jyx2.InputCore.Jyx2_Input.GetAllJoyStickJsonData();
        sb.Append(json);
        
        
        CommonNoticePanel.ShowNotice("手柄信息", sb.ToString(), onCopy, null, "复制");
    }
}
