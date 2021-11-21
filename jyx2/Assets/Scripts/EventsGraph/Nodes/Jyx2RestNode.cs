using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/休息")]
[NodeWidth(150)]
public class Jyx2RestNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "休息";
    }
    
    protected override void DoExecute()
	{
		Jyx2LuaBridge.Rest();
	}
}
