using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/等待")]
[NodeWidth(150)]
public class Jyx2WaitNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "等待";
	}
    
    [Header("等待时间(s)")]
    public float duration = 0;
    
	protected override void DoExecute()

	{
        Debug.Log($"Wait for {duration} secs");
		Jyx2LuaBridge.jyx2_Wait(duration);
	}
}