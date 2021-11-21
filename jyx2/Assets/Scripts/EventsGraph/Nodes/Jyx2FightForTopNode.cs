using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/武林大会")]
[NodeWidth(180)]
public class Jyx2FightForTopNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "武林大会";
    }
    
    protected override void DoExecute()
	{   
		Jyx2LuaBridge.FightForTop();
	}
}
