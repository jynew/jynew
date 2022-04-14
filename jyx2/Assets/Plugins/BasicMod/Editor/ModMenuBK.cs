//这个文件是项目用的MOD菜单的原始备份
//新项目请自行拷贝到项目目录，去掉宏，自行修改菜单名称。
//请勿直接修改该文件。
//以免在升级BasicMod的时候引起麻烦

#if BASICMOD_DEV_MODE
using HanSquirrel.OpenSource;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

///GG 这个文件是归属项目的，请随意修改调整
public class ModMenu
{
    [MenuItem("BasicMod/内置Addressable(修复配置)", false, 10001)]
    public static void BuildDefaultAddressable()
    {
        BasicModUtilsFacade.FixSettings();
        AddressableAssetSettings.BuildPlayerContent();
    }

    [MenuItem("BasicMod/所有Addressable(修复配置)", false, 10002)]
    public static void BuildAll()
    {
        BasicModUtilsFacade.Build();
        BuildDefaultAddressable();
    }

    [MenuItem("BasicMod/打开MOD输出目录", false, 10201)]
    public static void GotoModOutput() => HSLowMenu.OpenFolder(BasicModUtils.ModOutput);

    [MenuItem("BasicMod/打开内置资源输出目录", false, 10202)]
    public static void GotoOutput() => HSLowMenu.OpenFolder(BasicModUtils.InternalOutput);
    

    [MenuItem("BasicMod/Addressable_Settings", false, 10401)]
    public static void OpenSettings() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Settings");

    [MenuItem("BasicMod/Addressable_Profiles", false, 10402)]
    public static void OpenProfiles() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Profiles");

    [MenuItem("BasicMod/Addressable_Groups", false, 10403)]
    public static void OpenGroups() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");

    [MenuItem("BasicMod/Addressable_Analyze", false, 10404)]
    public static void OpenAnalyze() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Analyze");

    [MenuItem("BasicMod/Addressable_修复配置", false, 10405)]
    public static void FixSettings() => BasicModUtilsFacade.FixSettings();

}
#endif
