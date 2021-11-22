using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/打开所有场景")]
[NodeWidth(150)]
public class Jyx2OpenAllSceneNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "打开所有场景";
    }
    
    protected override void DoExecute()
	{   
		Jyx2LuaBridge.OpenAllScene();
	}
}
