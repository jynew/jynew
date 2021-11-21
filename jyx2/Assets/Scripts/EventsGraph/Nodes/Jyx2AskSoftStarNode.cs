using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/和软体娃娃对话")]
[NodeWidth(150)]
public class Jyx2AskSoftStarNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "和软体娃娃对话";
	}

	protected override void DoExecute()
	{
		Jyx2LuaBridge.AskSoftStar();
	}
}