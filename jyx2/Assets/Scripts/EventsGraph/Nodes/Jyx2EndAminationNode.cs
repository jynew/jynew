using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("音乐音效/播放片尾动画")]
[NodeWidth(150)]
public class Jyx2EndAminationNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "武林大会";
    }
    
    protected override void DoExecute()
	{   
		Jyx2LuaBridge.EndAmination();
	}
}
