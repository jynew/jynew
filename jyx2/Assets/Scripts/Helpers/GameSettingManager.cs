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
using System.IO;
using System.Linq;
using System.Threading;
using AClockworkBerry;
using Cysharp.Threading.Tasks;
using i18n;
using i18n.TranslatorDef;
using Jyx2;
using Jyx2.MOD;
using UnityEngine;
using UnityEngine.Events;

public static class GameSettingManager
{
	public enum Catalog
	{
		Volume,
		SoundEffect,
		Fullscreen,
		Resolution,
		Viewport,
		Difficulty,
		Language,
		DebugMode,
		MobileMoveMode,
	}

	/// <summary>
	/// 当前的所有游戏设置。
	/// 根据Catalog读取设置后，需要通过类型转换得到正确的数据类型。
	/// </summary>
	public static Dictionary<Catalog, object> settings { get; private set; } = new Dictionary<Catalog, object>();

	/// <summary>
	/// 游戏设置执行事件的集合。对于某些设置，如果其依赖的实例有其他manager进行管理，需要其manager订阅对应设置类目的事件，注册执行逻辑。
	/// GameSettingManager本身尽量不含有依赖实例，避免因为初始化顺序不同引发null exception。
	/// 对于当前没有manager的设置类目，GameSettingManager为其设定相应的执行逻辑。
	/// </summary>
	private static Dictionary<Catalog, UnityEvent<object>> enforceEvents = new Dictionary<Catalog, UnityEvent<object>>();

	private static Resolution[] resolutions => Screen.resolutions;

	private static GraphicSetting _graphicSetting;

	private static bool _hasInitialized = false;

	public static void Init()
	{
		if (_hasInitialized) return;

		Debug.Log("GameSetting init start");

		_graphicSetting = GraphicSetting.GlobalSetting;

		settings = GetSettings();

		// DEBUG
		PrintSettings();

		// 由GameSettingManager管理的设置生效委托: Resolution, Fullscreen, Language
		// 注册委托，且立刻生效。
		SubscribeEnforceEvent(Catalog.Resolution, SetResolution, true);
		SubscribeEnforceEvent(Catalog.Fullscreen, SetFullScreen, true);
		SubscribeEnforceEvent(Catalog.Language, SetLanguage, true);
		SubscribeEnforceEvent(Catalog.DebugMode, SetDebugMode, true);
		SubscribeEnforceEvent(Catalog.MobileMoveMode, SetMobileMoveMode, true);

		_hasInitialized = true;

		Debug.Log("GameSetting init end");
	}

	private static void PrintSettings()
	{
		var result = "Game settings: ";
		foreach (Catalog setting in Enum.GetValues(typeof(Catalog)))
		{
			result += $"\n{Enum.GetName(typeof(Catalog), setting)}: {settings[setting]}";
		}

		Debug.Log(result);
	}

