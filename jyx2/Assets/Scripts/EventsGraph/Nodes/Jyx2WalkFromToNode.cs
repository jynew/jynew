using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/强制移动")]
[NodeWidth(150)]
public class Jyx2WalkFromToNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "强制移动";
	}
    
    [Header("移动物体(-1为主角)")]
    public int fromObj = -1;
    [Header("目标物体")]
    public int toObj;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.jyx2_WalkFromTo(fromObj, toObj);
	}
}