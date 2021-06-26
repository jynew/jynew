using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEditor;
using UnityEngine;
using XNode;

[CreateNodeMenu("事件修改")]
[NodeWidth(200)]
public class Jyx2ModifyEventNode : Jyx2BaseNode
{
    private void Reset() {
        name = "事件修改";
    }
    
    public int SceneId = -2;
    public int EventId = -2;

    /// <summary>
    /// 交互事件ID
    /// </summary>
    public int InteractiveEventId = -2;
    
    /// <summary> 
    /// 使用道具ID
    /// </summary>
    public int UseItemEventId = -2;
    
    /// <summary>
    /// 进入事件ID
    /// </summary>
    public int EnterEventId = -2;
}
