using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/角色入队")]
[NodeWidth(180)]
public class Jyx2JoinNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "角色入队";
	}

	[Header("角色id")]
	public int roleId;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.Join(roleId);
	}
}