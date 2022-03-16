using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2NPCGetItemNode))]
public class Jyx2NPCGetItemNodeEditor : NodeEditor
{
    private Jyx2NodeEditorHelperItem _itemDrawer;
    private Jyx2NodeEditorHelperRole _roleDrawer;

    public override void OnCreate()
    {
        base.OnCreate();
        _itemDrawer = new Jyx2NodeEditorHelperItem(this);
        _roleDrawer = new Jyx2NodeEditorHelperRole(this);
    }

    public override void OnBodyGUI()
    {
        // Update serialized object's representation
        serializedObject.Update();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        _roleDrawer.DrawAll();
        _itemDrawer.DrawAll();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("quantity"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}