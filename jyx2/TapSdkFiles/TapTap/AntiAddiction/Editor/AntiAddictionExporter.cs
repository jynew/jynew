using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace TapTap.AntiAddiction.Editor {
    public class AntiAddictionExporter {
        [MenuItem("TapTap/Export PC Anti-Addiction SDK")]
        static void Export() {
            string path = EditorUtility.OpenFolderPanel("Export path", "", "");
            string[] assetPaths = new string[] {
                "Assets/TapTap/AntiAddiction"
            };
            string exportPath = Path.Combine(path, "pc-anti-addiction.unitypackage");
            AssetDatabase.ExportPackage(assetPaths, exportPath,
                ExportPackageOptions.Recurse);
            Debug.Log("Export done.");
        }
    }
}
