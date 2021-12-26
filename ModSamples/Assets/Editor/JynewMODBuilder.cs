using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JynewMODBuilder 
{

    [MenuItem("金庸3D/打包MOD")]
    public static void BuildMod()
    {
        string path = EditorUtility.OpenFolderPanel("选择输出路径", "F:/", "");
        if (string.IsNullOrEmpty(path))
            return;

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
