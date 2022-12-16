using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
# if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TapTap.Common.Editor;

#if UNITY_IOS
public class BuildPostProcessor
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);

            if (TapCommonCompile.CheckTarget(target))
            {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }
            if (TapCommonCompile.HandlerIOSSetting(path,
                Application.dataPath,
                "AntiAdictionResources",
                "com.tapsdk.antiaddiction",
                "AntiAddiction",
                new[] {"AntiAdictionResources.bundle"},
                target, projPath, proj))
            {
                Debug.Log("TapAntiAddiction add Bundle Success!");
                return;
            }

            Debug.LogWarning("TapAntiAddiction add Bundle Failed!");
        }
    }
}
#endif
