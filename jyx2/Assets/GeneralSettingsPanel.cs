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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GeneralSettingsPanel : Jyx2_UIBase
{

    public Dropdown resolutionDropdown;

    public Button m_CloseButton;

    private GraphicSetting _graphicSetting;
    private GameObject audioManager;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        _graphicSetting = GraphicSetting.GlobalSetting;
        audioManager = GameObject.Find("[AudioManager]");

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        m_CloseButton.onClick.AddListener(Close);
    }

    void Close()
    {
        _graphicSetting.Save();
        _graphicSetting.Execute();
        gameObject.SetActive(false);
        Jyx2_UIManager.Instance.HideUI(nameof(GraphicSettingsPanel));
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
            AudioSource audiosource = audioManager.GetComponent<AudioSource>();
            audiosource.volume = volume;
        }
    }

    /*音效，暂未实现*/
    public void SetSoundEffect(float volume)
    {
        if (audioManager != null)
        {
            AudioSource audiosource = audioManager.GetComponent<AudioSource>();
            audiosource.volume = volume;
        }
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
