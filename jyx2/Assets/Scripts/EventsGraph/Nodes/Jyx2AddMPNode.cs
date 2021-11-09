using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增加内力")]
[NodeWidth(200)]
public class Jyx2AddMPNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "增加内力";
	}

	[Header("角色id")]
	public int roleId;
    [Header("增加值")]
	public int increment;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.AddMp(roleId, increment);
	}
}