using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/摄像机锁定到主角")]
[NodeWidth(150)]
public class Jyx2CameraFollowPlayerNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "摄像机锁定到主角";
    }
    
    protected override void DoExecute()
	{
		Jyx2LuaBridge.jyx2_CameraFollowPlayer();
	}
}
