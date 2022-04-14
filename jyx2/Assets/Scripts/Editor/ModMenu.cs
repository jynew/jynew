using HanSquirrel.OpenSource;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

///GG 这个文件是归属项目的，请随意修改调整
public class ModMenu
{
    [MenuItem("一键打包/内置Addressable(修复配置)", false, 10001)]
    public static void BuildDefaultAddressable()
    {
        BasicModUtilsFacade.FixSettings();
        AddressableAssetSettings.BuildPlayerContent();
    }

    [MenuItem("一键打包/所有Addressable(修复配置)", false, 10002)]
    public static void BuildAll()
    {
        BasicModUtilsFacade.Build();
        BuildDefaultAddressable();
    }

    [MenuItem("一键打包/打开MOD输出目录", false, 10201)]
    public static void GotoModOutput() => HSLowMenu.OpenFolder(BasicModUtils.ModOutput);

    [MenuItem("一键打包/打开内置资源输出目录", false, 10202)]
    public static void GotoOutput() => HSLowMenu.OpenFolder(BasicModUtils.InternalOutput);
    

    [MenuItem("一键打包/Addressable/Settings", false, 10401)]
    public static void OpenSettings() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Settings");

    [MenuItem("一键打包/Addressable/Profiles", false, 10402)]
    public static void OpenProfiles() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Profiles");

    [MenuItem("一键打包/Addressable/Groups", false, 10403)]
    public static void OpenGroups() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");

    [MenuItem("一键打包/Addressable/Analyze", false, 10404)]
    public static void OpenAnalyze() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Analyze");

    [MenuItem("一键打包/Addressable/修复配置", false, 10405)]
    public static void FixSettings() => BasicModUtilsFacade.FixSettings();

}