	/// <summary>
	/// 得到所有游戏设置
	/// </summary>
	private static Dictionary<Catalog, object> GetSettings()
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
				case Catalog.Language:
					result.Add(Catalog.Language, GetLanguage());
					break;
				case Catalog.DebugMode:
					result.Add(Catalog.DebugMode, GetDebugMode());
					break;
				case Catalog.MobileMoveMode:
					result.Add(Catalog.MobileMoveMode, GetMobileMoveMode());
					break;
			}
		}

		return result;
	}

	/// <summary>
	/// 注册设置生效逻辑
	/// </summary>
	/// <param name="setting">设置类别</param>
	/// <param name="action">生效逻辑</param>
	/// <param name="enforceImmediately">是否立刻生效。这个设置的manager初始化时，此参数应该为true。</param>
	public static void SubscribeEnforceEvent(Catalog setting, UnityAction<object> action, bool enforceImmediately)
	{
		if (!enforceEvents.ContainsKey(setting))
		{
			enforceEvents.Add(setting, new UnityEvent<object>());
		}

		enforceEvents[setting].AddListener(action);

		Debug.Log($"GameSettingManager >> subscribe {Enum.GetName(typeof(Catalog), setting)}");

		if (!enforceImmediately) return;

		object value;
		if (settings.TryGetValue(setting, out value))
		{
			if (value != null)
				// enforceEvents[setting].Invoke(value);
				action(value);
		}
	}

	/// <summary>
	/// 注销设置生效逻辑
	/// </summary>
	/// <param name="setting">设置类别</param>
	/// <param name="action">生效逻辑</param>
	public static void UnsubscribeEnforceEvent(Catalog setting, UnityAction<object> action)
	{
		if (enforceEvents.ContainsKey(setting))
		{
			enforceEvents[setting].RemoveListener(action);
		}
	}

	/// <summary>
	/// 更新游戏设置，储存设置。如果有注册的生效委托，执行委托使设置生效。
	/// </summary>
	/// <param name="setting"></param>
	/// <param name="value"></param>
	public static void UpdateSetting(Catalog setting, object value)
	{
		// 记录
		UpdateSettingRecord(setting, value);

		// 某些设置在修改后无需执行委托，例如音效。
		if (!enforceEvents.ContainsKey(setting))
		{
			return;
		}

		// 生效
		enforceEvents[setting].Invoke(value);
	}

	/// <summary>
	/// 储存设置。
	/// </summary>
	/// <param name="setting"></param>
	/// <param name="value"></param>
	private static void UpdateSettingRecord(Catalog setting, object value)
	{
		Debug.Log($"更新游戏设置：{Enum.GetName(typeof(Catalog), setting)}, value {value}。");
		settings[setting] = value;
		switch (setting)
		{
			case Catalog.Resolution:
				Jyx2_PlayerPrefs.SetString(GameConst.PLAYER_PREF_RESOLUTION, (string)value);
				break;
			case Catalog.Fullscreen:
				Jyx2_PlayerPrefs.SetInt(GameConst.PLAYER_PREF_FULLSCREEN, (int)value);
				break;
			case Catalog.Difficulty:
				Jyx2_PlayerPrefs.SetInt(GameConst.PLAYER_PREF_Difficulty, (int)value);
				break;
			case Catalog.SoundEffect:
				Jyx2_PlayerPrefs.SetFloat(GameConst.PLAYER_PREF_SOUND_EFFECT, (float)value);
				break;
			case Catalog.Volume:
				Jyx2_PlayerPrefs.SetFloat(GameConst.PLAYER_PREF_VOLUME, (float)value);
				break;
			case Catalog.Viewport:
				Jyx2_PlayerPrefs.SetInt(GameConst.PLAYER_PREF_VIEWPORT_TYPE, (int)value);
				break;
			case Catalog.DebugMode:
				Jyx2_PlayerPrefs.SetInt(GameConst.PLAYER_PREF_DEBUGMODE, (int)value);
				break;
			case Catalog.MobileMoveMode:
				Jyx2_PlayerPrefs.SetInt(GameConst.PLAYER_MOBILE_MOVE_MODE, (int)value);
				break;
		}
		Debug.Log($"Update validation：{Enum.GetName(typeof(Catalog), setting)}, value {GetSettings()[setting]}。");
	}

	#region Resolution

	private static Vector2Int ParseResolution(string resolutionStr)
	{
		if(string.IsNullOrEmpty(resolutionStr))
			return Vector2Int.zero;
		
		if (!resolutionStr.Contains("x"))
			return Vector2Int.zero;
		
		try
		{
			var tmp = resolutionStr.Split('x');
			int width = int.Parse(tmp[0]);
			int height = int.Parse(tmp[1]);
			return new Vector2Int(width, height);
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
			Jyx2_PlayerPrefs.DeleteKey(GameConst.PLAYER_PREF_RESOLUTION);
		}
		
		return Vector2Int.zero;
	}
	
	private static void InitResolution()
	{
		var key = GameConst.PLAYER_PREF_RESOLUTION;

		if (Jyx2_PlayerPrefs.HasKey(key))
		{
			var value = Jyx2_PlayerPrefs.GetString(key);
			var resolution = ParseResolution(value);
			if (resolution != Vector2Int.zero)
			{
				Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
			}
		}
	}

	private static void SetResolution(object resolutionStr)
	{
		if (resolutionStr is string value)
		{
			var resolution = ParseResolution(value);
			if (resolution != Vector2Int.zero)
			{
				Debug.Log($"分辨率设置为：{resolution}");
				Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
			}
		}
		else
		{
			Debug.LogError("SetResolution: 参数必须是 string.");
		}
	}

	/// <summary>
	/// 返回当前的分辨率设置。如果没有储存分辨率设置，返回符合当前窗口尺寸的分辨率。
	/// </summary>
	private static string GetResolution()
	{
		var result = Jyx2_PlayerPrefs.HasKey(GameConst.PLAYER_PREF_RESOLUTION)
			? Jyx2_PlayerPrefs.GetString(GameConst.PLAYER_PREF_RESOLUTION)
			: GetDefaultResolution();

		return result;
	}

	/// <summary>
	/// 返回符合当前窗口尺寸的分辨率。
	/// </summary>
	/// <returns></returns>
	private static string GetDefaultResolution()
	{
		return $"{Screen.currentResolution.width}x{Screen.currentResolution.height}";
	}

	#endregion

	#region Volume

	private static float GetVolume()
	{
		float result = 1;
		var key = GameConst.PLAYER_PREF_VOLUME;
		if (Jyx2_PlayerPrefs.HasKey(key))
		{
			result = Jyx2_PlayerPrefs.GetFloat(key);
		}

		return result;
	}

	#endregion

	#region Fullscreen

	private static int GetFullscreen()
	{
		const string key = GameConst.PLAYER_PREF_FULLSCREEN;
		return Jyx2_PlayerPrefs.HasKey(key) ? Jyx2_PlayerPrefs.GetInt(key) : 1;
	}

	private static void SetFullScreen(object mode)
	{
#if !UNITY_ANDROID
		if (mode is int value)
		{
			Screen.fullScreen = value == 1;
		}
		else
		{
			Debug.LogError("SetWindowMode: 参数必须是int.");
		}
#endif
	}

	#endregion

	#region Difficulty

	private static int GetDifficulty()
	{
		return Jyx2_PlayerPrefs.HasKey(GameConst.PLAYER_PREF_Difficulty)
			? Jyx2_PlayerPrefs.GetInt(GameConst.PLAYER_PREF_Difficulty)
			: 0;
	}

	#endregion

	#region SoundEffect

	private static float GetSoundEffect()
	{
		float result = 1;
		var key = GameConst.PLAYER_PREF_SOUND_EFFECT;
		if (Jyx2_PlayerPrefs.HasKey(key))
		{
			result = Jyx2_PlayerPrefs.GetFloat(key);
		}

		return result;
	}

	#endregion

	#region Viewport

	private static int GetViewportType()
	{
		const string key = GameConst.PLAYER_PREF_VIEWPORT_TYPE;
		return Jyx2_PlayerPrefs.HasKey(key) ? Jyx2_PlayerPrefs.GetInt(key) : (int)GameViewPortManager.ViewportType.Topdown;
	}

	#endregion


	#region Language

	private static string GetLanguage()
	{
		const string key = GameConst.PLAYER_PREF_LANGUAGE;
		return Jyx2_PlayerPrefs.GetString(key, "默认");
	}
	

	private static void SetLanguage(object mode)
	{
		if (!GlobalAssetConfig.Instance) return;
		if (mode is string value)
		{
			if (value != "默认")
			{
				GlobalAssetConfig.Instance.defaultTranslator = ScriptableObject.CreateInstance<Translator>();
				GlobalAssetConfig.Instance.defaultTranslator.outPath = Path.Combine(Application.streamingAssetsPath,
					"Language");
				GlobalAssetConfig.Instance.defaultTranslator.currentLang =
					(TranslationUtility.LangFlag)Enum.Parse(typeof(TranslationUtility.LangFlag), value, true);
				GlobalAssetConfig.Instance.defaultTranslator.ReadFromJson();
			}
			else
			{
				GlobalAssetConfig.Instance.defaultTranslator = null;
			}
		}
		else
		{
			Debug.LogError("SetWindowMode: 参数必须是string.");
		}
	}

	#endregion
	
	#region debugMode
	private static int GetDebugMode()
	{
		int result = 0;
		var key = GameConst.PLAYER_PREF_DEBUGMODE;
		if (Jyx2_PlayerPrefs.HasKey(key))
		{
			result = Jyx2_PlayerPrefs.GetInt(key);
		}

		return result;
	}

	private static void SetDebugMode(object mode)
	{
		if (!GlobalAssetConfig.Instance) return;
		int debugMode = (int) mode;
		ScreenLogger.Instance.ShowLog = (debugMode == 1);
	}
	
	#endregion
	
	#region MobileMoveMode

	private static int GetMobileMoveMode()
	{
		int result = 0;
		var key = GameConst.PLAYER_MOBILE_MOVE_MODE;
		if (Jyx2_PlayerPrefs.HasKey(key))
		{
			result = Jyx2_PlayerPrefs.GetInt(key);
		}

		return result;
	}

	private static void SetMobileMoveMode(object mode)
	{
		int moveMode = (int) mode;
		MobileMoveMode = (MobileMoveModeType)moveMode;

		if (LevelMaster.Instance != null)
		{
			LevelMaster.Instance.UpdateMobileControllerUI();;
		}
	}

	public static MobileMoveModeType MobileMoveMode = MobileMoveModeType.Joystick;

	public enum MobileMoveModeType
	{
		Joystick = 0, //摇杆
		Click = 1, //点击
	}

	#endregion
}