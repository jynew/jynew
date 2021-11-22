using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2SetOneMagicNode))]
public class Jyx2SetOneMagicNodeEditor : NodeEditor
{
    private Jyx2NodeEditorHelperRole _roleDrawer;
    private Jyx2NodeEditorHelperSkill _skillDrawer;
    public override void OnCreate()
    {
        base.OnCreate();
        _roleDrawer = new Jyx2NodeEditorHelperRole(this);
        _skillDrawer = new Jyx2NodeEditorHelperSkill(this);
    }

    public override void OnBodyGUI() 
    {    
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        _roleDrawer.DrawAll();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("skillIndex"));
        _skillDrawer.DrawField();
        _skillDrawer.DrawPopup();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("skillLevel"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

}