using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增减道具")]
[NodeWidth(200)]
public class Jyx2AddItemNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "增减道具";
	}

	[Header("道具ID")] public int itemId;
	[Header("数量")] public int count;
	[Header("是否提示")] public bool isHint;


	protected override void DoExecute()
	{
		if (this.GetInputValue<bool>(nameof(isHint)))
		{
			Jyx2LuaBridge.AddItem(itemId, count);
		}
		else
		{
			Jyx2LuaBridge.AddItemWithoutHint(itemId, count);
		}
	}
}