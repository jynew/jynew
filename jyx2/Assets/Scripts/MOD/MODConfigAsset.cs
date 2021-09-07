/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// MOD根配置文件
/// </summary>
[CreateAssetMenu(fileName = "[MODConfig]", menuName = "金庸重制版/创建MOD配置文件")]
public class MODConfigAsset : ScriptableObject
{

 [LabelText("MOD名")] public string Name;
 [LabelText("版本号")] public string Version;
 [LabelText("作者名")] public string Author;

 [LabelText("地图文件夹")] public AssetReference GameMapDir;

 [Button("自动添加地图")]
 void AutoAddGameMap()
 {
#if UNITY_EDITOR
  var path = AssetDatabase.GUIDToAssetPath(GameMapDir.AssetGUID);
  Debug.Log(path);
#endif
 }
 
 [LabelText("地图引用")]
 public List<AssetReference> GameMapScenes;

 [LabelText("战斗地图引用")]
 public List<AssetReference> BattleMapScenes;
}
