using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/NPC获得道具")]
[NodeWidth(200)]
public class Jyx2NPCGetItemNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "NPC获得道具";
	}
    [Header("角色id")]
    public int roleId;
    [Header("物品id")]
    public int itemId;
    [Header("数量")]
    public int quantity;


	protected override void DoExecute()
	{
		Jyx2LuaBridge.NPCGetItem(roleId, itemId, quantity);
	}
}