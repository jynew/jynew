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
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class GameSetting
{
    private static GameObject _audioManager;
    private static AudioSource _audioSource;
    private static GraphicSetting _graphicSetting;

    private static Resolution[] _resolutions;

    private static GameSetting _globalSetting;

    public static GameSetting GlobalSetting
    {
        get
        {
            if (_globalSetting == null)
            {
                _globalSetting = new GameSetting();

                _graphicSetting = GraphicSetting.GlobalSetting;
                _audioManager = GameObject.Find("[AudioManager]");
                _audioSource = _audioManager.GetComponent<AudioSource>();
            }

            return _globalSetting;
        }
    }

    public void InitResolution()
    {
#if !UNITY_ANDROID
        _resolutions = Screen.resolutions;

        var key = GameConst.PLAYER_PREF_RESOLUTION;

        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetInt(key);
            Resolution resolution = _resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
#endif
    }

    public void InitWindowDropdown()
    {
        var key = GameConst.PLAYER_PREF_FULLSCREEN;

#if !UNITY_ANDROID
        if (PlayerPrefs.HasKey(key))
        {
            Screen.fullScreen = PlayerPrefs.GetInt(key) == 1;
        }
#endif
    }

    public void InitDifficultyDropdown()
    {
    }

    void InitVolumeSlider()
    {
        var key = GameConst.PLAYER_PREF_FULLSCREEN;

        if (PlayerPrefs.HasKey(key))
        {
            if (_audioSource != null)
            {
                _audioSource.volume = PlayerPrefs.GetFloat(key);
            }
        }
    }

    public void InitSoundEffectSlider()
    {
    }

    public void SetResolution(int index)
    {
        Resolution resolution = _resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        if (_audioManager != null)
        {
            _audioSource.volume = volume;
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

    public void SetViewport(int index, int value)
    {
        var key = GameConst.PLAYER_PREF_VIEWPORT_TYPE;
        PlayerPrefs.SetInt(key, value);
        GameViewPortManager.Instance.SetViewport((GameViewPortManager.ViewportType) (value));
    }

    /*游戏难度，暂未实现*/
    public void SetGameDifficulty(int index)
    {
    }
}