//这个是BasicMod开发者预留的，请勿删除
#if BASICMOD_DEV_MODE
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace HanSquirrel.OpenSource
{
    ///仅仅测试使用
    public class BasicModTestMenu
    {
        //[MenuItem("GG/测试FixProfile")]
        public static void FixModProfile() => BasicModUtils.FixModProfile("Icons", out _);

        //[MenuItem("GG/测试FixAddrSettings")]
        public static void FixAddrAssetSettings()
        {
            var modName = "Icons";
            if (BasicModUtils.FixModProfile(modName, out var profileId))
                BasicModUtils.FixRootSettingsForMod(modName, profileId);
        }

        //[MenuItem("GG/测试FixGroupSettings")]
        private static void FixAllModGroups()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.groups.ForEach(g => BasicModUtils.FixGroupSettings(settings, g));
        }

        //[MenuItem("GG/测试构建单个")]
        public static void TestBuildSingle() => BasicModUtils.Build("Icons");

        //[MenuItem("GG/测试构建所有MOD")]
        public static void TestBuildMods()
        {
            BasicModUtils.Build(false);
        }

        //[MenuItem("GG/测试构建内置")]
        public static void TestBuildInternal() => AddressableAssetSettings.BuildPlayerContent();

        //[MenuItem("GG/测试构建所有")]
        public static void TestBuild()
        {
            TestBuildMods();
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("内置资源打包完成");
        }

        //[MenuItem("GG/测试构建所有（保留新设置）")]
        public static void TestBuildNew() => BasicModUtils.Build(true);

        //[MenuItem("GG/修复缺省组设置")]
        public static void FixDefaultGroupSettings() => BasicModUtils.FixDefaultGroupSettings();
    }
}
#endif