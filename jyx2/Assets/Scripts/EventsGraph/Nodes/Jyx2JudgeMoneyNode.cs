using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断银两数")]
[NodeWidth(180)]
public class Jyx2JudgeMoneyNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;

    [Header("最小值")]
	public int minValue; 
    private void Reset() {
        name = "判断银两数";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.JudgeMoney(minValue);
        return ret ? nameof(yes) : nameof(no);
    }
}
