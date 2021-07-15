using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断队伍里是否有女性")]
[NodeWidth(180)]
public class Jyx2JudgeFemaleInTeamNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;
    
    private void Reset() {
        name = "判断队伍里是否有女性";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.JudgeFemaleInTeam();
        return ret ? nameof(yes) : nameof(no);
    }
}
