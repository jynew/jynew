using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增加道德")]
[NodeWidth(150)]
public class Jyx2AddEthicsNode : Jyx2SimpleNode
{
    public int AddValue;
    
    private void Reset() {
        name = "增加道德";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.AddEthics(AddValue);
    }
}
