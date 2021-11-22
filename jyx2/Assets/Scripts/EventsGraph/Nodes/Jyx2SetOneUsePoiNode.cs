using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/设置用毒能力")]
[NodeWidth(180)]
public class Jyx2SetOneUsePoiNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "设置用毒能力";
	}
    [Header("角色id")]
    public int roleId;
    [Header("用毒能力数值")]
    public int poiSkill;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.SetOneUsePoi(roleId, poiSkill);
	}
}