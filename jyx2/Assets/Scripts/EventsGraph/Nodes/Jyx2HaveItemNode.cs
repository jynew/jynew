using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/是否拥有道具")]
[NodeWidth(200)]
public class Jyx2HaveItemNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;
    [Header("物品id")]
	public int itemId;  
    private void Reset() {
        name = "判断是否拥有道具";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.HaveItem(itemId);
        return ret ? nameof(yes) : nameof(no);
    }
}
