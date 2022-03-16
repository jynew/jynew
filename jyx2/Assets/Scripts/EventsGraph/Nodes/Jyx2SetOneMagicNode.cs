using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/设置武功等级")]
[NodeWidth(200)]
public class Jyx2SetOneMagicNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "设置武功等级";
    }
    [Header("角色id")]
    public int roleId;
    [Header("武功槽位id(从0开始)")]
    public int skillIndex;
    [Header("武功id")]
    public int skillId;
    
    [Header("武功等级(0:1级, 900:10级)")]
    public int skillLevel;
    
    protected override void DoExecute()
	{   
		Jyx2LuaBridge.SetOneMagic(roleId, skillIndex, skillId, skillLevel);
	}
}
