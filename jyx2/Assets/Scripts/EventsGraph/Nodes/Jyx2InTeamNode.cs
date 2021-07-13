using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/角色是否在队里")]
[NodeWidth(180)]
public class Jyx2InTeamNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;
    [Header("角色id")]
	public int roleId;  
    private void Reset() {
        name = "判断角色是否在队里";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.InTeam(roleId);
        return ret ? nameof(yes) : nameof(no);
    }
}
