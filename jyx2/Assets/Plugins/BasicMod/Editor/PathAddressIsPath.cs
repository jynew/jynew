using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace HanSquirrel.OpenSource
{
    ///从Unity例子里面拷贝略微修改
    public class PathAddressIsPath : UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule
    {
        public override bool CanFix
        {
            get { return true; }
            set { }
        }

        public override string ruleName => "BasicMod的地址和路径相同";

        [SerializeField]
        List<AddressableAssetEntry> m_MisnamedEntries = new List<AddressableAssetEntry>();

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings) => RefreshAnalysis(settings, this);

        public static List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings, PathAddressIsPath THIS)
        {
            List<AnalyzeResult> results = new List<AnalyzeResult>();
            foreach (var group in settings.groups)
            {
                if (!group.HasSchema<BasicModSchema>())
                    continue;

                foreach (var e in group.entries)
                {
                    if (e.address.Contains("Assets") && e.address.Contains("/") && e.address != e.AssetPath)
                    {
                        THIS?.m_MisnamedEntries.Add(e);
                        results.Add(new AnalyzeResult { resultName = group.Name + kDelimiter + e.address, severity = MessageType.Error });
                    }
                }
            }

            return results;
        }

        public static bool Check(AddressableAssetSettings settings) => RefreshAnalysis(settings, null).IsNullOrEmptyG();

        public override void FixIssues(AddressableAssetSettings settings)
        {
            foreach (var e in m_MisnamedEntries)
                e.address = e.AssetPath;
            m_MisnamedEntries = new List<AddressableAssetEntry>();
        }

        public override void ClearAnalysis() => m_MisnamedEntries = new List<AddressableAssetEntry>();
    }

    [InitializeOnLoad]
    class RegisterPathAddressIsPath
    {
        static RegisterPathAddressIsPath() => AnalyzeSystem.RegisterNewRule<PathAddressIsPath>();
    }
}
