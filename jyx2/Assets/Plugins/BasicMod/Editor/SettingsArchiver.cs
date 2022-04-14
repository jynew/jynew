using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace HanSquirrel.OpenSource
{
    ///用于保存和恢复组的IncludeInBuild信息，AddressableAssetSettings的部分设置
    public class SettingsArchiver : IDisposable
    {
        public static bool Disabled { get; set; }

        private readonly AddressableAssetSettings _Settings = AddressableAssetSettingsDefaultObject.Settings;

        private readonly List<(BundledAssetGroupSchema, bool)> _Includes;

        private readonly bool   _ContiguousBundles;
        private readonly bool   _BuildRemoteCatalog;
        private readonly string _RemoteCatalogBuildPath;
        private readonly string _RemoteCatalogLoadPath;
        private readonly string _ActiveProfileId;

        public SettingsArchiver()
        {
            _Includes =
                _Settings.groups
                    .Where(x => x.HasSchema<BundledAssetGroupSchema>())
                    .Select(x => x.GetSchema<BundledAssetGroupSchema>())
                    .Select(x => (x, x.IncludeInBuild))
                    .ToList();

            _ActiveProfileId        = _Settings.activeProfileId;
            _ContiguousBundles      = _Settings.ContiguousBundles;
            _BuildRemoteCatalog     = _Settings.BuildRemoteCatalog;
            _RemoteCatalogBuildPath = _Settings.RemoteCatalogBuildPath.GetName(_Settings);
            _RemoteCatalogLoadPath  = _Settings.RemoteCatalogLoadPath.GetName(_Settings);
            Debug.Log("Addressable和Group的设置已备份");
        }

        public void Dispose()
        {
            if (Disabled)
            {
                Debug.LogWarning("SettingsArchiver被禁用，因此无法在构建模组后恢复正常设置");
                return;
            }

            _Includes.ForEach(x => x.Item1.IncludeInBuild = x.Item2);
            _Settings.activeProfileId    = _ActiveProfileId;
            _Settings.ContiguousBundles  = _ContiguousBundles;
            _Settings.BuildRemoteCatalog = _BuildRemoteCatalog;
            _Settings.RemoteCatalogBuildPath.SetVariableByName(_Settings, _RemoteCatalogBuildPath);
            _Settings.RemoteCatalogLoadPath.SetVariableByName(_Settings, _RemoteCatalogLoadPath);

            BasicModUtils.FixDefaultGroupSettings();
            Debug.Log("Addressable和Group的设置已恢复");
        }
    }
}
