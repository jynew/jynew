using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/修改场景内物体位置")]
[NodeWidth(150)]
public class Jyx2FixMapObjectNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "修改场景内物体位置";
	}
    
    [Header("修改物体")]
    public string key;
    [Header("修改值")]
    public string value;
    
	protected override void DoExecute()

	{
		Jyx2LuaBridge.jyx2_FixMapObject(key, value);
	}
}