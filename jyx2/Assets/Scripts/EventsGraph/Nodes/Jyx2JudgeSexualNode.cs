using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断主角的性别")]
[NodeWidth(180)]
public class Jyx2JudgeSexualNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;
    [Header("性别id(0:男, 1:女, 2:人妖)")]
    public int genderId;
    private void Reset() {
        name = "判断主角的性别";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.JudgeSexual(genderId);
        return ret ? nameof(yes) : nameof(no);
    }
}
