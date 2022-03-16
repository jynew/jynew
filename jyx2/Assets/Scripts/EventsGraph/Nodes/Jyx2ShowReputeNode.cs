using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/显示声望值")]
[NodeWidth(150)]
public class Jyx2ShowReputeNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "显示声望值";
	}


	protected override void DoExecute()
	{
		Jyx2LuaBridge.ShowRepute();
	}
}