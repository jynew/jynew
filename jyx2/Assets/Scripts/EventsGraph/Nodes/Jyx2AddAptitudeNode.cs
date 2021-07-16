using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using XNode;

[CreateNodeMenu("游戏数据/增加资质")]
[NodeWidth(180)]
public class Jyx2AddAptitudeNode : Jyx2SimpleNode
{
    [Header("角色id")]
    public int roleId;
    [Header("增加数值")]
    public int increment;
    
    private void Reset() {
        name = "增加资质";
    }

    protected override void DoExecute()
    {
        Jyx2LuaBridge.AddAptitude(roleId, increment);
    }
}
