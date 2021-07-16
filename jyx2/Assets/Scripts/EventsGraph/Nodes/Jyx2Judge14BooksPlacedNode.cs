using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断十四天书是否归位")]
[NodeWidth(150)]
public class Jyx2Judge14BooksPlacedNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;
    
    private void Reset() {
        name = "判断十四天书是否归位";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.Judge14BooksPlaced();
        return ret ? nameof(yes) : nameof(no);
    }
}
