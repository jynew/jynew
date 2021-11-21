using System;
using System.Collections;
using System.Collections.Generic;


using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断场景的图片")]
[NodeWidth(200)]
public class Jyx2JudgeScenePicNode : Jyx2BaseNode
{
    private void Reset() {
        name = "判断场景的图片";
    }
    [Output] public Node yes;
    [Output] public Node no;
    [Header("场景id")]
	public int sceneId; 
    [Header("事件id")]
	public int eventId;
    [Header("图片id")]
	public int picId;
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.JudgeScenePic(sceneId, eventId, picId);;
        return ret ? nameof(yes) : nameof(no);
    }
}
