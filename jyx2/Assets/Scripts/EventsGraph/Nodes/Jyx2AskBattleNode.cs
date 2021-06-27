using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/询问是否战斗")]
[NodeWidth(150)]
public class Jyx2AskBattleNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;

    private void Reset() {
        name = "询问是否战斗";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.AskBattle();
        return ret ? nameof(yes) : nameof(no);
    }
}
