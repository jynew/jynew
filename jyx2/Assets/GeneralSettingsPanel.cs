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

public class GeneralSettingsPanel : Jyx2_UIBase
{

    public Dropdown resolutionDropdown;
    public Dropdown windowDropdown;
    public Dropdown difficultyDropdown;

    public Slider volumeSlider;
    public Slider soundEffectSlider;

    public Button m_CloseButton;

    private GameObject audioManager;
    private AudioSource audiosource;

    private GraphicSetting _graphicSetting;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        _graphicSetting = GraphicSetting.GlobalSetting;
        audioManager = GameObject.Find("[AudioManager]");
        audiosource = audioManager.GetComponent<AudioSource>();

        InitWindowDropdown();
        InitResolutionDropdown();
        InitVolumeSlider();

        m_CloseButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        Save();
        _graphicSetting.Save();
        _graphicSetting.Execute();
        gameObject.SetActive(false);
        Jyx2_UIManager.Instance.HideUI(nameof(GraphicSettingsPanel));
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("volume", audiosource.volume);
        PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("fullscreen", 0);
        }
    }

    public void InitResolutionDropdown()
    {
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
        if (PlayerPrefs.HasKey("resolution"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
            Resolution resol = resolutions[resolutionDropdown.value];
            Screen.SetResolution(resol.width, resol.height, Screen.fullScreen);
        }
        else
        {
            resolutionDropdown.value = currentResolutionIndex;
        }
        resolutionDropdown.RefreshShownValue();

    }

    public void InitWindowDropdown()
    {
        if(PlayerPrefs.HasKey("fullscreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("fullscreen") == 1;
            windowDropdown.value = PlayerPrefs.GetInt("fullscreen");
            windowDropdown.RefreshShownValue();
        }
    }

    public void InitDifficultyDropdown()
    {
    }

    public void InitVolumeSlider()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            audiosource.volume = PlayerPrefs.GetFloat("volume");
            volumeSlider.value = PlayerPrefs.GetFloat("volume");
        }
    }

    public void InitSoundEffectSlider()
    {

    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        if(audioManager != null)
        {
            audiosource.volume = volume;
        }
    }

    /*音效，暂未实现*/
    public void SetSoundEffect(float volume)
    {
        
    }

    public void SetFullscreen(int index)
    {
        Screen.fullScreen = index == 1;
    }

    /*游戏难度，暂未实现*/
    public void SetGameDifficulty(int index)
    {

    }

    protected override void OnCreate()
    {

    }
}
