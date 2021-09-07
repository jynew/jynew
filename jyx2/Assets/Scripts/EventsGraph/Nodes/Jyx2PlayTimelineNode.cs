using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/播放动画")]
[NodeWidth(200)]
public class Jyx2PlayTimelineNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "播放动画";
	}
    
    [Header("动画名")]
    public string timelineName;
    [Header("播放模式")]
    [Tooltip("(0:播放时|1:播放后]执行下一个事件")]
    public int mode;
    [Header("是否克隆主角")]
    public bool isClone;
    [Header("角色:默认为主角")]
    public string tagRole="";


	protected override void DoExecute()
	{
		Jyx2LuaBridge.jyx2_PlayTimeline(timelineName, mode, isClone, tagRole);
	}
}

[CreateNodeMenu("场景/播放简单动画")]
[NodeWidth(200)]
public class Jyx2PlayTimelineSimpleNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "播放简单动画";
	}
    
	[Header("动画名")]
	public string timelineName;
	
	protected override void DoExecute()
	{
		Jyx2LuaBridge.jyx2_PlayTimelineSimple(timelineName);
	}
}