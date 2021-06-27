using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2UseItemNode))]
public class Jyx2UseItemNodeEditor : NodeEditor
{
    private Jyx2NodeEditorHelperItem _itemDrawer;
    public override void OnCreate()
    {
        base.OnCreate();
        _itemDrawer = new Jyx2NodeEditorHelperItem(this);
    }

    public override void OnBodyGUI() 
    {
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("yes"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("no"));
        
        _itemDrawer.DrawAll();
        
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}