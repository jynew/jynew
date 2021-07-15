using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;

public class Jyx2NodeEditorHelperScene : Jyx2NodeEditorHelperBase
{
    public Jyx2NodeEditorHelperScene(NodeEditor nodeEditor) : base(nodeEditor)
    {
    }

    protected override string Field
    {
        get => "sceneId";
    }
    protected override int TextureHeight
    {
        get => 0;
    }
    protected override string PopupTitle
    {
        get => "场景";
    }
    protected override string[] SelectContent
    {
        get => Jyx2EventsGraphStatic.s_sceneList;
    }
    protected override string PathFormat
    {
        get => "";
    }
}