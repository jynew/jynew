using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断触发器的交互事件")]
[NodeWidth(150)]
public class Jyx2JudgeEventNumNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;

    [Header("触发器ID")] public int GameEventId = 0;
    [Header("交互事件是否等于")] public int InteractiveEvtId = -1;
    
    private void Reset() {
        name = "判断触发器的交互事件";
    }


    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.JudgeEventNum(GameEventId, InteractiveEvtId);
        return ret ? nameof(yes) : nameof(no);
    }
}
