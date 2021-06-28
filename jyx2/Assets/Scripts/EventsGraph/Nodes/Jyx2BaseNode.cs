using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(256)]
[CreateNodeMenu("")]
public abstract class Jyx2BaseNode : Node
{
    [Input] public Node prev;
 
    private Node NextNode(string field)
    {
        var port = this.GetOutputPort(field);
        if (port == null || !port.IsConnected) return null;
        return port.GetConnection(0).node;
    }

    /// <summary>
    /// 注意本函数跑在非UI线程
    /// </summary>
    /// <returns></returns>
    public virtual Node Play()
    {
        return NextNode(OnPlay());
    }


    /// <summary>
    /// 执行节点逻辑
    /// 注意本函数跑在非UI线程
    /// </summary>
    /// <returns>返回nameof({nextNode})</returns>
    protected abstract string OnPlay();
}


//只有一个next的节点的Node
public abstract class Jyx2SimpleNode : Jyx2BaseNode
{
    [Output] public Node next;

    protected override string OnPlay()
    {
        DoExecute();
        return nameof(next);
    }

    /// <summary>
    /// 执行节点逻辑
    /// 注意本函数跑在非UI线程
    /// </summary>
    protected abstract void DoExecute();
}
