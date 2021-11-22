using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/修改触发器事件")]
[NodeWidth(150)]
public class Jyx2Add3EventNumNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "修改触发器事件";
	}
    [Header("场景id")]
    public int sceneId;
    [Header("事件id")]
    public int eventId;
    [Header("交互事件数值")]
    public int v1;
    [Header("使用物品数值")]
    public int v2;
    [Header("进入事件数值")]
    public int v3;
	protected override void DoExecute()
	{
		Jyx2LuaBridge.Add3EventNum(sceneId, eventId, v1, v2, v3);
	}
}