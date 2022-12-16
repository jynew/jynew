using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEditor.iOS.Xcode;

#endif

namespace TapTap.Common.Editor
{
#if UNITY_IOS || UNITY_STANDALONE_OSX
    public static class TapCommonCompile
    {
        public static string GetProjPath(string path)
        {
            return PBXProject.GetPBXProjectPath(path);
        }

        public static PBXProject ParseProjPath(string path)
        {
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(path));
            return proj;
        }

        public static string GetUnityFrameworkTarget(PBXProject proj)
        {
#if UNITY_2019_3_OR_NEWER
            string target = proj.GetUnityFrameworkTargetGuid();
            return target;
#endif
            var unityPhoneTarget = proj.TargetGuidByName("Unity-iPhone");
            return unityPhoneTarget;
        }

        public static string GetUnityTarget(PBXProject proj)
        {
#if UNITY_2019_3_OR_NEWER
            string target = proj.GetUnityMainTargetGuid();
            return target;
#endif
            var unityPhoneTarget = proj.TargetGuidByName("Unity-iPhone");
            return unityPhoneTarget;
        }


        public static bool CheckTarget(string target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static bool HandlerIOSSetting(string path, string appDataPath, string resourceName,
            string modulePackageName,
            string moduleName, string[] bundleNames, string target, string projPath, PBXProject proj)
        {
            var resourcePath = Path.Combine(path, resourceName);

            var parentFolder = Directory.GetParent(appDataPath).FullName;

            Debug.Log($"ProjectFolder path:{parentFolder}");

            if (Directory.Exists(resourcePath))
            {
                Directory.Delete(resourcePath, true);
            }

            Directory.CreateDirectory(resourcePath);

            var remotePackagePath =
                TapFileHelper.FilterFileByPrefix(parentFolder + "/Library/PackageCache/", $"{modulePackageName}@");

            var assetLocalPackagePath = TapFileHelper.FilterFileByPrefix(parentFolder + "/Assets/TapTap/", moduleName);

            var localPackagePath = TapFileHelper.FilterFileByPrefix(parentFolder, moduleName);

            var tdsResourcePath = "";

            if (!string.IsNullOrEmpty(remotePackagePath))
            {
                tdsResourcePath = remotePackagePath;
            }
            else if (!string.IsNullOrEmpty(assetLocalPackagePath))
            {
                tdsResourcePath = assetLocalPackagePath;
            }
            else if (!string.IsNullOrEmpty(localPackagePath))
            {
                tdsResourcePath = localPackagePath;
            }

            if (string.IsNullOrEmpty(tdsResourcePath))
            {
                Debug.LogError("tdsResourcePath is NUll");
                return false;
            }

            tdsResourcePath = $"{tdsResourcePath}/Plugins/iOS/Resource";

            Debug.Log($"Find {moduleName} path:{tdsResourcePath}");

            if (!Directory.Exists(tdsResourcePath))
            {
                Debug.LogError($"Can't Find {bundleNames}");
                return false;
            }

            TapFileHelper.CopyAndReplaceDirectory(tdsResourcePath, resourcePath);
            foreach (var name in bundleNames)
            {
                proj.AddFileToBuild(target,
                    proj.AddFile(Path.Combine(resourcePath, name), Path.Combine(resourcePath, name),
                        PBXSourceTree.Source));
            }

            File.WriteAllText(projPath, proj.WriteToString());
            return true;
        }

        public static bool HandlerPlist(string pathToBuildProject, string infoPlistPath, bool macos = false)
        {
// #if UNITY_2020_1_OR_NEWER
//             var macosXCodePlistPath =
//                 $"{pathToBuildProject}/{PlayerSettings.productName}/Info.plist";
// #elif UNITY_2019_1_OR_NEWER
//             var macosXCodePlistPath =
//                 $"{Path.GetDirectoryName(pathToBuildProject)}/{PlayerSettings.productName}/Info.plist";
// #endif

            string plistPath;

            if (pathToBuildProject.EndsWith(".app"))
            {
                plistPath = $"{pathToBuildProject}/Contents/Info.plist";
            }
            else
            {
                var macosXCodePlistPath =
                    $"{Path.GetDirectoryName(pathToBuildProject)}/{PlayerSettings.productName}/Info.plist";
                if (!File.Exists(macosXCodePlistPath))
                {
                    macosXCodePlistPath = $"{pathToBuildProject}/{PlayerSettings.productName}/Info.plist";
                }

                plistPath = !macos
                    ? pathToBuildProject + "/Info.plist"
                    : macosXCodePlistPath;
            }

            Debug.Log($"plist path:{plistPath}");

            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var rootDic = plist.root;

            var items = new List<string>
            {
                "tapsdk",
                "tapiosdk",
            };

            if (!(rootDic["LSApplicationQueriesSchemes"] is PlistElementArray plistElementList))
            {
                plistElementList = rootDic.CreateArray("LSApplicationQueriesSchemes");
            }

            foreach (var t in items)
            {
                plistElementList.AddString(t);
            }

            if (string.IsNullOrEmpty(infoPlistPath)) return false;
            var dic = (Dictionary<string, object>) Plist.readPlist(infoPlistPath);
            var taptapId = "";

            foreach (var item in dic)
            {
                if (item.Key.Equals("taptap"))
                {
                    var taptapDic = (Dictionary<string, object>) item.Value;
                    foreach (var taptapItem in taptapDic.Where(taptapItem => taptapItem.Key.Equals("client_id")))
                    {
                        taptapId = (string) taptapItem.Value;
                    }
                }
                else
                {
                    rootDic.SetString(item.Key, item.Value.ToString());
                }
            }

            //添加url
            var dict = plist.root.AsDict();
            if (!(dict["CFBundleURLTypes"] is PlistElementArray array))
            {
                array = dict.CreateArray("CFBundleURLTypes");
            }

            if (!macos)
            {
                var dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "TapTap");
                var array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString($"tt{taptapId}");
            }
            else
            {
                var dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "TapWeb");
                var array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString($"open-taptap-{taptapId}");
            }

            Debug.Log("TapSDK change plist Success");
            File.WriteAllText(plistPath, plist.WriteToString());
            return true;
        }

        public static string GetValueFromPlist(string infoPlistPath, string key)
        {
            if (infoPlistPath == null)
            {
                return null;
            }

            var dic = (Dictionary<string, object>) Plist.readPlist(infoPlistPath);
            return (from item in dic where item.Key.Equals(key) select (string) item.Value).FirstOrDefault();
        }
    }
#endif
}