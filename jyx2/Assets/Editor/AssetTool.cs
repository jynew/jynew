using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using HSFrameWork.ConfigTable.Editor;
using HSFrameWork.Common;
using HanSquirrel.ResourceManager.Editor;

public class AssetTool : Editor
{
    public static ABBuildArg GetABBuildArg(BuildTarget target)
    {
        return new ABBuildArg
        {
            AbFolderDict = ConStr.ABFolderDict,
            PrefabSearchPaths = ConStr.PrefabSearchPaths,
            Target = target,
            ClearWasteAB = true,
            AdditionalABs = ConStr.AdditionalABs,
            OutputFolder = HSCTC.StreamingAssetsPath,
            DebugFileLeftPart = HSCTC.InDebug("AB_"),
            FakeBuild = HSCTC.SkipBuildCurrent,
        };
    }

    public static void Build(BuildTarget target)
    {
        new AssetBundleEditorV2().Build(GetABBuildArg(target));
    }

    [MenuItem("Tools/ForceReloadAssets")]
    public static void ForceReloadAssets()
    {
        AssetDatabase.Refresh();
    }

    //[MenuItem("Tools/AssetBundle/BuildAssetBundleWin32")]
    public static void BuildAssetBundleWin32()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/AssetBundle/BuildAssetBundleWin64")]
    public static void BuildAssetBundleWin64()
    {
        Build(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Tools/AssetBundle/BuildAssetBundleIOS")]
    public static void BuildAssetBundleIOS()
    {
        Build(BuildTarget.iOS);
    }

    [MenuItem("Tools/AssetBundle/BuildAssetBundleAndroid")]
    public static void BuildAssetBundleAndroid()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Tools/AssetBundle/[BuildCurrent]")]
    public static void BuildCurrent()
    {
#if UNITY_STANDALONE_WIN
        //BuildAssetBundleWin32();
        //BuildAssetBundleAndroid();
        BuildAssetBundleWin64();
#elif UNITY_ANDROID
        BuildAssetBundleAndroid();
#elif UNITY_IPHONE
		BuildAssetBundleIOS();
#elif UNITY_STANDALONE_OSX
        BuildAssetBundleIOS();
#endif
    }

    static private void BuildBattleSprites(BuildTarget target)
    {

        List<AssetBundleBuild> buildMaps = new List<AssetBundleBuild>();

        foreach (var f in new DirectoryInfo("Assets/BuildSource/BattleSprites").GetFiles("*.png"))
        {
            AssetBundleBuild buildMap = new AssetBundleBuild();
            buildMap.assetBundleName = "battlesprite." + f.Name.Replace(".png", "");
            var flocation = "Assets/BuildSource/BattleSprites/" + f.Name;
            string[] resourcesAssets = new string[1] { flocation };
            buildMap.assetNames = resourcesAssets;
            buildMaps.Add(buildMap);
        }
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, target);
    }

    static private void BuildHeadAvataBodys(BuildTarget target)
    {

        List<AssetBundleBuild> buildMaps = new List<AssetBundleBuild>();

        foreach (var f in new DirectoryInfo("Assets/BuildSource/HeadAvataBodys").GetFiles("*.png"))
        {
            AssetBundleBuild buildMap = new AssetBundleBuild();
            buildMap.assetBundleName = "headavatabody." + f.Name.Replace(".png", "");
            var flocation = "Assets/BuildSource/HeadAvataBodys/" + f.Name;
            string[] resourcesAssets = new string[1] { flocation };
            buildMap.assetNames = resourcesAssets;
            buildMaps.Add(buildMap);
        }

        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, target);
    }

    static private void BuildTileRes(BuildTarget target)
    {

        List<AssetBundleBuild> buildMaps = new List<AssetBundleBuild>();

        foreach (var f in new DirectoryInfo("Assets/BuildSource/tilemap").GetFiles("*.png"))
        {
            AssetBundleBuild buildMap = new AssetBundleBuild();
            buildMap.assetBundleName = "tileres." + f.Name.Replace(".png", "");
            var flocation = "Assets/BuildSource/tilemap/" + f.Name;
            string[] resourcesAssets = new string[1] { flocation };
            buildMap.assetNames = resourcesAssets;
            buildMaps.Add(buildMap);
        }
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", buildMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, target);
    }


#if false //GG 20190425 这个目前无用了。
    /// <summary>  
    /// 生成资源ZIP文件
    /// </summary>
    [MenuItem("Tools/Zip/Generate")]
    public static void GenerateZip()
    {
        HSPackToolEx.IonicZip_EasyDes("Assets/Resources/TextAssets/values.bytes", "Assets/StreamingAssets/values.zip");
        HSPackToolEx.IonicZip_EasyDes("Assets/Resources/TextAssets/tilemaps.bytes", "Assets/StreamingAssets/tilemaps.zip");
        AssetDatabase.Refresh();
    }
#endif

    private static void CopyAssetbundleFiles(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogErrorFormat("目录不存在: [{0}]", path);
            return;
        }

        foreach (var f in new DirectoryInfo(path).GetFiles())
        {
            string filePath = "Assets/StreamingAssets/" + f.Name;
            Debug.LogFormat("[{0}] => [{1}]", f.FullName, filePath);
            f.CopyTo(filePath, true);
        }
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    static string OuterABPathTag = "win64";
#elif UNITY_ANDROID
	static string OuterABPathTag = "android";
#elif UNITY_IPHONE
	static string OuterABPathTag = "ios";
#endif

    //[MenuItem("Tools/AssetBundle/从当前目标AB拷贝")]
    public static void MoveInAllExternalAB_CurrentTarget()
    {
        //CopyAssetbundleFiles("Assets/3rd/CGAssetBundle/" + OuterABPathTag);
    }

    //[MenuItem("Tools/AssetBundle/从Win64AB拷贝")]
    public static void MoveInAllExternalAB_Win64()
    {
        //CopyAssetbundleFiles("Assets/3rd/CGAssetBundle/win64");
    }
}
