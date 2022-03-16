using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("音乐音效/播放片尾动画")]
[NodeWidth(150)]
public class Jyx2EndAminationNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "播放片尾动画";
    }
    public int v1;
    public int v2;
    public int v3;
    public int v4;
    public int v5;
    public int v6;
    public int v7;

    protected override void DoExecute()
	{   
		Jyx2LuaBridge.EndAmination(v1, v2, v3, v4, v5, v6, v7);
	}
}
