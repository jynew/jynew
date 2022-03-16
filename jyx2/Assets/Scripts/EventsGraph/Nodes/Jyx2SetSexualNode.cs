using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/设置性别")]
[NodeWidth(200)]
public class Jyx2SetSexualNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "设置角色内功适性";
	}
    [Header("角色id")]
    public int roleId;
    [Header("性别id(0:男, 1:女, 2:人妖)")]
    public int gender = 0;


	protected override void DoExecute()
	{
        Debug.Assert(gender <= 2 && gender >= 0, "Invalid gender value, should be an integer in [0, 2]");
		Jyx2LuaBridge.SetSexual(roleId, gender);
	}
}