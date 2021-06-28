using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断队伍是否满")]
[NodeWidth(150)]
public class Jyx2TeamIsFullNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;

    private void Reset() {
        name = "判断队伍是否满";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.TeamIsFull();
        return ret ? nameof(yes) : nameof(no);
    }
}
