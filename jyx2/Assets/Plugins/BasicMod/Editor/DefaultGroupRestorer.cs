using System;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace HanSquirrel.OpenSource
{
    public class DefaultGroupRestorer : IDisposable
    {
        private readonly AddressableAssetGroup _DefaultGroup;

        public DefaultGroupRestorer() => _DefaultGroup = AddressableAssetSettingsDefaultObject.Settings.groups.FirstOrDefault(x => x.Default);

        public void Dispose() => AddressableAssetSettingsDefaultObject.Settings.DefaultGroup = _DefaultGroup;
    }
}
