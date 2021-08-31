using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalAssetConfig", menuName = "金庸重制版/全局资源配置文件")]
public class GlobalAssetConfig : ScriptableObject
{
    static public GlobalAssetConfig Instance = null;
    
    [BoxGroup("游戏动作")] [LabelText("默认受击动作")]
    public AnimationClip defaultBeHitClip;
    
    [BoxGroup("游戏动作")] [LabelText("默认移动动作")]
    public AnimationClip defaultMoveClip;

    [BoxGroup("游戏动作")] [LabelText("默认角色动作控制器")]
    public RuntimeAnimatorController defaultAnimatorController;
    
    [BoxGroup("游戏动作")][LabelText("默认NPC动作控制器")]
    public RuntimeAnimatorController defaultNPCAnimatorController;

    [BoxGroup("游戏动作")] [LabelText("默认死亡动作")]
    public List<AnimationClip> defaultDieClips;
}
