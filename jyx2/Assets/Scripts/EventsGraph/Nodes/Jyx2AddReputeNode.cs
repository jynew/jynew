using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增加声望")]
[NodeWidth(150)]
public class Jyx2AddReputeNode : Jyx2SimpleNode
{
    [Header("增加数值")]
    public int addValue;
    
    private void Reset() {
        name = "增加声望";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.AddRepute(addValue);
    }
}
