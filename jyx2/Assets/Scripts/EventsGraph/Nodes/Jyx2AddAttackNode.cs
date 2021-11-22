using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增加攻击力")]
[NodeWidth(200)]
public class Jyx2AddAttackNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "增加攻击力";
	}

	[Header("角色id")]
	public int roleId;
    [Header("增加值")]
	public int increment;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.AddAttack(roleId, increment);
	}
}