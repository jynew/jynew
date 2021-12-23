using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "GlobalAssetConfig", menuName = "金庸重制版/全局资源配置文件")]
public class GlobalAssetConfig : ScriptableObject
{
    public static GlobalAssetConfig Instance = null;
    
    [BoxGroup("游戏动作")] [LabelText("默认受击动作")]
    public AnimationClip defaultBeHitClip;
    
    [BoxGroup("游戏动作")] [LabelText("默认移动动作")]
    public AnimationClip defaultMoveClip;
    
    [BoxGroup("游戏动作")] [LabelText("默认待机动作")]
    public AnimationClip defaultIdleClip;
    
    [BoxGroup("游戏动作")] [LabelText("默认眩晕动作")]
    public AnimationClip defaultStunClip;

    [BoxGroup("游戏动作")] [LabelText("使用暗器的动作")]
    public AnimationClip anqiClip;
    
    [BoxGroup("游戏动作")] [LabelText("使用药物的动作")]
    public AnimationClip useItemClip;
    
    [BoxGroup("游戏动作")] [LabelText("默认角色动作控制器")]
    public RuntimeAnimatorController defaultAnimatorController;
    
    [BoxGroup("游戏动作")][LabelText("默认NPC动作控制器")]
    public RuntimeAnimatorController defaultNPCAnimatorController;

    [BoxGroup("游戏动作")] [LabelText("默认死亡动作")]
    public List<AnimationClip> defaultDieClips;

    [BoxGroup("游戏动作")] [LabelText("大地图主角待机动作")]
    public List<AnimationClip> bigMapIdleClips;

    [BoxGroup("游戏相机配置")] [LabelText("默认过肩视角相机")]
    public GameObject vcam3rdPrefab;

    [BoxGroup("游戏相机配置")] [LabelText("相机偏移")]
    public Vector3 defaultVcamOffset = new Vector3(7, 10, 8);

    [BoxGroup("地图设置")] [LabelText("大地图")] 
    public AssetReference BigMap;

    [InfoBox("某些角色名与人物ID不严格对应，在此修正。用于对话中正确显示名字")] [BoxGroup("对话人物ID修正")] [TableList] 
    [HideLabel]
    public List<StoryIdNameFix> StoryIdNameFixes;

    [BoxGroup("预缓存Prefab")]
    [HideLabel]
    public List<GameObject> CachedPrefabs;

    public readonly Dictionary<string, GameObject> CachePrefabDict = new Dictionary<string, GameObject>();
    
    
    

    public void OnLoad()
    {
        //将prefab放置在Dictionary中，用来提高查找速度
        if (CachedPrefabs != null)
        {
            CachePrefabDict.Clear();
            foreach (var prefab in CachedPrefabs)
            {
                if (prefab == null) continue;
                CachePrefabDict.Add(prefab.name, prefab);
            }
        }
    }
}

[Serializable]
public class StoryIdNameFix
{
    [LabelText("ID")] public int Id;
    [LabelText("姓名")] public string Name;
}