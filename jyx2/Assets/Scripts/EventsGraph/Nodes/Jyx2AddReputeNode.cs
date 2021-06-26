using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("队伍属性变化/增加声望")]
[NodeWidth(150)]
public class Jyx2AddReputeNode : Jyx2BaseNode
{
    public int AddValue;
    
    private void Reset() {
        name = "增加声望";
    }
}
