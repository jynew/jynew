using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/GAME OVER")]
[NodeWidth(100)]
public class Jyx2DeadNode : Node
{
    [Input] public Node prev;
    
    private void Reset() {
        name = "GAME OVER";
    }
}
