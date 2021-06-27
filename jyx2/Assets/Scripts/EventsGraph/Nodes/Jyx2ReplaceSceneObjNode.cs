using System;
using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;

[CreateNodeMenu("场景/显示隐藏物体")]
[NodeWidth(200)]
public class Jyx2ReplaceSceneObjNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "显示隐藏物体";
    }

    [Header("场景id")] public string sceneId;
    [Header("是否显示")] public bool isShow;
    [Header("物体路径")] public string path;
    
    protected override void DoExecute()
    {
        Jyx2LuaBridge.jyx2_ReplaceSceneObject(sceneId, path, isShow ? "1" : "0");
    }
}
