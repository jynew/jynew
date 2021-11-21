using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/打开小宝商店")]
[NodeWidth(150)]
public class Jyx2WeiShopNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "打开小宝商店";
	}


	protected override void DoExecute()
	{
		Jyx2LuaBridge.WeiShop();
	}
}