using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("场景/停止播放动画")]
[NodeWidth(200)]
public class Jyx2StopTimelineNode : Jyx2SimpleNode
{
	private void Reset() {
		name = "播放动画";
	}
    
    [Header("动画名")]
    public string timelineName;

	protected override void DoExecute()
	{
		Jyx2LuaBridge.jyx2_StopTimeline(timelineName);
	}
}