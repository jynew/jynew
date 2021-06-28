using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;


public class Jyx2NodeEditorHelperItem : Jyx2NodeEditorHelperBase
{
    public Jyx2NodeEditorHelperItem(NodeEditor nodeEditor) : base(nodeEditor)
    {
    }

    protected override string Field
    {
        get => "itemId";
    }
    protected override int TextureHeight
    {
        get => 50;
    }
    protected override string PopupTitle
    {
        get => "道具";
    }
    protected override string[] SelectContent
    {
        get => Jyx2EventsGraphStatic.s_itemList;
    }
    protected override string PathFormat
    {
        get => "Assets/BuildSource/Jyx2Items/{0}.png";
    }
}
