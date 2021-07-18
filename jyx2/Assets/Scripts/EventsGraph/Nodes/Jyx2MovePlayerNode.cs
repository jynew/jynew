using System;
using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;

[CreateNodeMenu("场景/移动主角")]
[NodeWidth(150)]
public class Jyx2MovePlayerNode : Jyx2SimpleNode
{
    private void Reset() {
        name = "移动主角";
    }

    [Header("物体路径")] public string objPath;
    [Header("根目录")] public string parentDir;
    
    protected override void DoExecute()
    {
        Jyx2LuaBridge.jyx2_MovePlayer(objPath, parentDir);
    }
}
