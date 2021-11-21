using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/全体队友离队")]
[NodeWidth(180)]
public class Jyx2AllLeaveNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "全体队友离队";
    }
    
    protected override void DoExecute()
	{   
		Jyx2LuaBridge.AllLeave();
	}
}
