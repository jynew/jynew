using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2HaveItemNode))]
public class Jyx2HaveItemNodeEditor : NodeEditor
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
        _itemDrawer.DrawAll();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("yes"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("no"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}