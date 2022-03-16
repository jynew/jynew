using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/设置角色内功适性")]
[NodeWidth(180)]
public class Jyx2SetPersonMPProNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "设置角色内功适性";
	}
    [Header("角色id")]
    public int roleId;
    [Header("内功类型id(0:阴 1:阳 2:调和)")]
    public int type;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.SetPersonMPPro(roleId, type);
	}
}