using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
# if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

namespace TapTap.Common.Editor
{
# if UNITY_IOS
    public static class TapCommonIOSProcessor
    {
        // 添加标签，unity导出工程后自动执行该函数
        [PostProcessBuild(99)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;

            // 获得工程路径
            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);
            var unityFrameworkTarget = TapCommonCompile.GetUnityFrameworkTarget(proj);

            if (TapCommonCompile.CheckTarget(target))
            {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }

            proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            proj.AddBuildProperty(unityFrameworkTarget, "OTHER_LDFLAGS", "-ObjC");

            proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.SetBuildProperty(target, "SWIFT_VERSION", "5.0");
            proj.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");

            proj.SetBuildProperty(unityFrameworkTarget, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(unityFrameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            proj.SetBuildProperty(unityFrameworkTarget, "SWIFT_VERSION", "5.0");
            proj.SetBuildProperty(unityFrameworkTarget, "CLANG_ENABLE_MODULES", "YES");

            proj.AddFrameworkToProject(unityFrameworkTarget, "MobileCoreServices.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "WebKit.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "Security.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "CoreTelephony.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "SystemConfiguration.framework", false);

            proj.AddFileToBuild(unityFrameworkTarget,
                proj.AddFile("usr/lib/libc++.tbd", "libc++.tbd", PBXSourceTree.Sdk));

            if (TapCommonCompile.HandlerIOSSetting(path,
                Application.dataPath,
                "TapCommonResource",
                "com.taptap.tds.common",
                "Common",
                new[] {"TapCommonResource.bundle"},
                target, projPath, proj))
            {
                Debug.Log("TapCommon add Bundle Success!");
                return;
            }

            Debug.LogError("TapCommon add Bundle Failed!");
        }
    }
#endif
}