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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameSetting
{
    public enum Catalog
    {
        Volume,
        SoundEffect,
        Fullscreen,
        Resolution,
        Viewport,
        Difficulty,
    }

    private static AudioSource _audioSource;

    private static AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
            {
                // Game object [AudioManager] 可能尚未生成。
                _audioSource = GameObject.Find("[AudioManager]")?.GetComponent<AudioSource>();
            }

            return _audioSource;
        }
    }

    private static Resolution[] resolutions => Screen.resolutions;

    private static GraphicSetting _graphicSetting;

    private static bool _hasInitialized = false;

    public static void Init()
    {
        Debug.Log("GameSetting init start");

        if (_hasInitialized) return;

        _graphicSetting = GraphicSetting.GlobalSetting;

        InitVolume();
        InitResolution();
        InitFullscreen();
        // InitViewport();
        InitDifficulty();

        _hasInitialized = true;
    }

    public static Dictionary<Catalog, object> GetSettings()
    {
        var result = new Dictionary<Catalog, object>();
        foreach (Catalog setting in Enum.GetValues(typeof(Catalog)))
        {
            switch (setting)
            {
                case Catalog.Resolution:
                    result.Add(Catalog.Resolution, GetResolution());
                    break;
                case Catalog.Volume:
                    result.Add(Catalog.Volume, GetVolume());
                    break;
                case Catalog.SoundEffect:
                    result.Add(Catalog.SoundEffect, GetSoundEffect());
                    break;
                case Catalog.Viewport:
                    result.Add(Catalog.Viewport, GetViewportType());
                    break;
                case Catalog.Fullscreen:
                    result.Add(Catalog.Fullscreen, GetFullscreen());
                    break;
                case Catalog.Difficulty:
                    result.Add(Catalog.Difficulty, GetDifficulty());
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Get setting events map.
    /// </summary>
    /// <returns></returns>
    public static Dictionary<Catalog, UnityEvent<object>> GetEventsMap()
    {
        var eventsMap = new Dictionary<Catalog, UnityEvent<object>>();
        foreach (Catalog setting in Enum.GetValues(typeof(Catalog)))
        {
            var e = new UnityEvent<object>();
            switch (setting)
            {
                case Catalog.Resolution:
                    e.AddListener(SetResolution);
                    break;
                case Catalog.Volume:
                    e.AddListener(SetVolume);
                    break;
                case Catalog.Viewport:
                    e.AddListener(SetViewport);
                    break;
                case Catalog.Fullscreen:
                    e.AddListener(SetFullScreen);
                    break;
                default:
                    e.AddListener(NotImplemented);
                    break;
            }

            eventsMap.Add(setting, e);
        }

        return eventsMap;
    }

    #region Resolution

    private static void InitResolution()
    {
#if !UNITY_ANDROID
        var key = GameConst.PLAYER_PREF_RESOLUTION;

        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetInt(key);
            Resolution resolution = resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
#endif
    }

    private static void SetResolution(object index)
    {
        if (index is int value)
        {
            Resolution resolution = resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt(GameConst.PLAYER_PREF_RESOLUTION, value);
        }
        else
        {
            Debug.LogError($"SetResolution: 参数必须是float.");
        }
    }

    /// <summary>
    /// Return current resolution index.
    /// </summary>
    /// <returns>Return -1 if no pref key</returns>
    private static int GetResolution()
    {
        var result = PlayerPrefs.HasKey(GameConst.PLAYER_PREF_RESOLUTION)
            ? PlayerPrefs.GetInt(GameConst.PLAYER_PREF_RESOLUTION)
            : -1;

        return result;
    }

    #endregion

    #region Volume

    public static void InitVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = GetVolume();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="volume">确保是float type.</param>
    private static void SetVolume(object volume)
    {
        if (volume is float value)
        {
            if (audioSource != null)
            {
                audioSource.volume = value;
            }

            PlayerPrefs.SetFloat(GameConst.PLAYER_PREF_VOLUME, value);
        }
        else
        {
            Debug.LogError($"SetVolume: 参数必须是float.");
        }
    }

    private static float GetVolume()
    {
        float result = 1;
        var key = GameConst.PLAYER_PREF_VOLUME;
        if (PlayerPrefs.HasKey(key))
        {
            result = PlayerPrefs.GetFloat(key);
        }

        return result;
    }

    #endregion

    #region Fullscreen

    private static void InitFullscreen()
    {
#if !UNITY_ANDROID
        Screen.fullScreen = GetFullscreen() == 1;
#endif
    }

    private static int GetFullscreen()
    {
        const string key = GameConst.PLAYER_PREF_FULLSCREEN;
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : 1;
    }

    private static void SetFullScreen(object mode)
    {
        const string key = GameConst.PLAYER_PREF_FULLSCREEN;
        if (mode is int value)
        {
            PlayerPrefs.SetInt(key, value);
            Screen.fullScreen = value == 1;
        }
        else
        {
            Debug.LogError($"SetWindowMode: 参数必须是int.");
        }
    }

    #endregion

    #region Difficulty

    private static void InitDifficulty()
    {
    }

    private static int GetDifficulty()
    {
        return 0;
    }

    /*游戏难度，暂未实现*/
    public static void SetGameDifficulty(int index)
    {
    }

    #endregion

    #region SoundEffect
    
    private static float GetSoundEffect()
    {
        return 1;
    }
    
    /*音效，暂未实现*/
    public static void SetSoundEffect(float volume)
    {
    }
    
    #endregion

    #region Viewport

    private static void InitViewport()
    {
        var viewportValue = GetViewportType();
        GameViewPortManager.Instance?.SetViewport(
            (GameViewPortManager.ViewportType) (viewportValue)
        );
    }

    private static void SetViewport(object index)
    {
        if (index is int value)
        {
            const string key = GameConst.PLAYER_PREF_VIEWPORT_TYPE;
            GameViewPortManager.Instance.SetViewport((GameViewPortManager.ViewportType) (value));
            PlayerPrefs.SetInt(key, value);
        }
        else
        {
            Debug.LogError($"SetViewport: 参数必须是int.");
        }
    }

    private static int GetViewportType()
    {
        const string key = GameConst.PLAYER_PREF_VIEWPORT_TYPE;
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int) GameViewPortManager.ViewportType.Topdown;
    }

    #endregion

    private static void NotImplemented(object o)
    {
    }
}