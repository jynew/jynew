using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;

public class Jyx2NodeEditorHelperSkill : Jyx2NodeEditorHelperBase
{
    public Jyx2NodeEditorHelperSkill(NodeEditor nodeEditor) : base(nodeEditor)
    {
    }

    protected override string Field
    {
        get => "skillId";
    }
    protected override int TextureHeight
    {
        get => 0;
    }
    protected override string PopupTitle
    {
        get => "武功";
    }
    protected override string[] SelectContent
    {
        get => Jyx2EventsGraphStatic.s_skillList;
    }
    protected override string PathFormat
    {
        get => "";
    }
}