using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("流程控制/判断攻击力")]
[NodeWidth(180)]
public class Jyx2JudgeAttackNode : Jyx2BaseNode
{
    [Output] public Node yes;
    [Output] public Node no;
    [Header("角色id")]
	public int roleId;  
    [Header("最大值")]
	public int maxValue; 
    [Header("最小值")]
	public int minValue; 
    private void Reset() {
        name = "判断攻击力";
    }
    
    protected override string OnPlay()
    {
        bool ret = Jyx2LuaBridge.JudgeAttack(roleId, minValue, maxValue);
        return ret ? nameof(yes) : nameof(no);
    }
}
