using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/学会武功")]
[NodeWidth(200)]
public class Jyx2LearnMagic2Node : Jyx2SimpleNode
{
    private void Reset() {
        name = "学会武功";
    }
    [Header("角色id")]
    public int roleId;
    [Header("武功id")]
    public int skillId;
    [Header("是否显示(0:不显示,1:显示)")]
    public int visible;
    
    protected override void DoExecute()
	{   
		Jyx2LuaBridge.LearnMagic2(roleId, skillId, visible);
	}
}
