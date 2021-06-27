using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;

public class Jyx2NodeEditorHelperRole : Jyx2NodeEditorHelperBase
{
    public Jyx2NodeEditorHelperRole(NodeEditor nodeEditor) : base(nodeEditor)
    {
    }

    protected override string Field
    {
        get => "roleId";
    }

    protected override int TextureHeight
    {
        get => 50;
    }
    protected override string PopupTitle
    {
        get => "角色";
    }
    protected override string[] SelectContent
    {
        get => Jyx2EventsGraphStatic.s_roleList;
    }
    protected override string PathFormat
    {
        get => "Assets/BuildSource/head/{0}.png";
    }
}