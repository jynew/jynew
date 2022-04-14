using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace HanSquirrel.OpenSource
{
    ///不建议上层直接使用
    public class BasicModUtils
    {
        public static string GetRemoteLoadPath(string modName) => $"{{HanSquirrel.OpenSource.BasicModConfig.RootDir}}/{modName}";

        public static string GetRemoteBuildPath(string modName) => $"ModOutput/[BuildTarget]/{modName}";

        public static string GetRemoteBuildPathReal(string modName) => $"ModOutput/{EditorUserBuildSettings.activeBuildTarget}/{modName}";

        public static string ModOutput = $"ModOutput/{EditorUserBuildSettings.activeBuildTarget}";

        public static string InternalOutput = $"{UnityEngine.AddressableAssets.Addressables.BuildPath}/{EditorUserBuildSettings.activeBuildTarget}";

        public static List<string> GetAllModNames()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings.groups.Where(x => x.HasSchema<BasicModSchema>()).Select(x => x.Name).ToList();
        }

        public static bool FixModProfile(string modName, out string profileID)
        {
            var profileName     = $"Mod/{modName}";
            var profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
            profileID = profileSettings.GetProfileId(profileName);
            if (!profileID.Visible())
            {
                var defaultProfileID = profileSettings.GetProfileId("Default");
                if (!defaultProfileID.Visible())
                {
                    EditorUtility.DisplayDialog("请先设置", "需要有一个Default的Profile", "关闭");
                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Profiles");
                    return false;
                }

                profileID = profileSettings.AddProfile(profileName, defaultProfileID);
            }

            profileSettings.SetValue(profileID, AddressableAssetSettings.kRemoteBuildPath, GetRemoteBuildPath(modName));
            profileSettings.SetValue(profileID, AddressableAssetSettings.kRemoteLoadPath, GetRemoteLoadPath(modName));
            return true;
        }

        public static void FixRootSettings()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.ContiguousBundles  = true;
            settings.BuildRemoteCatalog = true;
            settings.ShaderBundleNaming = ShaderBundleNaming.DefaultGroupGuid;
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, "LocalBuildPath");
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, "LocalLoadPath");

            var dlg = settings.groups.FirstOrDefault(x => x.Name == "Default Local Group");
            if (!dlg.IsDefaultGroup())
                settings.DefaultGroup = dlg;
        }

        public static void FixRootSettingsForMod(string modName, string profileID)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.activeProfileId    = profileID;
            settings.ContiguousBundles  = true;
            settings.BuildRemoteCatalog = true;
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, "RemoteBuildPath");
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, "RemoteLoadPath");
            settings.ShaderBundleNaming = ShaderBundleNaming.DefaultGroupGuid;

            var bp  = settings.RemoteCatalogBuildPath.GetValue(settings);
            var bp1 = GetRemoteBuildPathReal(modName);
            if (bp != bp1)
                throw new Exception($"GG程序编写错误，RemoteBuildPath不一致 [{bp}] [{bp1}]");

            var lp  = settings.RemoteCatalogLoadPath.GetValue(settings);
            var lp1 = GetRemoteLoadPath(modName);

            if (lp != lp1)
                throw new Exception($"GG程序编写错误，RemoteLoadPath不一致 [{lp}] [{lp1}]");
        }

        public static void FixSettings_ExcludeAllModGroups()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (var og in settings.groups.Where(x => x.HasSchema<BasicModSchema>()))
                ExcludeGroupInBuild(og);
        }

        public static BundledAssetGroupSchema FixDefaultGroupSettings()
        {
            var sch = GetDefaultGroupBundledAssetGroupSchema();

            //缺省组始终需要包含
            sch.IncludeInBuild = true;

            //如下三个设置才能保证主干内容变化后，旧的MOD可以正常加载内置资源
            sch.BundleNaming         = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            sch.InternalBundleIdMode = BundledAssetGroupSchema.BundleInternalIdMode.GroupGuidProjectIdHash;
            sch.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;

            //缺省组只能设置为本地Build和Load
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            sch.BuildPath.SetVariableByName(settings, "LocalBuildPath");
            sch.LoadPath.SetVariableByName(settings, "LocalLoadPath");

            (sch.IncludeAddressInCatalog, sch.IncludeGUIDInCatalog, sch.IncludeLabelsInCatalog) = (true, true, true);

            return sch;
        }

        public static void ExcludeGroupInBuild(AddressableAssetGroup g)
        {
            var sch = g.GetSchema<BundledAssetGroupSchema>();
            if (sch != null)
                sch.IncludeInBuild = false;
        }

        private static BundledAssetGroupSchema GetDefaultGroupBundledAssetGroupSchema()
        {
            var defaultGroup = AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(x => x.Default);
            if (defaultGroup == null)
                throw new Exception("当前项目配置错误；没有缺省的AddressableGroup");

            if (!defaultGroup.HasSchema<BundledAssetGroupSchema>())
                throw new Exception("当前项目配置错误；缺省的AddressableGroup没有BundledAssetGroupSchema");

            return defaultGroup.GetSchema<BundledAssetGroupSchema>();
        }

        private static void FixDefaultGroupSettingsWhenBuildMod()
        {
            var sch = FixDefaultGroupSettings();
            (sch.IncludeAddressInCatalog, sch.IncludeGUIDInCatalog, sch.IncludeLabelsInCatalog) = (false, false, false);
        }

        public static void FixGroupSettings(AddressableAssetSettings settings, AddressableAssetGroup g)
        {
            bool isMod = g.HasSchema<BasicModSchema>();
            if (!isMod)
                return;

            var sch = g.GetSchema<BundledAssetGroupSchema>();
            if (sch == null)
                sch = g.AddSchema<BundledAssetGroupSchema>();

            sch.BuildPath.SetVariableByName(settings, "RemoteBuildPath");
            sch.LoadPath.SetVariableByName(settings, "RemoteLoadPath");

            sch.IncludeInBuild                 = true;
            sch.IncludeAddressInCatalog        = true;
            sch.IncludeGUIDInCatalog           = true;
            sch.IncludeLabelsInCatalog         = true;
            sch.InternalIdNamingMode           = BundledAssetGroupSchema.AssetNamingMode.FullPath;
            sch.InternalBundleIdMode           = BundledAssetGroupSchema.BundleInternalIdMode.GroupGuidProjectIdEntriesHash; //GG 这样可以确保各个项目AB包ID不同，参考FixTags
            sch.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
            sch.BundleNaming                   = BundledAssetGroupSchema.BundleNamingStyle.NoHash; //GG 这个名字没啥用，各个项目重复也没关系
            //sch.AssetLoadMode                  = AssetLoadMode.RequestedAssetAndDependencies; //这个在17版本没有

            Debug.Log($"Mod[{g.Name}] 对应的组设置完成");
        }

        public static void Build(string modName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var g        = settings.groups.FirstOrDefault(g => g.name == modName);
            if (g == null)
            {
                Debug.LogError($"在Addressable的Group中找不到[{modName}]");
                return;
            }

            FixGroupSettings(settings, g);
            foreach (var og in settings.groups.Where(x => x != g && !g.Default))
                ExcludeGroupInBuild(og);

            FixDefaultGroupSettingsWhenBuildMod();

            if (!FixModProfile(modName, out var profileId))
            {
                Debug.LogError($"Mod[{modName}] 构建失败。");
                return;
            }

            FixRootSettingsForMod(modName, profileId);

            var buildDstFolder = GetRemoteBuildPathReal(modName).CreateDir().ClearDirectory();

            using (new DefaultGroupRestorer())
            {
                settings.DefaultGroup = g;
                AddressableAssetSettings.BuildPlayerContent();
            }

            var cat = buildDstFolder.GetFiles("catalog_*.json", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.Name).First().FullName;
            cat.MoveFileTo(buildDstFolder.Sub("catalog.json"));
            cat.ReplaceFileExt(".hash").MoveFileTo(buildDstFolder.Sub("catalog.hash"));

            Debug.Log($"MOD[{modName}] 构建成功");
        }

        public static void Build(bool keepNewSettings)
        {
            if (!PathAddressIsPath.Check(AddressableAssetSettingsDefaultObject.Settings))
            {
                if (!EditorUtility.DisplayDialog("资源命名错误", "请在Analysis中检查并修复.", "关闭", "在浏览器中打开文档"))
                    Application.OpenURL("https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html");
                EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Analyze");
                return;
            }

            using (keepNewSettings ? DummyDisposable.I : new SettingsArchiver())
            {
                GetAllModNames().ForEach(Build);
            }
        }
    }
}
