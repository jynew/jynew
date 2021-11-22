using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/显示道德值")]
[NodeWidth(150)]
public class Jyx2ShowEthicsNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "显示道德值";
	}


	protected override void DoExecute()
	{
		Jyx2LuaBridge.ShowEthics();
	}
}