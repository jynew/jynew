/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;

public class GeneralSettingsPanel : Jyx2_UIBase
{

    public Dropdown resolutionDropdown;
    public Dropdown windowDropdown;
    public Dropdown difficultyDropdown;
    public Dropdown viewportDropdown;

    public Slider volumeSlider;
    public Slider soundEffectSlider;

    public Button m_CloseButton;

    private GameObject audioManager;
    private AudioSource audiosource;

    private GraphicSetting _graphicSetting;
    Resolution[] resolutions;

    private UnityEvent<float> OnVolumeChange;
    
    private Dictionary<GameSettingManager.Catalog, UnityEvent<object>> _gameSettingEvents;

    private Dictionary<GameSettingManager.Catalog, object> gameSetting => GameSettingManager.settings;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GeneralSettingsPanel Start()");
        _graphicSetting = GraphicSetting.GlobalSetting;
        audioManager = GameObject.Find("[AudioManager]");
        audiosource = audioManager?.GetComponent<AudioSource>();

        InitWindowDropdown();
        InitResolutionDropdown();
        InitVolumeSlider();
        InitViewportSetting();
        
        m_CloseButton.onClick.AddListener(Close);
        
        Debug.Log("GeneralSettingsPanel Start() END");
    }

    public void Close()
    {
        Save();
        _graphicSetting.Save();
        _graphicSetting.Execute();
        Jyx2_UIManager.Instance.HideUI(nameof(GraphicSettingsPanel));
    }

    public void Save()
    {
        // PlayerPrefs.SetFloat("volume", audiosource.volume);
        // PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        // if (Screen.fullScreen)
        // {
        //     PlayerPrefs.SetInt("fullscreen", 1);
        // }
        // else
        // {
        //     PlayerPrefs.SetInt("fullscreen", 0);
        // }
    }

    public void InitResolutionDropdown()
    {
#if !UNITY_ANDROID
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);

        var setting = (int) gameSetting[GameSettingManager.Catalog.Resolution];
        resolutionDropdown.value = setting >= 0 ? setting : currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
#endif
    }

    private void InitWindowDropdown()
    {
#if !UNITY_ANDROID
        var setting = (int) gameSetting[GameSettingManager.Catalog.Fullscreen];
        Debug.Log("InitWindowDropdown " + setting);
        windowDropdown.value = setting;
        windowDropdown.RefreshShownValue();
#endif
    }

    public void InitDifficultyDropdown()
    {
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

    }

    public void SetResolution(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Resolution, index);
    }

    public void SetVolume(float volume)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Volume, volume);
    }

    /*音效，暂未实现*/
    public void SetSoundEffect(float volume)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.SoundEffect, volume);
    }

    public void SetFullscreen(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Fullscreen, index);
    }

    public void SetViewport(int index)
    {
        GameSettingManager.UpdateSetting(GameSettingManager.Catalog.Viewport, index);
    }

    void InitViewportSetting()
    {
        int setting = 0;
        if (PlayerPrefs.HasKey("viewport_type"))
        {
            setting = PlayerPrefs.GetInt("viewport_type");    
        }
        viewportDropdown.value = setting;
    }

    /*游戏难度，暂未实现*/
    public void SetGameDifficulty(int index)
    {

    }

    protected override void OnCreate()
    {

    }
}
