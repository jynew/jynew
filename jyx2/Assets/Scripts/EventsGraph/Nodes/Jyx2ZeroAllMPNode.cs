using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/清空全队内力")]
[NodeWidth(150)]
public class Jyx2ZeroAllMPNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "清空全队内力";
	}


	protected override void DoExecute()
	{
		Jyx2LuaBridge.ZeroAllMP();
	}
}