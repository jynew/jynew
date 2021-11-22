using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/设置主角脸朝向")]
[NodeWidth(180)]
public class Jyx2SetRoleFaceNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "设置主角脸朝向";
	}
    [Header("方向id(0-3四个方向)")]
    public int dir;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.SetRoleFace(dir);
	}
}