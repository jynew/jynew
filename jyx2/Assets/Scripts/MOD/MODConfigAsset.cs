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
///
/// CG: TODO。。。未完成，待回头继续实现。
/// 需要做的功能：
/// []将MOD统一引用进行管理，调整目录结构
/// []将lua和配置表可以按MOD区分加载
/// []提供统一的MOD开发和打包发布环境
/// []验证MOD的Addressable加载
/// []让MOD可叠加
/// []修改游戏中各种索引的int格式，特别是配置表中，以防主键冲突
/// ....

[CreateAssetMenu(fileName = "MODConfig-", menuName = "金庸重制版/创建MOD配置文件")]
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

