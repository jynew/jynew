using System.Collections.Generic;
using UnityEngine;

public class AntiAddictionConst
{
    public const string MAIN_CANVAS_NAME = "Prefabs/TaptapAntiAddictionMainCanvas";

    private const string MOBILE_UI_FOLDER = "Prefabs/Mobile";
    private const string STANDALONE_UI_FOLDER = "Prefabs/Standalone";

    public const string TIME_SELECTOR_PANEL_NAME = "TapTapVietnamTimeSelectorPanel";
    
    public const string ID_NUMBER_INPUT_PANEL_NAME = "TapTapChinaIDInputPanel";

    public const string RETRY_ALERT_PANEL_NAME = "TapTapAntiAddictionRetryAlert";

    public const string HEALTH_REMINDER_PANEL_NAME = "TapTapHealthReminderPanel";
    public const string VERIFY_FINISH_PANEL_NAME = "TapTapChinaVerifyFinishPanel";

    public const string HEALTH_PAYMENT_PANEL_NAME = "TapTapHealthPaymentPanel";

    /// <summary>
    /// 获取预制体全路径
    /// </summary>
    /// <param name="prefabName">预制体名字</param>
    /// <param name="isMobile">是否需要移动版 Prefab,否则即为 Standalone 版本</param>
    /// <param name="fallback">如果寻找失败,是否用另一版本 UI 作为备选方案.比如: 找不到 Standalone 版本时, 自动使用移动版 UI</param>
    /// <returns></returns>
    public static string GetPrefabPath(string prefabName, bool isMobile = true, bool fallback = true)
    {
        var firstFolder = isMobile ? MOBILE_UI_FOLDER : STANDALONE_UI_FOLDER;
        var secondFolder = isMobile ? STANDALONE_UI_FOLDER : MOBILE_UI_FOLDER;
        var fullPath = $"{firstFolder}/{prefabName}";
        if (prefabStubs.TryGetValue(fullPath, out bool val) && val)
            return fullPath;
        if (Resources.Load<GameObject>(fullPath) != null)
        {
            prefabStubs[fullPath] = true;
            return fullPath;
        }

        if (!fallback) return null;
        fullPath = $"{secondFolder}/{prefabName}";
        if (prefabStubs.TryGetValue(fullPath, out bool val2) && val2)
            return fullPath;
        if (Resources.Load<GameObject>(fullPath) == null) return null;
        prefabStubs[fullPath] = true;
        return fullPath;
    }

    private static readonly Dictionary<string, bool> prefabStubs = new Dictionary<string, bool>();
}