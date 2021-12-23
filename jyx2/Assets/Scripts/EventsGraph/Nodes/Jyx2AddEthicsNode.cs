using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增加品德")]
[NodeWidth(150)]
public class Jyx2AddEthicsNode : Jyx2SimpleNode
{
    [Header("增加数值")]
    public int addValue;
    
    private void Reset() {
        name = "增加品德";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.AddEthics(addValue);
    }
}
